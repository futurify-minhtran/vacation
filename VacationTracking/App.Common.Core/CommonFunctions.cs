using System;
using System.Collections.Generic;
using System.Text;

namespace App.Common.Core
{
    public static class CommonFunctions
    {
        public static int ConvertToInt(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;
            return Int32.Parse(value);
        }
    }
}
