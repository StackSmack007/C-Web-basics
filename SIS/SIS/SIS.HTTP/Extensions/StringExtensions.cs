using System;
using System.Collections.Generic;
using System.Text;

namespace SIS.HTTP.Extensions
{
    public static class StringExtensions
    {

        public static string Capitalize(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            string firstLetter = str[0].ToString().ToUpper();
            string everythingBut = str.Substring(1).ToLower();
            str = firstLetter + everythingBut;
            return str;
        }



    }
}
