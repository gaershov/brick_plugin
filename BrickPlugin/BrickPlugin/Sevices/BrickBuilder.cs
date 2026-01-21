using BrickPluginModels.Models;
using BrickPluginModels.Services;
using Kompas6API5;

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
        /// Калькулятор распределения отверстий в кирпиче.
        /// </summary>
        private readonly HoleDistributionCalculator _distributionCalculator;

        /// <summary>
        /// Инициализирует новый экземпляр класса.
        /// </summary>
        public BrickBuilder()
        {
            _kompasWrapper = new KompasWrapper();
            _distributionCalculator = new HoleDistributionCalculator();
        }

        /// <summary>
        /// Закрывает текущий документ без сохранения.
        /// </summary>
        public void CloseDocument()
        {
            _kompasWrapper.CloseDocument();
        }

        /// <summary>
        /// Строит 3D-модель кирпича с заданными параметрами.
        /// </summary>
        /// <param name="parameters">Параметры кирпича.</param>
        public void Build(BrickParameters parameters)
        {
            _kompasWrapper.OpenKompas();
            _kompasWrapper.CreateDocument();
            BuildBrickBody(parameters);
            BuildHoles(parameters);
        }

        /// <summary>
        /// Строит основное тело кирпича в виде параллелепипеда.
        /// </summary>
        /// <param name="parameters">Параметры кирпича.</param>
        private void BuildBrickBody(BrickParameters parameters)
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
        /// Создает отверстия в кирпиче согласно заданным параметрам.
        /// </summary>
        /// <param name="parameters">Параметры кирпича.</param>
        private void BuildHoles(BrickParameters parameters)
        {
            int holesCount = (int)parameters[ParameterType.HolesCount];
            if (holesCount <= 0)
            {
                return;
            }

            double holeRadius = parameters[ParameterType.HoleRadius];
            if (holeRadius <= 0)
            {
                return;
            }

            double length = parameters[ParameterType.Length];
            double width = parameters[ParameterType.Width];
            HoleDistributionType distributionType = parameters.DistributionType;

            ksEntity sketch = _kompasWrapper.CreateSketchOnXOY();
            ksDocument2D document2D = _kompasWrapper.BeginSketch(sketch);

            PlaceHoles(document2D, holesCount, holeRadius, length, width, distributionType);

            _kompasWrapper.EndSketch(sketch);
            _kompasWrapper.Cut(sketch);
        }

        /// <summary>
        /// Размещает отверстия на плоскости эскиза согласно выбранному типу распределения.
        /// </summary>
        /// <param name="document2D">2D-документ для рисования окружностей.</param>
        /// <param name="holesCount">Количество отверстий.</param>
        /// <param name="holeRadius">Радиус отверстия.</param>
        /// <param name="length">Длина кирпича.</param>
        /// <param name="width">Ширина кирпича.</param>
        /// <param name="distributionType">Тип распределения отверстий.</param>
        private void PlaceHoles(
            ksDocument2D document2D,
            int holesCount,
            double holeRadius,
            double length,
            double width,
            HoleDistributionType distributionType)
        {
            if (holesCount == 1)
            {
                document2D.ksCircle(0, 0, holeRadius, 1);
                return;
            }

            HoleDistributionResult distribution
                = distributionType == HoleDistributionType.Straight
                ? _distributionCalculator.CalculateStraightDistribution
                (holesCount, length, width, holeRadius)
                : _distributionCalculator.CalculateStaggeredDistribution
                (holesCount, length, width, holeRadius);

            //прямое или шахматное
            if (distributionType == HoleDistributionType.Straight)
            {
                PlaceHolesStraight(document2D, distribution, holeRadius, length, width);
            }
            else
            {
                PlaceHolesStaggered(document2D, distribution, holeRadius, length, width);
            }
        }

        /// <summary>
        /// Размещает отверстия прямым способом без смещения между рядами.
        /// </summary>
        /// <param name="document2D">2D-документ для рисования окружностей.</param>
        /// <param name="distribution">Результат расчета распределения отверстий.</param>
        /// <param name="holeRadius">Радиус отверстия.</param>
        /// <param name="length">Длина кирпича.</param>
        /// <param name="width">Ширина кирпича.</param>
        private void PlaceHolesStraight(
            ksDocument2D document2D,
            HoleDistributionResult distribution,
            double holeRadius,
            double length,
            double width)
        {
            //TODO: RSDN +
            var availableArea =
                BrickParameters.CalculateAvailableArea(length, width, holeRadius);
            double holeDiameter = availableArea.diameter;
            double horizontalStep = holeDiameter + distribution.HorizontalGap;
            double verticalStep = holeDiameter + distribution.VerticalGap;

            double totalHeight = (distribution.Rows - 1) * verticalStep;
            double startY = totalHeight / 2.0;

            for (int row = 0; row < distribution.Rows; row++)
            {
                int holesInCurrentRow = distribution.Distribution[row];
                double yPosition = startY - row * verticalStep;

                double rowWidth = (holesInCurrentRow - 1) * horizontalStep;
                double startX = -rowWidth / 2.0;

                for (int column = 0; column < holesInCurrentRow; column++)
                {
                    double xPosition = startX + column * horizontalStep;
                    document2D.ksCircle(xPosition, yPosition, holeRadius, 1);
                }
            }
        }

        /// <summary>
        /// Размещает отверстия шахматным способом со смещением рядов.
        /// </summary>
        /// <param name="document2D">2D-документ для рисования окружностей.</param>
        /// <param name="distribution">Результат расчета распределения отверстий.</param>
        /// <param name="holeRadius">Радиус отверстия.</param>
        /// <param name="length">Длина кирпича.</param>
        /// <param name="width">Ширина кирпича.</param>
        private void PlaceHolesStaggered(
            ksDocument2D document2D,
            HoleDistributionResult distribution,
            double holeRadius,
            double length,
            double width)
        {
            var availableArea =
                BrickParameters.CalculateAvailableArea(length, width, holeRadius);
            double holeDiameter =
                availableArea.diameter;
            double horizontalStep =
                holeDiameter + distribution.HorizontalGap;

            int minHolesInRow = distribution.Distribution.Min();
            int maxHolesInRow = distribution.Distribution.Max();

            bool[] shouldOffsetRow = CreateOffsetPattern(distribution.Rows);

            double totalHeight = (distribution.Rows - 1) * distribution.VerticalGap;
            double startY = totalHeight / 2.0;

            double baseRowWidth = (minHolesInRow - 1) * horizontalStep;
            double baseStartX = -baseRowWidth / 2.0;

            for (int row = 0; row < distribution.Rows; row++)
            {
                int holesInCurrentRow = distribution.Distribution[row];
                double yPosition = startY - row * distribution.VerticalGap;

                double xStartPosition = baseStartX;

                if (shouldOffsetRow[row])
                {
                    xStartPosition -= distribution.StaggerOffset;
                }

                for (int column = 0; column < holesInCurrentRow; column++)
                {
                    double xPosition = xStartPosition + column * horizontalStep;
                    document2D.ksCircle(xPosition, yPosition, holeRadius, 1);
                }
            }
        }

        /// <summary>
        /// Создает шаблон смещения для рядов (нечетные ряды смещаются).
        /// </summary>
        /// <param name="rowsCount">Количество рядов.</param>
        /// <returns>Массив флагов, указывающих, нужно ли смещать каждый ряд.</returns>
        private bool[] CreateOffsetPattern(int rowsCount)
        {
            var offsetPattern = new bool[rowsCount];

            for (int i = 0; i < rowsCount; i++)
            {
                offsetPattern[i] = (i % 2 == 1);
            }

            return offsetPattern;
        }
    }
}