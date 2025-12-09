using System;
using System.Collections.Generic;
using System.Linq;
using BrickPlugin.Models;
using Kompas6API5;
using Kompas6Constants3D;

namespace BrickPlugin.Services
{
    /// <summary>
    /// Строит 3D-модель кирпича с отверстиями в КОМПАС-3D.
    /// </summary>
    public class BrickBuilder
    {
        /// <summary>
        /// Обертка для работы с API КОМПАС-3D.
        /// </summary>
        private readonly KompasWrapper _kompasWrapper;

        /// <summary>
        /// Инициализирует новый экземпляр класса BrickBuilder.
        /// </summary>
        public BrickBuilder()
        {
            _kompasWrapper = new KompasWrapper();
        }

        /// <summary>
        /// Строит 3D-модель кирпича с заданными параметрами.
        /// </summary>
        /// <param name="parameters">Параметры кирпича.</param>
        public void Build(BrickParameters parameters)
        {
            _kompasWrapper.OpenKompas();
            _kompasWrapper.CreateDocument();
            BuildBody(parameters);
            BuildHoles(parameters);
        }

        /// <summary>
        /// Строит тело кирпича (параллелепипед).
        /// </summary>
        /// <param name="parameters">Параметры кирпича.</param>
        private void BuildBody(BrickParameters parameters)
        {
            double length = parameters[ParameterType.Length];
            double width = parameters[ParameterType.Width];
            double height = parameters[ParameterType.Height];

            ksEntity sketch = _kompasWrapper.CreateSketchOnXOY();
            ksDocument2D document2D = _kompasWrapper.BeginSketch(sketch);

            double leftX = -length / 2;
            double bottomY = -width / 2;
            double rightX = length / 2;
            double topY = width / 2;

            document2D.ksLineSeg(leftX, bottomY, rightX, bottomY, 1);
            document2D.ksLineSeg(rightX, bottomY, rightX, topY, 1);
            document2D.ksLineSeg(rightX, topY, leftX, topY, 1);
            document2D.ksLineSeg(leftX, topY, leftX, bottomY, 1);

            _kompasWrapper.EndSketch(sketch);
            _kompasWrapper.Extrude(sketch, height);
        }

        /// <summary>
        /// Строит отверстия в кирпиче.
        /// </summary>
        /// <param name="parameters">Параметры кирпича.</param>
        private void BuildHoles(BrickParameters parameters)
        {
            int count = (int)parameters[ParameterType.HolesCount];
            if (count <= 0)
            {
                return;
            }

            double radius = parameters[ParameterType.HoleRadius];
            if (radius <= 0)
            {
                return;
            }

            double length = parameters[ParameterType.Length];
            double width = parameters[ParameterType.Width];

            ksEntity sketch = _kompasWrapper.CreateSketchOnXOY();
            ksDocument2D document2D = _kompasWrapper.BeginSketch(sketch);

            PlaceHoles(document2D, count, radius, length, width);

            _kompasWrapper.EndSketch(sketch);
            _kompasWrapper.Cut(sketch);
        }

        /// <summary>
        /// Размещает отверстия на плоскости эскиза с учетом оптимального распределения.
        /// </summary>
        /// <param name="document2D">2D-документ для рисования.</param>
        /// <param name="count">Количество отверстий.</param>
        /// <param name="radius">Радиус отверстия.</param>
        /// <param name="length">Длина кирпича.</param>
        /// <param name="width">Ширина кирпича.</param>
        private void PlaceHoles(ksDocument2D document2D, int count, double radius,
            double length, double width)
        {
            var area = BrickParameters.CalculateAvailableArea(length, width, radius);

            if (count == 1)
            {
                document2D.ksCircle(0, 0, radius, 1);
                return;
            }

            int maxPerRow = (int)Math.Floor(
                (area.availableLength + area.minGap) / (area.diameter + area.minGap));
            int maxRows = (int)Math.Floor(
                (area.availableWidth + area.minGap) / (area.diameter + area.minGap));

            if (maxPerRow <= 0 || maxRows <= 0)
            {
                return;
            }

            int rows = DetermineRowCount(count, maxPerRow, maxRows);
            List<int> distribution = DistributeHolesAcrossRows(count, rows);

            int maxHolesInRow = distribution.Max();

            double horizontalGap = CalculateHorizontalGap(
                maxHolesInRow, area.diameter, area.availableLength, area.minGap);
            double verticalGap = CalculateVerticalGap(
                rows, area.diameter, area.availableWidth, area.minGap);

            double startY = ((rows - 1) * (area.diameter + verticalGap)) / 2.0;

            double halfAvailableLength = area.availableLength / 2.0;
            double halfAvailableWidth = area.availableWidth / 2.0;

            for (int row = 0; row < rows; row++)
            {
                int holesInRow = distribution[row];
                double currentY = startY - row * (area.diameter + verticalGap);

                double rowWidth = (holesInRow - 1) * (area.diameter + horizontalGap);
                double startX = -rowWidth / 2.0;

                for (int column = 0; column < holesInRow; column++)
                {
                    double currentX = startX + column * (area.diameter + horizontalGap);

                    double minX = -halfAvailableLength + radius;
                    double maxX = halfAvailableLength - radius;
                    double minY = -halfAvailableWidth + radius;
                    double maxY = halfAvailableWidth - radius;

                    currentX = Math.Max(minX, Math.Min(maxX, currentX));
                    currentY = Math.Max(minY, Math.Min(maxY, currentY));

                    document2D.ksCircle(currentX, currentY, radius, 1);
                }
            }
        }

