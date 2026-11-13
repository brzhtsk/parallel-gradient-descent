namespace GradientDescent.Tests;

public class OptimizerTests
{
    private const double Precision = 1e-4;
    private const double Tolerance = 0.1;

    private readonly IGradientDescentOptimizer optimizer = new SequentialGradientDescent();

    [Theory]
    [InlineData(1, 3.0)]
    [InlineData(10, -2.5)]
    [InlineData(1000, 4.0)]
    public void Gradient_MatchesAnalyticalSphereGradient(int dimensions, double value)
    {
        double[] point = TestFunctions.Filled(dimensions, value);
        point[0] = -value;

        double[] gradient = new SequentialGradientCalculator().ComputeGradient(TestFunctions.Sphere, point);

        for (int i = 0; i < dimensions; i++)
        {
            Assert.True(Math.Abs(gradient[i] - 2.0 * point[i]) < 1e-3,
                $"Похідна за координатою {i}: очікувалось {2.0 * point[i]}, отримано {gradient[i]}");
        }
    }

    [Theory]
    [InlineData(2, 5.0, 0.01)]
    [InlineData(10, 10.0, 0.05)]
    [InlineData(50, -5.0, 0.01)]
    [InlineData(1000, 2.0, 0.01)]
    [InlineData(5000, 1.0, 0.05)]
    public void SphereFunction_ShouldConvergeToZero(int dimensions, double startValue, double learningRate)
    {
        double[] result = optimizer.Minimize(TestFunctions.Sphere, TestFunctions.Filled(dimensions, startValue),
            learningRate, 2000, Precision);

        AssertAllNearZero(result);
    }

    [Theory]
    [InlineData(2, 5.0, 0.1)]
    [InlineData(10, 5.0, 0.01)]
    [InlineData(50, 2.0, 0.005)]
    public void SumSquaresFunction_ShouldConvergeToZero(int dimensions, double startValue, double learningRate)
    {
        double[] result = optimizer.Minimize(TestFunctions.SumSquares, TestFunctions.Filled(dimensions, startValue),
            learningRate, 5000, Precision);

        AssertAllNearZero(result);
    }

    [Theory]
    [InlineData(2, -1.2, 0.001)]
    [InlineData(10, -1.2, 0.001)]
    [InlineData(100, -1.2, 0.0001)]
    [InlineData(1000, -1.2, 0.0001)]
    public void RosenbrockFunction_ShouldDecreaseLoss(int dimensions, double startX, double learningRate)
    {
        double[] start = TestFunctions.RosenbrockStart(dimensions, startX);

        AssertLossDecreases(TestFunctions.Rosenbrock, start, learningRate, 15000);
    }

    [Theory]
    [InlineData(2, 1.0, 0.001)]
    [InlineData(10, 1.0, 0.001)]
    [InlineData(50, 1.0, 0.001)]
    public void RastriginFunction_ShouldDecreaseLoss(int dimensions, double startValue, double learningRate)
    {
        AssertLossDecreases(TestFunctions.Rastrigin, TestFunctions.Filled(dimensions, startValue), learningRate, 1000);
    }

    [Fact]
    public void StartAtMinimum_ReturnsStartPoint()
    {
        double[] start = TestFunctions.Filled(100, 0.0);

        double[] result = optimizer.Minimize(TestFunctions.Sphere, start, 0.1, 1000, Precision);

        Assert.Equal(start, result);
    }

    [Fact]
    public void Constructor_NullCalculator_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new GradientDescentOptimizer(null!));
    }

    private static void AssertAllNearZero(double[] result)
    {
        Assert.All(result, value =>
            Assert.True(Math.Abs(value) < Tolerance, $"Відхилення від мінімуму (0) занадто велике: {value:F6}"));
    }

    private void AssertLossDecreases(Func<double[], double> function, double[] start, double learningRate, int maxIterations)
    {
        double initialValue = function(start);

        double finalValue = function(optimizer.Minimize(function, start, learningRate, maxIterations, Precision));

        Assert.True(double.IsFinite(finalValue), $"Отримано некоректне значення: {finalValue}");
        Assert.True(finalValue < initialValue, $"Значення не зменшилось. Початкове: {initialValue}, кінцеве: {finalValue}");
    }
}
