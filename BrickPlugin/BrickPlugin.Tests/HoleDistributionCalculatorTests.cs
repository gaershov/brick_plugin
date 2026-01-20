using NUnit.Framework;
using BrickPluginModels.Services;
using System.Linq;

namespace BrickPlugin.Tests
{
    [TestFixture]
    [Description("Тесты для класса HoleDistributionCalculator")]
    public class HoleDistributionCalculatorTests
    {
        private HoleDistributionCalculator Calculator => new HoleDistributionCalculator();

        [Test]
        [Description("CalculateMaxHolesStraight возвращает положительное значение")]
        public void CalculateMaxHolesStraight_ShouldReturnPositiveValue()
        {
            var maxHoles = Calculator.CalculateMaxHolesStraight(250, 120, 8);

            Assert.IsTrue(maxHoles > 0);
        }

        [TestCase(250, 120, 100, 0)]
        [TestCase(10, 120, 20, 0)]
        [TestCase(250, 10, 20, 0)]
        [Description("CalculateMaxHolesStraight возвращает " +
            "0 для большого радиуса или малых размеров")]
        public void CalculateMaxHolesStraight_InvalidParameters_ShouldReturnZero(
            double length, double width, double radius, int expected)
        {
            var maxHoles = Calculator.CalculateMaxHolesStraight(length, width, radius);

            Assert.AreEqual(expected, maxHoles);
        }

        [Test]
        [Description("CalculateMaxHolesStraight больше при меньшем радиусе")]
        public void CalculateMaxHolesStraight_SmallerRadius_MoreHoles()
        {
            var maxHoles1 = Calculator.CalculateMaxHolesStraight(250, 120, 10);
            var maxHoles2 = Calculator.CalculateMaxHolesStraight(250, 120, 5);

            Assert.IsTrue(maxHoles2 > maxHoles1);
        }

        [Test]
        [Description("CalculateMaxHolesStraight больше при большей длине")]
        public void CalculateMaxHolesStraight_LongerLength_MoreHoles()
        {
            var maxHoles1 = Calculator.CalculateMaxHolesStraight(250, 120, 8);
            var maxHoles2 = Calculator.CalculateMaxHolesStraight(500, 120, 8);

            Assert.IsTrue(maxHoles2 > maxHoles1);
        }

        [Test]
        [Description("CalculateMaxHolesStraight возвращает минимум 1 при доступном пространстве")]
        public void CalculateMaxHolesStraight_WithAvailableSpace_ShouldReturnAtLeastOne()
        {
            var maxHoles = Calculator.CalculateMaxHolesStraight(100, 50, 5);

            Assert.IsTrue(maxHoles >= 1);
        }

        [Test]
        [Description("CalculateMaxHolesStaggered возвращает положительное значение")]
        public void CalculateMaxHolesStaggered_ShouldReturnPositiveValue()
        {
            var maxHoles = Calculator.CalculateMaxHolesStaggered(250, 120, 8);

            Assert.IsTrue(maxHoles > 0);
        }

        [TestCase(250, 120, 100, 0)]
        [TestCase(10, 120, 20, 0)]
        [TestCase(250, 10, 20, 0)]
        [Description("CalculateMaxHolesStaggered возвращает 0 для большого радиуса")]
        public void CalculateMaxHolesStaggered_InvalidParameters_ShouldReturnZero(
            double length, double width, double radius, int expected)
        {
            var maxHoles = Calculator.CalculateMaxHolesStaggered(length, width, radius);

            Assert.AreEqual(expected, maxHoles);
        }

        [Test]
        [Description("CalculateMaxHolesStaggered обычно больше чем прямое")]
        public void CalculateMaxHolesStaggered_ShouldBeMoreThanStraight()
        {
            var straight = Calculator.CalculateMaxHolesStraight(250, 120, 8);
            var staggered = Calculator.CalculateMaxHolesStaggered(250, 120, 8);

            Assert.IsTrue(staggered >= straight);
        }

