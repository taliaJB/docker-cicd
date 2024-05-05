using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Eldan.TypeExtensions
{
    public static class DateExtensions
    {
        private const string ELDAN_DATE_FROMAT = "yyyyMMdd";
        private const string ELDAN_TIME_FROMAT = "HHmmss";
        private const string ISRAEL_DATE_TIME_FORMAT = "dd/MM/yyyy HH:mm:ss";
        private const string ISRAEL_DATE_FORMAT = "dd/MM/yyyy";
        private const string ISRAEL_TIME_FORMAT = "HH:mm:ss";
        private const string USA_DATE_TIME_FORMAT = "MM/dd/yyyy HH:mm:ss";
        private const string GENERIC_DATE_TIME_FORMAT = "yyyy-MM-ddTHH:mm:ss";
        private const string FILE_DATE_TIME_FORMAT = "yyyyMMddHHmmss";

        public static DateTime ParseEldanDate(this DateTime source, string EldanDate)
        {
            return DateTime.MinValue.ParseEldanDate(EldanDate, "000000");
        }

        public static DateTime ParseEldanDate(this DateTime source, string EldanDate, string EldanTime)
        {
            TryParseEldanDate(EldanDate, EldanTime, out source);

            return source;
        }

        public static bool TryParseEldanDate(string EldanDate, string EldanTime, out DateTime dtDate)
        {
            return TryFormatTextToDate(EldanDate + EldanTime, ELDAN_DATE_FROMAT + ELDAN_TIME_FROMAT, out dtDate);
        }

        public static DateTime FormatTextToDate(this DateTime source, string strText, string DateFormat)
        {
            TryFormatTextToDate(strText, DateFormat, out source);

            return source;
        }

        public static bool TryIsraelFormatToDate(string strText, out DateTime dtDate)
        {
            return TryFormatTextToDate(strText, ISRAEL_DATE_TIME_FORMAT, out dtDate);
        }

        public static bool TryFormatTextToDate(string strText, string DateFormat, out DateTime dtDate)
        {

            strText = strText.Trim();

            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US"); //he-IL
            DateTimeStyles styles = DateTimeStyles.None;

            return DateTime.TryParseExact(strText, DateFormat, culture, styles, out dtDate);
        }

        public static int ToEldanDayOfWeek(this DateTime source)
        {
            return (int)source.DayOfWeek + 1;
        }

        public static string ToEldanDate(this DateTime source)
        {
            return source.ToString(ELDAN_DATE_FROMAT);
        }

        public static string ToEldanTime(this DateTime source)
        {
            return source.ToString(ELDAN_TIME_FROMAT);
        }

        public static string ToIsraelDateTime(this DateTime source)
        {
            return source.ToString(ISRAEL_DATE_TIME_FORMAT);
        }

        public static string ToIsraelDate(this DateTime source)
        {
            return source.ToString(ISRAEL_DATE_FORMAT);
        }

        public static string ToIsraelTime(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return "";
            return DateTime.Now.ParseEldanDate("20000101", source).ToString(ISRAEL_TIME_FORMAT);
        }
        public static TimeSpan ToTimeSpan(this string source)
        {
            if (string.IsNullOrWhiteSpace(source) || source.Length!=6)
                return TimeSpan.Zero;
            return new TimeSpan( int.Parse(source.Substring(0,2)), int.Parse(source.Substring(2, 2)), int.Parse(source.Substring(4, 2)));
        }
        
        public static String ToUSADateTime(this DateTime source)
        {
            return source.ToString(USA_DATE_TIME_FORMAT);
        }

        public static String ToGenericDateTime(this DateTime source)
        {
            return source.ToString(GENERIC_DATE_TIME_FORMAT);
        }

        public static String ToFileDateTime(this DateTime source)
        {
            return source.ToString(FILE_DATE_TIME_FORMAT);
        }
    }
}
