using Eldan.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Eldan.TypeExtensions;
using System.Configuration;
using System.Xml.Linq;

namespace Eldan.Logger
{
    public delegate TResult Func<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, in T16, in T17, in T18, in T19, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19);

    public delegate TResult Func<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, in T16, in T17, in T18, in T19, in T20,
                                 in T21, in T22, in T23, in T24, in T25, in T26, in T27, in T28, in T29, in T30, in T31, in T32, in T33, in T34, in T35, in T36, in T37, in T38, in T39, in T40,
                                 in T41, in T42, in T43, in T44, in T45, in T46, in T47, in T48, in T49, in T50, in T51, in T52, in T53, in T54, in T55, out TResult>(
                                 T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, 
                                 T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20,
                                 T21 arg21, T22 arg22, T23 arg23, T24 arg24, T25 arg25, T26 arg26, T27 arg27, T28 arg28, T29 arg29, T30 arg30,
                                 T31 arg31, T32 arg32, T33 arg33, T34 arg34, T35 arg35, T36 arg36, T37 arg37, T38 arg38, T39 arg39, T40 arg40,
                                 T41 arg41, T42 arg42, T43 arg43, T44 arg44, T45 arg45, T46 arg46, T47 arg47, T48 arg48, T49 arg49, T50 arg50,
                                 T51 arg51, T52 arg52, T53 arg53, T54 arg54, T55 arg55);
    public class ServiceLogger
    {
        private enum enmMaskReturnValueType
        {
            None,
            All,
            ByMethod,
            AllExcept
        }

        private const string MASKED_VALUE = "(Value Masked)";

        private List<string> m_MaskMethodsNames;
        private List<string> m_MaskAllMethodsNamesExcept;
        private enmMaskReturnValueType m_MaskReturnValue;
        private List<string> m_MaskParamsNames;
        private IServiceLoggerEvents m_ServiceLoggerEvents;
        private List<Exception> m_ExceptionsToAvoid;
        private bool m_AddEldanIDInErrorMessage;
        private int? m_MaxChars;
        private string m_SessionID = "<Empty>";

        public ServiceLogger() : this("EldanService")
        {
        }

        public ServiceLogger(string serviceName)
        {
            m_ServiceName = serviceName;
            m_ThrowExceptionWhenError = true;
            m_LoggerFilePathKey = clsLogger.DEFAULT_FILE_PATH_KEY;
            //m_SerilizeParamSetBy = enmSerilizerType.JSONtonsoft;

            m_SessionID = UniqueIdGenerator.GenerateUniqueId();

            ClearMask();
            ClearExceptionsToAvoid();

            int MaxChars;
            if (int.TryParse(ConfigurationManager.AppSettings["serviceLoggerMaxChars"], out MaxChars))
                if (MaxChars < 0)
                    m_MaxChars = null;
                else
                    m_MaxChars = MaxChars;
            else
                m_MaxChars = null;

            if (!bool.TryParse(ConfigurationManager.AppSettings["serviceLoggerAddEldanIDInErrorMessage"], out m_AddEldanIDInErrorMessage))
                m_AddEldanIDInErrorMessage = false;

            string Methods = ConfigurationManager.AppSettings["serviceLoggerMaskMethodsReturnValue"];
            m_MaskReturnValue = enmMaskReturnValueType.ByMethod;
            if (Methods == null)
                m_MaskReturnValue = enmMaskReturnValueType.None;
            else
            {
                if (Methods.ToLower() == "[" + enmMaskReturnValueType.None.ToString().ToLower() + "]")
                    m_MaskReturnValue = enmMaskReturnValueType.None;
                if (Methods.ToLower() == "[" + enmMaskReturnValueType.All.ToString().ToLower() + "]")
                    m_MaskReturnValue = enmMaskReturnValueType.All;

                int AllExceptLength = enmMaskReturnValueType.AllExcept.ToString().Length + 2;

                if (Methods.ToLower().Left(AllExceptLength) == "[" + enmMaskReturnValueType.AllExcept.ToString().ToLower() + "]")
                {
                    m_MaskReturnValue = enmMaskReturnValueType.AllExcept;
                    m_MaskAllMethodsNamesExcept = Methods.Right(Methods.Length - AllExceptLength).Split(',').ToList();
                }
                if (m_MaskReturnValue == enmMaskReturnValueType.ByMethod)
                {
                    m_MaskMethodsNames = Methods.Split(',').ToList();
                }

            }

            string MaskParams = ConfigurationManager.AppSettings["serviceLoggerMaskParams"];

            if (!string.IsNullOrWhiteSpace(MaskParams))
                m_MaskParamsNames = MaskParams.Split(',').Select(x => x.Trim()).ToList();
        }

        public string SessionID
        {
            get { return m_SessionID; }
        }

        public bool ThrowFullError
        {
            get { return m_AddEldanIDInErrorMessage; }
        }

        private string m_ServiceName;

        public string ServiceName
        {
            get { return m_ServiceName; }
            set { m_ServiceName = value; }
        }

        private string m_LoggerFilePathKey;

        public string LoggerFilePathKey
        {
            get { return m_LoggerFilePathKey; }
            set { m_LoggerFilePathKey = value; }
        }

        private bool m_ThrowExceptionWhenError;

        public bool ThrowExceptionWhenError
        {
            get { return m_ThrowExceptionWhenError; }
            set { m_ThrowExceptionWhenError = value; }
        }

        public enum enmSerilizerType
        {
            JSON,
            JSONtonsoft,
            XML
        }

        private enmSerilizerType m_SerilizeParamSetBy = enmSerilizerType.JSONtonsoft;

        public enmSerilizerType SerilizeParamSetBy
        {
            get { return m_SerilizeParamSetBy; }
            set { m_SerilizeParamSetBy = value; }
        }

        private bool m_LogCreateInstance = true;

        public bool LogCreateInstance
        {
            get { return m_LogCreateInstance; }
            set { m_LogCreateInstance = value; }
        }


        public void MaskReturnValue()
        {
            m_MaskReturnValue = enmMaskReturnValueType.All;
        }

        public void MaskReturnValue(params string[] MethodsNames)
        {
            m_MaskReturnValue = enmMaskReturnValueType.ByMethod;
            m_MaskMethodsNames = MethodsNames.ToList();
        }

        public void MaskReturnValueExcept(params string[] MethodsNames)
        {
            m_MaskReturnValue = enmMaskReturnValueType.AllExcept;
            m_MaskAllMethodsNamesExcept = MethodsNames.ToList();
        }

        public void MaskParameters(params string[] ParamsNames)
        {
            m_MaskParamsNames = ParamsNames.ToList();
        }

        public void ClearMask()
        {
            m_MaskReturnValue = enmMaskReturnValueType.None;
            m_MaskParamsNames = new List<string>();
        }

        public void AvoidExceptions(params Exception[] ExceptionsToAvoid)
        {
            m_ExceptionsToAvoid = ExceptionsToAvoid.ToList();
        }

        public void ClearExceptionsToAvoid()
        {
            m_ExceptionsToAvoid = new List<Exception>();
        }


        public void BindEvents(IServiceLoggerEvents ServiceLoggerEvents)
        {
            m_ServiceLoggerEvents = ServiceLoggerEvents;
        }

        public T CreateInstance<T>() where T : new()
        {
            T Instance = default(T);

            try
            {
                if (m_LogCreateInstance)
                    clsLogger.Write(string.Format("{0}.{1}()- started (SessionID = {2})", m_ServiceName, m_ServiceName, m_SessionID), 
                        clsLogger.enmLogType.Information, m_LoggerFilePathKey);
                Instance = new T();
            }
            catch (Exception ex)
            {
                ThrowServiceException(ex, null);
            }

            LogCompleteAction(null);

            return Instance;
        }

        #region DoFunc
        public T DoFunc<T>(Func<T> BLMethod)
        {
            T res = default(T);
            try
            {
                LogMethod(BLMethod.Method);
                res = BLMethod();
            }
            catch (Exception ex)
            {
                ThrowServiceException(ex, BLMethod.Method);
            }

            LogCompleteFunc(BLMethod.Method, res);
            return res;
        }

        public T DoFunc<P1, T>(Func<P1, T> BLMethod, P1 Param1)
        {
            T res = default(T);
            try
            {
                LogMethod(BLMethod.Method, Param1);
                res = BLMethod(Param1);
            }
            catch (Exception ex)
            {
                ThrowServiceException(ex, BLMethod.Method, Param1);
            }

            LogCompleteFunc(BLMethod.Method, res, Param1);
            return res;
        }

        public T DoFunc<P1, P2, T>(Func<P1, P2, T> BLMethod, P1 Param1, P2 Param2)
        {
            T res = default(T);
            try
            {
                LogMethod(BLMethod.Method, Param1, Param2);
                res = BLMethod(Param1, Param2);
            }
            catch (Exception ex)
            {
                ThrowServiceException(ex, BLMethod.Method, Param1, Param2);
            }

            LogCompleteFunc(BLMethod.Method, res, Param1, Param2);
            return res;
        }

        public T DoFunc<P1, P2, P3, T>(Func<P1, P2, P3, T> BLMethod, P1 Param1, P2 Param2, P3 Param3)
        {
            T res = default(T);
            try
            {
                LogMethod(BLMethod.Method, Param1, Param2, Param3);
                res = BLMethod(Param1, Param2, Param3);
            }
            catch (Exception ex)
            {
                ThrowServiceException(ex, BLMethod.Method, Param1, Param2, Param3);
            }

            LogCompleteFunc(BLMethod.Method, res, Param1, Param2, Param3);
            return res;
        }


        public T DoFunc<P1, P2, P3, P4, T>(Func<P1, P2, P3, P4, T> BLMethod, P1 Param1, P2 Param2, P3 Param3, P4 Param4)
        {
            T res = default(T);
            try
            {
                LogMethod(BLMethod.Method, Param1, Param2, Param3, Param4);
                res = BLMethod(Param1, Param2, Param3, Param4);
            }
            catch (Exception ex)
            {
                ThrowServiceException(ex, BLMethod.Method, Param1, Param2, Param3, Param4);
            }

            LogCompleteFunc(BLMethod.Method, res, Param1, Param2, Param3, Param4);
            return res;
        }

        public T DoFunc<P1, P2, P3, P4, P5, T>(Func<P1, P2, P3, P4, P5, T> BLMethod, P1 Param1, P2 Param2, P3 Param3, P4 Param4, P5 Param5)
        {
            T res = default(T);
            try
            {
                LogMethod(BLMethod.Method, Param1, Param2, Param3, Param4, Param5);
                res = BLMethod(Param1, Param2, Param3, Param4, Param5);
            }
            catch (Exception ex)
            {
                ThrowServiceException(ex, BLMethod.Method, Param1, Param2, Param3, Param4, Param5);
            }

            LogCompleteFunc(BLMethod.Method, res, Param1, Param2, Param3, Param4, Param5);
            return res;
        }

        public T DoFunc<P1, P2, P3, P4, P5, P6, T>(Func<P1, P2, P3, P4, P5, P6, T> BLMethod, P1 Param1, P2 Param2, P3 Param3, P4 Param4, P5 Param5, P6 Param6)
        {
            T res = default(T);
            try
            {
                LogMethod(BLMethod.Method, Param1, Param2, Param3, Param4, Param5, Param6);
                res = BLMethod(Param1, Param2, Param3, Param4, Param5, Param6);
            }
            catch (Exception ex)
            {
                ThrowServiceException(ex, BLMethod.Method, Param1, Param2, Param3, Param4, Param5, Param6);
            }

            LogCompleteFunc(BLMethod.Method, res, Param1, Param2, Param3, Param4, Param5, Param6);
            return res;
        }

        public T DoFunc<P1, P2, P3, P4, P5, P6, P7, T>(Func<P1, P2, P3, P4, P5, P6, P7, T> BLMethod, P1 Param1, P2 Param2, P3 Param3, P4 Param4, P5 Param5, P6 Param6, P7 Param7)
        {
            T res = default(T);
            try
            {
                LogMethod(BLMethod.Method, Param1, Param2, Param3, Param4, Param5, Param6, Param7);
                res = BLMethod(Param1, Param2, Param3, Param4, Param5, Param6, Param7);
            }
            catch (Exception ex)
            {
                ThrowServiceException(ex, BLMethod.Method, Param1, Param2, Param3, Param4, Param5, Param6, Param7);
            }

            LogCompleteFunc(BLMethod.Method, res, Param1, Param2, Param3, Param4, Param5, Param6, Param7);
            return res;
        }

        public T DoFunc<P1, P2, P3, P4, P5, P6, P7, P8, T>(Func<P1, P2, P3, P4, P5, P6, P7, P8, T> BLMethod, P1 Param1, P2 Param2, P3 Param3, P4 Param4, P5 Param5, P6 Param6, P7 Param7, P8 Param8)
        {
            T res = default(T);
            try
            {
                LogMethod(BLMethod.Method, Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8);
                res = BLMethod(Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8);
            }
            catch (Exception ex)
            {
                ThrowServiceException(ex, BLMethod.Method, Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8);
            }

            LogCompleteFunc(BLMethod.Method, res, Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8);
            return res;
        }

        public T DoFunc<P1, P2, P3, P4, P5, P6, P7, P8, P9, T>(Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, T> BLMethod, P1 Param1, P2 Param2, P3 Param3, P4 Param4, P5 Param5, P6 Param6, P7 Param7, P8 Param8, P9 Param9)
        {
            T res = default(T);
            try
            {
                LogMethod(BLMethod.Method, Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9);
                res = BLMethod(Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9);
            }
            catch (Exception ex)
            {
                ThrowServiceException(ex, BLMethod.Method, Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9);
            }

            LogCompleteFunc(BLMethod.Method, res, Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9);
            return res;
        }

        public T DoFunc<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, T>(Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, T> BLMethod, P1 Param1, P2 Param2, P3 Param3, P4 Param4, P5 Param5, P6 Param6, P7 Param7, P8 Param8, P9 Param9, P10 Param10)
        {
            T res = default(T);
            try
            {
                LogMethod(BLMethod.Method, Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9, Param10);
                res = BLMethod(Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9, Param10);
            }
            catch (Exception ex)
            {
                ThrowServiceException(ex, BLMethod.Method, Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9, Param10);
            }

            LogCompleteFunc(BLMethod.Method, res, Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9, Param10);
            return res;
        }

        public T DoFunc<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, T>(Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, T> BLMethod, P1 Param1, P2 Param2, P3 Param3, P4 Param4, P5 Param5, P6 Param6, P7 Param7, P8 Param8, P9 Param9, P10 Param10, P11 Param11)
        {
            T res = default(T);
            try
            {
                LogMethod(BLMethod.Method, Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9, Param10, Param11);
                res = BLMethod(Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9, Param10, Param11);
            }
            catch (Exception ex)
            {
                ThrowServiceException(ex, BLMethod.Method, Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9, Param10, Param11);
            }

            LogCompleteFunc(BLMethod.Method, res, Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9, Param10, Param11);
            return res;
        }

        public T DoFunc<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, T>(Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, T> BLMethod, P1 Param1, P2 Param2, P3 Param3, P4 Param4, P5 Param5, P6 Param6, P7 Param7, P8 Param8, P9 Param9, P10 Param10, P11 Param11, P12 Param12)
        {
            T res = default(T);
            try
            {
                LogMethod(BLMethod.Method, Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9, Param10, Param11, Param12);
                res = BLMethod(Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9, Param10, Param11, Param12);
            }
            catch (Exception ex)
            {
                ThrowServiceException(ex, BLMethod.Method, Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9, Param10, Param11, Param12);
            }

            LogCompleteFunc(BLMethod.Method, res, Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9, Param10, Param11, Param12);
            return res;
        }


        public T DoFunc<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, T>(Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, T> BLMethod, P1 Param1, P2 Param2, P3 Param3, P4 Param4, P5 Param5, P6 Param6, P7 Param7, P8 Param8, P9 Param9, P10 Param10, P11 Param11, P12 Param12, P13 Param13)
        {
            T res = default(T);
            try
            {
                LogMethod(BLMethod.Method, Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9, Param10, Param11, Param12, Param13);
                res = BLMethod(Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9, Param10, Param11, Param12, Param13);
            }
            catch (Exception ex)
            {
                ThrowServiceException(ex, BLMethod.Method, Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9, Param10, Param11, Param12, Param13);
            }

            LogCompleteFunc(BLMethod.Method, res, Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9, Param10, Param11, Param12, Param13);
            return res;
        }

        public T DoFunc<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, P16, P17, P18, P19, T>(Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, P16, P17, P18, P19, T> BLMethod, P1 Param1, P2 Param2, P3 Param3, P4 Param4, P5 Param5, P6 Param6, P7 Param7, P8 Param8, P9 Param9, P10 Param10, P11 Param11, P12 Param12, P13 Param13, P14 Param14, P15 Param15, P16 Param16, P17 Param17, P18 Param18, P19 Param19)
        {
            T res = default(T);
            try
            {
                LogMethod(BLMethod.Method, Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9, Param10, Param11, Param12, Param13, Param14, Param15, Param16, Param17, Param18, Param19);
                res = BLMethod(Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9, Param10, Param11, Param12, Param13, Param14, Param15, Param16, Param17, Param18, Param19);
            }
            catch (Exception ex)
            {
                ThrowServiceException(ex, BLMethod.Method, Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9, Param10, Param11, Param12, Param13, Param14, Param15, Param16, Param17, Param18, Param19);
            }

            LogCompleteFunc(BLMethod.Method, res, Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9, Param10, Param11, Param12, Param13, Param14, Param15, Param16, Param17, Param18, Param19);
            return res;
        }

        public T DoFunc<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, P16, P17, P18, P19, P20,
                        P21, P22, P23, P24, P25, P26, P27, P28, P29, P30, P31, P32, P33, P34, P35, P36, P37, P38, P39, P40,
                        P41, P42, P43, P44, P45, P46, P47, P48, P49, P50, P51, P52, P53, P54, P55, T>(
                                Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, P16, P17, P18, P19, P20,
                                     P21, P22, P23, P24, P25, P26, P27, P28, P29, P30, P31, P32, P33, P34, P35, P36, P37, P38, P39, P40,
                                     P41, P42, P43, P44, P45, P46, P47, P48, P49, P50, P51, P52, P53, P54, P55, T> BLMethod, 
                                P1 Param1, P2 Param2, P3 Param3, P4 Param4, P5 Param5, P6 Param6, P7 Param7, P8 Param8, P9 Param9, P10 Param10, 
                                P11 Param11, P12 Param12, P13 Param13, P14 Param14, P15 Param15, P16 Param16, P17 Param17, P18 Param18, P19 Param19, P20 Param20,
                                P21 Param21, P22 Param22, P23 Param23, P24 Param24, P25 Param25, P26 Param26, P27 Param27, P28 Param28, P29 Param29, P30 Param30,
                                P31 Param31, P32 Param32, P33 Param33, P34 Param34, P35 Param35, P36 Param36, P37 Param37, P38 Param38, P39 Param39, P40 Param40,
                                P41 Param41, P42 Param42, P43 Param43, P44 Param44, P45 Param45, P46 Param46, P47 Param47, P48 Param48, P49 Param49, P50 Param50,
                                P51 Param51, P52 Param52, P53 Param53, P54 Param54, P55 Param55)
        {
            T res = default(T);

            List<object> Params = new List<object>();

            Params.Add(Param1);
            Params.Add(Param2);
            Params.Add(Param3);
            Params.Add(Param4);
            Params.Add(Param5);
            Params.Add(Param6);
            Params.Add(Param7);
            Params.Add(Param8);
            Params.Add(Param9);
            Params.Add(Param10);
            Params.Add(Param11);
            Params.Add(Param12);
            Params.Add(Param13);
            Params.Add(Param14);
            Params.Add(Param15);
            Params.Add(Param16);
            Params.Add(Param17);
            Params.Add(Param18);
            Params.Add(Param19);
            Params.Add(Param20);
            Params.Add(Param21);
            Params.Add(Param22);
            Params.Add(Param23);
            Params.Add(Param24);
            Params.Add(Param25);
            Params.Add(Param26);
            Params.Add(Param27);
            Params.Add(Param28);
            Params.Add(Param29);
            Params.Add(Param30);
            Params.Add(Param31);
            Params.Add(Param32);
            Params.Add(Param33);
            Params.Add(Param34);
            Params.Add(Param35);
            Params.Add(Param36);
            Params.Add(Param37);
            Params.Add(Param38);
            Params.Add(Param39);
            Params.Add(Param40);
            Params.Add(Param41);
            Params.Add(Param42);
            Params.Add(Param43);
            Params.Add(Param44);
            Params.Add(Param45);
            Params.Add(Param46);
            Params.Add(Param47);
            Params.Add(Param48);
            Params.Add(Param49);
            Params.Add(Param50);
            Params.Add(Param51);
            Params.Add(Param52);
            Params.Add(Param53);
            Params.Add(Param54);
            Params.Add(Param55);

            try
            {
                LogMethod(BLMethod.Method, Params.ToArray());
                res = BLMethod(Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9, Param10,
                                Param11, Param12, Param13, Param14, Param15, Param16, Param17, Param18, Param19, Param20,
                                Param21, Param22, Param23, Param24, Param25, Param26, Param27, Param28, Param29, Param30,
                                Param31, Param32, Param33, Param34, Param35, Param36, Param37, Param38, Param39, Param40,
                                Param41, Param42, Param43, Param44, Param45, Param46, Param47, Param48, Param49, Param50,
                                Param51, Param52, Param53, Param54, Param55);
            }
            catch (Exception ex)
            {
                ThrowServiceException(ex, BLMethod.Method, Params.ToArray());
            }

            LogCompleteFunc(BLMethod.Method, res, Params.ToArray());
            return res;
        }
        #endregion // DoFunc

        #region DoAction
        public void DoAction(Action BLMethod)
        {
            try
            {
                LogMethod(BLMethod.Method);
                BLMethod();
            }
            catch (Exception ex)
            {
                ThrowServiceException(ex, BLMethod.Method);
            }

            LogCompleteAction(BLMethod.Method);
        }

        public void DoAction<P1>(Action<P1> BLMethod, P1 Param1)
        {
            try
            {
                LogMethod(BLMethod.Method, Param1);
                BLMethod(Param1);
            }
            catch (Exception ex)
            {
                ThrowServiceException(ex, BLMethod.Method, Param1);
            }

            LogCompleteAction(BLMethod.Method, Param1);
        }

        public void DoAction<P1, P2>(Action<P1, P2> BLMethod, P1 Param1, P2 Param2)
        {
            try
            {
                LogMethod(BLMethod.Method, Param1, Param2);
                BLMethod(Param1, Param2);
            }
            catch (Exception ex)
            {
                ThrowServiceException(ex, BLMethod.Method, Param1, Param2);
            }

            LogCompleteAction(BLMethod.Method, Param1, Param2);
        }

        public void DoAction<P1, P2, P3>(Action<P1, P2, P3> BLMethod, P1 Param1, P2 Param2, P3 Param3)
        {
            try
            {
                LogMethod(BLMethod.Method, Param1, Param2, Param3);
                BLMethod(Param1, Param2, Param3);
            }
            catch (Exception ex)
            {
                ThrowServiceException(ex, BLMethod.Method, Param1, Param2, Param3);
            }

            LogCompleteAction(BLMethod.Method, Param1, Param2, Param3);
        }

        public void DoAction<P1, P2, P3, P4>(Action<P1, P2, P3, P4> BLMethod, P1 Param1, P2 Param2, P3 Param3, P4 Param4)
        {
            try
            {
                LogMethod(BLMethod.Method, Param1, Param2, Param3, Param4);
                BLMethod(Param1, Param2, Param3, Param4);
            }
            catch (Exception ex)
            {
                ThrowServiceException(ex, BLMethod.Method, Param1, Param2, Param3, Param4);
            }

            LogCompleteAction(BLMethod.Method, Param1, Param2, Param3, Param4);
        }

        public void DoAction<P1, P2, P3, P4, P5>(Action<P1, P2, P3, P4, P5> BLMethod, P1 Param1, P2 Param2, P3 Param3, P4 Param4, P5 Param5)
        {
            try
            {
                LogMethod(BLMethod.Method, Param1, Param2, Param3, Param4, Param5);
                BLMethod(Param1, Param2, Param3, Param4, Param5);
            }
            catch (Exception ex)
            {
                ThrowServiceException(ex, BLMethod.Method, Param1, Param2, Param3, Param4, Param5);
            }

            LogCompleteAction(BLMethod.Method, Param1, Param2, Param3, Param4, Param5);
        }

        public void DoAction<P1, P2, P3, P4, P5, P6>(Action<P1, P2, P3, P4, P5, P6> BLMethod, P1 Param1, P2 Param2, P3 Param3, P4 Param4, P5 Param5, P6 Param6)
        {
            try
            {
                LogMethod(BLMethod.Method, Param1, Param2, Param3, Param4, Param5, Param6);
                BLMethod(Param1, Param2, Param3, Param4, Param5, Param6);
            }
            catch (Exception ex)
            {
                ThrowServiceException(ex, BLMethod.Method, Param1, Param2, Param3, Param4, Param5, Param6);
            }

            LogCompleteAction(BLMethod.Method, Param1, Param2, Param3, Param4, Param5, Param6);
        }

        public void DoAction<P1, P2, P3, P4, P5, P6, P7>(Action<P1, P2, P3, P4, P5, P6, P7> BLMethod, P1 Param1, P2 Param2, P3 Param3, P4 Param4, P5 Param5, P6 Param6, P7 Param7)
        {
            try
            {
                LogMethod(BLMethod.Method, Param1, Param2, Param3, Param4, Param5, Param6, Param7);
                BLMethod(Param1, Param2, Param3, Param4, Param5, Param6, Param7);
            }
            catch (Exception ex)
            {
                ThrowServiceException(ex, BLMethod.Method, Param1, Param2, Param3, Param4, Param5, Param6, Param7);
            }

            LogCompleteAction(BLMethod.Method, Param1, Param2, Param3, Param4, Param5, Param6, Param7);
        }

        public void DoAction<P1, P2, P3, P4, P5, P6, P7, P8>(Action<P1, P2, P3, P4, P5, P6, P7, P8> BLMethod, P1 Param1, P2 Param2, P3 Param3, P4 Param4, P5 Param5, P6 Param6, P7 Param7, P8 Param8)
        {
            try
            {
                LogMethod(BLMethod.Method, Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8);
                BLMethod(Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8);
            }
            catch (Exception ex)
            {
                ThrowServiceException(ex, BLMethod.Method, Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8);
            }

            LogCompleteAction(BLMethod.Method, Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8);
        }

        public void DoAction<P1, P2, P3, P4, P5, P6, P7, P8, P9>(Action<P1, P2, P3, P4, P5, P6, P7, P8, P9> BLMethod, P1 Param1, P2 Param2, P3 Param3, P4 Param4, P5 Param5, P6 Param6, P7 Param7, P8 Param8, P9 Param9)
        {
            try
            {
                LogMethod(BLMethod.Method, Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9);
                BLMethod(Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9);
            }
            catch (Exception ex)
            {
                ThrowServiceException(ex, BLMethod.Method, Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9);
            }

            LogCompleteAction(BLMethod.Method, Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9);
        }

        public void DoAction<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(Action<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10> BLMethod, P1 Param1, P2 Param2, P3 Param3, P4 Param4, P5 Param5, P6 Param6, P7 Param7, P8 Param8, P9 Param9, P10 Param10)
        {
            try
            {
                LogMethod(BLMethod.Method, Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9, Param10);
                BLMethod(Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9, Param10);
            }
            catch (Exception ex)
            {
                ThrowServiceException(ex, BLMethod.Method, Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9, Param10);
            }

            LogCompleteAction(BLMethod.Method, Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9, Param10);
        }

        public void DoAction<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(Action<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11> BLMethod, P1 Param1, P2 Param2, P3 Param3, P4 Param4, P5 Param5, P6 Param6, P7 Param7, P8 Param8, P9 Param9, P10 Param10, P11 Param11)
        {
            try
            {
                LogMethod(BLMethod.Method, Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9, Param10, Param11);
                BLMethod(Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9, Param10, Param11);
            }
            catch (Exception ex)
            {
                ThrowServiceException(ex, BLMethod.Method, Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9, Param10, Param11);
            }

            LogCompleteAction(BLMethod.Method, Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9, Param10, Param11);
        }

        #endregion // DoAction

        private void LogCompleteAction(MethodInfo Method, params object[] Params)
        {
            string MethodName = m_ServiceName;

            bool LogComplete = true;
            if (Method == null && !m_LogCreateInstance)
                LogComplete = false;

            if (Method != null)
                MethodName = Method.Name;
            if (LogComplete)
                clsLogger.Write(string.Format("{0}.{1} - completed (SessionID = {2})", m_ServiceName, MethodName, m_SessionID), 
                    clsLogger.enmLogType.Information, m_LoggerFilePathKey);

            if (m_ServiceLoggerEvents != null)
            {
                if (Method == null)
                    m_ServiceLoggerEvents.logCreateBL(m_ServiceName);
                else
                    m_ServiceLoggerEvents.LogMethodEnd(m_ServiceName, Method, Params, null);
            }
        }

        private void LogCompleteFunc(MethodInfo Method, object RetrunValue, params object[] Params)
        {
            string Value = "";
            switch (m_MaskReturnValue)
            {
                case enmMaskReturnValueType.None:
                    Value = GetParamText(RetrunValue);
                    break;
                case enmMaskReturnValueType.All:
                    Value = MASKED_VALUE;
                    break;
                case enmMaskReturnValueType.ByMethod:
                    if (m_MaskMethodsNames.Exists(x => x.Trim() == Method.Name))
                        Value = MASKED_VALUE;
                    else
                        Value = GetParamText(RetrunValue);
                    break;
                case enmMaskReturnValueType.AllExcept:
                    if (m_MaskAllMethodsNamesExcept.Exists(x => x.Trim() == Method.Name))
                        Value = GetParamText(RetrunValue);
                    else
                        Value = MASKED_VALUE;
                    break;
                default:
                    break;
            }

            clsLogger.Write(string.Format("{0}.{1} - completed (SessionID = {2}), returns: {3}", m_ServiceName, Method.Name, m_SessionID, Value), 
                clsLogger.enmLogType.Information, m_LoggerFilePathKey);

            m_ServiceLoggerEvents?.LogMethodEnd(m_ServiceName, Method, Params, RetrunValue);
        }

        private void ThrowServiceException(Exception ex, MethodInfo Method, params object[] Params)
        {
            EldanError exEldan = new EldanError(ex);

            clsLogger.Write(string.Format("{0}.{1}", m_ServiceName, (Method == null) ? "<Constructor>" : Method.Name),
                            m_AddEldanIDInErrorMessage ? exEldan.GetExtendedException() : ex,
                            m_LoggerFilePathKey);

            if (m_ServiceLoggerEvents != null)
                m_ServiceLoggerEvents.LogMethodException(m_ServiceName,
                                                         Method,
                                                         Params,
                                                         m_AddEldanIDInErrorMessage ? exEldan.GetExtendedException() : exEldan.GetExtendedException(false));

            if (m_ThrowExceptionWhenError)
            {
                if (!m_ExceptionsToAvoid.Any(x => x.GetType() == ex.GetType()))
                    throw GetExceptionToThrow(exEldan);
            }
        }

        private void LogMethod(MethodInfo Method, params object[] Params)
        {
            string ParamsText = "";
            ParameterInfo[] MethodParamters = Method.GetParameters();

            for (int i = 0; i < Params.Length; i++)
            {
                ParamsText += MethodParamters[i].Name + ":=" + GetParamText(Method.Name, MethodParamters[i].Name, Params[i]) + ", ";
            }

            ParamsText = ParamsText.Left(ParamsText.Length - 2);

            clsLogger.Write(string.Format("{0}.{1}({2}) - started (SessionID = {3})", m_ServiceName, Method.Name, ParamsText, m_SessionID), 
                clsLogger.enmLogType.Information, m_LoggerFilePathKey);

            m_ServiceLoggerEvents?.LogMethodStart(m_ServiceName, Method, Params);
        }

        private string GetParamText(string methodName, string paramName, object paramValue)
        {
            string SubParam = m_MaskParamsNames.Find(x => IsParamExists(x, methodName, paramName) || IsParamExists(x, paramName));
            if (SubParam != null)
                return GetMaskedValue(paramValue, SubParam);

            if (paramValue != null && m_MaskParamsNames.Count > 0)
            {
                XDocument Doc;
                if (paramValue.GetType() == typeof(string))
                {
                    if (TryParseXML(paramValue.ToString(), out Doc))
                    {
                        return "'" + GetMaskedParamFieldValue(methodName, paramName, Doc, enmSerilizerType.XML) + "'";
                    }
                }
                if (!paramValue.IsPrimitive())
                {
                    Doc = XDocument.Parse(paramValue.ToXML());
                    return "'" + GetMaskedParamFieldValue(methodName, paramName, Doc, m_SerilizeParamSetBy) + "'";
                }
            }

            return "'" + GetParamText(paramValue) + "'";
        }

        private bool TryParseXML(string text, out XDocument doc)
        {
            doc = null;
            try
            {
                doc = XDocument.Parse(text);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private string GetMaskedParamFieldValue(string methodName, string paramName, XDocument doc, enmSerilizerType serilizeParamSetBy)
        {
            foreach (string MaskParamName in m_MaskParamsNames)
            {
                string FiledName = GetFieldName(MaskParamName);
                if (FiledName == null)
                {
                    if (MaskParamName.IndexOf('.') > -1)
                        continue;
                    else
                        FiledName = MaskParamName;
                }
                else
                {
                    if (!IsParamExists(MaskParamName, methodName, paramName, FiledName.Split('[')[0]))
                        continue;
                }

                GetSubParamParts(FiledName, out string SubParamName, out int rightFreeCharacters);

                var Nodes = doc.Descendants().Where(x => string.Compare(x.Name.LocalName, SubParamName,
                                                                            StringComparison.OrdinalIgnoreCase) == 0); // .Descendants(SubParamName);

                foreach (var Node in Nodes)
                {
                    Node.Value = Node.Value.Trim().MaskParam(rightFreeCharacters, MASKED_VALUE);
                }           
            }

            if (serilizeParamSetBy == enmSerilizerType.XML)
                return doc.ToString();
            else
                return doc.ToJSON();
        }
        
        private string GetFieldName(string maskParamName)
        {
            string[] ParamParts = maskParamName.Split('.');
            if (ParamParts.Length != 3)
                return null;

            string[] FieldParts = ParamParts[2].Split('[');

            return string.IsNullOrWhiteSpace(FieldParts[0]) ? null : ParamParts[2];
        }

        private string GetMaskedValue(object ParamValue, string SubParam)
        {
            if (GetSubParamParts(SubParam, out int rightFreeCharacters))
            {
                string ParamText = GetParamText(ParamValue);
                ParamText = ParamText.MaskParam(rightFreeCharacters, MASKED_VALUE);
                return "'" + ParamText + "'";
            }
            else
                return MASKED_VALUE;
        }

        private string GetParamText(object ParamValue)
        {
            if (ParamValue == null)
                return "null";

            string Value = null;

            if (ParamValue.IsPrimitive())
                Value = ParamValue.ToNullLessString("null");
            else
            {
                switch (m_SerilizeParamSetBy)
                {
                    case enmSerilizerType.JSON:
                        Value = ParamValue.ToJSONScriptSerializer();
                        break;
                    case enmSerilizerType.JSONtonsoft:
                        Value = ParamValue.ToJSON();
                        break;
                    case enmSerilizerType.XML:
                        Value = ParamValue.ToXML();
                        break;
                    default:
                        break;
                }
            }

            if (m_MaxChars.HasValue)
                return Value.GetMaxText(m_MaxChars.Value);
            else
                return Value;
        }

        //private string GetParamText(object ParamValue, List<string> SubParams)
        //{
        //    string ParamText = GetParamText(ParamValue);

        //    string SubParamName;
        //    int rightFreeCharacters;

        //    foreach (string SubParam in SubParams)
        //    {
        //        GetSubParamParts(SubParam, out SubParamName, out rightFreeCharacters);
        //        ParamText = ParamText.MaskJSONParam(SubParamName, rightFreeCharacters);
        //    }

        //    return ParamText;

        //}

        private bool GetSubParamParts(string SubParam, out int rightFreeCharacters)
        {
            string Dummy;
            return GetSubParamParts(SubParam, out Dummy, out rightFreeCharacters);
        }

        private bool GetSubParamParts(string SubParam, out string SubParamName, out int rightFreeCharacters)
        {
            string[] SubParamParts = SubParam.Split('.');

            string[] SubParamNameParts = SubParamParts[SubParamParts.Length - 1].Split('[');

            SubParamName = SubParamNameParts[0];

            if (SubParamNameParts.Length == 2)
            {
                rightFreeCharacters = int.Parse(SubParamNameParts[1].Left(SubParamNameParts[1].Length - 1));
                return true;
            }
            else
            {
                rightFreeCharacters = 0;
                return false;
            }
        }

        //private bool IsPrimitiveParam(object ParamValue)
        //{
        //    return ParamValue.IsPrimitive();

        //    //var type = ParamValue.GetType();
        //    //string ParamString = ParamValue.ToString().ToLower();

        //    //return !(ParamString.Contains(type.Name.ToLower()) && ParamString.Contains(type.Namespace.ToLower()));
        //}

        //private bool IsParamExists(string item, string MethodName, string ParamName)
        //{
        //    if (item.IndexOf('.') > -1)
        //        ParamName = MethodName + "." + ParamName;

        //    int Paramlenght = item.IndexOf('[');
        //    if (Paramlenght > -1)
        //        item = item.Left(Paramlenght);

        //    return (string.Compare(item.Trim(), ParamName) == 0);
        //}

        private bool IsParamExists(string item, params string[] fields)
        {
            string ParamName = string.Join(".", fields);

            int Paramlenght = item.IndexOf('[');
            if (Paramlenght > -1)
                item = item.Left(Paramlenght);

            return (string.Compare(item.Trim(), ParamName, StringComparison.OrdinalIgnoreCase) == 0);
        }

        private bool HasSubParam(string item, string MethodName, string ParamName)
        {
            string[] ItemParts = item.Split('.');
            if (ItemParts.Length >= 3)
                return IsParamExists(ItemParts[0] + "." + ItemParts[1], MethodName, ParamName);
            else
                return false;
        }

        public void Write(string Message, clsLogger.enmLogType LogType)
        {
            clsLogger.Write(Message, LogType);
        }

        public void Write(string Message, clsLogger.enmLogType LogType, string FilePathKey)
        {
            clsLogger.Write(Message, LogType, FilePathKey);
        }

        public Exception Write(string Message, Exception ex)
        {
            EldanError exEldan = (EldanError)ex;

            clsLogger.Write(Message, GetExceptionToWrite(exEldan));
            return GetExceptionToThrow(exEldan);
        }

        public Exception Write(string Message, Exception ex, string FilePathKey)
        {
            EldanError exEldan = new EldanError(ex);

            clsLogger.Write(Message, GetExceptionToWrite(exEldan), FilePathKey);
            return GetExceptionToThrow(exEldan);
        }

        public Exception Write(Exception ex)
        {
            EldanError exEldan = new EldanError(ex);

            clsLogger.Write(GetExceptionToWrite(exEldan));
            return GetExceptionToThrow(exEldan);
        }

        public Exception Write(Exception ex, string FilePathKey)
        {
            EldanError exEldan = new EldanError(ex);

            clsLogger.Write(GetExceptionToWrite(exEldan), FilePathKey);
            return GetExceptionToThrow(exEldan);
        }

        private Exception GetExceptionToWrite(EldanError exEldan)
        {
            return m_AddEldanIDInErrorMessage ? exEldan.GetExtendedException() : exEldan.GetOriginalException();
        }

        private Exception GetExceptionToThrow(EldanError exEldan)
        {
            return m_AddEldanIDInErrorMessage ? exEldan.GetShortException() : exEldan.GetOriginalException();
        }
    }
}
