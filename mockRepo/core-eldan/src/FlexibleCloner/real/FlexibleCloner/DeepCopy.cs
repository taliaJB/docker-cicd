using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Eldan.TypeExtensions;

namespace Eldan.FlexibleCloner
{
    public class DeepCopy
    {
        private object m_PlatformObject;

        public object PlatformObject
        {
            get { return m_PlatformObject; }
            set { m_PlatformObject = value; }
        }

        public void Clone(ref object dest)
        {
            Clone(m_PlatformObject, ref dest);
        }

        public void Clone(object source, ref object dest)
        {
            if (source == null)
                return;

            if (dest.IsPrimitiveNullble())
            {
                if (source.IsPrimitive())
                {
                    try
                    {
                        // Copy when Source is primitive
                        dest = source;
                    }
                    catch (Exception)
                    {
                    }

                    return;
                }

                // Exit when dest is null and not primitive (in this case, there is nothing to copy)
                if (dest == null)
                    return;
            }

            // Exit when source is primitive and dest is NOT primitive
            if (source.IsPrimitive())
                return;

            if (dest.IsList())
            {
                if (source.IsList())
                {
                    // Copy list recursively
                    CopyList(source, ref dest);
                }

                return;
            }

            if (dest.GetType().IsArray)
            {
                if (source.GetType().IsArray)
                {
                    // Copy array recursively
                    CopyArray(source, ref dest);
                }

                return;
            }

            // Copy members (fields and properties) recursively of class instance or struct instance
            CopyMembers(source, ref dest);
        }

        private void CopyList(object source, ref object dest)
        {
            dynamic DynamicSource = source;
            dynamic DynamicDest = dest;

            Type DestElementType = dest.GetType().GetGenericArguments()[0];
            Type SourceElementType = source.GetType().GetGenericArguments()[0];
            if (DestElementType == SourceElementType)
                dest = Activator.CreateInstance(dest.GetType(), source);
            else
            {
                if (dest.IsPrimitive() || source.IsPrimitive())
                    return;

                foreach (var SourceElement in DynamicSource)
                {
                    object DestElement = Activator.CreateInstance(DestElementType);

                    Clone(SourceElement, ref DestElement);

                    DynamicDest.Add(DestElement.CastToReflected(DestElementType));
                }
            }
        }

        private void CopyArray(object source, ref object dest)
        {
            Array ArraySource = source as Array;

            if (ArraySource.Length == 0)
                return;

            dest = Activator.CreateInstance(dest.GetType(), new object[] { ArraySource.Length });
            Array ArrayDest = dest as Array;

            Type SourceItemType = source.GetType().GetElementType();
            Type DestItemType = dest.GetType().GetElementType();
            bool EqualElementTypes = false;

            if (SourceItemType == DestItemType)
                EqualElementTypes = true;
            else
            {
                if (SourceItemType.IsSimpleType() || DestItemType.IsSimpleType())
                    return;
            }

            for (int i = 0; i < ArraySource.Length; i++)
            {
                object ItemSource = ArraySource.GetValue(i);

                if (EqualElementTypes)
                {
                    ArrayDest.SetValue(ItemSource, i);
                    continue;
                }

                object ItemDest = Activator.CreateInstance(DestItemType);

                Clone(ItemSource, ref ItemDest);

                ArrayDest.SetValue(ItemDest, i);
            }
        }

