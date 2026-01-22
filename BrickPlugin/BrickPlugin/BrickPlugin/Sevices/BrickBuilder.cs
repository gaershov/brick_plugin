using System;
using System.Collections.Generic;
using System.Linq;
using BrickPlugin.Models;
using Kompas6API5;
using Kompas6Constants3D;

namespace BrickPlugin.Services
{
    public class BrickBuilder
    {
        private readonly KompasWrapper _k;

        public BrickBuilder()
        {
            _k = new KompasWrapper();
        }

        public void Build(BrickParameters p)
        {
            _k.OpenKompas();
            _k.CreateDocument();
            BuildBody(p);
            BuildHoles(p);
        }

        private void BuildBody(BrickParameters p)
        {
            double L = p[ParameterType.Length];
            double W = p[ParameterType.Width];
            double H = p[ParameterType.Height];

            ksEntity sketch = _k.CreateSketchOnXOY();
            ksDocument2D d2 = _k.BeginSketch(sketch);

            double x1 = -L / 2;
            double y1 = -W / 2;
            double x2 = L / 2;
            double y2 = W / 2;

            d2.ksLineSeg(x1, y1, x2, y1, 1);
            d2.ksLineSeg(x2, y1, x2, y2, 1);
            d2.ksLineSeg(x2, y2, x1, y2, 1);
            d2.ksLineSeg(x1, y2, x1, y1, 1);

            _k.EndSketch(sketch);
            _k.Extrude(sketch, H);
        }

        private void BuildHoles(BrickParameters p)
        {
            int count = (int)p[ParameterType.HolesCount];
            if (count <= 0)
            {
                return;
            }

            double r = p[ParameterType.HoleRadius];
            if (r <= 0)
            {
                return;
            }

            double L = p[ParameterType.Length];
            double W = p[ParameterType.Width];

            ksEntity sketch = _k.CreateSketchOnXOY();
            ksDocument2D d2 = _k.BeginSketch(sketch);

            PlaceHoles(d2, count, r, L, W);

            _k.EndSketch(sketch);
            _k.Cut(sketch);
        }

        private void PlaceHoles(ksDocument2D d2, int count, double radius, double length, double width)
        {
            double diameter = 2 * radius;
            double edgeMargin = Math.Max(2 * radius, 10);
            double minGap = Math.Max(1.5 * radius, 5);

            double availableLength = length - 2 * edgeMargin;
            double availableWidth = width - 2 * edgeMargin;

            if (count == 1)
            {
                d2.ksCircle(0, 0, radius, 1);
                return;
            }

            int maxPerRow = (int)Math.Floor((availableLength + minGap) / (diameter + minGap));
            int maxRows = (int)Math.Floor((availableWidth + minGap) / (diameter + minGap));

            if (maxPerRow <= 0 || maxRows <= 0)
            {
                return;
            }

            int rows = DetermineRowCount(count, maxPerRow, maxRows);
            var distribution = DistributeHolesAcrossRows(count, rows);

            int maxHolesInRow = distribution.Max();

            double gapX = 0;
            if (maxHolesInRow > 1)
            {
                double totalHoleWidth = maxHolesInRow * diameter;
                double remainingSpace = availableLength - totalHoleWidth;
                gapX = Math.Max(minGap, remainingSpace / (maxHolesInRow - 1));
            }

            double gapY = 0;
            if (rows > 1)
            {
                double totalHoleHeight = rows * diameter;
                double remainingSpace = availableWidth - totalHoleHeight;
                gapY = Math.Max(minGap, remainingSpace / (rows - 1));
            }

            double startY = ((rows - 1) * (diameter + gapY)) / 2.0;

            for (int row = 0; row < rows; row++)
            {
                int holesInRow = distribution[row];
                double y = startY - row * (diameter + gapY);

                double rowWidth = (holesInRow - 1) * (diameter + gapX);
                double startX = -rowWidth / 2.0;

                for (int col = 0; col < holesInRow; col++)
                {
                    double x = startX + col * (diameter + gapX);
                    d2.ksCircle(x, y, radius, 1);
                }
            }
        }

        private int DetermineRowCount(int totalHoles, int maxPerRow, int maxRows)
        {
            int minRows = (int)Math.Ceiling((double)totalHoles / maxPerRow);
            minRows = Math.Max(1, Math.Min(minRows, maxRows));

            if (totalHoles % 2 == 1 && minRows == 2 && maxRows >= 3)
            {
                return 3;
            }

            return minRows;
        }

        private List<int> DistributeHolesAcrossRows(int totalHoles, int rows)
        {
            var result = new List<int>();
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