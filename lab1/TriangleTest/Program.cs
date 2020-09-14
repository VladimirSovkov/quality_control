using System;
using System.Diagnostics;
using System.IO;

namespace TriangleTest
{
    public class Program
    {
        private static readonly string pathToExecutableFile = @"..\..\..\..\Triangle\bin\Release\netcoreapp3.1\";
        private static readonly string pathToCheckResults = @"..\..\..\ResultOfCheck.txt";
        private static bool CheckProgramResponse(string inputValues, string example)
        {
            ProcessStartInfo procStartInfo = new ProcessStartInfo(pathToExecutableFile + "Triangle.exe");
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = false;
            procStartInfo.Arguments = inputValues;
            Process proc = new Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            string result = proc.StandardOutput.ReadToEnd().Trim();
            return result == example;
        }

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Некорректное кол-во элементов");
                return;
            }

            try
            {
                string path = args[0];
                using (StreamReader sr = new StreamReader(path))
                {
                    using (StreamWriter sw = new StreamWriter(pathToCheckResults, false))
                    {
                        for (int count = 1; !sr.EndOfStream; count++)
                        {
                            string inputValues = sr.ReadLine();
                            string example = sr.ReadLine();
                            if (CheckProgramResponse(inputValues, example))
                            {
                                sw.WriteLine($"{count} success");
                            }
                            else
                            {
                                sw.WriteLine($"{count} error");
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }
}
