using System;

namespace Triangle
{
    public class Triangle
    {
        private static bool IsEquilateral(double a, double b, double c)
        {
            return a == b && b == c;
        }

        private static bool IsIsosceles(double a, double b, double c)
        {
            return a == b || b == c || a == c;
        }

        private static bool IsExistingTriangle(double a, double b, double c)
        {
            return a + b > c && b + c > a && a + c > b;
        }

        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("неизвестная ошибка");
                return;
            }

            try
            {
                double a = Convert.ToDouble(args[0]);
                double b = Convert.ToDouble(args[1]);
                double c = Convert.ToDouble(args[2]);
                if (IsExistingTriangle(a, b, c))
                {
                    if (IsEquilateral(a, b, c))
                    {
                        Console.WriteLine("равносторонний");
                    }
                    else if (IsIsosceles(a, b, c))
                    {
                        Console.WriteLine("равнобедренный");
                    }
                    else
                    {
                        Console.WriteLine("обычный");
                    }
                }
                else
                {
                    Console.WriteLine("не треугольник");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("неизвестная ошибка");
            }
        }
    }
}
