using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Eldan.TypeExtensions
{
    public static class ObjectCloneExtensions
    {
        public static bool IsPrimitiveNullble(this object source)
        {
            if (source == null)
                return true;

            return IsPrimitive(source);
        }

        public static bool IsPrimitive(this object source)
        {
            return IsSimpleType(source.GetType());

            //Type T = source.GetType();
            //return (T.IsPrimitive || T.IsValueType || T == typeof(string));
        }

        public static bool IsSimpleType(this Type type)
        {
            return
                type.IsPrimitive ||
                new Type[] {
            typeof(string),
            typeof(decimal),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(TimeSpan),
            typeof(Guid)
                }.Contains(type) ||
                type.IsEnum ||
                Convert.GetTypeCode(type) != TypeCode.Object ||
                (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && IsSimpleType(type.GetGenericArguments()[0]));
        }

        public static bool TryParseType<T>(this string value, out T parsedType)
        {
            Type CurrentType = typeof(T);
            if (CurrentType == typeof(string))
            {
                parsedType = value.CastToReflected(typeof(T));
                return true;
            }
            Type UnderlyingType = Nullable.GetUnderlyingType(CurrentType);
            if (UnderlyingType != null)
            {
                if (value == null)
                {
                    parsedType = GetNullByType<T>();
                    return true;
                }

                CurrentType = UnderlyingType;
            }

            MethodInfo Method;
            if (CurrentType.IsEnum)
            {
                if (value.TryParseType(out int intVal))
                {
                    if (Enum.IsDefined(CurrentType, intVal))
                    {
                        parsedType = intVal.CastToReflected(CurrentType);
                        return true;
                    }
                    else
                    {
                        parsedType = default;
                        return false;
                    }
                }
                else
                {
                    Method = typeof(Enum).GetMethods(BindingFlags.Static | BindingFlags.Public).Single(x => x.Name == "TryParse" &&
                                                                                                            x.IsGenericMethod &&
                                                                                                            x.GetParameters().Length == 3);
                    MethodInfo GMethod = Method.MakeGenericMethod(CurrentType);

                    object[] GMethodParams = { value, true, null };
                    return InvokeTryParse(GMethod, GMethodParams, UnderlyingType != null, CurrentType, out parsedType);
                }
            }

            if (CurrentType == typeof(int) || CurrentType == typeof(uint) ||
                CurrentType == typeof(long) || CurrentType == typeof(ulong) ||
                CurrentType == typeof(short) || CurrentType == typeof(ushort))
            {
                value = value.GetWholeNumber();
            }

            Method = CurrentType.GetMethod("TryParse",
                      BindingFlags.Static | BindingFlags.Public,
                      null,
                      new Type[] { typeof(string), CurrentType.MakeByRefType() },
                      null);

            object[] MethodParams = { value, null };
            return InvokeTryParse(Method, MethodParams, UnderlyingType != null, CurrentType, out parsedType);
        }

        private static bool InvokeTryParse<T>(MethodInfo method, object[] methodParams, bool supportNullble, Type currentType, out T retVal)
        {
            bool Res = (bool)InvokeMethod(method, null, out _, methodParams);

            if (!Res && supportNullble)
                retVal = GetNullByType<T>();
            else
                retVal = methodParams[methodParams.Length - 1].CastToReflected(currentType);

            return Res;
        }

        private static T GetNullByType<T>()
        {
            object ob = null;
            return ob.CastToReflected(typeof(T));
        }

        private static string GetWholeNumber(this string value)
        {
            if (decimal.TryParse(value, out _))
            {
                string[] NumParts = value.Split('.');
                if (NumParts.Length > 1)
                {
                    if (long.Parse(NumParts[1]) == 0)
                        return NumParts[0];
                }
            }

            return value;
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
            if (PlatformObject == null)
                throw new Exception("ObjectCloneExtensions.RunMethod - PlatformObject must not be null");

            Type PlatformType = PlatformObject.GetType();

            MethodInfo Method = PlatformType.GetMethod(methodName);
            if (Method == null)
            {
                throw new Exception(string.Format("ObjectCloneExtensions.RunMethod - Can't run method: '{0}', since it not exists in platform type: '{1}'",
                                                        methodName.ToNullLessString("<NULL>"),
                                                        PlatformType.Name.ToNullLessString("<NULL>")));
            }

            try
            {
                return InvokeMethod(Method, PlatformObject, out hasReturnValue, methodParams);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("ObjectCloneExtensions.RunMethod - Failed to run method: '{0}' in type: '{1}' since: {2}",
                                                            methodName.ToNullLessString("<NULL>"),
                                                            PlatformType.Name.ToNullLessString("<NULL>"),
                                                            ex.ToString()));
            }
        }

        private static object InvokeMethod(MethodInfo Method, object PlatformObject, out bool hasReturnValue, params object[] methodParams)
        {
            if (Method == null)
                throw new Exception("ObjectCloneExtensions.InvokeMethod - Can't invoke null method");

            hasReturnValue = Method.ReturnType != typeof(void);
            try
            {
                return Method.Invoke(PlatformObject, methodParams);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("ObjectCloneExtensions.InvokeMethod - Failed to run method since: {0}",
                                                            (ex.InnerException == null) ? ex.ToString() : ex.InnerException.ToString()));
            }
        }
    }

    public static class ObjectCast
    {
        public static T CastTo<T>(this object o) => (T)o;

        public static dynamic CastToReflected(this object o, Type type)
        {
            var methodInfo = typeof(ObjectCast).GetMethod(nameof(CastTo), BindingFlags.Static | BindingFlags.Public);
            var genericArguments = new[] { type };
            var genericMethodInfo = methodInfo?.MakeGenericMethod(genericArguments);
            return genericMethodInfo?.Invoke(null, new[] { o });
        }
    }
}
