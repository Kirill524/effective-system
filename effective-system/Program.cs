using System;

namespace MultithreadingHomework
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // виберіть метод для запуску

            // Task1_ParallelMax();
            // Task2_ParallelWrite();
            // Task3_ParallelSort();
            // Task4_BackgroundTimer();
            // await Task5_ParallelReadSum();
            // Task6_MultiplicationTable();
            // Task7_WordCount();
            // await Task8_DownloadSimulation();
            // Task9_StudentGrades();
            Task10_PrimeNumbers();

            Console.WriteLine("\n--- Програма завершена. гатисніть будь-яку клавішу ---");
            Console.ReadKey();
        }

        static void Task1_ParallelMax()
        {
            int[] arr = { 10, 45, 12, 99, 3, 56, 102, 8, 44, 1 };
            int max1 = 0, max2 = 0;
            int mid = arr.Length / 2;

            Thread t1 = new Thread(() => max1 = arr.Take(mid).Max());
            Thread t2 = new Thread(() => max2 = arr.Skip(mid).Max());

            t1.Start(); t2.Start();
            t1.Join(); t2.Join();

            Console.WriteLine($"Завдання 1: глобальний максимум = {Math.Max(max1, max2)}");
        }

        static void Task2_ParallelWrite()
        {
            List<Thread> threads = new List<Thread>();
            for (int i = 1; i <= 5; i++)
            {
                int id = i;
                Thread t = new Thread(() => {
                    string content = string.Join("\n", Enumerable.Range(1, 10).Select(r => $"Thread {id}, Line {r}"));
                    File.WriteAllText($"log{id}.txt", content);
                });
                threads.Add(t);
                t.Start();
            }
            threads.ForEach(t => t.Join());
            for (int i = 1; i <= 5; i++) Console.WriteLine($"Вміст log{i}.txt:\n{File.ReadAllText($"log{i}.txt")}");
        }

        static void Task3_ParallelSort()
        {
            int[] arr = { 20, 5, 1, 18, 3, 9, 14, 2, 7, 11, 4, 15, 6, 19, 8, 10, 13, 17, 12, 16 };
            List<int[]> parts = new List<int[]>();
            List<Thread> threads = new List<Thread>();

            for (int i = 0; i < 4; i++)
            {
                int start = i * 5;
                int[] sub = arr.Skip(start).Take(5).ToArray();
                parts.Add(sub);
                Thread t = new Thread(() => Array.Sort(sub));
                threads.Add(t);
                t.Start();
            }
            threads.ForEach(t => t.Join());
            var result = parts.SelectMany(x => x).OrderBy(x => x).ToArray();
            Console.WriteLine("Завдання 3: відсортований масив: " + string.Join(", ", result));
        }

        static void Task4_BackgroundTimer()
        {
            Thread timer = new Thread(() => {
                for (int i = 10; i >= 0; i--)
                {
                    Console.WriteLine($"[Timer]: {i}");
                    Thread.Sleep(500);
                }
            });
            timer.Start();
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine("головний потік працює...");
                Thread.Sleep(1000);
            }
        }

        static async Task Task5_ParallelReadSum()
        {
            for (int i = 1; i <= 4; i++) File.WriteAllText($"data{i}.txt", "10\n20\n30");
            long totalSum = 0;
            List<Task<int>> tasks = new List<Task<int>>();

            for (int i = 1; i <= 4; i++)
            {
                int id = i;
                tasks.Add(Task.Run(() => File.ReadAllLines($"data{id}.txt").Sum(int.Parse)));
            }
            var results = await Task.WhenAll(tasks);
            Console.WriteLine($"Завдання 5: загальна сума = {results.Sum()}");
        }

        static void Task6_MultiplicationTable()
        {
            for (int i = 2; i <= 10; i++)
            {
                int n = i;
                new Thread(() => {
                    var lines = Enumerable.Range(1, 10).Select(x => $"{n} * {x} = {n * x}");
                    File.WriteAllLines($"table_{n}.txt", lines);
                }).Start();
            }
            Thread.Sleep(1000);
            Console.WriteLine("Завдання 6: таблиця на 5:\n" + File.ReadAllText("table_5.txt"));
        }

        static void Task7_WordCount()
        {
            string text = "Це приклад тексту для розрахунку кількості слів у різних частинах паралельно";
            string[] words = text.Split(' ');
            int total = 0, partSize = words.Length / 3;

            Parallel.For(0, 3, i => {
                int count = words.Skip(i * partSize).Take(i == 2 ? words.Length : partSize).Count();
                Interlocked.Add(ref total, count);
            });
            Console.WriteLine($"Завдання 7: слів усього = {total}");
        }

        static async Task Task8_DownloadSimulation()
        {
            string[] files = { "file1", "file2", "file3", "file4", "file5" };
            var tasks = files.Select(async f => {
                int delay = new Random().Next(1000, 4000);
                await Task.Delay(delay);
                Console.WriteLine($"Завантажено {f} за {delay}мс");
            });
            await Task.WhenAll(tasks);
        }

        static void Task9_StudentGrades()
        {
            string[] students = { "S1", "S2", "S3", "S4", "S5", "S6", "S7", "S8", "S9", "S10", "S11", "S12" };
            Parallel.For(0, 3, i => {
                var group = students.Skip(i * 4).Take(4);
                double avg = new Random().Next(60, 100);
                File.WriteAllText($"group{i + 1}.txt", $"Average: {avg}");
            });
            Console.WriteLine("Завдання 9: оцінки груп розраховані.");
        }

        static void Task10_PrimeNumbers()
        {
            int[] numbers = Enumerable.Range(1, 1000).ToArray();
            List<int> primes = new List<int>();
            object lockObj = new object();

            Parallel.ForEach(numbers, n => {
                if (IsPrime(n))
                {
                    lock (lockObj) primes.Add(n);
                }
            });
            Console.WriteLine($"Завдання 10: знайдено {primes.Count} простих чисел.");
        }

        static bool IsPrime(int n)
        {
            if (n < 2) return false;
            for (int i = 2; i * i <= n; i++) if (n % i == 0) return false;
            return true;
        }
    }
}