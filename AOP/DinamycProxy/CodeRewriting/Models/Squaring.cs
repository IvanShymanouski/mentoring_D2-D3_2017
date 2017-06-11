using System;

namespace CodeRewriting.Models
{
    public class Squaring : ISquaring
    {
        public double Square(double value, int power)
        {
            return Math.Pow(value, power);
        }
    }
}