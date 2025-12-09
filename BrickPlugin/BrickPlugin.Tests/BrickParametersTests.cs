using NUnit.Framework;
using BrickPlugin.Models;
using BrickPlugin.Services;
using System;

namespace BrickPlugin.Tests
{
    /// <summary>
    /// Тесты для класса BrickParameters.
    /// </summary>
    [TestFixture]
    public class BrickParametersTests
    {
        /// <summary>
        /// Проверяет, что конструктор инициализирует параметры со значениями по умолчанию.
        /// </summary>
        [Test]
        public void Constructor_ShouldInitializeDefaultValues()
        {
            // Arrange & Act
            var parameters = new BrickParameters();

            // Assert
            Assert.AreEqual(250, parameters[ParameterType.Length]);
            Assert.AreEqual(120, parameters[ParameterType.Width]);
            Assert.AreEqual(65, parameters[ParameterType.Height]);
            Assert.AreEqual(8, parameters[ParameterType.HoleRadius]);
            Assert.AreEqual(19, parameters[ParameterType.HolesCount]);
        }

        /// <summary>
        /// Проверяет, что индексатор корректно возвращает значение параметра.
        /// </summary>
        [Test]
        public void Indexer_Get_ShouldReturnCorrectValue()
        {
            // Arrange
            var parameters = new BrickParameters();

            // Act
            var length = parameters[ParameterType.Length];

            // Assert
            Assert.AreEqual(250, length);
        }

        /// <summary>
        /// Проверяет, что индексатор корректно устанавливает значение параметра.
        /// </summary>
        [Test]
        public void Indexer_Set_ShouldUpdateValue()
        {
            // Arrange
            var parameters = new BrickParameters();

            // Act
            parameters[ParameterType.Length] = 300;

            // Assert
            Assert.AreEqual(300, parameters[ParameterType.Length]);
        }

        /// <summary>
        /// Проверяет, что GetParameter возвращает корректный объект BrickParameter.
        /// </summary>
        [Test]
        public void GetParameter_ShouldReturnCorrectParameter()
        {
            // Arrange
            var parameters = new BrickParameters();

            // Act
            var lengthParam = parameters.GetParameter(ParameterType.Length);

            // Assert
            Assert.IsNotNull(lengthParam);
            Assert.AreEqual(250, lengthParam.Value);
            Assert.AreEqual(100, lengthParam.MinValue);
            Assert.AreEqual(1000, lengthParam.MaxValue);
        }

        /// <summary>
        /// Проверяет, что GetMaxRadiusHint возвращает корректную строку.
        /// </summary>
        [Test]
        public void GetMaxRadiusHint_ShouldReturnCorrectString()
        {
            // Arrange
            var parameters = new BrickParameters();

            // Act
            var hint = parameters.GetMaxRadiusHint();

            // Assert
            Assert.IsNotNull(hint);
            Assert.IsTrue(hint.StartsWith("Макс:"));
            Assert.IsTrue(hint.Contains("мм"));
        }

        /// <summary>
        /// Проверяет, что GetMaxHolesHint возвращает корректную строку.
        /// </summary>
        [Test]
        public void GetMaxHolesHint_ShouldReturnCorrectString()
        {
            // Arrange
            var parameters = new BrickParameters();

            // Act
            var hint = parameters.GetMaxHolesHint();

            // Assert
            Assert.IsNotNull(hint);
            Assert.IsTrue(hint.StartsWith("Макс:"));
            Assert.IsTrue(hint.Contains("шт"));
        }

        /// <summary>
        /// Проверяет, что CalculateDependent обновляет максимальный радиус при изменении ширины.
        /// </summary>
        [Test]
        public void CalculateDependent_WhenWidthChanges_ShouldUpdateMaxRadius()
        {
            // Arrange
            var parameters = new BrickParameters();
            var initialMaxRadius = parameters.GetParameter(ParameterType.HoleRadius).MaxValue;

            // Act
            parameters[ParameterType.Width] = 200;
            var newMaxRadius = parameters.GetParameter(ParameterType.HoleRadius).MaxValue;

            // Assert
            Assert.AreNotEqual(initialMaxRadius, newMaxRadius);
        }

