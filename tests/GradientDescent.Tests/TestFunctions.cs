namespace GradientDescent.Tests;

/// <summary>
/// Тестові функції з відомими мінімумами та генератори початкових точок.
/// </summary>
internal static class TestFunctions
{
    public static double Sphere(double[] x)
    {
        double sum = 0.0;
        for (int i = 0; i < x.Length; i++)
        {
            sum += x[i] * x[i];
        }
        return sum;
    }

    public static double SumSquares(double[] x)
    {
        double sum = 0.0;
        for (int i = 0; i < x.Length; i++)
        {
            sum += (i + 1) * x[i] * x[i];
        }
        return sum;
    }

    public static double Rosenbrock(double[] x)
    {
        double sum = 0.0;
        for (int i = 0; i < x.Length - 1; i++)
        {
            double a = x[i + 1] - x[i] * x[i];
            double b = 1.0 - x[i];
            sum += 100.0 * a * a + b * b;
        }
        return sum;
    }

    public static double Rastrigin(double[] x)
    {
        double sum = 10.0 * x.Length;
        for (int i = 0; i < x.Length; i++)
        {
            sum += x[i] * x[i] - 10.0 * Math.Cos(2.0 * Math.PI * x[i]);
        }
        return sum;
    }

    public static double[] Filled(int dimensions, double value)
    {
        double[] point = new double[dimensions];
        Array.Fill(point, value);
        return point;
    }

    public static double[] RosenbrockStart(int dimensions, double firstCoordinate)
    {
        double[] point = Filled(dimensions, 1.0);
        point[0] = firstCoordinate;
        return point;
    }

    public static double[] Random(int dimensions, int seed, double min, double max)
    {
        var random = new System.Random(seed);
        double[] point = new double[dimensions];
        for (int i = 0; i < dimensions; i++)
        {
            point[i] = min + random.NextDouble() * (max - min);
        }
        return point;
    }
}
