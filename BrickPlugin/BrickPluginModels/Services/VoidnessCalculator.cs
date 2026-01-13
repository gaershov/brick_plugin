using BrickPluginModels.Models;
using System;

namespace BrickPluginModels.Services
{
    /// <summary>
    /// Калькулятор для работы с пустотностью кирпича.
    /// </summary>
    public class VoidnessCalculator
    {
        /// <summary>
        /// Максимально допустимая пустотность в процентах.
        /// </summary>
        private const double MaxVoidnessPercent = 45.0;

        /// <summary>
        /// Минимальный отступ от края кирпича до отверстия.
        /// </summary>
        private const double MinimumEdgeMargin = 15.0;

        /// <summary>
        /// Минимальный радиус отверстия для расчетов.
        /// </summary>
        private const double MinSearchRadius = 2.0;

        /// <summary>
        /// Шаг поиска радиуса при оптимизации.
        /// </summary>
        private const double RadiusSearchStep = 0.5;

        /// <summary>
        /// Калькулятор распределения отверстий.
        /// </summary>
        private readonly HoleDistributionCalculator _distributionCalculator;

        /// <summary>
        /// Инициализирует новый экземпляр класса VoidnessCalculator.
        /// </summary>
        public VoidnessCalculator()
        {
            _distributionCalculator = new HoleDistributionCalculator();
        }

        /// <summary>
        /// Рассчитывает текущую пустотность кирпича.
        /// </summary>
        /// <param name="length">Длина кирпича.</param>
        /// <param name="width">Ширина кирпича.</param>
        /// <param name="height">Высота кирпича.</param>
        /// <param name="holeRadius">Радиус отверстия.</param>
        /// <param name="holesCount">Количество отверстий.</param>
        /// <returns>Пустотность в процентах.</returns>
        public double CalculateCurrentVoidness(double length, double width, double height,
            double holeRadius, int holesCount)
        {
            double brickVolume = length * width * height;
            if (brickVolume <= 0)
            {
                return 0;
            }

            double holeVolume = Math.PI * holeRadius * holeRadius * height;
            double totalHolesVolume = holeVolume * holesCount;

            return (totalHolesVolume / brickVolume) * 100.0;
        }

        /// <summary>
        /// Рассчитывает максимально возможную пустотность для заданных параметров.
        /// </summary>
        /// <param name="length">Длина кирпича.</param>
        /// <param name="width">Ширина кирпича.</param>
        /// <param name="height">Высота кирпича.</param>
        /// <param name="holeRadius">Радиус отверстия.</param>
        /// <param name="distributionType">Тип распределения отверстий.</param>
        /// <returns>Максимальная пустотность в процентах.</returns>
        public double CalculateMaxPossibleVoidness(double length,
            double width, double height,
            double holeRadius, HoleDistributionType distributionType)
        {
            int maxHoles = distributionType == HoleDistributionType.Straight
                ? _distributionCalculator.CalculateMaxHolesStraight
                (length, width, holeRadius)
                : _distributionCalculator.CalculateMaxHolesStaggered
                (length, width, holeRadius);

            return CalculateCurrentVoidness(length, width, height, holeRadius, maxHoles);
        }

        /// <summary>
        /// Рассчитывает минимально возможную пустотность для заданных параметров.
        /// </summary>
        /// <param name="length">Длина кирпича.</param>
        /// <param name="width">Ширина кирпича.</param>
        /// <param name="height">Высота кирпича.</param>
        /// <param name="holeRadius">Радиус отверстия.</param>
        /// <returns>Минимальная пустотность в процентах.</returns>
        public double CalculateMinPossibleVoidness(double length,
            double width, double height, double holeRadius)
        {
            return CalculateCurrentVoidness(length, width, height, holeRadius, 1);
        }

