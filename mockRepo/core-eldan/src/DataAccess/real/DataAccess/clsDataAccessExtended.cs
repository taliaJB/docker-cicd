using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eldan.SecurityBase;
using Eldan.TypeExtensions;

namespace Eldan.DataAccess
{
    public class clsDataAccessExtended : clsDataAccess
    {
        public string GetPassword(string EncryptedPassword) => StringCipher.Decrypt(EncryptedPassword, GetSALT());

        public string GetSALT()
        {
            return GetParamByEnv(89).TrimNullable();
        }

        public string GetParamByEnv(int paramCode)
        {
            if (int.TryParse(ConfigurationManager.AppSettings["env"], out int EnvCode))
                return GetParamByEnv(paramCode, EnvCode);

            throw new Exception(string.Format("clsDataAccessExtended:GetParamByEnv - Configuration perameter 'env' has value: '{0}' which can't be parsed. this parameter probably has not declared in configuration file",
                        ConfigurationManager.AppSettings["env"].ToNullLessString("<NULL>")));
        }

        public string GetParamByEnv(int paramCode, int envCode)
        {
            DataTable Dt = ExecuteSPdt("p_GetParamByEnv", new Parameter("@EnvCode", envCode),
                                                          new Parameter("@ParamCode", paramCode));

            if (Dt.Rows.Count == 0)
                throw new Exception(string.Format("clsDataAccessExtended:GetParamByEnv - No param secified for EnvCode: '{0}' and ParamCode: '{1}'", envCode, paramCode));

            return Dt.Rows[0].Field<string>("VALUE");
        }

        public T GetParam<T>(int code)
        {
            DataTable Dt = ExecuteSPdt("p_GetParamByCode", new Parameter("@CODE", code));

            if (Dt.Rows.Count == 0)
                throw new Exception(string.Format("{0}.{1} - No value found for code: '{2}'",
                        nameof(clsDataAccessExtended),
                        nameof(GetParam),
                        code));

            string value = Dt.Rows[0].Field<string>("VALUE");

            if (typeof(T) == typeof(DateTime))
            {
                if (value.Length == 8)
                {
                    if (DateExtensions.TryParseEldanDate(value, "000000", out DateTime res))
                        return res.CastToReflected(typeof(T));
                    else
                        throw new Exception(string.Format("{0}.{1} - value: '{2}' can not be parsed to DateTime (Date)",
                                nameof(clsDataAccessExtended),
                                nameof(GetParam),
                                value.MaskNull()));
                }
                else
                {
                    if (DateExtensions.TryParseEldanDate("00010101", value, out DateTime res))
                        return res.CastToReflected(typeof(T));
                    else
                        throw new Exception(string.Format("{0}.{1} - value: '{2}' can not be parsed to DateTime (Time)",
                                nameof(clsDataAccessExtended),
                                nameof(GetParam),
                                value.MaskNull()));
                }
            }

            return value.GetTypeValue<T>(false);
        }
    }
}
