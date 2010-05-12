using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoCS.BuildService.Business
{
    public class Utils
    {
        public static string TimeToString(DateTime dt)
        {
            string time = dt.Hour.ToString().PadLeft(2, '0') + dt.Minute.ToString().PadLeft(2, '0')
                + dt.Second.ToString().PadLeft(2, '0') + dt.Millisecond.ToString().PadLeft(3, '0');

            return time;
        }

        public static string DateToString(DateTime dt)
        {
            string date = dt.Year.ToString().PadLeft(4, '0') + dt.Month.ToString().PadLeft(2, '0')
    + dt.Day.ToString().PadLeft(2, '0');

            return date;
        }
    }
}
