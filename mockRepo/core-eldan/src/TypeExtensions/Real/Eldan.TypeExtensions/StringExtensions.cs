using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;
using System.Reflection;
using System.IO;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Eldan.TypeExtensions
{
    public enum enmNumberSign
    {
        Positive,
        Negative,
        Both
    }

    public static class StringExtensions
    {
        private const string MASK = "*****";

        public static string Right(this string source, int iMaxLength)
        {
            return GetStringBySide(source, iMaxLength, enmSide.Right);
        }

        public static string Left(this string source, int iMaxLength)
        {
            return GetStringBySide(source, iMaxLength, enmSide.Left);
        }

        public static string GetMaxText(this string source, int iMaxLength)
        {
            if (string.IsNullOrEmpty(source))
                return source;

            string left = source.Left(iMaxLength);

            if (iMaxLength >= source.Length)
                return left;
            else
                return left.Left(iMaxLength - 3) + "...";
        }

        private enum enmSide
        {
            Left,
            Right
        }

        private static string GetStringBySide(string source, int iMaxLength, enmSide Side)
        {
            //Check if the value is valid
            if (string.IsNullOrEmpty(source) || iMaxLength < 1)
            {
                //Set valid empty string as string could be null
                source = string.Empty;
            }
            else if (source.Length > iMaxLength)
            {
                switch (Side)
                {
                    case enmSide.Left:
                        //Make the string no longer than the max length
                        source = source.Substring(0, iMaxLength);
                        break;
                    case enmSide.Right:
                        //Make the string no longer than the max length
                        source = source.Substring(source.Length - iMaxLength, iMaxLength);
                        break;
                    default:
                        break;
                }
            }

            //Return the string
            return source;
        }

        public static string GetFixedPath(this string source)
        {

            if (!string.IsNullOrEmpty(source))
            {
                if (source.ToCharArray()[source.Length - 1] != '\\')
                {
                    source += @"\";
                }
            }

            return source;
        }

        public static string GetFileName(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return source;
            return Path.GetFileName(source);
        }

        public static string GetBinPath()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase).Split(new string[] { @"file:\" }, StringSplitOptions.RemoveEmptyEntries).First();
        }

        public static string GetFileExtension(this string source)
        {
            return Path.GetExtension(source);
        }

        public static string GetFileExtension(this string source, bool withDot)
        { 
            string ext = Path.GetExtension(source);
            if (withDot)
                return ext;

            if (ext == null)
                return null;

            return ext.Split('.').Last();
        }

        public static string GetFilePath(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return source;
            string[] arrFile = source.Split('\\');
            arrFile[arrFile.Length - 1] = null;
            string FixedPath = string.Join(@"\", arrFile);
            if (string.IsNullOrEmpty(FixedPath))
                return source;
            return FixedPath.Substring(0, FixedPath.Length - 1);
        }

        public static bool ContainsByWords(this string source, string value, char[] delimiters)
        {
            source.ValidateParamForNull(nameof(source));
            value.ValidateParamForNull(nameof(value));

            string[] SWords = source.GetArray(delimiters);
            string[] VWords = value.GetArray(delimiters);

            return VWords.All(x => SWords.Contains(x));
        }


        private static string[] GetArray(this string text, char[] delimiters)
        {
            var Arr = from item in text.Split(delimiters)
                      where !string.IsNullOrWhiteSpace(item)
                      select item.Trim().ToLower();

            return Arr.ToArray();
        }

        public static bool Contains(this string source, string value, StringComparison comparison)
        {
            if (source == null && value == null)
                return true;

            if (source == null || value == null)
                return false;

            return source.IndexOf(value, comparison) >= 0;
        }

        public static bool Equal(this string source, string value, StringComparison comparison)
        {
            if (source == null && value == null)
                return true;

            if (source == null || value == null)
                return false;

            return (source.Length == value.Length && source.Contains(value, comparison));

        }

        public static string TrimNullable(this string source)
        {
            return source?.Trim();
            //return source.TrimNullLess(null);
        }

        public static string TrimNullLess(this string source)
        {
            return source.TrimNullLess("");
        }

        public static string TrimNullLess(this string source, string nullValue)
        {
            return source?.Trim() ?? nullValue;
        }

        public static string MaskJSONParam(this string source, string paramName)
        {
            return source.MaskJSONParam(paramName, 0);
        }

        public static string MaskJSONParam(this string source, string paramName, int rightFreeCharacters)
        {
            if (string.IsNullOrEmpty(source))
                return source;

            string NewSource = source;

            string[] Dels = new string[1];
            Dels[0] = paramName + "\":\"";

            string[] SourceParts = source.Split(Dels, StringSplitOptions.None);

            for (int i = 1; i < SourceParts.Length; i++)
            {
                GetMaskParam(paramName, SourceParts[i], rightFreeCharacters, out string OldParam, out string NewParam);
                NewSource = NewSource.Replace(OldParam, NewParam);
            }

            return NewSource;

        }

        public static string MaskParam(this string source, int rightFreeCharacters)
        {
            return MaskParam(source, rightFreeCharacters, null);
        }

        public static string MaskParam(this string source, int rightFreeCharacters, string FullMaskValue)
        {
            if (FullMaskValue != null && rightFreeCharacters == 0)
                return FullMaskValue;

            return MASK + source.Right(rightFreeCharacters);
        }

        private static void GetMaskParam(string paramName, string containParamValue, int rightFreeCharacters, out string oldParam, out string newParam)
        {
            string[] ParamParts = containParamValue.Split('\"');

            oldParam = "\"" + paramName + "\":\"" + ParamParts[0] + "\"";
            newParam = "\"" + paramName + "\":\"" + MASK + ParamParts[0].Right(rightFreeCharacters) + "\"";
        }

        public static string ReplaceJSONName(this string source, string oldName, string newName)
        {
            return source.Replace("\"" + oldName + "\":", "\"" + newName + "\":");
        }

        public static ImageFormat GetImageFormat(this string source)
        {
            Type type = typeof(System.Drawing.Imaging.ImageFormat);
            BindingFlags flags = BindingFlags.GetProperty;
            object o = type.InvokeMember(source, flags, null, type, null);
            return (ImageFormat)o;
        }

        public static string GetRTLMessage(this string source)
        {
            return "<!DOCTYPE html><html><body dir=rtl style=\"font-family:arial;\">" + source.ToNullLessString() + "</body></html>";
        }

        public static T ParseEnumValue<T>(this string value, bool ignoreCase) where T : struct
        {
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }

        public static bool TryParseEnumValue<T>(string value, bool ignoreCase, out T result) where T : struct
        {
            return Enum.TryParse(value, ignoreCase, out result);
        }

        public static bool FilesAreEqual(string firstFileName, string secondFileName)
        {
            const int BYTES_TO_READ = sizeof(Int64);

            FileInfo first = new FileInfo(firstFileName);
            FileInfo second = new FileInfo(secondFileName);

            if (first.Length != second.Length)
                return false;

            if (string.Equals(first.FullName, second.FullName, StringComparison.OrdinalIgnoreCase))
                return true;

            int iterations = (int)Math.Ceiling((double)first.Length / BYTES_TO_READ);

            using (FileStream fs1 = first.OpenRead())
            using (FileStream fs2 = second.OpenRead())
            {
                byte[] one = new byte[BYTES_TO_READ];
                byte[] two = new byte[BYTES_TO_READ];

                for (int i = 0; i < iterations; i++)
                {
                    fs1.Read(one, 0, BYTES_TO_READ);
                    fs2.Read(two, 0, BYTES_TO_READ);

                    if (BitConverter.ToInt64(one, 0) != BitConverter.ToInt64(two, 0))
                        return false;
                }
            }

            return true;
        }

        public static bool IsFileExist(this string source, int tries, int delayInMs)
        {
            return source.IsFileExist(tries, delayInMs, null);
        }

        public static bool IsFileExist(this string source, int tries, int delayInMs, Action<string> writeMessage)
        {
            return IsFileExist(source, 1, tries, delayInMs, writeMessage);
        }

        private static bool IsFileExist(string fileName, int currentTry, int tries, int delayInMs, Action<string> writeMessage)
        {
            if (fileName == null)
                throw new Exception("File name can not be null");

            writeMessage?.Invoke(string.Format("Attempt to test existence of file: '{0}' for try: '{1}' out of: '{2}' tries", fileName, currentTry, tries));

            if (File.Exists(fileName))
            {
                writeMessage?.Invoke(string.Format("File: '{0}' found", fileName));
                return true;
            }
            else
            {
                if (currentTry >= tries)
                {
                    writeMessage?.Invoke(string.Format("File: '{0}' not found", fileName));
                    return false;
                }
                else
                {
                    Thread.Sleep(delayInMs);
                    return IsFileExist(fileName, currentTry + 1, tries, delayInMs, writeMessage);
                }
            }

        }

        public static T DeserializeXMLtoObject<T>(this string source)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            T deserializeObject;
            try
            {
                using (StringReader reader = new StringReader(source))
                {
                    deserializeObject = (T)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"StringExtensions:DeserializeXMLtoObject - XML '{source.MaskNull().Left(30)}' can not deserialize into object since: {ex.Message}");
            }

            return deserializeObject;  
        }

        public static T DeserializeJSONtoObject<T>(this string source)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(source);
            }
            catch (Exception ex)
            {
                throw new Exception($"StringExtensions:DeserializeJSONtoObject - JSON '{source.MaskNull().Left(30)}' can not deserialize into object since: {ex.Message}");
            }
        }

        public static string JSONtoXML(this string source)
        {
            return source.JSONtoXML(null);
        }

        public static string JSONtoXML(this string source, string root)
        {
            JObject jo = JObject.Parse(source);
            return jo.JObjectToXml(root);
        }

        public static string XMLtoJSON(this string source)
        {
            XDocument Doc = XDocument.Parse(source);
            return Doc.ToJSON();
        }

        public static bool IsDataFixed(string data, out string fixedData)
        {
            fixedData = null;
            if (string.IsNullOrWhiteSpace(data))
                return false;
            else
                fixedData = data.Trim();

            return true;
        }

        public static string GetFixedData(this string source, string paramName)
        {
            if (!IsDataFixed(source, out string FixedData))
                ParamInvalid(paramName);

            return FixedData;
        }

        public static void ValidateParamForNull(this object param, string paramName)
        {
            if (param == null)
                throw new Exception(string.Format("StringExtensions:ValidateParamForNull - '{0}' can't be null", paramName));
        }

        public static void ValidateParam(this string source, string paramName)
        {
            if (string.IsNullOrWhiteSpace(source))
                ParamInvalid(paramName);
        }

        private static void ParamInvalid(string paramName)
        {
            throw new Exception(string.Format("StringExtensions.ParamInvalid - parameter: '{0}' can't be null or empty", paramName));
        }

        public static bool IsNumericLong(this string source, int lenght)
        {
            return IsNumericLong(source, enmNumberSign.Both, lenght);
        }

        public static bool IsNumericLong(this string source, enmNumberSign numberSign, int lenght)
        {
            return IsNumericLong(source, numberSign) && source.Length == lenght;
        }

        public static bool IsNumericLong(this string source)
        {
            return IsNumericLong(source, enmNumberSign.Both);
        }

        public static bool IsNumericLong(this string source, enmNumberSign numberSign)
        {
            if (!long.TryParse(source, out long Num))
                return false;
            
            switch (numberSign)
            {
                case enmNumberSign.Positive:
                    return Num > 0;
                case enmNumberSign.Negative:
                    return Num < 0 ;
                case enmNumberSign.Both:
                    return true;
            }
            return true;
        }

        public static T GetTypeValue<T>(this string value)
        {
            return GetTypeValue<T>(value, true);
        }

        public static T GetTypeValue<T>(this string value, bool returnDefaulInFailure)
        {
            return GetTypeValue(value, returnDefaulInFailure, GetTypeDefualt<T>());
        }

        public static T GetTypeValue<T>(this string value, bool returnDefaulInFailure , T defaultValue)
        {
            Type UnderlyingType = Nullable.GetUnderlyingType(typeof(T));
            if (UnderlyingType != null)
            {
                object ob = null;
                if (string.IsNullOrWhiteSpace(value))
                    return ob.CastToReflected(typeof(T));
               
                if (value.Trim().ToLower() == "null")
                    return ob.CastToReflected(typeof(T));
            }

            if (typeof(T) == typeof(string))
                return value.TrimNullable().CastToReflected(typeof(T));

            try
            {
                if (value.TryParseType(out T ParsedValue))
                    return ParsedValue;
                else
                    return GetDefaultOrExeption(value, returnDefaulInFailure, defaultValue);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("StringExtensions.TypeValueException - type: '{0}' failed to be parsed since: '{1}'", 
                     typeof(T).Name, ex.ToString()));

            }
        }

        internal static T GetTypeDefualt<T>()
        {
            if (typeof(T) == typeof(string))
                return string.Empty.CastToReflected(typeof(T));

            return default;
        }

        private static T GetDefaultOrExeption<T>(string value, bool returnDefaulInFailure, T defaultValue)
        {
            if (returnDefaulInFailure)
                return defaultValue;
            else
                throw new Exception(string.Format("StringExtensions.GetDefaultOrExeption - value: '{0}' can not be parsed into type: '{1}'", 
                        value.ToNullLessString("<NULL>"), typeof(T).Name));
        }

        public static void Base64ToFile(this string base64Text, string fileName)
        {
            File.WriteAllBytes(fileName, Convert.FromBase64String(base64Text));
        }

        public static string FileToBase64(this string FileName)
        {
            byte[] pdfBytes = File.ReadAllBytes(FileName);
            return Convert.ToBase64String(pdfBytes);
        }

        public static string ToFileName(this string source)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                source = source.Replace(c, '_');
            }

            return source;
        }

        /// <summary>
        /// היפוך תו ממחרוזת. מצד אחד לצד השני
        /// </summary>
        /// <param name="originalText">מחרוזת מקורית</param>
        /// <returns>מחרוזת חדשה עם התו בצד השני</returns>
        public static string SwitchCharactersDirectionsInText(this string originalText)
        {
            if (string.IsNullOrEmpty(originalText))
                return string.Empty;

            string NewText = string.Empty;

            char[] OriginalTextArray = originalText.ToCharArray();

            int resX = 0;
            string charThatFound = string.Empty;
            int charIndex = 0;

            foreach (char item in OriginalTextArray)
            {
                if (item != ' ' && !int.TryParse(item.ToString(), out resX))
                {
                    charThatFound = item.ToString();
                    charIndex = originalText.IndexOf(charThatFound) + 1;
                    break;
                }
                else
                    continue;
            }

            if (charIndex == 1)
                return NewText = originalText.Replace(charThatFound, string.Empty) + charThatFound;
            else if (charIndex == originalText.Length)
                return NewText = charThatFound + originalText.Replace(charThatFound, string.Empty);
            else
                return NewText = originalText;
        }
    }
}
