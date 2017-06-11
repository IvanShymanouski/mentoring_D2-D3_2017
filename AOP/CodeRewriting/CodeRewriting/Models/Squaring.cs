using System;
using AOPLogger;

namespace CodeRewriting.Models
{
    public class Squaring
    {
        [LoggerPostSharp]
        public double Square(double value, int power)
        {
            return Math.Pow(value, power);
        }
    }
}