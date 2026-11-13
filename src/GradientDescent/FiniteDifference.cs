namespace GradientDescent;

/// <summary>
/// Обчислення часткових похідних методом центральної скінченної різниці.
/// Спільний код послідовного та паралельного калькуляторів градієнта.
/// </summary>
public static class FiniteDifference
{
    /// <summary>Крок диференціювання.</summary>
    public const double Step = 1e-6;

    /// <summary>
    /// Обчислює часткові похідні функції в точці для координат з індексами [start, end)
    /// і записує їх у відповідні елементи масиву градієнта.
    /// Вхідна точка не змінюється, тому метод безпечно викликати з кількох потоків
    /// для діапазонів, що не перетинаються.
    /// </summary>
    public static void ComputePartialDerivatives(
        Func<double[], double> function,
        double[] point,
        double[] gradient,
        int start,
        int end)
    {
        double[] probe = (double[])point.Clone();

        for (int i = start; i < end; i++)
        {
            double original = probe[i];

            probe[i] = original + Step;
            double valuePlus = function(probe);

            probe[i] = original - Step;
            double valueMinus = function(probe);

            probe[i] = original;
            gradient[i] = (valuePlus - valueMinus) / (2.0 * Step);
        }
    }
}
