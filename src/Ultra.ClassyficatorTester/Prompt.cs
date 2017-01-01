using System;
using System.IO;
using System.Windows.Forms;

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

        public static bool FolderPrompt(out string path)
        {
            using (var fbd = new FolderBrowserDialog() {SelectedPath = Directory.GetCurrentDirectory()})
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    path = fbd.SelectedPath;
                    return true;
                }
                path = "";
                return false;
            }
        }
    }
}