        /// <summary>
        /// Подбирает оптимальные параметры отверстий для достижения заданной пустотности.
        /// </summary>
        /// <param name="length">Длина кирпича.</param>
        /// <param name="width">Ширина кирпича.</param>
        /// <param name="height">Высота кирпича.</param>
        /// <param name="targetVoidness">Желаемая пустотность в процентах.</param>
        /// <param name="distributionType">Тип распределения отверстий.</param>
        /// <returns>Результат расчета.</returns>
        public VoidnessCalculationResult CalculateOptimalParameters
            (double length, double width,
            double height, double targetVoidness, HoleDistributionType distributionType)
        {
            if (targetVoidness > MaxVoidnessPercent || targetVoidness <= 0)
            {
                return new VoidnessCalculationResult
                {
                    Success = false,
                    ErrorMessage = $"Пустотность должна быть в диапазоне " +
                    $"(0, {MaxVoidnessPercent}]%"
                };
            }

            // Максимальный радиус по геометрии
            double geometricMaxRadius = (width - 2 * MinimumEdgeMargin) / 2.0;

            // Максимальный радиус по пустотности (не более 45% при 1 отверстии)
            double voidnessMaxRadius
                = Math.Sqrt(MaxVoidnessPercent / 100.0 * length * width / Math.PI);

            // Берём минимум из двух ограничений
            double maxRadius = Math.Min(geometricMaxRadius, voidnessMaxRadius);

            VoidnessCalculationResult bestResult = null;
            double minDifference = double.MaxValue;

            for (double radius = MinSearchRadius;
                radius <= maxRadius; radius += RadiusSearchStep)
            {
                int maxHoles = distributionType == HoleDistributionType.Straight
                    ? _distributionCalculator.CalculateMaxHolesStraight
                    (length, width, radius)
                    : _distributionCalculator.CalculateMaxHolesStaggered
                    (length, width, radius);


                for (int holes = 1; holes <= maxHoles; holes++)
                {
                    double voidness =
                        CalculateCurrentVoidness(length, width, height, radius, holes);
                    double difference = Math.Abs(voidness - targetVoidness);

                    if (difference < minDifference && voidness <= MaxVoidnessPercent)
                    {
                        minDifference = difference;
                        bestResult = new VoidnessCalculationResult
                        {
                            HoleRadius = radius,
                            HolesCount = holes,
                            ActualVoidness = voidness,
                            Success = true
                        };

                        if (difference < 0.1)
                        {
                            return bestResult;
                        }
                    }
                }
            }

            if (bestResult == null)
            {
                return new VoidnessCalculationResult
                {
                    Success = false,
                    ErrorMessage = "Не удалось подобрать параметры" +
                    " для заданной пустотности"
                };
            }

            return bestResult;
        }

        /// <summary>
        /// Рассчитывает количество отверстий для 
        /// достижения заданной пустотности с фиксированным радиусом.
        /// </summary>
        /// <param name="length">Длина кирпича.</param>
        /// <param name="width">Ширина кирпича.</param>
        /// <param name="height">Высота кирпича.</param>
        /// <param name="holeRadius">Фиксированный радиус отверстия.</param>
        /// <param name="targetVoidness">Желаемая пустотность в процентах.</param>
        /// <param name="distributionType">Тип распределения отверстий.</param>
        /// <returns>Результат расчета.</returns>
        public VoidnessCalculationResult CalculateHolesCountForVoidness
            (double length, double width,
            double height, double holeRadius,
            double targetVoidness, HoleDistributionType distributionType)
        {
            if (targetVoidness > MaxVoidnessPercent || targetVoidness <= 0)
            {
                return new VoidnessCalculationResult
                {
                    Success = false,
                    ErrorMessage = $"Пустотность должна быть в диапазоне " +
                    $"(0, {MaxVoidnessPercent}]%"
                };
            }

            int maxHoles = distributionType == HoleDistributionType.Straight
                ? _distributionCalculator.CalculateMaxHolesStraight
                (length, width, holeRadius)
                : _distributionCalculator.CalculateMaxHolesStaggered
                (length, width, holeRadius);

            if (maxHoles <= 0)
            {
                return new VoidnessCalculationResult
                {
                    Success = false,
                    ErrorMessage = "Невозможно разместить отверстия с заданным радиусом"
                };
            }

            double brickVolume = length * width * height;
            double holeVolume = Math.PI * holeRadius * holeRadius * height;
            double targetHolesVolume = (targetVoidness / 100.0) * brickVolume;
            int calculatedHoles = (int)Math.Round(targetHolesVolume / holeVolume);

            calculatedHoles = Math.Max(1, Math.Min(calculatedHoles, maxHoles));
            double actualVoidness
                = CalculateCurrentVoidness(length, width, height, holeRadius, calculatedHoles);

            if (actualVoidness > MaxVoidnessPercent)
            {
                return new VoidnessCalculationResult
                {
                    Success = false,
                    ErrorMessage = $"Достижимая пустотность " +
                    $"({actualVoidness:F2}%) превышает максимум ({MaxVoidnessPercent}%)"
                };
            }

            return new VoidnessCalculationResult
            {
                HoleRadius = holeRadius,
                HolesCount = calculatedHoles,
                ActualVoidness = actualVoidness,
                Success = true
            };
        }

        /// <summary>
        /// Получает диапазон возможной пустотности для заданного радиуса.
        /// </summary>
        /// <param name="length">Длина кирпича.</param>
        /// <param name="width">Ширина кирпича.</param>
        /// <param name="height">Высота кирпича.</param>
        /// <param name="holeRadius">Радиус отверстия.</param>
        /// <param name="distributionType">Тип распределения отверстий.</param>
        /// <returns>Кортеж с минимальной и максимальной пустотностью.</returns>
        public (double min, double max) GetVoidnessRange
            (double length, double width, double height,
            double holeRadius, HoleDistributionType distributionType)
        {
            double min
                = CalculateMinPossibleVoidness(length, width, height, holeRadius);
            double max = CalculateMaxPossibleVoidness
                (length, width, height, holeRadius, distributionType);

            max = Math.Min(max, MaxVoidnessPercent);

            return (min, max);
        }
    }
}