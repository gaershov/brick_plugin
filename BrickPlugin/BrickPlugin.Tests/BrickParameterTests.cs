using NUnit.Framework;
using BrickPlugin.Models;

namespace BrickPlugin.Tests
{
    /// <summary>
    /// Тесты для класса BrickParameter.
    /// </summary>
    [TestFixture]
    public class BrickParameterTests
    {
        /// <summary>
        /// Проверяет, что конструктор корректно инициализирует все поля.
        /// </summary>
        [Test]
        public void Constructor_ShouldInitializeAllFields()
        {
            // Arrange & Act
            var parameter = new BrickParameter(10, 100, 50);

            // Assert
            Assert.AreEqual(10, parameter.MinValue);
            Assert.AreEqual(100, parameter.MaxValue);
            Assert.AreEqual(50, parameter.Value);
        }

        /// <summary>
        /// Проверяет, что Value корректно возвращает установленное значение.
        /// </summary>
        [Test]
        public void Value_Get_ShouldReturnSetValue()
        {
            // Arrange
            var parameter = new BrickParameter(10, 100, 50);

            // Act
            var value = parameter.Value;

            // Assert
            Assert.AreEqual(50, value);
        }

        /// <summary>
        /// Проверяет, что Value корректно устанавливает новое значение.
        /// </summary>
        [Test]
        public void Value_Set_ShouldUpdateValue()
        {
            // Arrange
            var parameter = new BrickParameter(10, 100, 50);

            // Act
            parameter.Value = 75;

            // Assert
            Assert.AreEqual(75, parameter.Value);
        }

        /// <summary>
        /// Проверяет, что MinValue корректно устанавливается и возвращается.
        /// </summary>
        [Test]
        public void MinValue_SetAndGet_ShouldWork()
        {
            // Arrange
            var parameter = new BrickParameter(10, 100, 50);

            // Act
            parameter.MinValue = 20;

            // Assert
            Assert.AreEqual(20, parameter.MinValue);
        }

        /// <summary>
        /// Проверяет, что MaxValue корректно устанавливается и возвращается.
        /// </summary>
        [Test]
        public void MaxValue_SetAndGet_ShouldWork()
        {
            // Arrange
            var parameter = new BrickParameter(10, 100, 50);

            // Act
            parameter.MaxValue = 200;

            // Assert
            Assert.AreEqual(200, parameter.MaxValue);
        }

        /// <summary>
        /// Проверяет, что IsValid возвращает true для значения в допустимом диапазоне.
        /// </summary>
        [Test]
        public void IsValid_WhenValueInRange_ShouldReturnTrue()
        {
            // Arrange
            var parameter = new BrickParameter(10, 100, 50);

            // Act & Assert
            Assert.IsTrue(parameter.IsValid);
        }

        /// <summary>
        /// Проверяет, что IsValid возвращает false для значения меньше минимального.
        /// </summary>
        [Test]
        public void IsValid_WhenValueBelowMin_ShouldReturnFalse()
        {
            // Arrange
            var parameter = new BrickParameter(10, 100, 5);

            // Act & Assert
            Assert.IsFalse(parameter.IsValid);
        }

        /// <summary>
        /// Проверяет, что IsValid возвращает false для значения больше максимального.
        /// </summary>
        [Test]
        public void IsValid_WhenValueAboveMax_ShouldReturnFalse()
        {
            // Arrange
            var parameter = new BrickParameter(10, 100, 150);

            // Act & Assert
            Assert.IsFalse(parameter.IsValid);
        }

        /// <summary>
        /// Проверяет, что IsValid возвращает true для значения, равного минимальному.
        /// </summary>
        [Test]
        public void IsValid_WhenValueEqualsMin_ShouldReturnTrue()
        {
            // Arrange
            var parameter = new BrickParameter(10, 100, 10);

            // Act & Assert
            Assert.IsTrue(parameter.IsValid);
        }

        /// <summary>
        /// Проверяет, что IsValid возвращает true для значения, равного максимальному.
        /// </summary>
        [Test]
        public void IsValid_WhenValueEqualsMax_ShouldReturnTrue()
        {
            // Arrange
            var parameter = new BrickParameter(10, 100, 100);

            // Act & Assert
            Assert.IsTrue(parameter.IsValid);
        }

        /// <summary>
        /// Проверяет, что IsValid корректно обновляется при изменении Value.
        /// </summary>
        [Test]
        public void IsValid_WhenValueChanges_ShouldUpdateCorrectly()
        {
            // Arrange
            var parameter = new BrickParameter(10, 100, 50);

            // Act & Assert
            Assert.IsTrue(parameter.IsValid);

            parameter.Value = 5;
            Assert.IsFalse(parameter.IsValid);

            parameter.Value = 75;
            Assert.IsTrue(parameter.IsValid);

            parameter.Value = 150;
            Assert.IsFalse(parameter.IsValid);
        }

        /// <summary>
        /// Проверяет, что IsValid корректно обновляется при изменении MinValue.
        /// </summary>
        [Test]
        public void IsValid_WhenMinValueChanges_ShouldUpdateCorrectly()
        {
            // Arrange
            var parameter = new BrickParameter(10, 100, 15);

            // Act & Assert
            Assert.IsTrue(parameter.IsValid);

            parameter.MinValue = 20;
            Assert.IsFalse(parameter.IsValid);
        }

        /// <summary>
        /// Проверяет, что IsValid корректно обновляется при изменении MaxValue.
        /// </summary>
        [Test]
        public void IsValid_WhenMaxValueChanges_ShouldUpdateCorrectly()
        {
            // Arrange
            var parameter = new BrickParameter(10, 100, 90);

            // Act & Assert
            Assert.IsTrue(parameter.IsValid);

            parameter.MaxValue = 50;
            Assert.IsFalse(parameter.IsValid);
        }
    }
}