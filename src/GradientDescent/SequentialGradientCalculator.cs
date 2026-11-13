namespace GradientDescent;

/// <summary>
/// Послідовне обчислення градієнта: усі координати обробляються в одному потоці.
/// </summary>
public class SequentialGradientCalculator : IGradientCalculator
{
    public double[] ComputeGradient(Func<double[], double> function, double[] point)
    {
        double[] gradient = new double[point.Length];
        FiniteDifference.ComputePartialDerivatives(function, point, gradient, 0, point.Length);
        return gradient;
    }
}
