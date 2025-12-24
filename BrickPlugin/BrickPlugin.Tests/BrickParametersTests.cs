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

            Assert.AreEqual(250, parameters[ParameterType.Length]);
            Assert.AreEqual(120, parameters[ParameterType.Width]);
            Assert.AreEqual(65, parameters[ParameterType.Height]);
            Assert.AreEqual(8, parameters[ParameterType.HoleRadius]);
            Assert.AreEqual(19, parameters[ParameterType.HolesCount]);
        }

        [Test]
        [Description("Индексатор корректно возвращает значение параметра")]
        public void Indexer_Get_ShouldReturnCorrectValue()
        {
            var parameters = new BrickParameters();

            var length = parameters[ParameterType.Length];

            Assert.AreEqual(250, length);
        }

        [Test]
        [Description("Индексатор корректно устанавливает значение параметра")]
        public void Indexer_Set_ShouldUpdateValue()
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

            Assert.IsNotNull(lengthParam);
            Assert.AreEqual(250, lengthParam.Value);
            Assert.AreEqual(100, lengthParam.MinValue);
            Assert.AreEqual(1000, lengthParam.MaxValue);
        }

        [Test]
        [Description("GetMaxRadiusHint возвращает корректную строку")]
        public void GetMaxRadiusHint_ShouldReturnCorrectString()
        {
            var parameters = new BrickParameters();

            var hint = parameters.GetMaxRadiusHint();

            Assert.IsNotNull(hint);
            Assert.IsTrue(hint.StartsWith("Макс:"));
            Assert.IsTrue(hint.Contains("мм"));
        }

        [Test]
        [Description("GetMaxHolesHint возвращает корректную строку")]
        public void GetMaxHolesHint_ShouldReturnCorrectString()
        {
            var parameters = new BrickParameters();

            var hint = parameters.GetMaxHolesHint();

            Assert.IsNotNull(hint);
            Assert.IsTrue(hint.StartsWith("Макс:"));
            Assert.IsTrue(hint.Contains("шт"));
        }

        [Test]
        [Description("Изменение ширины обновляет максимальный радиус отверстия")]
        public void CalculateDependent_WhenWidthChanges_ShouldUpdateMaxRadius()
        {
            var parameters = new BrickParameters();
            var initialMaxRadius = 
                parameters.GetParameter(ParameterType.HoleRadius).MaxValue;

            parameters[ParameterType.Width] = 200;
            var newMaxRadius = 
                parameters.GetParameter(ParameterType.HoleRadius).MaxValue;

            Assert.AreNotEqual(initialMaxRadius, newMaxRadius);
        }

        [Test]
        [Description("При пересчёте вызывается событие MaxRadiusChanged")]
        public void CalculateDependent_ShouldRaiseMaxRadiusChangedEvent()
        {
            var parameters = new BrickParameters();
            bool eventRaised = false;
            parameters.MaxRadiusChanged += (sender, hint) => eventRaised = true;

            parameters[ParameterType.Width] = 200;

            Assert.IsTrue(eventRaised);
        }

        [Test]
        [Description("При пересчёте вызывается событие MaxHolesChanged")]
        public void CalculateDependent_ShouldRaiseMaxHolesChangedEvent()
        {
            var parameters = new BrickParameters();
            bool eventRaised = false;
            parameters.MaxHolesChanged += (sender, hint) => eventRaised = true;

            parameters[ParameterType.Width] = 200;

            Assert.IsTrue(eventRaised);
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
        [Description("Validate возвращает false, если значение " +
            "параметра выходит за допустимый диапазон")]
        public void Validate_WithValueOutOfRange_ShouldReturnFalse()
        {
            var parameters = new BrickParameters();
            bool errorMessageRaised = false;
            parameters.ErrorMessage += (sender, message) => errorMessageRaised = true;

            parameters[ParameterType.Length] = 10000;
            var result = parameters.Validate();

            Assert.IsFalse(result);
            Assert.IsTrue(errorMessageRaised);
        }

        [Test]
        [Description("Validate возвращает false для слишком большого радиуса")]
        public void Validate_WithTooLargeRadius_ShouldReturnFalse()
        {
            var parameters = new BrickParameters();
            parameters[ParameterType.Width] = 50;
            bool errorRaised = false;
            parameters.ErrorMessage += (sender, message) => errorRaised = true;

            parameters[ParameterType.HoleRadius] = 50;
            var result = parameters.Validate();

            Assert.IsFalse(result);
            Assert.IsTrue(errorRaised);
        }

        [Test]
        //TODO: RSDN
        [Description("Validate возвращает false для слишком большого количества отверстий")]
        public void Validate_WithTooManyHoles_ShouldReturnFalse()
        {
            var parameters = new BrickParameters();
            bool errorRaised = false;
            parameters.ErrorMessage += (sender, message) => errorRaised = true;

            parameters[ParameterType.HolesCount] = 10000;
            var result = parameters.Validate();

            Assert.IsFalse(result);
            Assert.IsTrue(errorRaised);
        }

        [Test]
        [Description("Validate возвращает true для нулевого количества отверстий")]
        public void Validate_WithZeroHoles_ShouldReturnTrue()
        {
            var parameters = new BrickParameters();

            parameters[ParameterType.HolesCount] = 0;
            var result = parameters.Validate();

            Assert.IsTrue(result);
        }

        [Test]
        [Description("DistributionType устанавливается и возвращается корректно")]
        public void DistributionType_SetAndGet_ShouldWork()
        {
            var parameters = new BrickParameters();

            parameters.DistributionType = HoleDistributionType.Staggered;

            Assert.AreEqual(HoleDistributionType.Staggered, parameters.DistributionType);
        }

        [Test]
        [Description("Изменение DistributionType обновляет зависимые параметры")]
        public void DistributionType_Change_ShouldUpdateDependentParameters()
        {
            var parameters = new BrickParameters();
            //TODO: RSDN
            var initialMaxHoles = parameters.GetParameter(ParameterType.HolesCount).MaxValue;

            parameters.DistributionType = HoleDistributionType.Staggered;
            var newMaxHoles = parameters.GetParameter(ParameterType.HolesCount).MaxValue;

            Assert.IsTrue(newMaxHoles > 0);
        }

        [Test]
        [Description("CalculateCurrentVoidness возвращает 0 для нуля отверстий")]
        public void CalculateCurrentVoidness_WithZeroHoles_ShouldReturnZero()
        {
            var parameters = new BrickParameters();
            parameters[ParameterType.HolesCount] = 0;

            var voidness = parameters.CalculateCurrentVoidness();

            Assert.AreEqual(0, voidness, 0.01);
        }

        [Test]
        [Description("CalculateCurrentVoidness возвращает положительное значение")]
        public void CalculateCurrentVoidness_WithHoles_ShouldReturnPositiveValue()
        {
            var parameters = new BrickParameters();
            parameters[ParameterType.HolesCount] = 10;

            var voidness = parameters.CalculateCurrentVoidness();

            Assert.IsTrue(voidness > 0);
        }

        [Test]
        [Description("CalculateCurrentVoidness увеличивается с количеством отверстий")]
        public void CalculateCurrentVoidness_MoreHoles_HigherVoidness()
        {
            var parameters = new BrickParameters();
            parameters[ParameterType.HolesCount] = 5;
            var voidness1 = parameters.CalculateCurrentVoidness();

            parameters[ParameterType.HolesCount] = 10;
            var voidness2 = parameters.CalculateCurrentVoidness();

            Assert.IsTrue(voidness2 > voidness1);
        }

        [Test]
        [Description("GetVoidnessRange возвращает корректный диапазон")]
        public void GetVoidnessRange_ShouldReturnValidRange()
        {
            var parameters = new BrickParameters();

            var (min, max) = parameters.GetVoidnessRange();

            Assert.IsTrue(min >= 0);
            Assert.IsTrue(max <= 45);
            Assert.IsTrue(min <= max);
        }

        [Test]
        [Description("CalculateOptimalParameters возвращает успешный результат")]
        public void CalculateOptimalParameters_WithValidVoidness_ShouldReturnSuccess()
        {
            var parameters = new BrickParameters();

            var result = parameters.CalculateOptimalParameters(20);

            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.HoleRadius > 0);
            Assert.IsTrue(result.HolesCount > 0);
        }

        [Test]
        //TODO: RSDN
        [Description("CalculateOptimalParameters возвращает ошибку для пустотности больше 45")]
        public void CalculateOptimalParameters_Above45_ShouldReturnError()
        {
            var parameters = new BrickParameters();

            var result = parameters.CalculateOptimalParameters(50);

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.ErrorMessage);
        }

        [Test]
        [Description("CalculateHolesCountForVoidness возвращает успешный результат")]
        public void CalculateHolesCountForVoidness_ShouldReturnSuccess()
        {
            var parameters = new BrickParameters();
            parameters[ParameterType.HoleRadius] = 8;

            var result = parameters.CalculateHolesCountForVoidness(15);

            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.HolesCount > 0);
        }

        [Test]
        //TODO: RSDN
        [Description("CalculateHolesCountForVoidness возвращает ошибку для пустотности больше 45")]
        public void CalculateHolesCountForVoidness_Above45_ShouldReturnError()
        {
            var parameters = new BrickParameters();
            parameters[ParameterType.HoleRadius] = 8;

            var result = parameters.CalculateHolesCountForVoidness(50);

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.ErrorMessage);
        }

        [Test]
        [Description("CalculateAvailableArea возвращает корректные значения")]
        public void CalculateAvailableArea_ShouldReturnCorrectValues()
        {
            var result = BrickParameters.CalculateAvailableArea(250, 120, 8);

            Assert.IsTrue(result.diameter > 0);
            Assert.IsTrue(result.edgeMargin > 0);
            Assert.IsTrue(result.minGap > 0);
            Assert.IsTrue(result.availableLength > 0);
            Assert.IsTrue(result.availableWidth > 0);
        }

        [Test]
        [Description("Validate возвращает false при превышении пустотности 45%")]
        public void Validate_VoidnessAbove45_ShouldReturnFalse()
        {
            var parameters = new BrickParameters();
            bool errorRaised = false;
            parameters.ErrorMessage += (sender, message) => errorRaised = true;

            parameters[ParameterType.HoleRadius] = 25;
            parameters[ParameterType.HolesCount] = 20;

            var result = parameters.Validate();

            Assert.IsFalse(result);
            Assert.IsTrue(errorRaised);
        }

        [Test]
        [Description("Изменение длины пересчитывает максимум отверстий")]
        public void ChangeLength_ShouldRecalculateMaxHoles()
        {
            var parameters = new BrickParameters();
            var initialMax = parameters.GetParameter(ParameterType.HolesCount).MaxValue;

            parameters[ParameterType.Length] = 500;
            var newMax = parameters.GetParameter(ParameterType.HolesCount).MaxValue;

            Assert.IsTrue(newMax > initialMax);
        }
    }
}