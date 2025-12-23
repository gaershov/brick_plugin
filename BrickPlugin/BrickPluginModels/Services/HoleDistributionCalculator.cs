using BrickPluginModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BrickPluginModels.Services
{
    /// <summary>
    /// Калькулятор распределения отверстий в кирпиче.
    /// Поддерживает два режима: прямое распределение (без смещения) 
    /// и шахматное распределение (со смещением чередующихся рядов).
    /// </summary>
    public class HoleDistributionCalculator
    {
        /// <summary>
        /// Рассчитывает максимальное количество отверстий для прямого распределения.
        /// </summary>
        /// <param name="length">Длина кирпича в миллиметрах.</param>
        /// <param name="width">Ширина кирпича в миллиметрах.</param>
        /// <param name="holeRadius">Радиус отверстия в миллиметрах.</param>
        /// <returns>Максимальное количество отверстий.</returns>
        public int CalculateMaxHolesStraight(double length, double width, double holeRadius)
        {
            var availableArea = BrickParameters.CalculateAvailableArea(length, width, holeRadius);

            if (availableArea.availableLength <= 0 || availableArea.availableWidth <= 0)
            {
                return 0;
            }

            int columns = (int)Math.Floor(
                (availableArea.availableLength + availableArea.minGap) /
                (availableArea.diameter + availableArea.minGap));

            int rows = (int)Math.Floor(
                (availableArea.availableWidth + availableArea.minGap) /
                (availableArea.diameter + availableArea.minGap));

            return Math.Max(1, columns) * Math.Max(1, rows);
        }

        /// <summary>
        /// Рассчитывает максимальное количество отверстий для шахматного распределения.
        /// </summary>
        /// <param name="length">Длина кирпича в миллиметрах.</param>
        /// <param name="width">Ширина кирпича в миллиметрах.</param>
        /// <param name="holeRadius">Радиус отверстия в миллиметрах.</param>
        /// <returns>Максимальное количество отверстий.</returns>
        public int CalculateMaxHolesStaggered(double length, double width, double holeRadius)
        {
            var availableArea = BrickParameters.CalculateAvailableArea(length, width, holeRadius);

            if (availableArea.availableLength <= 0 || availableArea.availableWidth <= 0)
            {
                return 0;
            }

            double horizontalStep = availableArea.diameter + availableArea.minGap;
            double verticalStep = availableArea.diameter + availableArea.minGap;
            double staggerOffset = horizontalStep / 2.0;

            // Рассчитываем максимально возможное количество рядов по физическим ограничениям
            int maxRows = Math.Max(1,
                (int)Math.Floor((availableArea.availableWidth + verticalStep)
                / verticalStep));

            int columnsInEvenRows = Math.Max(1,
                (int)Math.Floor((availableArea.availableLength + availableArea.minGap)
                / horizontalStep));

            int columnsInOddRows = Math.Max(1,
                (int)Math.Floor((availableArea.availableLength -
                staggerOffset + availableArea.minGap) / horizontalStep));

            int evenRowsCount = (maxRows + 1) / 2;
            int oddRowsCount = maxRows / 2;

            return evenRowsCount * columnsInEvenRows + oddRowsCount * columnsInOddRows;
        }

        /// <summary>
        /// Вычисляет распределение отверстий для прямого размещения (без смещения).
        /// </summary>
        /// <param name="totalHoles">Общее количество отверстий.</param>
        /// <param name="length">Длина кирпича в миллиметрах.</param>
        /// <param name="width">Ширина кирпича в миллиметрах.</param>
        /// <param name="holeRadius">Радиус отверстия в миллиметрах.</param>
        /// <returns>Результат распределения отверстий по рядам.</returns>
        public HoleDistributionResult CalculateStraightDistribution(
            int totalHoles, double length, double width, double holeRadius)
        {
            var result = new HoleDistributionResult { StaggerOffset = 0 };
            var availableArea = BrickParameters.CalculateAvailableArea(length, width, holeRadius);

            if (totalHoles <= 1)
            {
                result.Rows = 1;
                result.Distribution.Add(totalHoles);
                result.MaxHolesInRow = totalHoles;
                return result;
            }

            int maxHolesPerRow = Math.Max(1,
                (int)Math.Floor((availableArea.availableLength + availableArea.minGap) /
                                (availableArea.diameter + availableArea.minGap)));

            int rowsCount = (int)Math.Ceiling((double)totalHoles / maxHolesPerRow);
            rowsCount = Math.Max(1, rowsCount);

            result.Rows = rowsCount;
            result.Distribution = DistributeEvenly(totalHoles, rowsCount);
            result.MaxHolesInRow = result.Distribution.Max();

            result.HorizontalGap = CalculateGap(
                result.MaxHolesInRow,
                availableArea.availableLength,
                availableArea.diameter,
                availableArea.minGap);

            result.VerticalGap = CalculateGap(
                rowsCount,
                availableArea.availableWidth,
                availableArea.diameter,
                availableArea.minGap);

            return result;
        }

        /// <summary>
        /// Вычисляет распределение отверстий для шахматного размещения (со смещением).
        /// </summary>
        /// <param name="totalHoles">Общее количество отверстий.</param>
        /// <param name="length">Длина кирпича в миллиметрах.</param>
        /// <param name="width">Ширина кирпича в миллиметрах.</param>
        /// <param name="holeRadius">Радиус отверстия в миллиметрах.</param>
        /// <returns>Результат распределения отверстий по рядам с параметрами смещения.</returns>
        public HoleDistributionResult CalculateStaggeredDistribution(
            int totalHoles, double length, double width, double holeRadius)
        {
            var result = new HoleDistributionResult();
            var availableArea = 
                BrickParameters.CalculateAvailableArea(length, width, holeRadius);

            double horizontalStep = availableArea.diameter + availableArea.minGap;
            double verticalStep = availableArea.diameter + availableArea.minGap;
            double staggerOffset = horizontalStep / 2.0;

            int maxPossibleRows = Math.Max(3,
                (int)Math.Floor((availableArea.availableWidth + verticalStep) 
                / verticalStep));

            int optimalRowsCount = FindOptimalRowsCount(
                totalHoles, maxPossibleRows, availableArea);

            result.Rows = optimalRowsCount;
            result.Distribution = DistributeAlternating(totalHoles, optimalRowsCount);
            result.MaxHolesInRow = result.Distribution.Max();
            result.HorizontalGap = availableArea.minGap;
            result.VerticalGap = verticalStep;
            result.StaggerOffset = staggerOffset;

            return result;
        }

        /// <summary>
        /// Находит оптимальное количество рядов для размещения заданного количества отверстий.
        /// </summary>
        /// <param name="totalHoles">Общее количество отверстий.</param>
        /// <param name="maxRows">Максимально возможное количество рядов.</param>
        /// <param name="availableArea">Параметры доступной области для размещения.</param>
        /// <returns>Оптимальное количество рядов.</returns>
        private int FindOptimalRowsCount(
            int totalHoles,
            int maxRows,
            (double diameter, double edgeMargin, double minGap,
             double availableLength, double availableWidth) availableArea)
        {
            // начинаем с минимум 2 рядов
            // если физически помещается только 1 ряд, используем 1
            int minRows = Math.Min(2, maxRows);

            for (int candidateRows = minRows; candidateRows <= maxRows; candidateRows++)
            {
                if (CanFitHoles(totalHoles, candidateRows, availableArea))
                {
                    return candidateRows;
                }
            }

            return maxRows;
        }

        /// <summary>
        /// Проверяет, может ли заданное количество отверстий поместиться в указанное количество рядов.
        /// </summary>
        /// <param name="totalHoles">Общее количество отверстий.</param>
        /// <param name="rows">Количество рядов.</param>
        /// <param name="availableArea">Параметры доступной области для размещения.</param>
        /// <returns>True, если отверстия помещаются; иначе False.</returns>
        private bool CanFitHoles(
            int totalHoles,
            int rows,
            (double diameter, double edgeMargin, double minGap,
             double availableLength, double availableWidth) availableArea)
        {
            double horizontalStep = availableArea.diameter + availableArea.minGap;
            double staggerOffset = horizontalStep / 2.0;

            int columnsInEvenRows = Math.Max(1,
                (int)Math.Floor((availableArea.availableLength +
                availableArea.minGap) / horizontalStep));

            int columnsInOddRows = Math.Max(1,
                (int)Math.Floor((availableArea.availableLength -
                staggerOffset + availableArea.minGap) / horizontalStep));

            int evenRowsCount = (rows + 1) / 2;
            int oddRowsCount = rows / 2;

            int totalCapacity = evenRowsCount * columnsInEvenRows +
                oddRowsCount * columnsInOddRows;

            return totalCapacity >= totalHoles;
        }

        /// <summary>
        /// Равномерно распределяет отверстия по рядам.
        /// </summary>
        /// <param name="totalHoles">Общее количество отверстий.</param>
        /// <param name="rows">Количество рядов.</param>
        /// <returns>Список с количеством отверстий в каждом ряду.</returns>
        private List<int> DistributeEvenly(int totalHoles, int rows)
        {
            var distributionList = new List<int>(rows);
            int baseCountPerRow = totalHoles / rows;
            int remainderHoles = totalHoles % rows;

            for (int i = 0; i < rows; i++)
            {
                int holesInCurrentRow = baseCountPerRow + (i < remainderHoles ? 1 : 0);
                distributionList.Add(holesInCurrentRow);
            }

            return distributionList;
        }

        /// <summary>
        /// Распределяет отверстия с чередованием количества по рядам (например: 6-7-6-7-6-7).
        /// </summary>
        /// <param name="totalHoles">Общее количество отверстий.</param>
        /// <param name="rows">Количество рядов.</param>
        /// <returns>Список с количеством отверстий в каждом ряду.</returns>
        private List<int> DistributeAlternating(int totalHoles, int rows)
        {
            var distribution = new int[rows];
            int baseCountPerRow = totalHoles / rows;
            int remainderHoles = totalHoles % rows;

            for (int i = 0; i < rows; i++)
            {
                distribution[i] = baseCountPerRow;
            }

            for (int i = 1; i < rows && remainderHoles > 0; i += 2)
            {
                distribution[i]++;
                remainderHoles--;
            }

            for (int i = 0; i < rows && remainderHoles > 0; i += 2)
            {
                distribution[i]++;
                remainderHoles--;
            }

            return distribution.ToList();
        }

        /// <summary>
        /// Вычисляет зазор между отверстиями на основе доступного пространства.
        /// </summary>
        /// <param name="count">Количество отверстий в ряду.</param>
        /// <param name="availableSpace">Доступное пространство для размещения.</param>
        /// <param name="diameter">Диаметр отверстия.</param>
        /// <param name="minimumGap">Минимальный допустимый зазор.</param>
        /// <returns>Рассчитанный зазор между отверстиями.</returns>
        private double CalculateGap(int count, double availableSpace,
            double diameter, double minimumGap)
        {
            if (count <= 1)
            {
                return 0;
            }

            double freeSpace = availableSpace - count * diameter;
            double calculatedGap = freeSpace / (count - 1);

            return Math.Max(minimumGap, calculatedGap);
        }
    }
}