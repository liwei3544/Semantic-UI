using System;
using System.Globalization;

namespace Lottery.Gather
{
    public static  class Utility
    {

        /// <summary>
        /// 将日期按照指定的格式转换为数字
        /// </summary>
        /// <param name="dateTime">待转换日期</param>
        /// <param name="format">转换格式</param>
        /// <returns>转换的结果,转换失败则返回0</returns>
        public static int ToDateInt(this DateTime dateTime, string format = "yyyyMMdd")
        {
            int result;
            Int32.TryParse(dateTime.ToString(format, DateTimeFormatInfo.InvariantInfo), out result);
            return result;
        }
         

        #region 将字符串转换为整数(int)

        /// <summary>
        /// 将字符串转换为数字(int)
        /// </summary>
        /// <param name="str">待转换字符</param>
        /// <param name="result">转换结果</param>
        ///  <param name="defaultval">转换失败时，返回默认值</param>
        /// <returns>转换是否成功</returns>
        private static bool TryToInt32(this string str, out int result, int defaultval = 0)
        {
            result = defaultval;
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            if (Int32.TryParse(str, out result))
            {
                return true;
            }
            result = defaultval;
            return false;
        }

        /// <summary>
        /// 将字符串转换为数字(int)
        /// </summary>
        /// <param name="str">待转换字符</param>
        ///  <param name="defaultval">转换失败时，返回默认值</param>
        /// <returns>返回转换后的值</returns>
        /// <example>
        /// <code lang="c#">
        /// <![CDATA[
        ///     string str="123";
        ///     int ret=str.ToInt32(0); //return 123
        ///     
        ///     str="abc";
        ///     ret=abc.ToInt32(); //return 0;
        /// ]]>
        /// </code>
        /// </example>
        public static int ToInt32(this string str, int defaultval = 0)
        {
            int result;
            TryToInt32(str, out result, defaultval);
            return result;
        }
        #endregion
    }
}
