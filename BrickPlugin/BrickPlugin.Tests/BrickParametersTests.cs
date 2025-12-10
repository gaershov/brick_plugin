using NUnit.Framework;
using BrickPlugin.Models;
using BrickPlugin.Services;
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
            var initialMaxRadius = parameters.GetParameter(ParameterType.HoleRadius).MaxValue;

            parameters[ParameterType.Width] = 200;
            var newMaxRadius = parameters.GetParameter(ParameterType.HoleRadius).MaxValue;

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
        //TODO: RSDN
        [Description("Validate возвращает false, если значение параметра выходит за допустимый диапазон")]
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
        [Description("Validate возвращает false при слишком большом радиусе отверстия")]
        public void Validate_WithTooLargeRadius_ShouldReturnFalse()
        {
            var parameters = new BrickParameters();
            parameters[ParameterType.Width] = 50;
            bool errorMessageRaised = false;
            parameters.ErrorMessage += (sender, message) => errorMessageRaised = true;

            parameters[ParameterType.HoleRadius] = 50;
            var result = parameters.Validate();

            Assert.IsFalse(result);
            Assert.IsTrue(errorMessageRaised);
        }

        [Test]
        [Description("Validate возвращает false при слишком большом количестве отверстий")]
        public void Validate_WithTooManyHoles_ShouldReturnFalse()
        {
            var parameters = new BrickParameters();
            bool errorMessageRaised = false;
            parameters.ErrorMessage += (sender, message) => errorMessageRaised = true;

            parameters[ParameterType.HolesCount] = 10000;
            var result = parameters.Validate();

            Assert.IsFalse(result);
            Assert.IsTrue(errorMessageRaised);
        }

        [Test]
        [Description("Нулевое количество отверстий всегда считается валидным")]
        public void Validate_WithZeroHoles_ShouldReturnTrue()
        {
            var parameters = new BrickParameters();

            parameters[ParameterType.HolesCount] = 0;
            var result = parameters.Validate();

            Assert.IsTrue(result);
        }
    }
}
