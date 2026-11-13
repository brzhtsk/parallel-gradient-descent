namespace GradientDescent.Tests;

public class ParallelOptimizerTests
{
    private const int MaxIterations = 10;
    private const double Precision = 1e-6;
    private const double Tolerance = 1e-9;

    [Theory]
    [InlineData(1000)]
    [InlineData(10000)]
    [InlineData(50000)]
    public void SphereFunction_ResultsMatchSequential(int dimensions)
    {
        AssertMatchesSequential(TestFunctions.Sphere, TestFunctions.Filled(dimensions, 5.0), 0.05);
    }

    [Theory]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(5000)]
    public void RosenbrockFunction_ResultsMatchSequential(int dimensions)
    {
        AssertMatchesSequential(TestFunctions.Rosenbrock, TestFunctions.RosenbrockStart(dimensions, -1.2), 0.0001);
    }

    [Theory]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(5000)]
    public void SumSquaresFunction_ResultsMatchSequential(int dimensions)
    {
        AssertMatchesSequential(TestFunctions.SumSquares, TestFunctions.Filled(dimensions, 2.0), 1.0 / (2.0 * dimensions));
    }

    [Theory]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(5000)]
    public void RastriginFunction_ResultsMatchSequential(int dimensions)
    {
        AssertMatchesSequential(TestFunctions.Rastrigin, TestFunctions.Filled(dimensions, 1.0), 0.001);
    }

    [Theory]
    [InlineData(20000, 1)]
    [InlineData(50000, 2)]
    public void RandomStartPoint_ResultsMatchSequential(int dimensions, int seed)
    {
        AssertMatchesSequential(TestFunctions.Sphere, TestFunctions.Random(dimensions, seed, -50.0, 50.0), 0.05);
    }

    [Theory]
    [InlineData(10007, 1)]
    [InlineData(10007, 3)]
    [InlineData(10007, 7)]
    [InlineData(10007, 12)]
    [InlineData(10007, 64)]
    [InlineData(5, 16)]
    public void UnevenPartition_ResultsMatchSequential(int dimensions, int taskCount)
    {
        AssertMatchesSequential(TestFunctions.Sphere, TestFunctions.Random(dimensions, taskCount, -10.0, 10.0), 0.05, taskCount);
    }

    [Fact]
    public void Constructor_NonPositiveTaskCount_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ParallelGradientCalculator(0));
    }

    private static void AssertMatchesSequential(
        Func<double[], double> function,
        double[] start,
        double learningRate,
        int? taskCount = null)
    {
        double[] expected = new SequentialGradientDescent().Minimize(function, start, learningRate, MaxIterations, Precision);
        double[] actual = new ParallelGradientDescent(taskCount).Minimize(function, start, learningRate, MaxIterations, Precision);

        Assert.Equal(expected.Length, actual.Length);
        for (int i = 0; i < expected.Length; i++)
        {
            Assert.True(Math.Abs(expected[i] - actual[i]) < Tolerance,
                $"Невідповідність на індексі {i}: послідовний = {expected[i]}, паралельний = {actual[i]}");
        }
    }
}
