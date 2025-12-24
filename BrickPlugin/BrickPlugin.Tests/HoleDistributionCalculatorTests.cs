using NUnit.Framework;
using BrickPluginModels.Services;
using BrickPluginModels.Models;
using System.Linq;

namespace BrickPlugin.Tests
{
    [TestFixture]
    [Description("Тесты для класса HoleDistributionCalculator")]
    public class HoleDistributionCalculatorTests
    {
        private HoleDistributionCalculator _calculator;

        [SetUp]
        public void Setup()
        {
            _calculator = new HoleDistributionCalculator();
        }

        [Test]
        [Description("CalculateMaxHolesStraight возвращает положительное значение")]
        public void CalculateMaxHolesStraight_ShouldReturnPositiveValue()
        {
            var maxHoles = 
                _calculator.CalculateMaxHolesStraight(250, 120, 8);

            Assert.IsTrue(maxHoles > 0);
        }

        [Test]
        [Description("CalculateMaxHolesStraight возвращает 0 для большого радиуса")]
        public void CalculateMaxHolesStraight_LargeRadius_ShouldReturnZero()
        {
            var maxHoles = 
                _calculator.CalculateMaxHolesStraight(250, 120, 100);

            Assert.AreEqual(0, maxHoles);
        }

        [Test]
        [Description("CalculateMaxHolesStraight больше при меньшем радиусе")]
        public void CalculateMaxHolesStraight_SmallerRadius_MoreHoles()
        {
            var maxHoles1 = 
                _calculator.CalculateMaxHolesStraight(250, 120, 10);
            var maxHoles2 = 
                _calculator.CalculateMaxHolesStraight(250, 120, 5);

            Assert.IsTrue(maxHoles2 > maxHoles1);
        }

        [Test]
        [Description("CalculateMaxHolesStraight больше при большей длине")]
        public void CalculateMaxHolesStraight_LongerLength_MoreHoles()
        {
            var maxHoles1 = 
                _calculator.CalculateMaxHolesStraight(250, 120, 8);
            var maxHoles2 = 
                _calculator.CalculateMaxHolesStraight(500, 120, 8);

            Assert.IsTrue(maxHoles2 > maxHoles1);
        }

        [Test]
        [Description("CalculateMaxHolesStaggered " +
            "возвращает положительное значение")]
        public void CalculateMaxHolesStaggered_ShouldReturnPositiveValue()
        {
            var maxHoles =
                _calculator.CalculateMaxHolesStaggered(250, 120, 8);

            Assert.IsTrue(maxHoles > 0);
        }

        [Test]
        [Description("CalculateMaxHolesStaggered " +
            "возвращает 0 для большого радиуса")]
        public void CalculateMaxHolesStaggered_LargeRadius_ShouldReturnZero()
        {
            var maxHoles = 
                _calculator.CalculateMaxHolesStaggered(250, 120, 100);

            Assert.AreEqual(0, maxHoles);
        }

        [Test]
        [Description("CalculateMaxHolesStaggered обычно больше чем прямое")]
        public void CalculateMaxHolesStaggered_ShouldBeMoreThanStraight()
        {
            var straight = 
                _calculator.CalculateMaxHolesStraight(250, 120, 8);
            var staggered = 
                _calculator.CalculateMaxHolesStaggered(250, 120, 8);

            Assert.IsTrue(staggered >= straight);
        }

        [Test]
        [Description("CalculateStraightDistribution " +
            "для 1 отверстия возвращает 1 ряд")]
        public void CalculateStraightDistribution_OneHole_ShouldReturnOneRow()
        {
            var result = 
                _calculator.CalculateStraightDistribution(1, 250, 120, 8);

            Assert.AreEqual(1, result.Rows);
            Assert.AreEqual(1, result.Distribution[0]);
            Assert.AreEqual(0, result.StaggerOffset);
        }

        [Test]
        [Description("CalculateStraightDistribution корректно распределяет отверстия")]
        public void CalculateStraightDistribution_ShouldDistributeCorrectly()
        {
            var result = 
                _calculator.CalculateStraightDistribution(10, 250, 120, 8);

            Assert.IsTrue(result.Rows > 0);
            Assert.AreEqual(10, result.Distribution.Sum());
            Assert.AreEqual(0, result.StaggerOffset);
        }

        [Test]
        [Description("CalculateStraightDistribution устанавливает " +
            "положительные зазоры")]
        public void CalculateStraightDistribution_ShouldSetPositiveGaps()
        {
            var result = 
                _calculator.CalculateStraightDistribution(10, 250, 120, 8);

            Assert.IsTrue(result.HorizontalGap >= 0);
            Assert.IsTrue(result.VerticalGap >= 0);
        }

