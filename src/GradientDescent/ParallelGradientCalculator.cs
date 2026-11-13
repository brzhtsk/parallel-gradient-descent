namespace GradientDescent;

/// <summary>
/// Паралельне обчислення градієнта у пулі потоків .NET.
/// Координати діляться на неперетинні блоки, кожен блок обробляється окремою підзадачею
/// з власною копією точки, тому спільні дані не потребують блокувань.
/// Кількість підзадач визначає максимальну кількість одночасно задіяних потоків.
/// </summary>
public class ParallelGradientCalculator : IGradientCalculator
{
    private readonly int taskCount;

    public ParallelGradientCalculator(int taskCount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(taskCount, 1);
        this.taskCount = taskCount;
    }

    public double[] ComputeGradient(Func<double[], double> function, double[] point)
    {
        int dimensions = point.Length;
        double[] gradient = new double[dimensions];
        if (dimensions == 0)
        {
            return gradient;
        }

        int chunkSize = (dimensions + taskCount - 1) / taskCount;
        int chunkCount = (dimensions + chunkSize - 1) / chunkSize;

        using var countdown = new CountdownEvent(chunkCount);
        for (int chunk = 0; chunk < chunkCount; chunk++)
        {
            int start = chunk * chunkSize;
            int end = Math.Min(start + chunkSize, dimensions);

            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    FiniteDifference.ComputePartialDerivatives(function, point, gradient, start, end);
                }
                finally
                {
                    countdown.Signal();
                }
            });
        }
        countdown.Wait();

        return gradient;
    }
}
