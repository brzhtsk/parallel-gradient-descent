namespace GradientDescent;

/// <summary>
/// Градієнтний спуск, спільний для послідовної та паралельної реалізацій.
/// Реалізації відрізняються лише способом обчислення градієнта.
/// </summary>
public class GradientDescentOptimizer : IGradientDescentOptimizer
{
    private readonly IGradientCalculator gradientCalculator;

    public GradientDescentOptimizer(IGradientCalculator gradientCalculator)
    {
        this.gradientCalculator = gradientCalculator ?? throw new ArgumentNullException(nameof(gradientCalculator));
    }

    public double[] Minimize(
        Func<double[], double> function,
        double[] initialPoint,
        double learningRate,
        int maxIterations,
        double precision)
    {
        ArgumentNullException.ThrowIfNull(function);
        ArgumentNullException.ThrowIfNull(initialPoint);

        double[] currentPoint = (double[])initialPoint.Clone();
        double precisionSquared = precision * precision;

        for (int iteration = 0; iteration < maxIterations; iteration++)
        {
            double[] gradient = gradientCalculator.ComputeGradient(function, currentPoint);

            if (SquaredNorm(gradient) < precisionSquared)
            {
                break;
            }

            for (int i = 0; i < currentPoint.Length; i++)
            {
                currentPoint[i] -= learningRate * gradient[i];
            }
        }

        return currentPoint;
    }

    private static double SquaredNorm(double[] vector)
    {
        double sum = 0.0;
        for (int i = 0; i < vector.Length; i++)
        {
            sum += vector[i] * vector[i];
        }
        return sum;
    }
}
