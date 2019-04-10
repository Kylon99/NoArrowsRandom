using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoArrowsRandom
{
    public static class Logging
    {
        public static void Info(string message) => Log("INFO", message);
        private static void Log(string level, string message) => Console.WriteLine($"[NoArrowsRandom | {level}] {message}");
    }
}
