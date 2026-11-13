import pandas as pd
import matplotlib.pyplot as plt
import os
import glob

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
PLOTS_DIR = os.path.join(ROOT, 'plots')
RESULTS_DIR = os.path.join(ROOT, 'bench_results')


def main():
    os.makedirs(PLOTS_DIR, exist_ok=True)

    # Знаходимо найновіший файл результатів
    list_of_files = glob.glob(os.path.join(RESULTS_DIR, '*.csv'))
    if not list_of_files:
        print("Помилка: Не знайдено жодного CSV файлу в папці bench_results/")
        return
    
    latest_file = max(list_of_files, key=os.path.getmtime)
    print(f"Зчитування даних з файлу: {latest_file}")

    # Завантажуємо дані
    df = pd.read_csv(latest_file)

    # ---------------------------------------------------------
    # 1. Графік часу виконання послідовного алгоритму
    # ---------------------------------------------------------
    df_seq = df[df['name'] == 'Sequential'].copy()
    df_seq = df_seq.sort_values(by='n')

    plt.figure(figsize=(10, 6))
    plt.plot(df_seq['n'], df_seq['time'], marker='o', linestyle='-', color='red', linewidth=2)
    plt.title('Час виконання послідовного алгоритму від розмірності', fontsize=14)
    plt.xlabel('Розмірність задачі (N)', fontsize=12)
    plt.ylabel('Середній час виконання (мс)', fontsize=12)
    plt.grid(True, linestyle='--', alpha=0.7)
    
    plt.tight_layout()
    seq_plot_path = os.path.join(PLOTS_DIR, 'sequential_time.png')
    plt.savefig(seq_plot_path, dpi=300)
    print(f"Графік успішно збережено: {seq_plot_path}")
    plt.close()

    # ---------------------------------------------------------
    # 2. Графік прискорення паралельних алгоритмів (Speedup)
    # ---------------------------------------------------------
    # Прискорення (Speedup) обчислюється як S = T_seq / T_par
    
    # Створюємо словник часу послідовного виконання для кожної розмірності (примусово приводимо ключі до int)
    seq_times = dict(zip(df_seq['n'].astype(int), df_seq['time']))

    df_par = df[df['name'] == 'Parallel'].copy()
    
    # Додаємо стовпець прискорення
    df_par['speedup'] = df_par.apply(
        lambda row: seq_times[int(row['n'])] / float(row['time']) if int(row['n']) in seq_times else 0, 
        axis=1
    )

    plt.figure(figsize=(12, 7))

    # Додаємо Baseline (послідовний алгоритм має прискорення = 1)
    baseline_y = [1] * len(df_seq['n'])
    plt.plot(df_seq['n'], baseline_y, color='black', linestyle='--', linewidth=2, label='Baseline (Sequential = 1x)')

    # Малюємо лінії для кожної кількості потоків
    threads_list = sorted(df_par['threads'].unique())
    
    # Використовуємо різні кольори/маркери для потоків
    for t in threads_list:
        df_t = df_par[df_par['threads'] == t].sort_values(by='n')
        plt.plot(df_t['n'], df_t['speedup'], marker='s', linewidth=2, label=f'Parallel [{t} підзадач]')

    plt.title('Прискорення паралельного алгоритму від розмірності задачі (Speedup)', fontsize=14)
    plt.xlabel('Розмірність задачі (N)', fontsize=12)
    plt.ylabel('Прискорення (в X разів)', fontsize=12)
    plt.legend(bbox_to_anchor=(1.05, 1), loc='upper left')
    plt.grid(True, linestyle='--', alpha=0.7)
    
    plt.tight_layout()
    speedup_plot_path = os.path.join(PLOTS_DIR, 'parallel_speedup.png')
    plt.savefig(speedup_plot_path, dpi=300)
    print(f"Графік успішно збережено: {speedup_plot_path}")
    plt.close()

if __name__ == "__main__":
    main()