        [Test]
        [Description("CalculateMaxHolesStaggered возвращает " +
            "минимум 1 при доступном пространстве")]
        public void CalculateMaxHolesStaggered_WithAvailableSpace_ShouldReturnAtLeastOne()
        {
            var maxHoles = Calculator.CalculateMaxHolesStaggered(100, 50, 5);

            Assert.IsTrue(maxHoles >= 1);
        }

        [Test]
        [Description("CalculateStraightDistribution для 1 отверстия возвращает 1 ряд")]
        public void CalculateStraightDistribution_OneHole_ShouldReturnOneRow()
        {
            var result = Calculator.CalculateStraightDistribution(1, 250, 120, 8);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, result.Rows);
                Assert.AreEqual(1, result.Distribution[0]);
                Assert.AreEqual(0, result.StaggerOffset);
                Assert.AreEqual(0, result.HorizontalGap);
            });
        }

        [Test]
        [Description("CalculateStraightDistribution корректно распределяет отверстия")]
        public void CalculateStraightDistribution_ShouldDistributeCorrectly()
        {
            var result = Calculator.CalculateStraightDistribution(10, 250, 120, 8);

            Assert.Multiple(() =>
            {
                Assert.IsTrue(result.Rows > 0);
                Assert.AreEqual(10, result.Distribution.Sum());
                Assert.AreEqual(0, result.StaggerOffset);
            });
        }

        [Test]
        [Description("CalculateStraightDistribution устанавливает положительные зазоры")]
        public void CalculateStraightDistribution_ShouldSetPositiveGaps()
        {
            var result = Calculator.CalculateStraightDistribution(10, 250, 120, 8);

            Assert.Multiple(() =>
            {
                Assert.IsTrue(result.HorizontalGap >= 0);
                Assert.IsTrue(result.VerticalGap >= 0);
            });
        }

        [Test]
        [Description("CalculateStraightDistribution равномерно распределяет по рядам")]
        public void CalculateStraightDistribution_ShouldDistributeEvenly()
        {
            var result = Calculator.CalculateStraightDistribution(20, 250, 120, 8);

            var maxInRow = result.Distribution.Max();
            var minInRow = result.Distribution.Min();
            Assert.IsTrue(maxInRow - minInRow <= 1);
        }

        [Test]
        [Description("Distribution не содержит нулей")]
        public void Distribution_ShouldNotContainZeros()
        {
            var result = Calculator.CalculateStraightDistribution(10, 250, 120, 8);

            Assert.IsFalse(result.Distribution.Contains(0));
        }

        [Test]
        [Description("Distribution Count равен Rows")]
        public void Distribution_CountShouldEqualRows()
        {
            var result = Calculator.CalculateStraightDistribution(10, 250, 120, 8);

            Assert.AreEqual(result.Rows, result.Distribution.Count);
        }

        [Test]
        [Description("MaxHolesInRow равен максимуму из Distribution")]
        public void MaxHolesInRow_ShouldMatchDistributionMax()
        {
            var result = Calculator.CalculateStraightDistribution(15, 250, 120, 8);

            Assert.AreEqual(result.Distribution.Max(), result.MaxHolesInRow);
        }

        [Test]
        [Description("CalculateStraightDistribution использует минимум 1 ряд")]
        public void CalculateStraightDistribution_ShouldUseAtLeastOneRow()
        {
            var result = Calculator.CalculateStraightDistribution(5, 250, 120, 8);

            Assert.IsTrue(result.Rows >= 1);
        }

        [Test]
        [Description("CalculateStaggeredDistribution корректно распределяет отверстия")]
        public void CalculateStaggeredDistribution_ShouldDistributeCorrectly()
        {
            var result = Calculator.CalculateStaggeredDistribution(10, 250, 120, 8);

            Assert.Multiple(() =>
            {
                Assert.IsTrue(result.Rows > 0);
                Assert.AreEqual(10, result.Distribution.Sum());
            });
        }

        [Test]
        [Description("CalculateStaggeredDistribution устанавливает ненулевое смещение")]
        public void CalculateStaggeredDistribution_ShouldSetNonZeroOffset()
        {
            var result = Calculator.CalculateStaggeredDistribution(10, 250, 120, 8);

            Assert.IsTrue(result.StaggerOffset > 0);
        }

        [Test]
        [Description("CalculateStaggeredDistribution устанавливает положительные зазоры")]
        public void CalculateStaggeredDistribution_ShouldSetPositiveGaps()
        {
            var result = Calculator.CalculateStaggeredDistribution(10, 250, 120, 8);

            Assert.Multiple(() =>
            {
                Assert.IsTrue(result.HorizontalGap >= 0);
                Assert.IsTrue(result.VerticalGap > 0);
            });
        }

        [Test]
        [Description("CalculateStaggeredDistribution использует минимум 2 ряда")]
        public void CalculateStaggeredDistribution_ShouldUseAtLeastTwoRows()
        {
            var result = Calculator.CalculateStaggeredDistribution(10, 250, 120, 8);

            Assert.IsTrue(result.Rows >= 2);
        }

        [Test]
        [Description("Повторные вызовы дают одинаковый результат")]
        public void Calculator_SameParameters_SameResults()
        {
            var result1 = Calculator.CalculateStraightDistribution(15, 250, 120, 8);
            var result2 = Calculator.CalculateStraightDistribution(15, 250, 120, 8);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(result1.Rows, result2.Rows);
                Assert.AreEqual(result1.MaxHolesInRow, result2.MaxHolesInRow);
            });
        }

        [Test]
        [Description("MaxHoles и Distribution согласованы для прямого")]
        public void MaxHolesStraight_AndDistribution_ShouldBeConsistent()
        {
            var maxHoles = Calculator.CalculateMaxHolesStraight(250, 120, 8);
            var distribution = Calculator.CalculateStraightDistribution(maxHoles, 250, 120, 8);

            Assert.AreEqual(maxHoles, distribution.Distribution.Sum());
        }

        [Test]
        [Description("MaxHoles и Distribution согласованы для шахматного")]
        public void MaxHolesStaggered_AndDistribution_ShouldBeConsistent()
        {
            var maxHoles = Calculator.CalculateMaxHolesStaggered(250, 120, 8);
            var distribution = Calculator.CalculateStaggeredDistribution(maxHoles, 250, 120, 8);

            Assert.AreEqual(maxHoles, distribution.Distribution.Sum());
        }

        [Test]
        [Description("CalculateStraightDistribution корректно " +
            "обрабатывает случай с большим количеством отверстий")]
        public void CalculateStraightDistribution_ManyHoles_ShouldCalculateCorrectly()
        {
            var result = Calculator.CalculateStraightDistribution(50, 250, 120, 8);

            Assert.Multiple(() =>
            {
                Assert.IsTrue(result.Rows > 0);
                Assert.AreEqual(50, result.Distribution.Sum());
                Assert.IsTrue(result.MaxHolesInRow > 0);
            });
        }

        [Test]
        [Description("CalculateStaggeredDistribution находит оптимальное количество рядов")]
        public void CalculateStaggeredDistribution_ShouldFindOptimalRowCount()
        {
            var result = Calculator.CalculateStaggeredDistribution(15, 250, 120, 8);

            Assert.Multiple(() =>
            {
                Assert.IsTrue(result.Rows >= 2);
                Assert.AreEqual(15, result.Distribution.Sum());
            });
        }

        [Test]
        [Description("CalculateStaggeredDistribution проверяет вместимость отверстий")]
        public void CalculateStaggeredDistribution_ShouldCheckIfHolesFit()
        {
            var result = Calculator.CalculateStaggeredDistribution(100, 250, 120, 8);

            Assert.IsTrue(result.Distribution.Sum() <= 100);
        }

        [Test]
        [Description("DistributeEvenly равномерно распределяет остаток")]
        public void CalculateStraightDistribution_DistributeEvenly_ShouldDistributeRemainder()
        {
            var result = Calculator.CalculateStraightDistribution(23, 250, 120, 8);

            var maxInRow = result.Distribution.Max();
            var minInRow = result.Distribution.Min();

            Assert.Multiple(() =>
            {
                Assert.IsTrue(maxInRow - minInRow <= 1);
                Assert.AreEqual(23, result.Distribution.Sum());
            });
        }

        [Test]
        [Description("DistributeAlternating распределяет отверстия с чередованием")]
        public void CalculateStaggeredDistribution_DistributeAlternating_ShouldWork()
        {
            var result = Calculator.CalculateStaggeredDistribution(25, 250, 120, 8);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(25, result.Distribution.Sum());
                var maxInRow = result.Distribution.Max();
                var minInRow = result.Distribution.Min();
                Assert.IsTrue(maxInRow - minInRow <= 1);
            });
        }

        [Test]
        [Description("CalculateStaggeredDistribution обрабатывает большое количество отверстий")]
        public void CalculateStaggeredDistribution_VeryManyHoles_HandlesCorrectly()
        {
            var result = Calculator.CalculateStaggeredDistribution(1000, 250, 120, 8);

            Assert.IsTrue(result.Rows > 0);
        }

        [Test]
        [Description("CalculateStaggeredDistribution вертикальный зазор больше нуля")]
        public void CalculateStaggeredDistribution_VerticalGap_ShouldBePositive()
        {
            var result = Calculator.CalculateStaggeredDistribution(10, 250, 120, 8);

            Assert.IsTrue(result.VerticalGap > 0);
        }

        [Test]
        [Description("CalculateStraightDistribution правильно вычисляет зазоры для множества рядов")]
        public void CalculateStraightDistribution_MultipleRows_ShouldCalculateGapsCorrectly()
        {
            var result = Calculator.CalculateStraightDistribution(50, 250, 120, 5);

            Assert.Multiple(() =>
            {
                Assert.IsTrue(result.Rows > 1);
                Assert.IsTrue(result.HorizontalGap >= 0);
                Assert.IsTrue(result.VerticalGap >= 0);
            });
        }

        [Test]
        [Description("DistributeAlternating распределяет " +
            "остаток сначала по нечетным, затем по четным индексам")]
        public void CalculateStaggeredDistribution_DistributeAlternating_UsesEvenIndicesAfterOdd()
        {
            var result = Calculator.CalculateStaggeredDistribution(35, 300, 120, 7);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(35, result.Distribution.Sum());
                Assert.IsTrue(result.Rows >= 2);
                var maxInRow = result.Distribution.Max();
                var minInRow = result.Distribution.Min();
                Assert.IsTrue(maxInRow - minInRow <= 1, "Разница должна быть не более 1");
            });
        }

        [Test]
        [Description("DistributeAlternating с большим остатком использует оба цикла распределения")]
        public void CalculateStaggeredDistribution_LargeRemainder_UsesBothLoops()
        {
            var result = Calculator.CalculateStaggeredDistribution(55, 350, 150, 6);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(55, result.Distribution.Sum());
                Assert.IsTrue(result.Rows >= 2); 
                var maxInRow = result.Distribution.Max();
                var minInRow = result.Distribution.Min();
                Assert.IsTrue(maxInRow - minInRow <= 1);
            });
        }

        [Test]
        [Description("DistributeAlternating когда остаток примерно равен количеству рядов")]
        public void CalculateStaggeredDistribution_RemainderNearRows_DistributesCorrectly()
        {
            var result = Calculator.CalculateStaggeredDistribution(29, 250, 120, 8);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(29, result.Distribution.Sum());
                var maxInRow = result.Distribution.Max();
                var minInRow = result.Distribution.Min();
                Assert.IsTrue(maxInRow - minInRow <= 1);
            });
        }

        [Test]
        [Description("DistributeAlternating проверяет цикл четных индексов")]
        public void CalculateStaggeredDistribution_EvenIndexLoop_ShouldExecute()
        {
            var result = Calculator.CalculateStaggeredDistribution(43, 280, 130, 7);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(43, result.Distribution.Sum());
                var maxInRow = result.Distribution.Max();
                var minInRow = result.Distribution.Min();
                Assert.IsTrue(maxInRow - minInRow <= 1);
                Assert.IsTrue(result.Distribution.Count(x => x == maxInRow) > 0);
            });
        }
    }
}