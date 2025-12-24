using NUnit.Framework;
using BrickPluginModels.Services;

namespace BrickPlugin.Tests
{
    [TestFixture]
    [Description("Тесты для класса VoidnessCalculationResult")]
    public class VoidnessCalculationResultTests
    {
        [Test]
        [Description("Конструктор создаёт объект со значениями по умолчанию")]
        public void Constructor_ShouldCreateDefaultObject()
        {
            var result = new VoidnessCalculationResult();

            Assert.AreEqual(0, result.HoleRadius);
            Assert.AreEqual(0, result.HolesCount);
            Assert.AreEqual(0, result.ActualVoidness);
            Assert.IsFalse(result.Success);
            Assert.IsNull(result.ErrorMessage);
        }

        [Test]
        [Description("HoleRadius устанавливается и возвращается")]
        public void HoleRadius_SetAndGet_ShouldWork()
        {
            var result = new VoidnessCalculationResult { HoleRadius = 8.5 };

            Assert.AreEqual(8.5, result.HoleRadius, 0.01);
        }

        [Test]
        [Description("HolesCount устанавливается и возвращается")]
        public void HolesCount_SetAndGet_ShouldWork()
        {
            var result = new VoidnessCalculationResult { HolesCount = 15 };

            Assert.AreEqual(15, result.HolesCount);
        }

        [Test]
        [Description("ActualVoidness устанавливается и возвращается")]
        public void ActualVoidness_SetAndGet_ShouldWork()
        {
            var result = new VoidnessCalculationResult { ActualVoidness = 25.67 };

            Assert.AreEqual(25.67, result.ActualVoidness, 0.01);
        }

        [Test]
        [Description("Success устанавливается и возвращается")]
        public void Success_SetAndGet_ShouldWork()
        {
            var result = new VoidnessCalculationResult { Success = true };

            Assert.IsTrue(result.Success);
        }

        [Test]
        [Description("ErrorMessage устанавливается и возвращается")]
        public void ErrorMessage_SetAndGet_ShouldWork()
        {
            var message = "Ошибка";
            var result = new VoidnessCalculationResult { ErrorMessage = message };

            Assert.AreEqual(message, result.ErrorMessage);
        }

        [Test]
        [Description("Успешный результат содержит корректные данные")]
        public void SuccessfulResult_ShouldContainValidData()
        {
            var result = new VoidnessCalculationResult
            {
                HoleRadius = 8.0,
                HolesCount = 12,
                ActualVoidness = 22.5,
                Success = true
            };

            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.HoleRadius > 0);
            Assert.IsTrue(result.HolesCount > 0);
            Assert.IsNull(result.ErrorMessage);
        }

        [Test]
        [Description("Неуспешный результат содержит сообщение об ошибке")]
        public void FailedResult_ShouldContainErrorMessage()
        {
            var result = new VoidnessCalculationResult
            {
                Success = false,
                ErrorMessage = "Ошибка расчета"
            };

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.ErrorMessage);
        }

        [Test]
        [Description("Можно создать результат с нулевыми параметрами")]
        public void ZeroParameters_ShouldWork()
        {
            var result = new VoidnessCalculationResult
            {
                HoleRadius = 0,
                HolesCount = 0,
                ActualVoidness = 0,
                Success = false,
                ErrorMessage = "Невозможно рассчитать"
            };

            Assert.AreEqual(0, result.HoleRadius);
            Assert.AreEqual(0, result.HolesCount);
            Assert.IsFalse(result.Success);
        }

        [Test]
        [Description("Можно изменять свойства после создания")]
        public void Properties_CanBeModified()
        {
            var result = new VoidnessCalculationResult { Success = false };

            result.Success = true;
            result.HoleRadius = 10.0;
            result.HolesCount = 20;

            Assert.IsTrue(result.Success);
            Assert.AreEqual(10.0, result.HoleRadius);
            Assert.AreEqual(20, result.HolesCount);
        }
    }
}