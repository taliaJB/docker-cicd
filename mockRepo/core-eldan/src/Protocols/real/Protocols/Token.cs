using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Eldan.TypeExtensions;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Net.Http;

namespace Eldan.Protocols
{
    internal class Token
    {
        const int EXPIRATION_DELTA = 100;

        private Token() { }

        public static Token CreateInstance(string tokenFileName)
        {
            Token token;
            if (!File.Exists(tokenFileName))
            {
                token = Reset(tokenFileName);
            }
            else
            {
                string tokenJSON = File.ReadAllText(tokenFileName);
                token = JsonConvert.DeserializeObject<Token>(tokenJSON);
                token.TokenFileName = tokenFileName;
            }
            return token;
        }


        public Token Reset()
        {
            return Reset(TokenFileName);
        }

        public static Token Reset(string tokenFileName)
        {
            Token token = new Token { TokenFileName = tokenFileName };
            token.Save();
            return token;
        }

        public void Save()
        {
            Save(this.ToJSON(true));
        }

        public void Save(string tokenJSON)
        {
            if (string.IsNullOrWhiteSpace(TokenFileName))
                throw new Exception("Token:Save - TokenFileName is empty");

            File.WriteAllText(TokenFileName, tokenJSON);
        }

        public Token Refresh()
        {
            if (string.IsNullOrWhiteSpace(TokenFileName))
                throw new Exception("Token:Refresh - TokenFileName is empty");
            
            return CreateInstance(TokenFileName);
        }

        [JsonIgnore]
        public string TokenEndpoint { get; set; }
        [JsonIgnore]
        public string ClientId { get; set; }
        [JsonIgnore]
        public string ClientSecret { get; set; }
        [JsonIgnore]
        public int TimeoutMinutes { get; set; } = 2;
        [JsonIgnore]
        public string TokenFileName { get; set; }

        public bool IsCurrentlyUpdated { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.MinValue;

        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; set; } = "Bearer";

        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }

        [JsonProperty(PropertyName = "scope")]
        public string Scope { get; set; } = "scope";

        [JsonProperty(PropertyName = "expires_in")]
        public int AccessTokenExpiresIn { get; set; }
        
        public DateTime? AccessTokenExpiresAt 
        {
            get 
            {
                if (LastUpdated == DateTime.MinValue)
                    return null;
                else
                    return LastUpdated.AddSeconds(AccessTokenExpiresIn - EXPIRATION_DELTA); 
            }
        }

        public bool IsAccessTokenExpired 
        { 
            get 
            {
                if (AccessTokenExpiresAt.HasValue)
                    return AccessTokenExpiresAt <= DateTime.Now;
                else
                    return true;
            }
        }

        [JsonProperty(PropertyName = "consented_on")]
        public long ConsentedOn { get; set; }

        [JsonProperty(PropertyName = "refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty(PropertyName = "refresh_token_expires_in")]
        public int RefreshTokenExpiresIn { get; set; }
        public DateTime? RefreshTokenExpiresAt
        {
            get 
            { 
                if (ConsentedOn == 0)
                    return null;
                else
                    return GetDateTime(ConsentedOn).AddSeconds(RefreshTokenExpiresIn - EXPIRATION_DELTA); 
            }
        }
        public bool IsRefreshTokenExpired
        {
            get 
            {
                if (RefreshTokenExpiresAt.HasValue)
                    return RefreshTokenExpiresAt < DateTime.Now;
                else
                    return true;
            }
        }

        public async Task<Token> GetAccessToken()
        {
            return await GetAccessToken(TimeSpan.FromMinutes(TimeoutMinutes));
        }

        private async Task<Token> GetAccessToken(TimeSpan timeRemain)
        {
            const int WAIT = 500;

            if (!IsAccessTokenExpired)
                return this;

            if (IsRefreshTokenExpired)
                throw new Exception("Token:GetCurrentAccessToken - Refresh token has expired");
            
            Token token = CreateInstance(TokenFileName);
            if (token.IsCurrentlyUpdated)
            {
                if (timeRemain <= TimeSpan.Zero)
                    throw new TimeoutException("Token:GetUpdatedAccessToken - Timeout expired for updating token file");

                Thread.Sleep(WAIT);

                return await GetAccessToken(timeRemain.Subtract(TimeSpan.FromMilliseconds(WAIT)));
            }
            
            // Lock file
            token.IsCurrentlyUpdated = true;
            token.Save();

            try
            {
                await UpdateToken();
            }
            catch (Exception ex)
            {
                // Unlock file
                token.IsCurrentlyUpdated = false;
                token.Save();
                throw ex;
            }

            return CreateInstance(TokenFileName);
        }

        private async Task UpdateToken()
        {
            if (string.IsNullOrWhiteSpace(ClientId))
                throw new Exception("Token:UpdateToken - ClientId (Cridentials.UserName) is empty");

            if (string.IsNullOrWhiteSpace(ClientSecret))
                throw new Exception("Token:UpdateToken - ClientSecret (Cridentials.Password) is empty");

            if (string.IsNullOrWhiteSpace(TokenEndpoint))
                throw new Exception("Token:UpdateToken - TokenEndpoint is empty");

            var client = new HttpClient();

            var requestContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", RefreshToken),
                new KeyValuePair<string, string>("client_id", ClientId),
                new KeyValuePair<string, string>("client_secret", ClientSecret)
            });

            var response = await client.PostAsync(TokenEndpoint, requestContent);

            if (!response.IsSuccessStatusCode)
            {
                // Handle the error response
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new Exception($"Token:UpdateToken - Token request failed: {response.StatusCode} - {errorResponse}");
            }

            var tokenJSON = await response.Content.ReadAsStringAsync();

            Token token = JsonConvert.DeserializeObject<Token>(tokenJSON);
            // Unlock file
            token.IsCurrentlyUpdated = false;
            token.TokenFileName = TokenFileName;
            token.LastUpdated = DateTime.Now;
            token.Save();
        }

        private DateTime GetDateTime(long dateTimeNum)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 2, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(dateTimeNum);
        }
    }
}

