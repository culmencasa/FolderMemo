using System;
using System.Collections.Generic;
using System.Text;

namespace Utils.Misc
{

    /// <summary>
    /// Object类型转换为某个基本类型默认值转换类
    /// </summary>
    public static class Conversion
    {
        #region Object转基本类型

        /// <summary>
        /// 转换成字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToDefaultString(this object value)
        {
            return ToDefaultString(value, string.Empty);
        }
        /// <summary>
        /// 转换成字符串
        /// </summary>
        /// <param name="value"></param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static string ToDefaultString(this object value, string def)
        {
            string s = def;
            if (value == null)
            {
                return s;
            }
            else
            {
                if (value is System.Security.SecureString ss)
                {
                    if (ss.Length == 0)
                    {
                        return s;
                    }
                }
                return value.ToString();
            }
        }

        /// <summary>
        /// 转换成整型Int32
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToDefaultInt32(this object value)
        {
            return ToDefaultInt32(value, default(int));
        }
        /// <summary>
        /// 转换成整型Int64
        /// </summary>
        /// <param name="value"></param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static int ToDefaultInt32(this object value, int def)
        {
            int i = def;
            if (value == null)
            {
                return i;
            }
            else
            {
                if (value.GetType().IsEnum)
                {
                    i = (int)value;
                }
                else if (!int.TryParse(value.ToString(), out i))
                {
                    decimal d = 0;
                    if (decimal.TryParse(value.ToString(), out d))
                    {
                        i = (int)d;
                    }
                    else
                    {
                        i = def;
                    }
                }

                return i;
            }

        }

        public static long ToDefaultLong(this object value)
        {
            return ToDefaultLong(value, default(long));
        }
        /// <summary>
        /// 转换成整型Int64
        /// </summary>
        /// <param name="value"></param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static long ToDefaultLong(this object value, long def)
        {
            long i = def;
            if (value == null)
            {
                return i;
            }
            else
            {
                if (value.GetType().IsEnum)
                {
                    i = (long)value;
                }
                else if (!long.TryParse(value.ToString(), out i))
                {
                    decimal d = 0;
                    if (decimal.TryParse(value.ToString(), out d))
                    {
                        i = (long)d;
                    }
                }

                return i;
            }

        }

        /// <summary>
        /// 转换成Decimal
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal ToDefaultDecimal(this object value)
        {
            return ToDefaultDecimal(value, default(decimal));
        }
        /// <summary>
        /// 转换成Decimal
        /// </summary>
        /// <param name="value"></param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static decimal ToDefaultDecimal(this object value, decimal def)
        {
            decimal d = def;
            if (value == null)
            {
                return d;
            }
            else
            {
                if (!decimal.TryParse(value.ToString(), out d))
                {
                    d = def;
                }
                return d;
            }
        }

        /// <summary>
        /// 转换成Float
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float ToDefaultFloat(this object value)
        {
            return ToDefaultFloat(value, default(float));
        }
        /// <summary>
        /// 转换成Float
        /// </summary>
        /// <param name="value"></param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static float ToDefaultFloat(this object value, float def)
        {
            float d = def;
            if (value == null)
            {
                return d;
            }
            else
            {
                if (!float.TryParse(value.ToString(), out d))
                {
                    d = def;
                }
                return d;
            }
        }

        /// <summary>
        /// 转换成Double
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double ToDefaultDouble(this object value)
        {
            return ToDefaultDouble(value, default(double));
        }
        /// <summary>
        /// 转换成Double
        /// </summary>
        /// <param name="value"></param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static double ToDefaultDouble(this object value, double def)
        {
            double d = def;
            if (value == null)
            {
                return d;
            }
            else
            {
                if (!double.TryParse(value.ToString(), out d))
                {
                    d = def;
                }
                return d;
            }
        }
        /// <summary>
        /// 转换成布尔
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ToDefaultBoolean(this object value)
        {
            return ToDefaultBoolean(value, false);
        }
        /// <summary>
        /// 转换成布尔
        /// </summary>
        /// <param name="value"></param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static bool ToDefaultBoolean(this object value, bool def)
        {
            bool returnValue = def;
            if (value == null)
            {
                return returnValue;
            }

            if (!bool.TryParse(value.ToString(), out returnValue))
            {
                returnValue = def;
            }

            return returnValue;
        }

        /// <summary>
        /// 转换成日期
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime ToDefaultDateTime(this object value)
        {
            return ToDefaultDateTime(value, DateTime.MinValue);
        }
        /// <summary>
        /// 转换成日期
        /// </summary>
        /// <param name="value"></param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static DateTime ToDefaultDateTime(this object value, DateTime def)
        {
            DateTime dt = def;
            if (value == null)
            {
                return dt;
            }

            if (!DateTime.TryParse(value.ToString(), out dt))
            {
                dt = def;
            }
            return dt;
        }

        #endregion
    }
}
