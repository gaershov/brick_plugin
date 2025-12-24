using NUnit.Framework;
using BrickPluginModels.Models;

namespace BrickPlugin.Tests
{
    [TestFixture]
    [Description("Тесты для класса BrickParameter")]
    public class BrickParameterTests
    {
        [Test]
        [Description("Конструктор корректно инициализирует все поля")]
        public void Constructor_ShouldInitializeAllFields()
        {
            var parameter = new BrickParameter(10, 100, 50);

            Assert.AreEqual(10, parameter.MinValue);
            Assert.AreEqual(100, parameter.MaxValue);
            Assert.AreEqual(50, parameter.Value);
        }

        [Test]
        [Description("Value корректно устанавливается и возвращается")]
        public void Value_SetAndGet_ShouldWork()
        {
            var parameter = new BrickParameter(10, 100, 50);

            parameter.Value = 75;

            Assert.AreEqual(75, parameter.Value);
        }

        [Test]
        [Description("MinValue корректно устанавливается и возвращается")]
        public void MinValue_SetAndGet_ShouldWork()
        {
            var parameter = new BrickParameter(10, 100, 50);

            parameter.MinValue = 20;

            Assert.AreEqual(20, parameter.MinValue);
        }

        [Test]
        [Description("MaxValue корректно устанавливается и возвращается")]
        public void MaxValue_SetAndGet_ShouldWork()
        {
            var parameter = new BrickParameter(10, 100, 50);

            parameter.MaxValue = 200;

            Assert.AreEqual(200, parameter.MaxValue);
        }

        [Test]
        [Description("IsValid возвращает true для значения в диапазоне")]
        public void IsValid_WhenValueInRange_ShouldReturnTrue()
        {
            var parameter = new BrickParameter(10, 100, 50);

            Assert.IsTrue(parameter.IsValid);
        }

        [Test]
        [Description("IsValid возвращает false для значения меньше минимального")]
        public void IsValid_WhenValueBelowMin_ShouldReturnFalse()
        {
            var parameter = new BrickParameter(10, 100, 5);

            Assert.IsFalse(parameter.IsValid);
        }

        [Test]
        [Description("IsValid возвращает false для значения больше максимального")]
        public void IsValid_WhenValueAboveMax_ShouldReturnFalse()
        {
            var parameter = new BrickParameter(10, 100, 150);

            Assert.IsFalse(parameter.IsValid);
        }

        [Test]
        [Description("IsValid возвращает true для значения равного минимальному")]
        public void IsValid_WhenValueEqualsMin_ShouldReturnTrue()
        {
            var parameter = new BrickParameter(10, 100, 10);

            Assert.IsTrue(parameter.IsValid);
        }

        [Test]
        [Description("IsValid возвращает true для значения равного максимальному")]
        public void IsValid_WhenValueEqualsMax_ShouldReturnTrue()
        {
            var parameter = new BrickParameter(10, 100, 100);

            Assert.IsTrue(parameter.IsValid);
        }

        [Test]
        [Description("IsValid обновляется при изменении Value")]
        public void IsValid_WhenValueChanges_ShouldUpdate()
        {
            var parameter = new BrickParameter(10, 100, 50);
            Assert.IsTrue(parameter.IsValid);

            parameter.Value = 5;
            Assert.IsFalse(parameter.IsValid);

            parameter.Value = 75;
            Assert.IsTrue(parameter.IsValid);
        }

        [Test]
        [Description("IsValid обновляется при изменении MinValue")]
        public void IsValid_WhenMinValueChanges_ShouldUpdate()
        {
            var parameter = new BrickParameter(10, 100, 15);
            Assert.IsTrue(parameter.IsValid);

            parameter.MinValue = 20;
            Assert.IsFalse(parameter.IsValid);
        }

        [Test]
        [Description("IsValid обновляется при изменении MaxValue")]
        public void IsValid_WhenMaxValueChanges_ShouldUpdate()
        {
            var parameter = new BrickParameter(10, 100, 90);
            Assert.IsTrue(parameter.IsValid);

            parameter.MaxValue = 50;
            Assert.IsFalse(parameter.IsValid);
        }
    }
}