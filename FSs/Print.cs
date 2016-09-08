using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSs
{
    static class Print
    {
        public static void Success(string s)
        {
            print("[^2Success^7] "+s+"\n");
        }
        public static void Request(string s)
        {
            print("[^5Request^7] " + s + "\n");
        }
        public static void Response(string s)
        {
            print("[^3Response^7] " + s + "\n");
        }
        public static void Error(string s,bool Critical = false)
        {
            if (s.Contains("Only one usage of each socket address"))
                s = "Port in use by another application/service";

            if (!Critical)
                print("[^1Error^7] " + s + "\n");
            else
            {
                print("[^1Critical Error^7] " + s + "\n");
                Console.Read();
                Environment.Exit(1);
            }
        }
        public static void Info(string s)
        {
            print("> ^8" + s + "\n");
        }
        public static void print(string s)
        {
            Console.ForegroundColor = ConsoleColor.White;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '^')
                {
                    switch (s[i + 1])
                    {
                        case '1': Console.ForegroundColor = ConsoleColor.Red; break;
                        case '2': Console.ForegroundColor = ConsoleColor.Green; break;
                        case '3': Console.ForegroundColor = ConsoleColor.Yellow; break;
                        case '4': Console.ForegroundColor = ConsoleColor.Blue; break;
                        case '5': Console.ForegroundColor = ConsoleColor.Cyan; break;
                        case '6': Console.ForegroundColor = ConsoleColor.Magenta; break;
                        case '7': Console.ForegroundColor = ConsoleColor.White; break;
                        case '8': Console.ForegroundColor = ConsoleColor.Gray; break;
                        case '9': Console.ForegroundColor = ConsoleColor.DarkYellow; break;
                        case '0': Console.ForegroundColor = ConsoleColor.Black; break;
                    }
                    i += 2;
                }
                    Console.Write(s[i]);
            }
        }

    }
}
