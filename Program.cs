using System;
using System.Collections.Generic;
using System.Numerics;

interface IMyNumber<T> where T : IMyNumber<T>
{
    T Add(T b);
    T Subtract(T b);
    T Multiply(T b);
    T Divide(T b);
}

public class MyFrac : IMyNumber<MyFrac>, IComparable<MyFrac>
{
    private BigInteger nom;
    private BigInteger denom;

    public MyFrac(BigInteger nom, BigInteger denom)
    {
        if (denom == 0)
            throw new DivideByZeroException("Denominator cannot be zero.");
        this.nom = nom;
        this.denom = denom;
        Simplify();
    }

    public MyFrac(int nom, int denom) : this((BigInteger)nom, (BigInteger)denom) { }

    private void Simplify()
    {
        BigInteger gcd = BigInteger.GreatestCommonDivisor(nom, denom);
        nom /= gcd;
        denom /= gcd;
        if (denom < 0)
        {
            nom = -nom;
            denom = -denom;
        }
    }

    public MyFrac Add(MyFrac that)
    {
        return new MyFrac(this.nom * that.denom + that.nom * this.denom, this.denom * that.denom);
    }

    public MyFrac Subtract(MyFrac that)
    {
        return new MyFrac(this.nom * that.denom - that.nom * this.denom, this.denom * that.denom);
    }

    public MyFrac Multiply(MyFrac that)
    {
        return new MyFrac(this.nom * that.nom, this.denom * that.denom);
    }

    public MyFrac Divide(MyFrac that)
    {
        if (that.nom == 0)
            throw new DivideByZeroException("Cannot divide by zero.");
        return new MyFrac(this.nom * that.denom, this.denom * that.nom);
    }

    public override string ToString()
    {
        return $"{nom}/{denom}";
    }

    public int CompareTo(MyFrac that)
    {
        return (this.nom * that.denom).CompareTo(this.denom * that.nom);
    }
}

public class MyComplex : IMyNumber<MyComplex>
{
    private double re;
    private double im;

    public MyComplex(double re, double im)
    {
        this.re = re;
        this.im = im;
    }

    public MyComplex Add(MyComplex that)
    {
        return new MyComplex(this.re + that.re, this.im + that.im);
    }

    public MyComplex Subtract(MyComplex that)
    {
        return new MyComplex(this.re - that.re, this.im - that.im);
    }

    public MyComplex Multiply(MyComplex that)
    {
        return new MyComplex(
            this.re * that.re - this.im * that.im,
            this.re * that.im + this.im * that.re
        );
    }

    public MyComplex Divide(MyComplex that)
    {
        double denominator = that.re * that.re + that.im * that.im;
        if (denominator == 0)
            throw new DivideByZeroException("Cannot divide by zero.");
        return new MyComplex(
            (this.re * that.re + this.im * that.im) / denominator,
            (this.im * that.re - this.re * that.im) / denominator
        );
    }

    public override string ToString()
    {
        return $"{re}+{im}i";
    }
}

class Program
{
    static void testAPlusBSquare<T>(T a, T b) where T : IMyNumber<T>
    {
        Console.WriteLine($"=== Testing (a+b)^2 = a^2 + 2ab + b^2 for a = {a}, b = {b} ===");
        T aPlusB = a.Add(b);
        Console.WriteLine($"a + b = {aPlusB}");
        T aPlusBSquare = aPlusB.Multiply(aPlusB);
        Console.WriteLine($"(a + b)^2 = {aPlusBSquare}");

        T aSquare = a.Multiply(a);
        T bSquare = b.Multiply(b);
        T abTwice = a.Multiply(b).Add(a.Multiply(b));
        T rightSide = aSquare.Add(abTwice).Add(bSquare);
        Console.WriteLine($"a^2 + 2ab + b^2 = {rightSide}");
        Console.WriteLine($"=== Finished ===");
    }

    static void testSquaresDifference<T>(T a, T b) where T : IMyNumber<T>
    {
        Console.WriteLine($"=== Testing (a-b)*(a+b) = a^2 - b^2 for a = {a}, b = {b} ===");
        T aMinusB = a.Subtract(b);
        T aPlusB = a.Add(b);
        T product = aMinusB.Multiply(aPlusB);
        Console.WriteLine($"(a - b)*(a + b) = {product}");

        T aSquare = a.Multiply(a);
        T bSquare = b.Multiply(b);
        T difference = aSquare.Subtract(bSquare);
        Console.WriteLine($"a^2 - b^2 = {difference}");
        Console.WriteLine($"=== Finished ===");
    }

    static void Main(string[] args)
    {
        testAPlusBSquare(new MyFrac(1, 3), new MyFrac(1, 6));
        testAPlusBSquare(new MyComplex(1, 3), new MyComplex(1, 6));

        testSquaresDifference(new MyFrac(1, 3), new MyFrac(1, 6));
        testSquaresDifference(new MyComplex(1, 3), new MyComplex(1, 6));

        var fractions = new List<MyFrac>
        {
            new MyFrac(3, 4),
            new MyFrac(1, 2),
            new MyFrac(5, 6)
        };

        fractions.Sort();
        Console.WriteLine("Sorted fractions:");
        foreach (var frac in fractions)
        {
            Console.WriteLine(frac);
        }

        Console.ReadKey();
    }
}
