using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Eldan.FlexibleCloner
{
    internal enum MemberType
    {
        Field,
        Property
    }

    internal static class Common
    {
        internal static bool IsList(this object obj)
        {
            Type type = obj.GetType();

            return type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>));
        }

        internal static bool IsMemberHasAtt<T>(this MemberInfo memberInfo) where T : System.Attribute
        {
            return memberInfo.GetCustomAttributes<T>().Count() > 0;
        }

        internal static object GetMemberValue(this object src, string memberName)
        {
            MemberType? FoundMemberType = GetMemberType(src, memberName);
            if (!FoundMemberType.HasValue)
                return null;

            if (GetMemberType(src, memberName) == MemberType.Property)
                return GetPropValue(src, memberName);
            else
                return GetFieldValue(src, memberName);
        }

        internal static MemberType? GetMemberType(object src, string memberName)
        {
            if (src.GetType().GetProperties().Any(item => item.Name == memberName))
                return MemberType.Property;

            if (src.GetType().GetFields().Any(item => item.Name == memberName))
                return MemberType.Field;

            return null;
        }

        private static object GetPropValue(object src, string propName)
        {
            var PropInfo = src.GetType().GetProperty(propName);

            if (PropInfo == null)
                return null;
            else
                return PropInfo.GetValue(src);
        }

        private static object GetFieldValue(object src, string fieldName)
        {
            var FldInfo = src.GetType().GetField(fieldName);

            if (FldInfo == null)
                return null;
            else
                return FldInfo.GetValue(src);
        }
    }
}
