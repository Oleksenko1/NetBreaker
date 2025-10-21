using UnityEngine;
using System;

[System.Serializable]
public struct BigNum
{
    public double m; // Mantissa
    public int e; // Exponent

    private static readonly string[] suffixes = new string[]
    {
        "",    // 10^0
        "k",   // 10^3
        "m",   // 10^6
        "b",   // 10^9
        "t",   // 10^12
        "qa",  // 10^15
        "qi",  // 10^18
        "sx",  // 10^21
        "sp"   // 10^24
    };

    public BigNum(double mantissa, int exponent)
    {
        m = mantissa;
        e = exponent;
        Normalize();
    }

    public BigNum(double value)
    {
        if (value == 0)
        {
            m = 0;
            e = 0;
            return;
        }

        e = (int)Math.Floor(Math.Log10(Math.Abs(value)));
        m = value / Math.Pow(10, e);
        Normalize();
    }

    private void Normalize()
    {
        if (m == 0)
        {
            e = 0;
            return;
        }

        while (Math.Abs(m) >= 10)
        {
            m /= 10;
            e++;
        }

        while (Math.Abs(m) < 1 && m != 0)
        {
            m *= 10;
            e--;
        }
    }

    public static BigNum operator +(BigNum a, BigNum b)
    {
        if (a.m == 0) return b;
        if (b.m == 0) return a;

        int expDiff = a.e - b.e;

        if (expDiff > 15) return a;
        if (expDiff < -15) return b;

        double newM;
        int newE;

        if (expDiff > 0)
        {
            newM = a.m + b.m / Math.Pow(10, expDiff);
            newE = a.e;
        }
        else if (expDiff < 0)
        {
            newM = a.m / Math.Pow(10, -expDiff) + b.m;
            newE = b.e;
        }
        else
        {
            newM = a.m + b.m;
            newE = a.e;
        }

        BigNum result = new BigNum(newM, newE);
        result.Normalize();
        return result;
    }

    public static BigNum operator -(BigNum a, BigNum b)
    {
        BigNum result = a + new BigNum(-b.m, b.e);
        result.Normalize();
        return result;
    }

    public static BigNum operator *(BigNum a, BigNum b)
    {
        BigNum result = new BigNum(a.m * b.m, a.e + b.e);
        result.Normalize();
        return result;
    }

    public static BigNum operator /(BigNum a, BigNum b)
    {
        if (b.m == 0) throw new DivideByZeroException();
        BigNum result = new BigNum(a.m / b.m, a.e - b.e);
        result.Normalize();
        return result;
    }

    public static bool operator >(BigNum a, BigNum b)
    {
        if (a.e != b.e) return a.e > b.e;
        return a.m > b.m;
    }

    public static bool operator <(BigNum a, BigNum b)
    {
        if (a.e != b.e) return a.e < b.e;
        return a.m < b.m;
    }

    public static bool operator >=(BigNum a, BigNum b)
    {
        return a > b || (a.e == b.e && Math.Abs(a.m - b.m) < 0.0001);
    }

    public static bool operator <=(BigNum a, BigNum b)
    {
        return a < b || (a.e == b.e && Math.Abs(a.m - b.m) < 0.0001);
    }
    public static bool operator ==(BigNum a, BigNum b)
    {
        if (a.m == 0 && b.m == 0)
            return true;

        if (a.e != b.e)
            return false;

        return Math.Abs(a.m - b.m) < 0.0001;
    }

    public static bool operator !=(BigNum a, BigNum b)
    {
        return !(a == b);
    }
    public string ToString(int decimals = 2)
    {
        if (m == 0) return "0,00";

        int suffixIndex = e / 3;
        int remainder = e % 3;

        if (suffixIndex < suffixes.Length)
        {
            double displayValue = m * Math.Pow(10, remainder);
            return $"{displayValue.ToString($"F{decimals}")}{suffixes[suffixIndex]}";
        }
        else
        {
            double displayValue = m * Math.Pow(10, remainder);
            return $"{displayValue.ToString($"F{decimals}")}e{e}";
        }
    }

    public override string ToString()
    {
        return ToString(2);
    }

    public static BigNum Parse(string value)
    {
        value = value.Trim().ToUpper();

        if (value.Contains("E"))
        {
            string[] parts = value.Split('E');
            double mantissa = double.Parse(parts[0]);
            int exponent = int.Parse(parts[1]);
            return new BigNum(mantissa, exponent);
        }

        foreach (var suffix in suffixes)
        {
            if (string.IsNullOrEmpty(suffix)) continue;

            if (value.EndsWith(suffix))
            {
                string numPart = value.Substring(0, value.Length - suffix.Length);
                double mantissa = double.Parse(numPart);
                int exponent = Array.IndexOf(suffixes, suffix) * 3;
                return new BigNum(mantissa, exponent);
            }
        }

        return new BigNum(double.Parse(value));
    }
}
