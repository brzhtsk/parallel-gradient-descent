namespace GradientDescent;

/// <summary>
/// Послідовний градієнтний спуск.
/// </summary>
public class SequentialGradientDescent : GradientDescentOptimizer
{
    public SequentialGradientDescent()
        : base(new SequentialGradientCalculator())
    {
    }
}
