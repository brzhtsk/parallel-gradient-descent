namespace GradientDescent;

/// <summary>
/// Паралельний градієнтний спуск.
/// </summary>
public class ParallelGradientDescent : GradientDescentOptimizer
{
    /// <param name="taskCount">Кількість підзадач; за замовчуванням дорівнює кількості логічних процесорів.</param>
    public ParallelGradientDescent(int? taskCount = null)
        : base(new ParallelGradientCalculator(taskCount ?? Environment.ProcessorCount))
    {
    }
}
