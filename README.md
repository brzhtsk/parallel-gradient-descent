# Паралельний градієнтний спуск (C#)

Алгоритм пошуку оптимальних значень методом градієнтного спуску та його паралельна реалізація мовою C#.

Градієнт обчислюється методом центральної скінченної різниці. У паралельній версії координати
вектора діляться на неперетинні блоки, кожен з яких обробляється окремою підзадачею у пулі потоків
.NET (`ThreadPool`); завершення підзадач очікується за допомогою `CountdownEvent`.

## Структура

- `src/GradientDescent` — бібліотека алгоритму:
  - `FiniteDifference` — спільне для обох версій обчислення часткових похідних;
  - `GradientDescentOptimizer` — спільний цикл градієнтного спуску;
  - `SequentialGradientCalculator`, `ParallelGradientCalculator` — послідовне та паралельне обчислення градієнта;
  - `SequentialGradientDescent`, `ParallelGradientDescent` — готові до використання оптимізатори.
- `tests/GradientDescent.Tests` — модульні тести (xUnit).
- `benchmarks/GradientDescent.Benchmarks` — експериментальне дослідження швидкодії.
- `plots/plot_results.py` — побудова графіків за результатами дослідження.

## Запуск

Потрібні .NET 10 SDK та Python 3 з пакетами `pandas`, `matplotlib`.

```
dotnet test
dotnet run -c Release --project benchmarks/GradientDescent.Benchmarks/
python plots/plot_results.py
```

Результати дослідження зберігаються у `bench_results/results_<дата>_<час>.csv`
(колонки: назва алгоритму, розмірність, кількість підзадач, середній час у мс за 20 прогонів).