        private void CopyMembers(object source, ref object dest)
        {
            var SourceMembersInfos = GetMembersInfos(source);
            var DestMembersInfos = GetMembersInfos(dest);

            EnmMappingDirection MappingDirection = GetMappingDirection(SourceMembersInfos, DestMembersInfos);

            foreach (MemberInfo DestMemberInfo in DestMembersInfos)
            {
                List<object> SourceMembersValues = GetMembersValues(source, SourceMembersInfos, DestMemberInfo, MappingDirection, out string MethodName);
                object SourceMembersResult = null;

                if (SourceMembersValues.Count > 0)
                {
                    if (MethodName == null)
                        SourceMembersResult = SourceMembersValues.First();
                    else
                    {
                        bool HasReturnValue;
                        object ReturnValue;
                        try
                        {
                            ReturnValue = ObjectCloneExtensions.RunMethod(m_PlatformObject, MethodName, out HasReturnValue, SourceMembersValues.ToArray());
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(string.Format("DeepCopy.CopyMembers - method: '{0}' relateed to DestMemberName: '{1}' can't execute since: '{2}'",
                                                               MethodName,
                                                               DestMemberInfo.Name,
                                                               ex.ToString()));
                        }

                        if (HasReturnValue)
                            SourceMembersResult = ReturnValue;
                    }
                }
                else
                    continue;

                object DestMemberValue = Common.GetMemberValue(dest, DestMemberInfo.Name);
                if (DestMemberValue == null && SourceMembersResult != null)
                {
                    if (!SourceMembersResult.IsPrimitive())
                    {
                        try
                        {
                            DestMemberValue = CreateObject(DestMemberInfo);
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                }

                Clone(SourceMembersResult, ref DestMemberValue);

                try
                {
                    SetMemberValue(dest, DestMemberInfo.Name, DestMemberValue);
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        private enum EnmMappingDirection
        {
            SourceToDestination,
            DestibationToSource
        }

        private static EnmMappingDirection GetMappingDirection(List<MemberInfo> sourceMembersInfos, List<MemberInfo> destMembersInfos)
        {
            bool SourceAttExist = false;

            foreach (MemberInfo MemberInfo in sourceMembersInfos)
            {
                if (MemberInfo.IsMemberHasAtt<DestinationAttribute>())
                    throw new Exception("DeepCopy.GetMappingDirection - Source object can't contain Destination attribute");

                if (MemberInfo.IsMemberHasAtt<SouceAttribute>())
                {
                    SourceAttExist = true;
                    break;
                }
            }

            bool DestinationAttExist = false;
            foreach (MemberInfo MemberInfo in destMembersInfos)
            {
                if (MemberInfo.IsMemberHasAtt<SouceAttribute>())
                    throw new Exception("DeepCopy.GetMappingDirection - Destination object can't contain Source attribute");

                if (MemberInfo.IsMemberHasAtt<DestinationAttribute>())
                {
                    DestinationAttExist = true;
                    break;
                }
            }

            if (SourceAttExist && DestinationAttExist)
                throw new Exception("DeepCopy.GetMappingDirection - Cass\\Struct can not have both source\\detination attributes");

            if (!SourceAttExist && DestinationAttExist)
                return EnmMappingDirection.DestibationToSource;
            else
                return EnmMappingDirection.SourceToDestination;
        }

        private static object CreateObject(MemberInfo memberInfo)
        {
            Type MemberType;
            if (memberInfo.MemberType == MemberTypes.Field)
                MemberType = ((System.Reflection.FieldInfo)memberInfo).FieldType;
            else
                MemberType = ((System.Reflection.PropertyInfo)memberInfo).PropertyType;

            if (MemberType.IsArray)
                return Activator.CreateInstance(MemberType, new object[] { 0 });
            else
                return Activator.CreateInstance(MemberType);

        }

        private static List<MemberInfo> GetMembersInfos(object obj)
        {
            const BindingFlags BindingFlags = BindingFlags.Public | BindingFlags.Instance;
            List<MemberInfo> Members = obj.GetType().GetFields(BindingFlags).Cast<MemberInfo>()
                .Concat(obj.GetType().GetProperties(BindingFlags)).ToList();

            return Members;

        }

        private static List<object> GetMembersValues(object src, List<MemberInfo> sourceMembersInfos, MemberInfo destMemberInfo, EnmMappingDirection mappingDirection, out string methodName)
        {
            List<string> MemberNames;

            if (mappingDirection == EnmMappingDirection.SourceToDestination)
                MemberNames = GetSourceMembersNames(sourceMembersInfos, destMemberInfo.Name, out methodName);
            else
                MemberNames = GetSourceMembersNames(destMemberInfo, out methodName);

            var MembersValues = from MemberName in MemberNames
                                where IsSourceExists(sourceMembersInfos, MemberName)
                                select src.GetMemberValue(MemberName);

            return MembersValues.ToList();
        }

        private static bool IsSourceExists(List<MemberInfo> sourceMembersInfos, string sourceMemberName)
        {
            return sourceMembersInfos.Exists(x => x.Name == sourceMemberName);
        }

        private static void SetMemberValue(object src, string memberName, object value)
        {
            MemberType? MemType = Common.GetMemberType(src, memberName);
            if (!MemType.HasValue)
                return;

            if (MemType == MemberType.Property)
                SetPropValue(src, memberName, value);
            else
                SetFieldValue(src, memberName, value);
        }

        private static void SetPropValue(object src, string propName, object value)
        {
            src.GetType().GetProperty(propName).SetValue(src, value);
        }

        private static void SetFieldValue(object src, string fieldName, object value)
        {
            src.GetType().GetField(fieldName).SetValue(src, value);
        }

        private struct SourceMember
        {
            public string MemberName;
            public string MethodName;
            public int ParamOrder;
        }

        private static List<string> GetSourceMembersNames(MemberInfo destMemberInfo, out string methodName)
        {
            methodName = null;

            if (destMemberInfo.GetCustomAttribute<IgnoreMemberNameAttribute>() != null)
                return new List<string>();

            List<DestinationAttribute> DestMemberAttributes = destMemberInfo.GetCustomAttributes<DestinationAttribute>().ToList();
            List<SourceMember> SourceMembers;
            if (DestMemberAttributes.Count == 0)
            {
                SourceMembers = new List<SourceMember> { new SourceMember { MemberName = destMemberInfo.Name,
                                                                            MethodName = null,
                                                                            ParamOrder = 0} };
            }
            else
            {
                SourceMembers = (from DestMemberAttribute in DestMemberAttributes
                                 select new SourceMember
                                 {
                                     MemberName = DestMemberAttribute.SourceMemberName,
                                     MethodName = DestMemberAttribute.MethodName,
                                     ParamOrder = DestMemberAttribute.ParamOrder
                                 }).ToList();
            }

            return GetAndValidateSourceMembersNames(SourceMembers, destMemberInfo.Name, out methodName);
        }

        private static List<string> GetSourceMembersNames(List<MemberInfo> SourceMembersInfos, string attDestMemberName, out string methodName)
        {
            methodName = null;

            var SourceMembers = (from MemberInfo in SourceMembersInfos
                                 where HasSourceName(MemberInfo.GetCustomAttributes<SouceAttribute>().ToList(),
                                                     MemberInfo.GetCustomAttribute<IgnoreMemberNameAttribute>(),
                                                     MemberInfo.Name, attDestMemberName)
                                 select new SourceMember
                                 {
                                     MemberName = MemberInfo.Name,
                                     MethodName = GetMethodName(MemberInfo.GetCustomAttributes<SouceAttribute>().ToList(), MemberInfo.Name, attDestMemberName),
                                     ParamOrder = GetParamOrder(MemberInfo.GetCustomAttributes<SouceAttribute>().ToList(), attDestMemberName)
                                 }).ToList();

            return GetAndValidateSourceMembersNames(SourceMembers, attDestMemberName, out methodName);
        }

        private static bool HasSourceName(List<SouceAttribute> sourceAttributes,
                                         IgnoreMemberNameAttribute ignorMemberNameAttribute,
                                         string memberName, string attDestMemberName)
        {
            if (sourceAttributes.Count > 0)
            {
                var FielterdAttributes = (from SourceAttribute in sourceAttributes
                                          where SourceAttribute.DestMemberName == attDestMemberName
                                          select SourceAttribute).ToList();

                if (FielterdAttributes.Count > 1)
                    throw new Exception(string.Format("DeepCopy.HasSourceName - Member name: '{0}' has more then one SourceAttribute with the same DestMemberName: '{1}'",
                                                memberName.ToNullLessString(),
                                                memberName.ToNullLessString()));

                if (FielterdAttributes.Count == 1)
                    return true;

                // Check if there is no source attributes without DestMemberName 
                //if (!sourceAttributes.Exists(x => x.DestMemberName == null))
                //    return false;
            }

            if (ignorMemberNameAttribute != null)
                return false;

            return memberName == attDestMemberName;
        }

        private static string GetMethodName(List<SouceAttribute> sourceAttributes, string memberName, string attDestMemberName)
        {
            if (sourceAttributes.Count == 0)
                return null;
            SouceAttribute AttFound = GetSouceAttribute(sourceAttributes, attDestMemberName);
            if (AttFound == null)
            {
                SouceAttribute AttWithNoDestMemberNameFound = GetSouceAttribute(sourceAttributes, null);
                if (AttWithNoDestMemberNameFound == null)
                    return null;
                else
                    return AttWithNoDestMemberNameFound.MethodName;
            }
            else
                return AttFound.MethodName;
        }

        private static int GetParamOrder(List<SouceAttribute> sourceAttributes, string attDestMemberName)
        {
            if (sourceAttributes.Count == 0)
                return 0;
            SouceAttribute AttFound = GetSouceAttribute(sourceAttributes, attDestMemberName);
            if (AttFound == null)
                return 0;
            else
                return AttFound.ParamOrder;
        }

        private static SouceAttribute GetSouceAttribute(List<SouceAttribute> SourceAttributes, string attDestMemberName)
        {
            return SourceAttributes.Find(x => x.DestMemberName == attDestMemberName);
        }

        private static List<string> GetAndValidateSourceMembersNames(List<SourceMember> sourceMembers, string attDestMemberName, out string methodName)
        {
            List<string> MethodsNames = new List<string>();
            foreach (SourceMember SourceMember in sourceMembers)
            {
                if (SourceMember.ParamOrder > 0 && SourceMember.MethodName == null)
                    throw new Exception(string.Format("DeepCopy.GetAndValidateSourceMembersNames - source member name: '{0}' or destination member name: '{1}' has source\\destination attribute ParamOrder without source\\destination attribute MethodName",
                                SourceMember.MemberName,
                                attDestMemberName.TrimNullLess()));
                if (SourceMember.MethodName != null)
                {
                    if (!MethodsNames.Contains(SourceMember.MethodName))
                        MethodsNames.Add(SourceMember.MethodName);
                }
            }

            // Check if there is more then one source member 
            if (sourceMembers.Count > 1)
            {
                // Check if there is no methods
                if (MethodsNames.Count == 0)
                    throw new Exception(string.Format("DeepCopy.GetAndValidateSourceMembersNames - No source attribute MethodName found for multiple Source attribute DestMemberName = '{0}'",
                                 attDestMemberName));
                // Check if there is more the one method
                if (MethodsNames.Count > 1)
                    throw new Exception(string.Format("DeepCopy.GetAndValidateSourceMembersNames - Multiple source attribute MethodName found for multiple Source attribute DestMemberName = '{0}'",
                                 attDestMemberName));
            }

            methodName = null;
            if (MethodsNames.Count > 0)
            {
                string TempMethodName = MethodsNames.First();
                // Check if the method name found exists in all source menbers
                if (sourceMembers.Exists(x => x.MethodName != TempMethodName))
                    throw new Exception(string.Format("DeepCopy.GetAndValidateSourceMembersNames - There is at least one source attribute that contains MethodName and other source attribute that dose not contain MethodName or contains diffrent MethodName which is inconsistent for the same DestMemberName = '{0}",
                                  attDestMemberName));
                methodName = TempMethodName;
            }

            var SourceMembersNames = from SourceMember in sourceMembers
                                         //where HasSourceMembersName(TempMethodName, SourceMember)
                                     orderby SourceMember.ParamOrder
                                     select SourceMember.MemberName;

            return SourceMembersNames.ToList();
        }

        public static object RunMethod<T>(string methodName, out bool hasReturnValue, params object[] methodParams) where T : new()
        {
            Type PlatformType = typeof(T);
            ConstructorInfo Constructor = PlatformType.GetConstructor(Type.EmptyTypes);
            object PlatformObject = Constructor.Invoke(new object[] { });

            return RunMethod(PlatformObject, methodName, out hasReturnValue, methodParams);
        }

        public static object RunMethod(object PlatformObject, string methodName, out bool hasReturnValue, params object[] methodParams)
        {
            Type PlatformType = PlatformObject.GetType();

            MethodInfo Method = PlatformType.GetMethod(methodName);
            if (Method == null)
            {
                throw new Exception(string.Format("DeepCopy.RunMethod - Can't run method: '{0}', since it not exists in platform type: '{1}'",
                                                        methodName.ToNullLessString("<NULL>"),
                                                        PlatformType.Name.ToNullLessString("<NULL>")));
            }

            try
            {
                return InvokeMethod(Method, PlatformObject, out hasReturnValue, methodParams);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("DeepCopy.RunMethod - Failed to run method: '{0}' in type: '{1}' since: {2}",
                                                            methodName.ToNullLessString("<NULL>"),
                                                            PlatformType.Name.ToNullLessString("<NULL>"),
                                                            ex.ToString()));
            }
        }

        private static object InvokeMethod(MethodInfo Method, object PlatformObject, out bool hasReturnValue, params object[] methodParams)
        {
            if (Method == null)
                throw new Exception("DeepCopy.InvokeMethod - Can't invoke null method");

            hasReturnValue = Method.ReturnType != typeof(void);
            try
            {
                return Method.Invoke(PlatformObject, methodParams);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("DeepCopy.InvokeMethod - Failed to run method since: {0}",
                                                            (ex.InnerException == null) ? ex.ToString() : ex.InnerException.ToString()));
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field,
                           AllowMultiple = true)]
    public class SouceAttribute : Attribute
    {
        public string DestMemberName { get; set; }
        public string MethodName { get; set; }
        public int ParamOrder { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field,
                       AllowMultiple = true)]
    public class DestinationAttribute : Attribute
    {
        public string SourceMemberName { get; set; }
        public string MethodName { get; set; }
        public int ParamOrder { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field,
                   AllowMultiple = false)]
    public class IgnoreMemberNameAttribute : Attribute
    {

    }
}