        /// <summary>
        /// Проверяет, что CalculateDependent вызывает событие MaxRadiusChanged.
        /// </summary>
        [Test]
        public void CalculateDependent_ShouldRaiseMaxRadiusChangedEvent()
        {
            // Arrange
            var parameters = new BrickParameters();
            bool eventRaised = false;
            parameters.MaxRadiusChanged += (sender, hint) => eventRaised = true;

            // Act
            parameters[ParameterType.Width] = 200;

            // Assert
            Assert.IsTrue(eventRaised);
        }

        /// <summary>
        /// Проверяет, что CalculateDependent вызывает событие MaxHolesChanged.
        /// </summary>
        [Test]
        public void CalculateDependent_ShouldRaiseMaxHolesChangedEvent()
        {
            // Arrange
            var parameters = new BrickParameters();
            bool eventRaised = false;
            parameters.MaxHolesChanged += (sender, hint) => eventRaised = true;

            // Act
            parameters[ParameterType.Width] = 200;

            // Assert
            Assert.IsTrue(eventRaised);
        }

        /// <summary>
        /// Проверяет, что Validate возвращает true для корректных параметров по умолчанию.
        /// </summary>
        [Test]
        public void Validate_WithDefaultValues_ShouldReturnTrue()
        {
            // Arrange
            var parameters = new BrickParameters();

            // Act
            var result = parameters.Validate();

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Проверяет, что Validate возвращает false для параметра вне диапазона.
        /// </summary>
        [Test]
        public void Validate_WithValueOutOfRange_ShouldReturnFalse()
        {
            // Arrange
            var parameters = new BrickParameters();
            bool errorMessageRaised = false;
            parameters.ErrorMessage += (sender, message) => errorMessageRaised = true;

            // Act
            parameters[ParameterType.Length] = 10000; // Превышает максимум
            var result = parameters.Validate();

            // Assert
            Assert.IsFalse(result);
            Assert.IsTrue(errorMessageRaised);
        }

        /// <summary>
        /// Проверяет, что Validate возвращает false для слишком большого радиуса.
        /// </summary>
        [Test]
        public void Validate_WithTooLargeRadius_ShouldReturnFalse()
        {
            // Arrange
            var parameters = new BrickParameters();
            parameters[ParameterType.Width] = 50; // Минимальная ширина
            bool errorMessageRaised = false;
            parameters.ErrorMessage += (sender, message) => errorMessageRaised = true;

            // Act
            parameters[ParameterType.HoleRadius] = 50; // Слишком большой радиус
            var result = parameters.Validate();

            // Assert
            Assert.IsFalse(result);
            Assert.IsTrue(errorMessageRaised);
        }

        /// <summary>
        /// Проверяет, что Validate возвращает false для слишком большого количества отверстий.
        /// </summary>
        [Test]
        public void Validate_WithTooManyHoles_ShouldReturnFalse()
        {
            // Arrange
            var parameters = new BrickParameters();
            bool errorMessageRaised = false;
            parameters.ErrorMessage += (sender, message) => errorMessageRaised = true;

            // Act
            parameters[ParameterType.HolesCount] = 10000; // Намного больше возможного
            var result = parameters.Validate();

            // Assert
            Assert.IsFalse(result);
            Assert.IsTrue(errorMessageRaised);
        }

        /// <summary>
        /// Проверяет, что CalculateAvailableArea корректно рассчитывает параметры области.
        /// </summary>
        [Test]
        public void CalculateAvailableArea_ShouldReturnCorrectValues()
        {
            // Arrange
            double length = 250;
            double width = 120;
            double holeRadius = 8;

            // Act
            var area = BrickParameters.CalculateAvailableArea(length, width, holeRadius);

            // Assert
            Assert.AreEqual(16, area.diameter);
            Assert.Greater(area.edgeMargin, 0);
            Assert.Greater(area.minGap, 0);
            Assert.Greater(area.availableLength, 0);
            Assert.Greater(area.availableWidth, 0);
        }

        /// <summary>
        /// Проверяет, что CalculateAvailableArea корректно рассчитывает диаметр.
        /// </summary>
        [Test]
        public void CalculateAvailableArea_ShouldCalculateCorrectDiameter()
        {
            // Arrange
            double holeRadius = 10;

            // Act
            var area = BrickParameters.CalculateAvailableArea(200, 100, holeRadius);

            // Assert
            Assert.AreEqual(20, area.diameter);
        }

        /// <summary>
        /// Проверяет, что CalculateAvailableArea использует минимальное значение для edgeMargin.
        /// </summary>
        [Test]
        public void CalculateAvailableArea_ShouldUseMinimumEdgeMargin()
        {
            // Arrange
            double holeRadius = 2; // Маленький радиус

            // Act
            var area = BrickParameters.CalculateAvailableArea(200, 100, holeRadius);

            // Assert
            Assert.AreEqual(10, area.edgeMargin); // Минимум EdgeMarginMinimum = 10
        }

        /// <summary>
        /// Проверяет, что CalculateAvailableArea использует вычисленное значение для edgeMargin.
        /// </summary>
        [Test]
        public void CalculateAvailableArea_ShouldUseCalculatedEdgeMargin()
        {
            // Arrange
            double holeRadius = 20; // Большой радиус

            // Act
            var area = BrickParameters.CalculateAvailableArea(200, 100, holeRadius);

            // Assert
            // 0.75 * 20 + 5 = 20 (больше минимума 10)
            Assert.AreEqual(20, area.edgeMargin);
        }

        /// <summary>
        /// Проверяет, что CalculateAvailableArea использует минимальное значение для minGap.
        /// </summary>
        [Test]
        public void CalculateAvailableArea_ShouldUseMinimumMinGap()
        {
            // Arrange
            double holeRadius = 2; // Маленький радиус

            // Act
            var area = BrickParameters.CalculateAvailableArea(200, 100, holeRadius);

            // Assert
            Assert.AreEqual(5, area.minGap); // Минимум MinGapMinimum = 5
        }

        /// <summary>
        /// Проверяет, что CalculateAvailableArea использует вычисленное значение для minGap.
        /// </summary>
        [Test]
        public void CalculateAvailableArea_ShouldUseCalculatedMinGap()
        {
            // Arrange
            double holeRadius = 10; // Средний радиус

            // Act
            var area = BrickParameters.CalculateAvailableArea(200, 100, holeRadius);

            // Assert
            // 0.75 * 10 = 7.5 (больше минимума 5)
            Assert.AreEqual(7.5, area.minGap);
        }

        /// <summary>
        /// Проверяет, что максимальное количество отверстий корректно вычисляется.
        /// </summary>
        [Test]
        public void MaxHolesCount_ShouldBeCalculatedCorrectly()
        {
            // Arrange
            var parameters = new BrickParameters();
            parameters[ParameterType.Length] = 250;
            parameters[ParameterType.Width] = 120;
            parameters[ParameterType.HoleRadius] = 8;

            // Act
            var maxHoles = parameters.GetParameter(ParameterType.HolesCount).MaxValue;

            // Assert
            Assert.Greater(maxHoles, 0);
        }

        /// <summary>
        /// Проверяет, что максимальное количество отверстий равно 1 при большом радиусе,
        /// но достаточных размерах кирпича для одного отверстия.
        /// </summary>
        [Test]
        public void MaxHolesCount_WithLargeRadius_ButFitsOne_ShouldBeOne()
        {
            // Arrange
            var parameters = new BrickParameters();
            parameters[ParameterType.Width] = 100;
            parameters[ParameterType.Length] = 250;

            // Act
            parameters[ParameterType.HoleRadius] = 30; // Большой радиус (диаметр 60)
            var maxHoles = parameters.GetParameter(ParameterType.HolesCount).MaxValue;

            // Assert
            // Диаметр = 60, длина и ширина кирпича больше 60, поэтому должно поместиться 1 отверстие
            Assert.AreEqual(1, maxHoles);
        }

        /// <summary>
        /// Проверяет, что максимальное количество отверстий равно 0 при радиусе,
        /// превышающем половину минимального размера кирпича.
        /// </summary>
        [Test]
        public void MaxHolesCount_WithRadiusLargerThanBrick_ShouldBeZero()
        {
            // Arrange
            var parameters = new BrickParameters();
            parameters[ParameterType.Width] = 50; // Минимальная ширина
            parameters[ParameterType.Length] = 100; // Минимальная длина

            // Act
            parameters[ParameterType.HoleRadius] = 30; // Радиус 30 -> диаметр 60 > ширины 50
            var maxHoles = parameters.GetParameter(ParameterType.HolesCount).MaxValue;

            // Assert
            Assert.AreEqual(0, maxHoles);
        }

        /// <summary>
        /// Проверяет, что нулевое количество отверстий всегда валидно.
        /// </summary>
        [Test]
        public void Validate_WithZeroHoles_ShouldReturnTrue()
        {
            // Arrange
            var parameters = new BrickParameters();

            // Act
            parameters[ParameterType.HolesCount] = 0;
            var result = parameters.Validate();

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Проверяет корректность валидации при минимальных значениях всех параметров.
        /// </summary>
        [Test]
        public void Validate_WithMinimumValues_ShouldWork()
        {
            // Arrange
            var parameters = new BrickParameters();

            // Act
            parameters[ParameterType.Length] = 100;
            parameters[ParameterType.Width] = 50;
            parameters[ParameterType.Height] = 30;
            parameters[ParameterType.HoleRadius] = 2;
            parameters[ParameterType.HolesCount] = 0;
            var result = parameters.Validate();

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Проверяет корректность валидации при максимальных значениях всех параметров.
        /// </summary>
        [Test]
        public void Validate_WithMaximumValues_ShouldWork()
        {
            // Arrange
            var parameters = new BrickParameters();

            // Act
            parameters[ParameterType.Length] = 1000;
            parameters[ParameterType.Width] = 500;
            parameters[ParameterType.Height] = 300;

            // Радиус и количество отверстий пересчитаются автоматически
            var maxRadius = parameters.GetParameter(ParameterType.HoleRadius).MaxValue;
            parameters[ParameterType.HoleRadius] = maxRadius;

            var maxHoles = parameters.GetParameter(ParameterType.HolesCount).MaxValue;
            parameters[ParameterType.HolesCount] = maxHoles;

            var result = parameters.Validate();

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Проверяет, что изменение длины влияет на максимальное количество отверстий.
        /// </summary>
        [Test]
        public void MaxHolesCount_ShouldChangeWhenLengthChanges()
        {
            // Arrange
            var parameters = new BrickParameters();
            var initialMaxHoles = parameters.GetParameter(ParameterType.HolesCount).MaxValue;

            // Act
            parameters[ParameterType.Length] = 500;
            var newMaxHoles = parameters.GetParameter(ParameterType.HolesCount).MaxValue;

            // Assert
            Assert.Greater(newMaxHoles, initialMaxHoles);
        }

        /// <summary>
        /// Проверяет, что изменение ширины влияет на максимальное количество отверстий.
        /// </summary>
        [Test]
        public void MaxHolesCount_ShouldChangeWhenWidthChanges()
        {
            // Arrange
            var parameters = new BrickParameters();
            var initialMaxHoles = parameters.GetParameter(ParameterType.HolesCount).MaxValue;

            // Act
            parameters[ParameterType.Width] = 300;
            var newMaxHoles = parameters.GetParameter(ParameterType.HolesCount).MaxValue;

            // Assert
            Assert.Greater(newMaxHoles, initialMaxHoles);
        }

        /// <summary>
        /// Проверяет, что изменение радиуса влияет на максимальное количество отверстий.
        /// </summary>
        [Test]
        public void MaxHolesCount_ShouldChangeWhenRadiusChanges()
        {
            // Arrange
            var parameters = new BrickParameters();
            var initialMaxHoles = parameters.GetParameter(ParameterType.HolesCount).MaxValue;

            // Act
            parameters[ParameterType.HoleRadius] = 5;
            var newMaxHoles = parameters.GetParameter(ParameterType.HolesCount).MaxValue;

            // Assert
            Assert.Greater(newMaxHoles, initialMaxHoles);
        }
    }
}