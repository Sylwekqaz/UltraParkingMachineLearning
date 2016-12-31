using System;

namespace Ultra.ClassyficatorTester
{
    public static class Prompt
    {
        public static bool YesNo(string messege)
        {
            Console.Write(messege + " Y/N: ");
            while (true)
            {
                var key = Console.ReadKey(intercept: true);
                switch (key.Key)
                {
                    case ConsoleKey.Y:
                        Console.WriteLine("Y");
                        return true;
                    case ConsoleKey.N:
                        Console.WriteLine("N");
                        return false;
                }
            }
        }
    }
}