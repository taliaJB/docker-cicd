using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;

namespace Eldan.TypeExtensions
{
    public static class ObjectExtensions
    {
        public static bool ToBool(this object source)
        {
            return ToBool(source, false);
        }

        public static bool ToBool(this object source, bool DefaultValue)
        {
            string strVal = source.ToNullLessString().Trim().ToLower();
            if (strVal == "1" || strVal == "true")
                return true;

            if (strVal == "0" || strVal == "false")
                return false;

            return DefaultValue;

        }

        public static T ToNumeric<T>(this object source)
            where T : struct,
                      IComparable,
                      IComparable<T>,
                      IConvertible,
                      IEquatable<T>,
                      IFormattable
        {
            return (T)Convert.ChangeType(source, typeof(T));
        }

        public static string MaskNull(this object source)
        {
            return source.ToNullLessString("<NULL>");
        }

        public static string ToNullLessString(this object source)
        {
            return source.ToNullLessString("");
        }

        public static string ToNullLessString(this object source, string NullValue)
        {
            return (source == null) ? NullValue : source.ToString();
        }

        public static string ToXML(this object source)
        {
            if (source == null)
                return null;

            if (source is JObject)
                return ((JObject)source).JObjectToXml("Root");

            XmlSerializer xs = new XmlSerializer(source.GetType());
            System.IO.StringWriter sw = new System.IO.StringWriter();
            xs.Serialize((TextWriter)sw, source);
            return sw.ToString();
        }

        public static string JObjectToXml(this JObject jo, string rootElementName)
        {
            XmlDocument doc = JsonConvert.DeserializeXmlNode(jo.ToString(), rootElementName);
            StringBuilder sb = new StringBuilder();
            StringWriter sr = new StringWriter(sb);
            XmlTextWriter xw = new XmlTextWriter(sr);
            xw.Formatting = System.Xml.Formatting.Indented;
            doc.WriteTo(xw);
            return sb.ToString();
        }

        public static string ToJSONScriptSerializer(this object source)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(source);
        }

        public static string ToJSONScriptSerializer(this object source, int recursionDepth)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.RecursionLimit = recursionDepth;
            return serializer.Serialize(source);
        }

        public static string ToJSON(this object source)
        {
            return JsonConvert.SerializeObject(source);
        }

        public static string ToJSON(this object source, bool keepIndented)
        {
            return JsonConvert.SerializeObject(source, keepIndented ? Newtonsoft.Json.Formatting.Indented : Newtonsoft.Json.Formatting.None);
        }

        public static string ToJSON(this object source, int recursionDepth)
        {
            return source.ToJSON(true, recursionDepth);
        }

        public static string ToJSON(this object source, bool keepIndented, int recursionDepth)
        {
            return JsonConvert.SerializeObject(source,
                                               keepIndented ? Newtonsoft.Json.Formatting.Indented : Newtonsoft.Json.Formatting.None, 
                                               new JsonSerializerSettings() { MaxDepth = recursionDepth });
        }

        [ObsoleteAttribute("This method will soon be deprecated. Use ToJSON instead.")]
        public static string ToJSONtonsoft(this object source)
        {
            return JsonConvert.SerializeObject(source);
        }

        [ObsoleteAttribute("This method will soon be deprecated. Use ToJSON instead.")]
        public static string ToJSONtonsoft(this object source, int recursionDepth)
        {
            return JsonConvert.SerializeObject(source, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings() { MaxDepth = recursionDepth });
        }
    }
}
