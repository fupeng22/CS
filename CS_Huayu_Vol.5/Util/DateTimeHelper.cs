using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Util
{
    public enum EnumDateCompare
    {
        year = 1,
        month = 2,
        day = 3,
        hour = 4,
        minute = 5,
        second = 6
    }

    public class DateTimeHelper
    {
        /// <summary>
        /// 计算两个日期相差的年、月份、日期、小时、分钟 、秒
        /// </summary>
        /// <param name="howtocompare">需要获取的部分信息</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns></returns>
        public static double DateDiff(EnumDateCompare howtocompare, System.DateTime startDate, System.DateTime endDate)
        {
            double diff = 0;
            System.TimeSpan TS = new System.TimeSpan(endDate.Ticks - startDate.Ticks);

            switch (howtocompare)
            {
                case EnumDateCompare.year:
                    diff = Convert.ToDouble(TS.TotalDays / 365);
                    break;
                case EnumDateCompare.month:
                    diff = Convert.ToDouble((TS.TotalDays / 365) * 12);
                    break;
                case EnumDateCompare.day:
                    diff = Convert.ToDouble(TS.TotalDays);
                    break;
                case EnumDateCompare.hour:
                    diff = Convert.ToDouble(TS.TotalHours);
                    break;
                case EnumDateCompare.minute:
                    diff = Convert.ToDouble(TS.TotalMinutes);
                    break;
                case EnumDateCompare.second:
                    diff = Convert.ToDouble(TS.TotalSeconds);
                    break;
            }
            return diff;
        }
    }
}
