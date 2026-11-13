namespace GradientDescent;

public interface IGradientDescentOptimizer
{
    /// <summary>
    /// Мінімізує багатовимірну функцію методом градієнтного спуску.
    /// </summary>
    /// <param name="function">Функція для мінімізації.</param>
    /// <param name="initialPoint">Початкова точка.</param>
    /// <param name="learningRate">Швидкість навчання (множник кроку).</param>
    /// <param name="maxIterations">Максимальна кількість ітерацій.</param>
    /// <param name="precision">Поріг евклідової норми градієнта, нижче якого пошук завершується.</param>
    /// <returns>Наближення точки мінімуму.</returns>
    double[] Minimize(
        Func<double[], double> function,
        double[] initialPoint,
        double learningRate,
        int maxIterations,
        double precision);
}
