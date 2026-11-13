namespace GradientDescent;

public interface IGradientCalculator
{
    /// <summary>
    /// Обчислює вектор градієнта багатовимірної функції в заданій точці.
    /// </summary>
    /// <param name="function">Цільова функція.</param>
    /// <param name="point">Точка, в якій обчислюється градієнт.</param>
    /// <returns>Вектор часткових похідних.</returns>
    double[] ComputeGradient(Func<double[], double> function, double[] point);
}
