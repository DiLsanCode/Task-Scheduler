using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskApp.Business.Constants
{
    public static class FibonacciNumbers
    {
        private static List<string> fibonacciNumbers = new List<string>() { "0", "0,5", "1", "2", "3", "5", "8", "13", "20", "40", "100" };

        public static List<string> GetList() { return  fibonacciNumbers; }
    }
}
