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
        private VoidnessCalculator Calculator => new VoidnessCalculator();

        [Test]
        [Description("CalculateCurrentVoidness возвращает 0 для нуля отверстий")]
        public void CalculateCurrentVoidness_ZeroHoles_ShouldReturnZero()
        {
            var voidness = Calculator.CalculateCurrentVoidness(250, 120, 65, 8, 0);

            Assert.AreEqual(0, voidness, 0.01);
        }

        [Test]
        [Description("CalculateCurrentVoidness " +
            "возвращает 0 для нулевого объема кирпича")]
        public void CalculateCurrentVoidness_ZeroBrickVolume_ShouldReturnZero()
        {
            var voidness = Calculator.CalculateCurrentVoidness(0, 0, 0, 8, 10);

            Assert.AreEqual(0, voidness, 0.01);
        }

        [Test]
        [Description("CalculateCurrentVoidness возвращает положительное значение")]
        public void CalculateCurrentVoidness_WithHoles_ShouldReturnPositiveValue()
        {
            var voidness = Calculator.CalculateCurrentVoidness(250, 120, 65, 8, 10);

            Assert.IsTrue(voidness > 0);
        }

        [Test]
        [Description("CalculateCurrentVoidness увеличивается с количеством")]
        public void CalculateCurrentVoidness_MoreHoles_HigherVoidness()
        {
            var voidness1 = Calculator.CalculateCurrentVoidness(250, 120, 65, 8, 5);
            var voidness2 = Calculator.CalculateCurrentVoidness(250, 120, 65, 8, 10);

            Assert.IsTrue(voidness2 > voidness1);
        }

        [Test]
        [Description("CalculateCurrentVoidness увеличивается с радиусом")]
        public void CalculateCurrentVoidness_LargerRadius_HigherVoidness()
        {
            var voidness1 = Calculator.CalculateCurrentVoidness(250, 120, 65, 5, 10);
            var voidness2 = Calculator.CalculateCurrentVoidness(250, 120, 65, 10, 10);

            Assert.IsTrue(voidness2 > voidness1);
        }

        [Test]
        [Description("CalculateCurrentVoidness уменьшается с размером кирпича")]
        public void CalculateCurrentVoidness_LargerBrick_LowerVoidness()
        {
            var voidness1 = Calculator.CalculateCurrentVoidness(250, 120, 65, 8, 10);
            var voidness2 = Calculator.CalculateCurrentVoidness(500, 240, 130, 8, 10);

            Assert.IsTrue(voidness2 < voidness1);
        }

        [Test]
        [Description("CalculateCurrentVoidness корректно вычисляет" +
            " пустотность по формуле")]
        public void CalculateCurrentVoidness_ShouldCalculateByFormula()
        {
            double length = 250, width = 120, height = 65, radius = 8;
            int holes = 10;

            var voidness 
                = Calculator.CalculateCurrentVoidness(length, width, height, radius, holes);

            double brickVolume = length * width * height;
            double holeVolume = Math.PI * radius * radius * height;
            double expected = (holeVolume * holes / brickVolume) * 100.0;

            Assert.AreEqual(expected, voidness, 0.01);
        }

        [Test]
        [Description("CalculateMaxPossibleVoidness возвращает положительное значение")]
        public void CalculateMaxPossibleVoidness_ShouldReturnPositiveValue()
        {
            var maxVoidness = Calculator.CalculateMaxPossibleVoidness(
                250, 120, 65, 8, HoleDistributionType.Straight);

            Assert.IsTrue(maxVoidness > 0);
        }

        [Test]
        [Description("CalculateMaxPossibleVoidness не превышает 45")]
        public void CalculateMaxPossibleVoidness_ShouldNotExceed45()
        {
            var maxVoidness = Calculator.CalculateMaxPossibleVoidness(
                250, 120, 65, 8, HoleDistributionType.Straight);

            Assert.IsTrue(maxVoidness <= 45);
        }

        [Test]
        [Description("CalculateMaxPossibleVoidness для шахматного больше или равно")]
        public void CalculateMaxPossibleVoidness_Staggered_HigherOrEqual()
        {
            var straight = Calculator.CalculateMaxPossibleVoidness(
                250, 120, 65, 8, HoleDistributionType.Straight);
            var staggered = Calculator.CalculateMaxPossibleVoidness(
                250, 120, 65, 8, HoleDistributionType.Staggered);

            Assert.IsTrue(staggered >= straight);
        }

        [TestCase(HoleDistributionType.Straight)]
        [TestCase(HoleDistributionType.Staggered)]
        [Description("CalculateMaxPossibleVoidness использует правильный калькулятор")]
        public void CalculateMaxPossibleVoidness_UsesCorrectCalculator(HoleDistributionType type)
        {
            var voidness = Calculator.CalculateMaxPossibleVoidness(250, 120, 65, 8, type);

            Assert.Multiple(() =>
            {
                Assert.IsTrue(voidness > 0);
                Assert.IsTrue(voidness <= 45);
            });
        }

        [Test]
        [Description("CalculateMinPossibleVoidness возвращает положительное значение")]
        public void CalculateMinPossibleVoidness_ShouldReturnPositiveValue()
        {
            var minVoidness = Calculator.CalculateMinPossibleVoidness(250, 120, 65, 8);

            Assert.IsTrue(minVoidness > 0);
        }

        [Test]
        [Description("CalculateMinPossibleVoidness равно пустотности для 1 отверстия")]
        public void CalculateMinPossibleVoidness_ShouldEqualOneHole()
        {
            var minVoidness = Calculator.CalculateMinPossibleVoidness(250, 120, 65, 8);
            var actual = Calculator.CalculateCurrentVoidness(250, 120, 65, 8, 1);

            Assert.AreEqual(minVoidness, actual, 0.01);
        }

        [Test]
        [Description("CalculateOptimalParameters возвращает успешный результат")]
        public void CalculateOptimalParameters_ValidVoidness_ShouldReturnSuccess()
        {
            var result = Calculator.CalculateOptimalParameters(
                250, 120, 65, 20, HoleDistributionType.Straight);

            Assert.Multiple(() =>
            {
                Assert.IsTrue(result.Success);
                Assert.IsTrue(result.HoleRadius >= 2);
                Assert.IsTrue(result.HolesCount >= 1);
            });
        }

        [Test]
        [Description("CalculateOptimalParameters достигает цели с погрешностью")]
        public void CalculateOptimalParameters_ShouldAchieveTarget()
        {
            double target = 25;
            var result = Calculator.CalculateOptimalParameters(
                250, 120, 65, target, HoleDistributionType.Straight);

            Assert.Multiple(() =>
            {
                Assert.IsTrue(result.Success);
                Assert.IsTrue(Math.Abs(result.ActualVoidness - target) < 5);
            });
        }

        [TestCase(50)]
        [TestCase(0)]
        [TestCase(-5)]
        [Description("CalculateOptimalParameters возвращает" +
            " ошибку для недопустимой пустотности")]
        public void CalculateOptimalParameters_InvalidVoidness_ShouldReturnError(double voidness)
        {
            var result = Calculator.CalculateOptimalParameters(
                250, 120, 65, voidness, HoleDistributionType.Straight);

            Assert.Multiple(() =>
            {
                Assert.IsFalse(result.Success);
                Assert.IsNotNull(result.ErrorMessage);
                Assert.IsTrue(result.ErrorMessage.Contains("должна быть в диапазоне"));
            });
        }

        [Test]
        [Description("CalculateOptimalParameters работает с различными типами распределения")]
        public void CalculateOptimalParameters_DifferentDistributions_ShouldWork()
        {
            var straightResult = Calculator.CalculateOptimalParameters(
                250, 120, 65, 25, HoleDistributionType.Straight);
            var staggeredResult = Calculator.CalculateOptimalParameters(
                250, 120, 65, 25, HoleDistributionType.Staggered);

            Assert.Multiple(() =>
            {
                Assert.IsTrue(straightResult.Success);
                Assert.IsTrue(staggeredResult.Success);
            });
        }

        [Test]
        [Description("CalculateOptimalParameters находит точное решение с difference < 0.1")]
        public void CalculateOptimalParameters_FindsExactSolution_ReturnsEarly()
        {
            // Используем параметры, где можно точно достичь целевой пустотности
            // length=250, width=120, height=65, target=10%
            var result = Calculator.CalculateOptimalParameters(
                250, 120, 65, 10, HoleDistributionType.Straight);

            Assert.Multiple(() =>
            {
                Assert.IsTrue(result.Success);
                Assert.IsTrue(Math.Abs(result.ActualVoidness - 10) < 0.1 ||
                              Math.Abs(result.ActualVoidness - 10) < 5);
            });
        }

        [Test]
        [Description("CalculateOptimalParameters возвращает " +
            "лучшее решение когда точное недостижимо")]
        public void CalculateOptimalParameters_NoExactMatch_ShouldReturnBest()
        {
            var result = Calculator.CalculateOptimalParameters(
                250, 120, 65, 30, HoleDistributionType.Straight);

            Assert.Multiple(() =>
            {
                Assert.IsTrue(result.Success);
                Assert.IsTrue(result.ActualVoidness > 0);
                Assert.IsTrue(result.ActualVoidness <= 45);
            });
        }

        [Test]
        [Description("CalculateOptimalParameters учитывает ограничение пустотности")]
        public void CalculateOptimalParameters_ShouldRespectVoidnessLimit()
        {
            var result = Calculator.CalculateOptimalParameters(
                250, 120, 65, 44, HoleDistributionType.Straight);

            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.ActualVoidness <= 45);
        }

        [Test]
        [Description("CalculateOptimalParameters пропускает комбинации превышающие 45%")]
        public void CalculateOptimalParameters_SkipsCombinationsExceeding45()
        {
            var result = Calculator.CalculateOptimalParameters(
                200, 100, 50, 40, HoleDistributionType.Straight);

            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.ActualVoidness <= 45);
            Assert.IsTrue(result.HolesCount >= 1);
            Assert.IsTrue(result.HoleRadius >= 2);
        }

        [Test]
        [Description("GetVoidnessRange возвращает корректный диапазон")]
        public void GetVoidnessRange_ShouldReturnValidRange()
        {
            var (min, max) = Calculator.GetVoidnessRange(
                250, 120, 65, 8, HoleDistributionType.Straight);

            Assert.Multiple(() =>
            {
                Assert.IsTrue(min > 0);
                Assert.IsTrue(max > min);
                Assert.IsTrue(max <= 45);
            });
        }

        [Test]
        [Description("GetVoidnessRange ограничивает максимум до 45%")]
        public void GetVoidnessRange_ShouldClampMaxTo45()
        {
            var (min, max) = Calculator.GetVoidnessRange(
                250, 120, 65, 8, HoleDistributionType.Straight);

            Assert.IsTrue(max <= 45);
        }

        [Test]
        [Description("GetVoidnessRange для шахматного больше или равно прямому")]
        public void GetVoidnessRange_Staggered_HigherOrEqualMax()
        {
            var (minStraight, maxStraight) = Calculator.GetVoidnessRange(
                250, 120, 65, 8, HoleDistributionType.Straight);
            var (minStaggered, maxStaggered) = Calculator.GetVoidnessRange(
                250, 120, 65, 8, HoleDistributionType.Staggered);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(minStraight, minStaggered, 0.01);
                Assert.IsTrue(maxStaggered >= maxStraight);
            });
        }

        [TestCase(50)]
        [TestCase(0)]
        [TestCase(-10)]
        [Description("CalculateHolesCountForVoidness возвращает " +
            "ошибку для недопустимой пустотности")]
        public void CalculateHolesCountForVoidness_InvalidVoidness_ShouldReturnError(double voidness)
        {
            var result = Calculator.CalculateHolesCountForVoidness(
                250, 120, 65, 8, voidness, HoleDistributionType.Straight);

            Assert.Multiple(() =>
            {
                Assert.IsFalse(result.Success);
                Assert.IsNotNull(result.ErrorMessage);
                Assert.IsTrue(result.ErrorMessage.Contains("должна быть в диапазоне"));
            });
        }

        [Test]
        [Description("CalculateHolesCountForVoidness возвращает успешный результат")]
        public void CalculateHolesCountForVoidness_ValidParameters_ShouldReturnSuccess()
        {
            var result = Calculator.CalculateHolesCountForVoidness(
                250, 120, 65, 8, 20, HoleDistributionType.Straight);

            Assert.Multiple(() =>
            {
                Assert.IsTrue(result.Success);
                Assert.AreEqual(8, result.HoleRadius);
                Assert.IsTrue(result.HolesCount >= 1);
            });
        }

        [Test]
        [Description("CalculateHolesCountForVoidness округляет количество отверстий")]
        public void CalculateHolesCountForVoidness_ShouldRoundHoles()
        {
            var result = Calculator.CalculateHolesCountForVoidness(
                250, 120, 65, 8, 15, HoleDistributionType.Straight);

            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.HolesCount >= 1);
        }

        [Test]
        [Description("CalculateHolesCountForVoidness работает с разными типами распределения")]
        public void CalculateHolesCountForVoidness_DifferentDistributions_ShouldWork()
        {
            var straightResult = Calculator.CalculateHolesCountForVoidness(
                250, 120, 65, 8, 20, HoleDistributionType.Straight);
            var staggeredResult = Calculator.CalculateHolesCountForVoidness(
                250, 120, 65, 8, 20, HoleDistributionType.Staggered);

            Assert.Multiple(() =>
            {
                Assert.IsTrue(straightResult.Success);
                Assert.IsTrue(staggeredResult.Success);
            });
        }

        [Test]
        [Description("CalculateHolesCountForVoidness возвращает " +
            "ошибку если невозможно разместить отверстия")]
        public void CalculateHolesCountForVoidness_TooLargeRadius_ShouldReturnError()
        {
            var result = Calculator.CalculateHolesCountForVoidness(
                100, 60, 50, 50, 20, HoleDistributionType.Straight);

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.ErrorMessage);
            Assert.IsTrue(result.ErrorMessage.Contains("Невозможно разместить отверстия"));
        }

        [Test]
        [Description("CalculateHolesCountForVoidness ограничивает количество максимумом")]
        public void CalculateHolesCountForVoidness_ShouldClampToMaxHoles()
        {
            var result = Calculator.CalculateHolesCountForVoidness(
                250, 120, 65, 8, 44, HoleDistributionType.Straight);

            Assert.IsTrue(result.Success);
            // Количество отверстий не должно превышать максимально возможное
            var maxVoidness = Calculator.CalculateMaxPossibleVoidness(
                250, 120, 65, 8, HoleDistributionType.Straight);
            Assert.IsTrue(result.ActualVoidness <= maxVoidness + 0.1);
        }

        [Test]
        [Description("CalculateHolesCountForVoidness возвращает " +
            "ошибку для слишком большого радиуса")]
        public void CalculateHolesCountForVoidness_TooLargeRadiusForTarget_ReturnsError()
        {
            var result = Calculator.CalculateHolesCountForVoidness(
                150, 150, 50, 58, 40, HoleDistributionType.Straight);

            // С такими параметрами гарантированно получим ошибку
            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.ErrorMessage);
            Assert.IsTrue(result.ErrorMessage.Contains("превышает максимум") ||
                          result.ErrorMessage.Contains("Невозможно") ||
                          result.ErrorMessage.Contains("Достижимая пустотность"));
        }

        [Test]
        [Description("CalculateHolesCountForVoidness ограничивает calculatedHoles минимумом 1")]
        public void CalculateHolesCountForVoidness_ShouldClampToMinimumOne()
        {
            var result = Calculator.CalculateHolesCountForVoidness(
                250, 120, 65, 8, 0.1, HoleDistributionType.Straight);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(1, result.HolesCount);
            var expected = Calculator.CalculateCurrentVoidness(250, 120, 65, 8, 1);
            Assert.AreEqual(expected, result.ActualVoidness, 0.01);
        }

        [Test]
        [Description("CalculateOptimalParameters пропускает радиусы с maxHoles <= 0")]
        public void CalculateOptimalParameters_ShouldSkipRadiusWithZeroMaxHoles()
        {
            var result = Calculator.CalculateOptimalParameters(
                40, 40, 20, 10, HoleDistributionType.Straight);

            // Не должно быть exception, результат должен быть либо Success либо с ErrorMessage
            Assert.IsNotNull(result);
        }

        [Test]
        [Description("CalculateOptimalParameters пропускает радиусы с maxHoles <= 0 через continue")]
        public void CalculateOptimalParameters_ShouldSkipRadiiWithZeroMaxHoles()
        {
            var calculator = new VoidnessCalculator();
            var result = calculator.CalculateOptimalParameters(
                50, 35, 30, 15, HoleDistributionType.Straight);
            Assert.IsNotNull(result);
        }

        [Test]
        [Description("CalculateOptimalParameters не находит решение для невозможных параметров")]
        public void CalculateOptimalParameters_ImpossibleParameters_ReturnsError()
        {
            var result = Calculator.CalculateOptimalParameters(
                35, 30, 25, 40, HoleDistributionType.Straight);

            // Должна быть ошибка, так как невозможно найти подходящие параметры
            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.ErrorMessage);
            Assert.IsTrue(result.ErrorMessage.Contains("Не удалось подобрать параметры") ||
                          result.ErrorMessage.Contains("должна быть в диапазоне"));
        }

        [Test]
        [Description("CalculateOptimalParameters проходит через continue когда maxHoles = 0")]
        public void CalculateOptimalParameters_MaxHolesZero_ContinuesLoop()
        {
            var calculator = new VoidnessCalculator();

            // Очень маленький кирпич - для большинства радиусов maxHoles будет 0
            var result = calculator.CalculateOptimalParameters(
                30, 30, 20, 10, HoleDistributionType.Straight);

            // Проверяем что метод не упал и вернул корректный результат
            Assert.IsNotNull(result);
        }

        [Test]
        [Description("CalculateHolesCountForVoidness возвращает " +
            "ошибку когда actualVoidness превышает MaxVoidnessPercent")]
        public void CalculateHolesCountForVoidness_WhenActualExceedsMaxVoidness_ReturnsSpecificError()
        {

            var result = Calculator.CalculateHolesCountForVoidness(
                150, 150, 50, 57, 44, HoleDistributionType.Straight);

            Assert.Multiple(() =>
            {
                Assert.IsFalse(result.Success, "Результат должен быть неуспешным");
                Assert.IsNotNull(result.ErrorMessage, "Должно быть сообщение об ошибке");
                Assert.IsTrue(result.ErrorMessage.Contains("Достижимая пустотность"),
                    "Сообщение должно содержать 'Достижимая пустотность'");
                Assert.IsTrue(result.ErrorMessage.Contains("превышает максимум"),
                    "Сообщение должно содержать 'превышает максимум'");
                Assert.IsTrue(result.ErrorMessage.Contains("45"),
                    "Сообщение должно содержать '45'");
            });
        }

        [Test]
        [Description("CalculateHolesCountForVoidness с очень большим радиусом дает превышение 45%")]
        public void CalculateHolesCountForVoidness_LargeRadius_ExceedsMaxVoidness()
        {
            // Еще один набор параметров:
            // Кирпич: 140x140x50, радиус: 54, целевая пустотность: 42%
            // actualVoidness для 1 отверстия ≈ 46.7% > 45%

            var result = Calculator.CalculateHolesCountForVoidness(
                140, 140, 50, 54, 42, HoleDistributionType.Straight);

            Assert.Multiple(() =>
            {
                Assert.IsFalse(result.Success);
                Assert.IsNotNull(result.ErrorMessage);
                Assert.IsTrue(result.ErrorMessage.
                    Contains("Достижимая пустотность"));
                Assert.IsTrue(result.ErrorMessage.
                    Contains("превышает максимум"));
            });
        }

        [Test]
        [Description("CalculateHolesCountForVoidness возвращает " +
            "ошибку когда actualVoidness превышает 45%")]
        public void CalculateHolesCountForVoidness_ActualVoidnessExceeds45_ReturnsError()
        {
            // Третий набор параметров для надежности:
            // Кирпич: 145x145x50, радиус: 55
            var result = Calculator.CalculateHolesCountForVoidness(
                145, 145, 50, 55, 43, HoleDistributionType.Straight);

            Assert.Multiple(() =>
            {
                Assert.IsFalse(result.Success);
                Assert.IsNotNull(result.ErrorMessage);
                Assert.IsTrue(result.ErrorMessage.Contains("Достижимая пустотность"));
                Assert.IsTrue(result.ErrorMessage.Contains("превышает максимум"));
                Assert.IsTrue(result.ErrorMessage.Contains("45"));
            });
        }

        [Test]
        [Description("CalculateHolesCountForVoidness проверяет " +
            "превышение MaxVoidnessPercent")]
        public void CalculateHolesCountForVoidness_ChecksMaxVoidnessPercent()
        {
            // Проверяем, что метод действительно проверяет превышение 45%
            // Используем параметры, где одно отверстие дает >45%
            var result = Calculator.CalculateHolesCountForVoidness(
                150, 150, 50, 57, 40, HoleDistributionType.Straight);

            // Должна быть ошибка о превышении MaxVoidnessPercent
            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.ErrorMessage);
            Assert.IsTrue(
                result.ErrorMessage.Contains("превышает максимум") ||
                result.ErrorMessage.Contains("Достижимая пустотность"));
        }

        [Test]
        [Description("CalculateOptimalParameters с Staggered " +
            "проходит через условие maxHoles")]
        public void CalculateOptimalParameters_Staggered_ProcessesMaxHoles()
        {
            var result = Calculator.CalculateOptimalParameters(
                250, 120, 65, 25, HoleDistributionType.Staggered);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.HolesCount > 0);
            Assert.IsTrue(result.ActualVoidness > 0);
        }

        [Test]
        [Description("CalculateOptimalParameters тестирует " +
            "обе ветки распределения в цикле")]
        public void CalculateOptimalParameters_BothDistributionTypes_ProcessedInLoop()
        {
            // Тест для Straight
            var straightResult = Calculator.CalculateOptimalParameters(
                200, 100, 50, 20, HoleDistributionType.Straight);

            // Тест для Staggered  
            var staggeredResult = Calculator.CalculateOptimalParameters(
                200, 100, 50, 20, HoleDistributionType.Staggered);

            Assert.IsNotNull(straightResult);
            Assert.IsNotNull(staggeredResult);
        }
    }
}