        [Test]
        [Description("CalculateStraightDistribution равномерно " +
            "распределяет по рядам")]
        public void CalculateStraightDistribution_ShouldDistributeEvenly()
        {
            var result = 
                _calculator.CalculateStraightDistribution(20, 250, 120, 8);

            var maxInRow = result.Distribution.Max();
            var minInRow = result.Distribution.Min();
            Assert.IsTrue(maxInRow - minInRow <= 1);
        }

        [Test]
        [Description("CalculateStaggeredDistribution корректно " +
            "распределяет отверстия")]
        public void CalculateStaggeredDistribution_ShouldDistributeCorrectly()
        {
            var result = 
                _calculator.CalculateStaggeredDistribution(10, 250, 120, 8);

            Assert.IsTrue(result.Rows > 0);
            Assert.AreEqual(10, result.Distribution.Sum());
        }

        [Test]
        [Description("CalculateStaggeredDistribution устанавливает " +
            "ненулевое смещение")]
        public void CalculateStaggeredDistribution_ShouldSetNonZeroOffset()
        {
            var result = 
                _calculator.CalculateStaggeredDistribution(10, 250, 120, 8);

            Assert.IsTrue(result.StaggerOffset > 0);
        }

        [Test]
        [Description("CalculateStaggeredDistribution устанавливает " +
            "положительные зазоры")]
        public void CalculateStaggeredDistribution_ShouldSetPositiveGaps()
        {
            var result = 
                _calculator.CalculateStaggeredDistribution(10, 250, 120, 8);

            Assert.IsTrue(result.HorizontalGap >= 0);
            Assert.IsTrue(result.VerticalGap > 0);
        }

        [Test]
        [Description("CalculateStaggeredDistribution использует минимум 2 ряда")]
        public void CalculateStaggeredDistribution_ShouldUseAtLeastTwoRows()
        {
            var result = 
                _calculator.CalculateStaggeredDistribution(10, 250, 120, 8);

            Assert.IsTrue(result.Rows >= 2);
        }

        [Test]
        [Description("Distribution не содержит нулей")]
        public void Distribution_ShouldNotContainZeros()
        {
            var result = 
                _calculator.CalculateStraightDistribution(10, 250, 120, 8);

            Assert.IsFalse(result.Distribution.Contains(0));
        }

        [Test]
        [Description("Distribution Count равен Rows")]
        public void Distribution_CountShouldEqualRows()
        {
            var result = 
                _calculator.CalculateStraightDistribution(10, 250, 120, 8);

            Assert.AreEqual(result.Rows, result.Distribution.Count);
        }

        [Test]
        [Description("MaxHolesInRow равен максимуму из Distribution")]
        public void MaxHolesInRow_ShouldMatchDistributionMax()
        {
            var result = 
                _calculator.CalculateStraightDistribution(15, 250, 120, 8);

            Assert.AreEqual(result.Distribution.Max(), result.MaxHolesInRow);
        }

        [Test]
        [Description("Повторные вызовы дают одинаковый результат")]
        public void Calculator_SameParameters_SameResults()
        {
            var result1 = 
                _calculator.CalculateStraightDistribution(15, 250, 120, 8);
            var result2 = 
                _calculator.CalculateStraightDistribution(15, 250, 120, 8);

            Assert.AreEqual(result1.Rows, result2.Rows);
            Assert.AreEqual(result1.MaxHolesInRow, result2.MaxHolesInRow);
        }

        [Test]
        [Description("MaxHoles и Distribution согласованы для прямого")]
        public void MaxHolesStraight_AndDistribution_ShouldBeConsistent()
        {
            var maxHoles = 
                _calculator.CalculateMaxHolesStraight(250, 120, 8);
            var distribution = 
                _calculator.CalculateStraightDistribution(maxHoles, 250, 120, 8);

            Assert.AreEqual(maxHoles, distribution.Distribution.Sum());
        }

        [Test]
        [Description("MaxHoles и Distribution согласованы для шахматного")]
        public void MaxHolesStaggered_AndDistribution_ShouldBeConsistent()
        {
            var maxHoles = 
                _calculator.CalculateMaxHolesStaggered(250, 120, 8);
            var distribution = 
                _calculator.CalculateStaggeredDistribution(maxHoles, 250, 120, 8);

            Assert.AreEqual(maxHoles, distribution.Distribution.Sum());
        }
    }
}