        /// <summary>
        /// Рассчитывает горизонтальный зазор между отверстиями в ряду.
        /// </summary>
        /// <param name="maxHolesInRow">Максимальное количество отверстий в ряду.</param>
        /// <param name="diameter">Диаметр отверстия.</param>
        /// <param name="availableLength">Доступная длина для размещения.</param>
        /// <param name="minGap">Минимальный зазор.</param>
        /// <returns>Рассчитанный горизонтальный зазор.</returns>
        private double CalculateHorizontalGap(int maxHolesInRow, double diameter,
            double availableLength, double minGap)
        {
            if (maxHolesInRow <= 1)
            {
                return 0;
            }

            double totalHoleWidth = maxHolesInRow * diameter;
            double remainingSpace = availableLength - totalHoleWidth;
            return Math.Max(minGap, remainingSpace / (maxHolesInRow - 1));
        }

        /// <summary>
        /// Рассчитывает вертикальный зазор между рядами отверстий.
        /// </summary>
        /// <param name="rows">Количество рядов.</param>
        /// <param name="diameter">Диаметр отверстия.</param>
        /// <param name="availableWidth">Доступная ширина для размещения.</param>
        /// <param name="minGap">Минимальный зазор.</param>
        /// <returns>Рассчитанный вертикальный зазор.</returns>
        private double CalculateVerticalGap(int rows, double diameter,
            double availableWidth, double minGap)
        {
            if (rows <= 1)
            {
                return 0;
            }

            double totalHoleHeight = rows * diameter;
            double remainingSpace = availableWidth - totalHoleHeight;
            return Math.Max(minGap, remainingSpace / (rows - 1));
        }

        /// <summary>
        /// Определяет оптимальное количество рядов для размещения отверстий.
        /// </summary>
        /// <param name="totalHoles">Общее количество отверстий.</param>
        /// <param name="maxPerRow">Максимум отверстий в одном ряду.</param>
        /// <param name="maxRows">Максимальное количество рядов.</param>
        /// <returns>Оптимальное количество рядов.</returns>
        private int DetermineRowCount(int totalHoles, int maxPerRow, int maxRows)
        {
            int minimumRows = (int)Math.Ceiling((double)totalHoles / maxPerRow);
            minimumRows = Math.Max(1, Math.Min(minimumRows, maxRows));

            if (totalHoles % 2 == 1 && minimumRows == 2 && maxRows >= 3)
            {
                return 3;
            }

            return minimumRows;
        }

        /// <summary>
        /// Распределяет отверстия по рядам с симметричным размещением.
        /// </summary>
        /// <param name="totalHoles">Общее количество отверстий.</param>
        /// <param name="rows">Количество рядов.</param>
        /// <returns>Список с количеством отверстий в каждом ряду.</returns>
        private List<int> DistributeHolesAcrossRows(int totalHoles, int rows)
        {
            List<int> result = new List<int>();
            int baseCount = totalHoles / rows;
            int remainder = totalHoles % rows;

            for (int i = 0; i < rows; i++)
            {
                result.Add(baseCount);
            }

            if (remainder == 0)
            {
                return result;
            }

            if (remainder % 2 == 1)
            {
                int center = rows / 2;
                result[center]++;
                remainder--;
            }

            int left = 0;
            int right = rows - 1;

            while (remainder > 0)
            {
                if (left <= right && remainder > 0)
                {
                    result[left]++;
                    remainder--;
                }

                if (right > left && remainder > 0)
                {
                    result[right]++;
                    remainder--;
                }

                left++;
                right--;
            }

            return result;
        }
    }
}