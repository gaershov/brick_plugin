using NUnit.Framework;
using BrickPluginModels.Services;
using BrickPluginModels.Models;
using System;

namespace BrickPlugin.Tests
{
    [TestFixture]
    [Description("Тесты для класса VoidnessCalculator")]
    public class VoidnessCalculatorTests
    {
        private VoidnessCalculator _calculator;

        [SetUp]
        //TODO: refactor
        public void Setup()
        {
            _calculator = new VoidnessCalculator();
        }

        [Test]
        [Description("CalculateCurrentVoidness возвращает 0 для нуля отверстий")]
        public void CalculateCurrentVoidness_ZeroHoles_ShouldReturnZero()
        {
            //TODO: RSDN
            var voidness = _calculator.CalculateCurrentVoidness(250, 120, 65, 8, 0);

            Assert.AreEqual(0, voidness, 0.01);
        }

        [Test]
        [Description("CalculateCurrentVoidness возвращает положительное значение")]
        public void CalculateCurrentVoidness_WithHoles_ShouldReturnPositiveValue()
        {
            //TODO: RSDN
            var voidness = _calculator.CalculateCurrentVoidness(250, 120, 65, 8, 10);

            Assert.IsTrue(voidness > 0);
        }

        [Test]
        [Description("CalculateCurrentVoidness увеличивается с количеством")]
        public void CalculateCurrentVoidness_MoreHoles_HigherVoidness()
        {
            //TODO: RSDN
            var voidness1 = _calculator.CalculateCurrentVoidness(250, 120, 65, 8, 5);
            var voidness2 = _calculator.CalculateCurrentVoidness(250, 120, 65, 8, 10);

            Assert.IsTrue(voidness2 > voidness1);
        }

        [Test]
        [Description("CalculateCurrentVoidness увеличивается с радиусом")]
        public void CalculateCurrentVoidness_LargerRadius_HigherVoidness()
        {
            //TODO: RSDN
            var voidness1 = _calculator.CalculateCurrentVoidness(250, 120, 65, 5, 10);
            var voidness2 = _calculator.CalculateCurrentVoidness(250, 120, 65, 10, 10);

            Assert.IsTrue(voidness2 > voidness1);
        }

        [Test]
        [Description("CalculateCurrentVoidness уменьшается с размером кирпича")]
        public void CalculateCurrentVoidness_LargerBrick_LowerVoidness()
        {
            //TODO: RSDN
            var voidness1 = _calculator.CalculateCurrentVoidness(250, 120, 65, 8, 10);
            var voidness2 = _calculator.CalculateCurrentVoidness(500, 240, 130, 8, 10);

            Assert.IsTrue(voidness2 < voidness1);
        }

        [Test]
        [Description("CalculateMaxPossibleVoidness возвращает положительное значение")]
        public void CalculateMaxPossibleVoidness_ShouldReturnPositiveValue()
        {
            var maxVoidness = _calculator.CalculateMaxPossibleVoidness(
                250, 120, 65, 8, HoleDistributionType.Straight);

            Assert.IsTrue(maxVoidness > 0);
        }

        [Test]
        [Description("CalculateMaxPossibleVoidness не превышает 45")]
        public void CalculateMaxPossibleVoidness_ShouldNotExceed45()
        {
            var maxVoidness = _calculator.CalculateMaxPossibleVoidness(
                250, 120, 65, 8, HoleDistributionType.Straight);

            Assert.IsTrue(maxVoidness <= 45);
        }

        [Test]
        [Description("CalculateMaxPossibleVoidness для шахматного больше или равно")]
        public void CalculateMaxPossibleVoidness_Staggered_HigherOrEqual()
        {
            var straight = _calculator.CalculateMaxPossibleVoidness(
                250, 120, 65, 8, HoleDistributionType.Straight);
            var staggered = _calculator.CalculateMaxPossibleVoidness(
                250, 120, 65, 8, HoleDistributionType.Staggered);

            Assert.IsTrue(staggered >= straight);
        }

        [Test]
        [Description("CalculateMinPossibleVoidness возвращает положительное значение")]
        public void CalculateMinPossibleVoidness_ShouldReturnPositiveValue()
        {
            var minVoidness = _calculator.CalculateMinPossibleVoidness(250, 120, 65, 8);

            Assert.IsTrue(minVoidness > 0);
        }

        [Test]
        [Description("CalculateMinPossibleVoidness равно пустотности для 1 отверстия")]
        public void CalculateMinPossibleVoidness_ShouldEqualOneHole()
        {
            //TODO: RSDN
            var minVoidness = _calculator.CalculateMinPossibleVoidness(250, 120, 65, 8);
            var actual = _calculator.CalculateCurrentVoidness(250, 120, 65, 8, 1);

            Assert.AreEqual(minVoidness, actual, 0.01);
        }

        [Test]
        [Description("CalculateOptimalParameters возвращает успешный результат")]
        public void CalculateOptimalParameters_ValidVoidness_ShouldReturnSuccess()
        {
            var result = _calculator.CalculateOptimalParameters(
                250, 120, 65, 20, HoleDistributionType.Straight);

            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.HoleRadius >= 2);
            Assert.IsTrue(result.HolesCount >= 1);
        }

