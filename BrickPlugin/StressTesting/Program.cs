using System.Diagnostics;
using Microsoft.VisualBasic.Devices;
using BrickPluginModels.Models;
using BrickPlugin.Services;

namespace StressTesting
{
    /// <summary>
    /// Программа для нагрузочного тестирования плагина построения кирпичей.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Коэффициент преобразования байтов в гигабайты.
        /// </summary>
        private const double GigabyteInByte = 0.000000000931322574615478515625;

        /// <summary>
        /// Точка входа в программу.
        /// </summary>
        /// <param name="args">Аргументы командной строки.</param>
        static void Main(string[] args)
        {
            Console.WriteLine("=== Brick Plugin Stress Testing ===");
            Console.WriteLine();
            Console.WriteLine("Выберите режим тестирования:");
            Console.WriteLine("1 - Минимальные параметры");
            Console.WriteLine("2 - Средние параметры");
            Console.WriteLine("3 - Максимальные параметры");
            Console.WriteLine("4 - Все три режима последовательно");
            Console.Write("Ваш выбор: ");

            var choice = Console.ReadLine();

            Console.WriteLine();
            Console.WriteLine("Выберите режим длительности теста:");
            Console.WriteLine("1 - По количеству итераций");
            Console.WriteLine("2 - По времени (минуты)");
            Console.Write("Ваш выбор: ");

            var durationChoice = Console.ReadLine();

            int? buildCount = null;
            double? durationMinutes = null;

            if (durationChoice == "1")
            {
                Console.Write("\nВведите количество построений: ");
                if (!int.TryParse(Console.ReadLine(), out int count) || count <= 0)
                {
                    Console.WriteLine("Некорректное число! " +
                        "Используется значение по умолчанию: 1000");
                    buildCount = 1000;
                }
                else
                {
                    buildCount = count;
                }
            }
            else if (durationChoice == "2")
            {
                Console.Write("\nВведите длительность теста (минуты): ");
                if (!double.TryParse(Console.ReadLine(), out double minutes) || minutes <= 0)
                {
                    Console.WriteLine("Некорректная длительность! " +
                        "Используется значение по умолчанию: 5 минут");
                    durationMinutes = 5;
                }
                else
                {
                    durationMinutes = minutes;
                }
            }
            else
            {
                Console.WriteLine("Некорректный выбор! " +
                    "Используется режим по итерациям с 1000 построений");
                buildCount = 1000;
            }

            switch (choice)
            {
                case "1":
                    RunStressTest("minimal", 
                        GetMinimalParameters(), buildCount, durationMinutes);
                    break;
                case "2":
                    RunStressTest("average", 
                        GetAverageParameters(), buildCount, durationMinutes);
                    break;
                case "3":
                    RunStressTest("maximal", 
                        GetMaximalParameters(), buildCount, durationMinutes);
                    break;
                case "4":
                    RunStressTest("minimal", 
                        GetMinimalParameters(), buildCount, durationMinutes);
                    RunStressTest("average", 
                        GetAverageParameters(), buildCount, durationMinutes);
                    RunStressTest("maximal", 
                        GetMaximalParameters(), buildCount, durationMinutes);
                    break;
                default:
                    Console.WriteLine("Некорректный выбор!");
                    return;
            }

            Console.WriteLine();
            Console.WriteLine("Тестирование завершено!");
            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        /// <summary>
        /// Выполняет нагрузочное тестирование.
        /// </summary>
        /// <param name="testName">Название теста.</param>
        /// <param name="parameters">Параметры кирпича.</param>
        /// <param name="buildCount">Количество построений (если задано).</param>
        /// <param name="durationMinutes">Длительность теста в минутах (если задано).</param>
        private static void RunStressTest(string testName, BrickParameters parameters,
            int? buildCount, double? durationMinutes)
        {
            Console.WriteLine();
            Console.WriteLine($"=== Начало теста: {testName} ===");
            Console.WriteLine("Параметры:");
            Console.WriteLine($"  Длина: " +
                $"{parameters[ParameterType.Length]} мм");
            Console.WriteLine($"  Ширина: " +
                $"{parameters[ParameterType.Width]} мм");
            Console.WriteLine($"  Высота: " +
                $"{parameters[ParameterType.Height]} мм");
            Console.WriteLine($"  Радиус отверстия: " +
                $"{parameters[ParameterType.HoleRadius]} мм");
            Console.WriteLine($"  Количество отверстий: " +
                $"{parameters[ParameterType.HolesCount]}");
            Console.WriteLine($"  Тип распределения: " +
                $"{parameters.DistributionType}");

            if (buildCount.HasValue)
            {
                Console.WriteLine($"  Режим: {buildCount.Value} итераций");
            }
            else if (durationMinutes.HasValue)
            {
                Console.WriteLine($"  Режим: {durationMinutes.Value} минут");
            }
            Console.WriteLine();

            var fileName = $"log_{testName}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            var streamWriter = new StreamWriter(fileName);
            streamWriter.WriteLine("№\tВремя (мс)\tОЗУ (ГБ)");

            var builder = new BrickBuilder();
            var stopWatch = new Stopwatch();
            var testStopwatch = new Stopwatch();
            int count = 0;

            try
            {
                testStopwatch.Start();

                // Режим по количеству итераций
                if (buildCount.HasValue)
                {
                    for (count = 1; count <= buildCount.Value; count++)
                    {
                        stopWatch.Start();
                        builder.Build(parameters);
                        stopWatch.Stop();

                        // Закрываем созданный документ в КОМПАС
                        builder.CloseDocument();

                        var computerInfo = new ComputerInfo();
                        var usedMemory = (computerInfo.TotalPhysicalMemory
                                          - computerInfo.AvailablePhysicalMemory)
                                         * GigabyteInByte;

                        var timeMs = stopWatch.Elapsed.TotalMilliseconds;

                        streamWriter.WriteLine($"{count}\t{timeMs:F0}\t{usedMemory:F9}");
                        streamWriter.Flush();

                        // Выводим прогресс в консоль
                        if (count % 10 == 0 || count == buildCount.Value)
                        {
                            Console.Write($"\rПрогресс: {count}/{buildCount.Value} " +
                                         $"({(count * 100.0 / buildCount.Value):F1}%) | " +
                                         $"Время: {timeMs:F0} мс | " +
                                         $"ОЗУ: {usedMemory:F2} ГБ");
                        }

                        stopWatch.Reset();
                    }
                }
                // Режим по времени
                else if (durationMinutes.HasValue)
                {
                    var targetDuration = TimeSpan.FromMinutes(durationMinutes.Value);
                    count = 0;

                    while (testStopwatch.Elapsed < targetDuration)
                    {
                        count++;
                        stopWatch.Start();
                        builder.Build(parameters);
                        stopWatch.Stop();

                        // Закрываем созданный документ в КОМПАС
                        builder.CloseDocument();

                        var computerInfo = new ComputerInfo();
                        var usedMemory = (computerInfo.TotalPhysicalMemory
                                          - computerInfo.AvailablePhysicalMemory)
                                         * GigabyteInByte;

                        var timeMs = stopWatch.Elapsed.TotalMilliseconds;

                        streamWriter.WriteLine($"{count}\t{timeMs:F0}\t{usedMemory:F9}");
                        streamWriter.Flush();

                        // Выводим прогресс в консоль
                        if (count % 10 == 0)
                        {
                            var elapsed = testStopwatch.Elapsed;
                            var remaining = targetDuration - elapsed;
                            Console.Write($"\rПрогресс: {count} построений | " +
                                         $"Прошло: {elapsed.Minutes:D2}:{elapsed.Seconds:D2} / " +
                                         $"{durationMinutes.Value:F1} мин | " +
                                         $"Осталось: {remaining.Minutes:D2}" +
                                         $":{remaining.Seconds:D2} | " +
                                         $"Время: {timeMs:F0} мс | " +
                                         $"ОЗУ: {usedMemory:F2} ГБ");
                        }

                        stopWatch.Reset();
                    }
                }

                testStopwatch.Stop();
                Console.WriteLine(); // Переход на новую строку после завершения
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
            finally
            {
                streamWriter.Close();
                streamWriter.Dispose();

                var totalTime = testStopwatch.Elapsed;
                Console.WriteLine($"\nТест завершен. Построено моделей: {count}");
                Console.WriteLine($"Общее время: {totalTime.Minutes:D2}:{totalTime.Seconds:D2}");
                if (count > 0)
                {
                    Console.WriteLine($"Среднее время на построение: " +
                        $"{totalTime.TotalMilliseconds / count:F0} мс");
                }
                Console.WriteLine($"Результаты сохранены в файл: {fileName}");
            }
        }

        /// <summary>
        /// Получает минимальные параметры кирпича.
        /// </summary>
        /// <returns>Параметры кирпича.</returns>
        private static BrickParameters GetMinimalParameters()
        {
            var parameters = new BrickParameters();
            parameters[ParameterType.Length] = 180.0;
            parameters[ParameterType.Width] = 80.0;
            parameters[ParameterType.Height] = 50.0;
            parameters[ParameterType.HoleRadius] = 5.0;
            parameters[ParameterType.HolesCount] = 1;
            parameters.DistributionType = HoleDistributionType.Straight;
            return parameters;
        }

        /// <summary>
        /// Получает средние параметры кирпича.
        /// </summary>
        /// <returns>Параметры кирпича.</returns>
        private static BrickParameters GetAverageParameters()
        {
            var parameters = new BrickParameters();
            parameters[ParameterType.Length] = 240.0;
            parameters[ParameterType.Width] = 115.0;
            parameters[ParameterType.Height] = 70.0;
            parameters[ParameterType.HoleRadius] = 17.5;
            parameters[ParameterType.HolesCount] = 10;
            parameters.DistributionType = HoleDistributionType.Straight;
            return parameters;
        }

        /// <summary>
        /// Получает максимальные параметры кирпича.
        /// </summary>
        /// <returns>Параметры кирпича.</returns>
        private static BrickParameters GetMaximalParameters()
        {
            var parameters = new BrickParameters();
            parameters[ParameterType.Length] = 300.0;
            parameters[ParameterType.Width] = 150.0;
            parameters[ParameterType.Height] = 90.0;
            parameters[ParameterType.HoleRadius] = 30.0;
            parameters[ParameterType.HolesCount] = 20;
            parameters.DistributionType = HoleDistributionType.Staggered;
            return parameters;
        }
    }
}