using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskApp.Business.Interfaces;

namespace TaskApp.Business.Services
{
    public class NamingService : INamingService
    {
        public string UppercaseFirst(string str)
        {
            TextInfo textInfo = new CultureInfo("bg-BG", false).TextInfo;

            if (string.IsNullOrEmpty(str))
                return string.Empty;

            return textInfo.ToTitleCase(str);
            //return char.ToUpper(str[0]) + str.Substring(1).ToLower();
        }
    }
}
