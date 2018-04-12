using System;

namespace AntiPlagiarism.Tests.CodeAnalyzing.CSharp.TestData
{
    public class Rational
    {
        private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);

		private int Numerator { get; }
		private int Denominator { get; }
		private bool IsNan { get; }

        public Rational(int numerator, int denominator=1, bool isNan=false)
        {
            var gcd = Gcd(numerator, denominator);
            Numerator = numerator / gcd;
            Denominator = denominator / gcd;
            if (Denominator < 0)
            {
                Numerator *= -1;
                Denominator *= -1;
            }
            IsNan = isNan || denominator == 0;
        }

        public static Rational operator+(Rational a, Rational b)
        {
            var numerator = a.Numerator * b.Denominator + b.Numerator * a.Denominator;
            var denominator = a.Denominator * b.Denominator;
            return new Rational(numerator, denominator);
        }

        public static Rational operator-(Rational a, Rational b)
        {
            var numerator = a.Numerator * b.Denominator - b.Numerator * a.Denominator;
            var denominator = a.Denominator * b.Denominator;
            return new Rational(numerator, denominator);
        }

        public static Rational operator*(Rational a, Rational b)
        {
            return new Rational(a.Numerator * b.Numerator, a.Denominator * b.Denominator, a.IsNan || b.IsNan);
        }

        public static Rational operator/(Rational a, Rational b)
        {
            return new Rational(a.Numerator * b.Denominator, a.Denominator * b.Numerator, a.IsNan || b.IsNan);
        }

        public static implicit operator double(Rational a) => (double) a.Numerator / a.Denominator;

        public static implicit operator int(Rational a)
        {
            if (a.Numerator % a.Denominator == 0)
                return a.Numerator / a.Denominator;
            throw new ArgumentException();
        }

        public static implicit operator Rational(int a) => new Rational(a);
    }
}