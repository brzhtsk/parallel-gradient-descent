using System.Diagnostics;
using System.Globalization;

namespace GradientDescent.Benchmarks;

public static class Program
{
    private static readonly int[] Dimensions = { 100, 1000, 5000, 10000, 25000, 50000, 100000 };
    private static readonly int[] TaskCounts = { 1, 2, 4, 6, 8, 12, 16, 24 };

    private const int MeasuredRuns = 20;
    private const int MaxIterations = 10;
    private const double LearningRate = 0.05;
    private const double Precision = 1e-6;
    private const double StartValue = 5.0;

    private static readonly Func<double[], double> Sphere = x =>
    {
        double sum = 0.0;
        for (int i = 0; i < x.Length; i++)
        {
            sum += x[i] * x[i];
        }
        return sum;
    };

    public static void Main()
    {
#if DEBUG
        Console.WriteLine("УВАГА: збірка Debug. Для вимірювань оберіть конфігурацію Release.");
#endif
        if (Debugger.IsAttached)
        {
            Console.WriteLine("УВАГА: підключено відлагоджувач. Запускайте без відлагодження (Ctrl+F5).");
        }

        string resultsDirectory = Path.Combine(FindRepositoryRoot(), "bench_results");
        Directory.CreateDirectory(resultsDirectory);
        string csvPath = Path.Combine(resultsDirectory, $"results_{DateTime.Now:yyyyMMdd_HHmmss}.csv");

        using var writer = new StreamWriter(csvPath);
        writer.WriteLine("name,n,threads,time");

        Console.WriteLine($"Логічних процесорів: {Environment.ProcessorCount}");
        Console.WriteLine("Прогрів середовища виконання .NET...");
        Warmup();

        foreach (int n in Dimensions)
        {
            Console.WriteLine($"\nРозмірність задачі: {n}");
            double[] start = CreateStartPoint(n);

            foreach (int tasks in TaskCounts)
            {
                ThreadPool.SetMinThreads(tasks, tasks);
                Measure("Parallel", n, tasks, new ParallelGradientDescent(tasks), start, writer);
            }

            Measure("Sequential", n, 1, new SequentialGradientDescent(), start, writer);
        }

        Console.WriteLine($"\nРезультати збережено у {csvPath}");
    }

    private static void Warmup()
    {
        double[] start = CreateStartPoint(1000);
        var sequential = new SequentialGradientDescent();
        var parallel = new ParallelGradientDescent(Environment.ProcessorCount);

        for (int i = 0; i < 5; i++)
        {
            sequential.Minimize(Sphere, start, LearningRate, MaxIterations, Precision);
            parallel.Minimize(Sphere, start, LearningRate, MaxIterations, Precision);
        }
    }

    private static void Measure(
        string name,
        int n,
        int tasks,
        IGradientDescentOptimizer optimizer,
        double[] start,
        StreamWriter writer)
    {
        string label = name == "Sequential" ? name : $"{name} [{tasks,2} thr]";
        double totalMs = 0.0;

        for (int run = 0; run <= MeasuredRuns; run++)
        {
            Console.Write($"\r  {label,-20} [{run,2} / {MeasuredRuns}]");

            GC.Collect();
            GC.WaitForPendingFinalizers();

            long timestamp = Stopwatch.GetTimestamp();
            optimizer.Minimize(Sphere, start, LearningRate, MaxIterations, Precision);
            double elapsedMs = Stopwatch.GetElapsedTime(timestamp).TotalMilliseconds;

            if (run > 0)
            {
                totalMs += elapsedMs;
            }
        }

        double averageMs = totalMs / MeasuredRuns;
        Console.WriteLine($"\r  {label,-20} {averageMs,14:F2} ms");

        writer.WriteLine(string.Create(CultureInfo.InvariantCulture, $"{name},{n},{tasks},{averageMs}"));
        writer.Flush();
    }

    private static double[] CreateStartPoint(int n)
    {
        double[] point = new double[n];
        Array.Fill(point, StartValue);
        return point;
    }

    private static string FindRepositoryRoot()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory != null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "ParallelGd.slnx")))
            {
                return directory.FullName;
            }
        }
        return Directory.GetCurrentDirectory();
    }
}