        [Test]
        [Description("CalculateOptimalParameters достигает цели с погрешностью")]
        public void CalculateOptimalParameters_ShouldAchieveTarget()
        {
            double target = 25;
            var result = _calculator.CalculateOptimalParameters(
                250, 120, 65, target, HoleDistributionType.Straight);

            Assert.IsTrue(result.Success);
            Assert.IsTrue(Math.Abs(result.ActualVoidness - target) < 5);
        }

        [Test]
        //TODO: RSDN
        [Description("CalculateOptimalParameters возвращает ошибку для пустотности больше 45")]
        public void CalculateOptimalParameters_Above45_ShouldReturnError()
        {
            var result = _calculator.CalculateOptimalParameters(
                250, 120, 65, 50, HoleDistributionType.Straight);

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.ErrorMessage);
        }

        [Test]
        [Description("CalculateOptimalParameters возвращает ошибку для нуля")]
        public void CalculateOptimalParameters_Zero_ShouldReturnError()
        {
            var result = _calculator.CalculateOptimalParameters(
                250, 120, 65, 0, HoleDistributionType.Straight);

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.ErrorMessage);
        }

        [Test]
        [Description("CalculateOptimalParameters возвращает ошибку для отрицательного")]
        public void CalculateOptimalParameters_Negative_ShouldReturnError()
        {
            var result = _calculator.CalculateOptimalParameters(
                250, 120, 65, -10, HoleDistributionType.Straight);

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.ErrorMessage);
        }

        [Test]
        [Description("CalculateHolesCountForVoidness возвращает успешный результат")]
        public void CalculateHolesCountForVoidness_ShouldReturnSuccess()
        {
            var result = _calculator.CalculateHolesCountForVoidness(
                250, 120, 65, 8, 15, HoleDistributionType.Straight);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(8, result.HoleRadius);
            Assert.IsTrue(result.HolesCount > 0);
        }

        [Test]
        [Description("CalculateHolesCountForVoidness не изменяет радиус")]
        public void CalculateHolesCountForVoidness_ShouldNotChangeRadius()
        {
            double fixedRadius = 8;
            var result = _calculator.CalculateHolesCountForVoidness(
                250, 120, 65, fixedRadius, 15, HoleDistributionType.Straight);

            Assert.AreEqual(fixedRadius, result.HoleRadius);
        }

        [Test]
        //TODO: RSDN
        [Description("CalculateHolesCountForVoidness возвращает ошибку для пустотности больше 45")]
        public void CalculateHolesCountForVoidness_Above45_ShouldReturnError()
        {
            var result = _calculator.CalculateHolesCountForVoidness(
                250, 120, 65, 8, 50, HoleDistributionType.Straight);

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.ErrorMessage);
        }

        [Test]
        [Description("CalculateHolesCountForVoidness возвращает минимум 1 отверстие")]
        public void CalculateHolesCountForVoidness_ShouldReturnAtLeastOne()
        {
            var result = _calculator.CalculateHolesCountForVoidness(
                250, 120, 65, 8, 2, HoleDistributionType.Straight);

            if (result.Success)
            {
                Assert.IsTrue(result.HolesCount >= 1);
            }
        }

        [Test]
        [Description("GetVoidnessRange возвращает корректный диапазон")]
        public void GetVoidnessRange_ShouldReturnValidRange()
        {
            var (min, max) = _calculator.GetVoidnessRange(
                250, 120, 65, 8, HoleDistributionType.Straight);

            Assert.IsTrue(min >= 0);
            Assert.IsTrue(max <= 45);
            Assert.IsTrue(min <= max);
        }

        [Test]
        [Description("GetVoidnessRange для нулевого радиуса возвращает нули")]
        public void GetVoidnessRange_ZeroRadius_ShouldReturnZeros()
        {
            var (min, max) = _calculator.GetVoidnessRange(
                250, 120, 65, 0, HoleDistributionType.Straight);

            Assert.AreEqual(0, min, 0.01);
            Assert.AreEqual(0, max, 0.01);
        }

        [Test]
        [Description("GetVoidnessRange минимум равен пустотности для 1 отверстия")]
        public void GetVoidnessRange_MinShouldMatchOneHole()
        {
            var (min, max) = _calculator.GetVoidnessRange(
                250, 120, 65, 8, HoleDistributionType.Straight);
            var oneHole = _calculator.CalculateCurrentVoidness(250, 120, 65, 8, 1);

            Assert.AreEqual(oneHole, min, 0.01);
        }

        [Test]
        [Description("GetVoidnessRange для шахматного даёт больший максимум")]
        public void GetVoidnessRange_Staggered_HigherMax()
        {
            var (minStraight, maxStraight) = _calculator.GetVoidnessRange(
                250, 120, 65, 8, HoleDistributionType.Straight);
            var (minStaggered, maxStaggered) = _calculator.GetVoidnessRange(
                250, 120, 65, 8, HoleDistributionType.Staggered);

            Assert.AreEqual(minStraight, minStaggered, 0.01);
            Assert.IsTrue(maxStaggered >= maxStraight);
        }
    }
}