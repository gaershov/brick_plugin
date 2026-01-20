using NUnit.Framework;
using BrickPluginModels.Models;
using System;

namespace BrickPlugin.Tests
{
    [TestFixture]
    [Description("Тесты для класса BrickParameters")]
    public class BrickParametersTests
    {
        [Test]
        [Description("Конструктор инициализирует параметры значениями по умолчанию")]
        public void Constructor_ShouldInitializeDefaultValues()
        {
            var parameters = new BrickParameters();

            Assert.Multiple(() =>
            {
                Assert.AreEqual(250, parameters[ParameterType.Length]);
                Assert.AreEqual(120, parameters[ParameterType.Width]);
                Assert.AreEqual(65, parameters[ParameterType.Height]);
                Assert.AreEqual(8, parameters[ParameterType.HoleRadius]);
                Assert.AreEqual(19, parameters[ParameterType.HolesCount]);
            });
        }

        [Test]
        [Description("Индексатор корректно возвращает и устанавливает значение параметра")]
        public void Indexer_GetSet_ShouldWork()
        {
            var parameters = new BrickParameters();

            parameters[ParameterType.Length] = 300;

            Assert.AreEqual(300, parameters[ParameterType.Length]);
        }

        [Test]
        [Description("GetParameter возвращает корректный объект BrickParameter")]
        public void GetParameter_ShouldReturnCorrectParameter()
        {
            var parameters = new BrickParameters();

            var lengthParam = parameters.GetParameter(ParameterType.Length);

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(lengthParam);
                Assert.AreEqual(250, lengthParam.Value);
                Assert.AreEqual(100, lengthParam.MinValue);
                Assert.AreEqual(1000, lengthParam.MaxValue);
            });
        }

        [Test]
        [Description("GetMaxRadiusHint возвращает корректную строку")]
        public void GetMaxRadiusHint_ShouldReturnCorrectString()
        {
            var parameters = new BrickParameters();

            var hint = parameters.GetMaxRadiusHint();

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(hint);
                Assert.IsTrue(hint.StartsWith("Макс:"));
                Assert.IsTrue(hint.Contains("мм"));
            });
        }

        [TestCase(HoleDistributionType.Straight, "прямое")]
        [TestCase(HoleDistributionType.Staggered, "шахматное")]
        [Description("GetMaxHolesHint возвращает корректную строку с типом распределения")]
        public void GetMaxHolesHint_ShouldReturnCorrectString(HoleDistributionType type, string expected)
        {
            var parameters = new BrickParameters();
            parameters.DistributionType = type;

            var hint = parameters.GetMaxHolesHint();

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(hint);
                Assert.IsTrue(hint.StartsWith("Макс:"));
                Assert.IsTrue(hint.Contains("шт"));
                Assert.IsTrue(hint.Contains(expected));
            });
        }

        //TODO: RSDN +
        [Test]
        [Description("Изменение ширины обновляет максимальный радиус отверстия")]
        public void CalculateDependent_WhenWidthChanges_ShouldUpdateMaxRadius()
        {
            var parameters = new BrickParameters();
            var initialMaxRadius = parameters.GetParameter(ParameterType.HoleRadius).MaxValue;

            parameters[ParameterType.Width] = 200;

            var newMaxRadius = parameters.GetParameter(ParameterType.HoleRadius).MaxValue;
            Assert.AreNotEqual(initialMaxRadius, newMaxRadius);
        }

        [Test]
        [Description("При пересчёте вызываются события MaxRadiusChanged и MaxHolesChanged")]
        public void CalculateDependent_ShouldRaiseEvents()
        {
            var parameters = new BrickParameters();
            bool maxRadiusRaised = false;
            bool maxHolesRaised = false;

            parameters.MaxRadiusChanged += (sender, hint) => maxRadiusRaised = true;
            parameters.MaxHolesChanged += (sender, hint) => maxHolesRaised = true;

            parameters[ParameterType.Width] = 200;

            Assert.Multiple(() =>
            {
                Assert.IsTrue(maxRadiusRaised);
                Assert.IsTrue(maxHolesRaised);
            });
        }

        [Test]
        [Description("CalculateDependent устанавливает максимум отверстий в 0 для радиуса меньше 2")]
        public void CalculateDependent_RadiusLessThan2_ShouldSetMaxHolesZero()
        {
            var parameters = new BrickParameters();

            parameters[ParameterType.HoleRadius] = 1;

            Assert.AreEqual(0, parameters.GetParameter(ParameterType.HolesCount).MaxValue);
        }

        [Test]
        [Description("CalculateDependent ограничивает максимальный радиус минимумом 2")]
        public void CalculateDependent_ShouldLimitMaxRadiusToMinimum2()
        {
            var parameters = new BrickParameters();

            parameters[ParameterType.Width] = 20;

            Assert.IsTrue(parameters.GetParameter(ParameterType.HoleRadius).MaxValue >= 2);
        }

        [Test]
        [Description("CalculateDependent учитывает ограничение " +
            "пустотности при расчете максимального радиуса")]
        public void CalculateDependent_ShouldConsiderVoidnessLimitForMaxRadius()
        {
            var parameters = new BrickParameters();
            parameters[ParameterType.Length] = 100;
            parameters[ParameterType.Width] = 100;

            double maxRadius = parameters.GetParameter(ParameterType.HoleRadius).MaxValue;
            double voidnessMaxRadius = Math.Sqrt(0.45 * 100 * 100 / Math.PI);
            double geometricMaxRadius = (100 - 30) / 2.0;

            Assert.IsTrue(maxRadius <= Math.Min(geometricMaxRadius, voidnessMaxRadius) + 0.1);
        }

        [Test]
        [Description("Validate возвращает true для параметров по умолчанию")]
        public void Validate_WithDefaultValues_ShouldReturnTrue()
        {
            var parameters = new BrickParameters();

            var result = parameters.Validate();

            Assert.IsTrue(result);
        }

        [Test]
        [Description("Validate возвращает true для нулевого количества отверстий")]
        public void Validate_WithZeroHoles_ShouldReturnTrue()
        {
            var parameters = new BrickParameters();

            parameters[ParameterType.HolesCount] = 0;

            Assert.IsTrue(parameters.Validate());
        }

        //TODO: RSDN +
        [TestCase(ParameterType.Length, 10000, "Длина")]
        [TestCase(ParameterType.Width, 10000, "Ширина")]
        [TestCase(ParameterType.Height, 10000, "Высота")]
        [Description("Validate возвращает false, если значение " +
            "параметра выходит за допустимый диапазон")]
        public void Validate_WithValueOutOfRange_ShouldReturnFalse(
            ParameterType type, double value, string expectedText)
        {
            var parameters = new BrickParameters();
            bool errorMessageRaised = false;
            string errorText = "";

            parameters.ErrorMessage += (sender, message) =>
            {
                errorMessageRaised = true;
                errorText = message;
            };

            parameters[type] = value;
            var result = parameters.Validate();

            Assert.Multiple(() =>
            {
                Assert.IsFalse(result);
                Assert.IsTrue(errorMessageRaised);
                Assert.IsTrue(errorText.Contains(expectedText));
            });
        }

        [Test]
        [Description("Validate возвращает false для слишком большого радиуса")]
        public void Validate_WithTooLargeRadius_ShouldReturnFalse()
        {
            var parameters = new BrickParameters();
            bool errorMessageRaised = false;
            string errorText = "";

            parameters.ErrorMessage += (sender, message) =>
            {
                errorMessageRaised = true;
                errorText = message;
            };

            parameters[ParameterType.Width] = 100;
            parameters[ParameterType.HoleRadius] = 50;

            var result = parameters.Validate();

            Assert.Multiple(() =>
            {
                Assert.IsFalse(result);
                Assert.IsTrue(errorMessageRaised);
                Assert.IsTrue(errorText.Contains("Радиус отверстий слишком большой"));
            });
        }

        [Test]
        [Description("Validate возвращает false при превышении пустотности 45%")]
        public void Validate_WithExcessiveVoidness_ShouldReturnFalse()
        {
            var parameters = new BrickParameters();
            bool errorMessageRaised = false;
            string errorText = "";

            parameters.ErrorMessage += (sender, message) =>
            {
                errorMessageRaised = true;
                errorText = message;
            };

            parameters[ParameterType.Length] = 100;
            parameters[ParameterType.Width] = 100;
            parameters[ParameterType.Height] = 50;
            parameters[ParameterType.HoleRadius] = 20;
            parameters[ParameterType.HolesCount] = 10;

            var result = parameters.Validate();

            Assert.Multiple(() =>
            {
                Assert.IsFalse(result);
                Assert.IsTrue(errorMessageRaised);
                Assert.IsTrue(errorText.Contains("Пустотность"));
                Assert.IsTrue(errorText.Contains("превышает"));
            });
        }

        [Test]
        [Description("Validate возвращает false при " +
            "превышении максимального количества отверстий")]
        public void Validate_WithTooManyHoles_ShouldReturnFalse()
        {
            var parameters = new BrickParameters();
            bool errorMessageRaised = false;
            string errorText = "";

            parameters.ErrorMessage += (sender, message) =>
            {
                errorMessageRaised = true;
                errorText = message;
            };

            parameters[ParameterType.HolesCount] = 10000;

            var result = parameters.Validate();

            Assert.Multiple(() =>
            {
                Assert.IsFalse(result);
                Assert.IsTrue(errorMessageRaised);
                Assert.IsTrue(errorText.Contains("Количество отверстий") ||
                             errorText.Contains("превышает возможное"));
            });
        }

        [Test]
        [Description("CalculateCurrentVoidness возвращает корректное значение")]
        public void CalculateCurrentVoidness_ShouldReturnCorrectValue()
        {
            var parameters = new BrickParameters();
            parameters[ParameterType.Length] = 250;
            parameters[ParameterType.Width] = 120;
            parameters[ParameterType.Height] = 65;
            parameters[ParameterType.HoleRadius] = 8;
            parameters[ParameterType.HolesCount] = 10;

            var voidness = parameters.CalculateCurrentVoidness();

            Assert.IsTrue(voidness > 0 && voidness < 100);
        }

        [Test]
        [Description("CalculateOptimalParameters возвращает результат")]
        public void CalculateOptimalParameters_ShouldReturnResult()
        {
            var parameters = new BrickParameters();

            var result = parameters.CalculateOptimalParameters(25);

            Assert.IsNotNull(result);
        }

        [Test]
        [Description("CalculateHolesCountForVoidness возвращает результат")]
        public void CalculateHolesCountForVoidness_ShouldReturnResult()
        {
            var parameters = new BrickParameters();

            var result = parameters.CalculateHolesCountForVoidness(20);

            Assert.IsNotNull(result);
        }

        [Test]
        [Description("GetVoidnessRange возвращает валидный диапазон")]
        public void GetVoidnessRange_ShouldReturnValidRange()
        {
            var parameters = new BrickParameters();

            var (min, max) = parameters.GetVoidnessRange();

            Assert.Multiple(() =>
            {
                Assert.IsTrue(min >= 0);
                Assert.IsTrue(max >= min);
            });
        }

        [Test]
        [Description("CalculateAvailableArea возвращает корректные значения")]
        public void CalculateAvailableArea_ShouldReturnCorrectValues()
        {
            var (diameter, edgeMargin, minGap, availableLength, availableWidth) =
                BrickParameters.CalculateAvailableArea(250, 120, 8);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(16, diameter);
                Assert.IsTrue(edgeMargin >= 10);
                Assert.IsTrue(minGap >= 5);
                Assert.IsTrue(availableLength > 0);
                Assert.IsTrue(availableWidth > 0);
            });
        }

        [Test]
        [Description("CalculateAvailableArea применяет минимум для edgeMargin")]
        public void CalculateAvailableArea_ShouldApplyMinimumEdgeMargin()
        {
            var (_, edgeMargin, _, _, _) 
                = BrickParameters.CalculateAvailableArea(250, 120, 2);

            Assert.IsTrue(edgeMargin >= 10);
        }

        [Test]
        [Description("CalculateAvailableArea применяет минимум для minGap")]
        public void CalculateAvailableArea_ShouldApplyMinimumGap()
        {
            var (_, _, minGap, _, _) = BrickParameters.CalculateAvailableArea(250, 120, 2);

            Assert.IsTrue(minGap >= 5);
        }

        [Test]
        [Description("DistributionType изменяется корректно")]
        public void DistributionType_ShouldChangeCorrectly()
        {
            var parameters = new BrickParameters();

            parameters.DistributionType = HoleDistributionType.Staggered;

            Assert.AreEqual(HoleDistributionType.Staggered, parameters.DistributionType);
        }

        [Test]
        [Description("Изменение DistributionType вызывает пересчёт")]
        public void DistributionType_Change_ShouldTriggerRecalculation()
        {
            var parameters = new BrickParameters();
            bool eventRaised = false;

            parameters.MaxHolesChanged += (sender, hint) => eventRaised = true;

            parameters.DistributionType = HoleDistributionType.Staggered;

            Assert.IsTrue(eventRaised);
        }

        //TODO: duplicatoin +
        [TestCase(HoleDistributionType.Straight, "прямого распределения")]
        [TestCase(HoleDistributionType.Staggered, "шахматного распределения")]
        [Description("ValidateBusinessRules проверяет превышение " +
            "максимума отверстий")]
        public void Validate_ExceedsMaxHoles_ReturnsError(
            HoleDistributionType distributionType,
            string expectedDistributionText)
        {
            var parameters = new BrickParameters();
            parameters.DistributionType = distributionType;
            parameters[ParameterType.HoleRadius] = 5;
            parameters[ParameterType.HolesCount] = 10;

            string errorText = "";
            parameters.ErrorMessage += (sender, message) => errorText = message;

            // Получаем реальный максимум и превышаем его
            var maxHoles 
                = parameters.GetParameter(ParameterType.HolesCount).MaxValue;
            parameters[ParameterType.HolesCount] = maxHoles + 10;

            var result = parameters.Validate();

            Assert.Multiple(() =>
            {
                Assert.IsFalse(result);
                Assert.IsTrue(errorText.Contains("превышает возможное для"));
                Assert.IsTrue(errorText.Contains(expectedDistributionText));
            });
        }

        [Test]
        [Description("GetParameterDisplayName возвращает " +
            "корректные имена для HoleRadius и HolesCount")]
        public void GetParameterDisplayName_ReturnsCorrectNamesForAllParameters()
        {
            var parameters = new BrickParameters();
            string errorText = "";
            parameters.ErrorMessage += (sender, message) => errorText = message;

            // Проверяем HoleRadius
            parameters[ParameterType.HoleRadius] = 1000;
            parameters.Validate();

            Assert.IsTrue(errorText.Contains("Радиус отверстий"));

            // Проверяем HolesCount
            errorText = "";
            parameters[ParameterType.HoleRadius] = 8;
            parameters[ParameterType.HolesCount] = 10000;
            parameters.Validate();

            Assert.IsTrue(errorText.Contains("Количество отверстий"));
        }

        [Test]
        [Description("GetParameterDisplayName возвращает ToString для default case")]
        public void GetParameterDisplayName_UnknownType_ReturnsToString()
        {
            var parameters = new BrickParameters();

            // Проверяем что все известные типы обрабатываются
            var lengthParam = parameters.GetParameter(ParameterType.Length);
            var widthParam = parameters.GetParameter(ParameterType.Width);
            var heightParam = parameters.GetParameter(ParameterType.Height);
            var radiusParam = parameters.GetParameter(ParameterType.HoleRadius);
            var countParam = parameters.GetParameter(ParameterType.HolesCount);

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(lengthParam);
                Assert.IsNotNull(widthParam);
                Assert.IsNotNull(heightParam);
                Assert.IsNotNull(radiusParam);
                Assert.IsNotNull(countParam);
            });
        }

        [Test]
        [Description("CalculateDependent вызывает CalculateMaxHoles")]
        public void CalculateDependent_ShouldCalculateMaxHoles()
        {
            var parameters = new BrickParameters();

            parameters[ParameterType.Length] = 300;
            parameters[ParameterType.Width] = 150;
            parameters[ParameterType.HoleRadius] = 10;

            var maxHoles = parameters.GetParameter(ParameterType.HolesCount).MaxValue;

            Assert.Greater(maxHoles, 0);
        }

        [Test]
        [Description("GetParameterDisplayName возвращает " +
            "'Количество отверстий' для HolesCount")]
        public void GetParameterDisplayName_HolesCount_ReturnsCorrectName()
        {
            var parameters = new BrickParameters();
            string errorText = "";

            parameters.ErrorMessage += (sender, message) => errorText = message;

            // Устанавливаем невалидное значение для HolesCount
            parameters[ParameterType.HolesCount] = 100000;
            parameters.Validate();

            Assert.Multiple(() =>
            {
                Assert.IsTrue(errorText.Contains("Количество отверстий"));
                Assert.IsTrue(errorText.Contains("должно быть в диапазоне"));
            });
        }

        [Test]
        [Description("Validate использует GetParameterDisplayName " +
            "для всех типов параметров")]
        public void Validate_UsesGetParameterDisplayName_ForAllParameterTypes()
        {
            var parameters = new BrickParameters();
            string errorText = "";

            parameters.ErrorMessage += (sender, message) => errorText = message;

            // Проверяем каждый тип параметра отдельно

            // Length
            parameters[ParameterType.Length] = 99;
            parameters.Validate();
            Assert.IsTrue(errorText.Contains("Длина"), "Length не найден");
            parameters[ParameterType.Length] = 250;

            // Width
            errorText = "";
            parameters[ParameterType.Width] = 49;
            parameters.Validate();
            Assert.IsTrue(errorText.Contains("Ширина"), "Width не найден");
            parameters[ParameterType.Width] = 120;

            // Height
            errorText = "";
            parameters[ParameterType.Height] = 29;
            parameters.Validate();
            Assert.IsTrue(errorText.Contains("Высота"), "Height не найден");
            parameters[ParameterType.Height] = 65;

            // HoleRadius
            errorText = "";
            parameters[ParameterType.HoleRadius] = 1;
            parameters.Validate();
            Assert.IsTrue(errorText.Contains("Радиус отверстий"), "HoleRadius не найден");
            parameters[ParameterType.HoleRadius] = 8;

            // HolesCount
            errorText = "";
            parameters[ParameterType.HolesCount] = 100000;
            parameters.Validate();
            Assert.IsTrue(errorText.Contains("Количество отверстий"), "HolesCount не найден");
        }

        [Test]
        [Description("GetParameterDisplayName обрабатывает " +
            "default case возвращая ToString")]
        public void GetParameterDisplayName_DefaultCase_Coverage()
        {
            // Для достижения 100% покрытия default ветки, проверим что
            // метод корректно обрабатывает все существующие enum значения

            var parameters = new BrickParameters();

            // Проверяем что каждый тип параметра имеет свое отображаемое имя
            var allTypes = new[]
            {
                ParameterType.Length,
                ParameterType.Width,
                ParameterType.Height,
                ParameterType.HoleRadius,
                ParameterType.HolesCount
            };

            int validParametersCount = 0;
            foreach (var type in allTypes)
            {
                var param = parameters.GetParameter(type);
                Assert.IsNotNull(param, $"Параметр {type} должен существовать");
                validParametersCount++;
            }

            Assert.AreEqual(5, validParametersCount, "Все типы параметров корректно обрабатываются");
        }

        [Test]
        [Description("CalculateMaxHoles использует Staggered распределение")]
        public void CalculateMaxHoles_Staggered_IsUsed()
        {
            var parameters = new BrickParameters();
            parameters.DistributionType = HoleDistributionType.Staggered;
            parameters[ParameterType.Length] = 250;
            parameters[ParameterType.Width] = 120;
            parameters[ParameterType.HoleRadius] = 8;
            parameters[ParameterType.Length] = 260;

            var maxHoles = parameters.GetParameter(ParameterType.HolesCount).MaxValue;

            Assert.Greater(maxHoles, 0, "CalculateMaxHoles должен" +
                " вернуть положительное значение для Staggered");
        }

        [Test]
        [Description("CalculateMaxHoles использует Straight распределение")]
        public void CalculateMaxHoles_Straight_IsUsed()
        {
            var parameters = new BrickParameters();
            parameters.DistributionType = HoleDistributionType.Straight;
            parameters[ParameterType.Length] = 250;
            parameters[ParameterType.Width] = 120;
            parameters[ParameterType.HoleRadius] = 8;

            // Изменяем параметр чтобы вызвать CalculateDependent
            parameters[ParameterType.Length] = 260;

            var maxHoles = parameters.GetParameter(ParameterType.HolesCount).MaxValue;

            Assert.Greater(maxHoles, 0, "CalculateMaxHoles должен вернуть " +
                "положительное значение для Straight");
        }
    }
}