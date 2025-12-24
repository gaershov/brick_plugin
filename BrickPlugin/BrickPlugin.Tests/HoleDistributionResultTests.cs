using NUnit.Framework;
using BrickPluginModels.Models;
using System.Collections.Generic;

namespace BrickPlugin.Tests
{
    [TestFixture]
    [Description("Тесты для класса HoleDistributionResult")]
    public class HoleDistributionResultTests
    {
        [Test]
        [Description("Конструктор инициализирует пустой список Distribution")]
        public void Constructor_ShouldInitializeEmptyDistribution()
        {
            var result = new HoleDistributionResult();

            Assert.IsNotNull(result.Distribution);
            Assert.AreEqual(0, result.Distribution.Count);
        }

        [Test]
        [Description("Конструктор устанавливает значения по умолчанию")]
        public void Constructor_ShouldSetDefaultValues()
        {
            var result = new HoleDistributionResult();

            Assert.AreEqual(0, result.Rows);
            Assert.AreEqual(0, result.MaxHolesInRow);
            Assert.AreEqual(0, result.HorizontalGap);
            Assert.AreEqual(0, result.VerticalGap);
            Assert.AreEqual(0, result.StaggerOffset);
        }

        [Test]
        [Description("Rows устанавливается и возвращается")]
        public void Rows_SetAndGet_ShouldWork()
        {
            var result = new HoleDistributionResult { Rows = 5 };

            Assert.AreEqual(5, result.Rows);
        }

        [Test]
        [Description("Distribution устанавливается и возвращается")]
        public void Distribution_SetAndGet_ShouldWork()
        {
            var distribution = new List<int> { 5, 6, 5 };
            var result = new HoleDistributionResult { Distribution = distribution };

            Assert.AreEqual(distribution, result.Distribution);
        }

        [Test]
        [Description("MaxHolesInRow устанавливается и возвращается")]
        public void MaxHolesInRow_SetAndGet_ShouldWork()
        {
            var result = new HoleDistributionResult { MaxHolesInRow = 10 };

            Assert.AreEqual(10, result.MaxHolesInRow);
        }

        [Test]
        [Description("HorizontalGap устанавливается и возвращается")]
        public void HorizontalGap_SetAndGet_ShouldWork()
        {
            var result = new HoleDistributionResult { HorizontalGap = 12.5 };

            Assert.AreEqual(12.5, result.HorizontalGap, 0.01);
        }

        [Test]
        [Description("VerticalGap устанавливается и возвращается")]
        public void VerticalGap_SetAndGet_ShouldWork()
        {
            var result = new HoleDistributionResult { VerticalGap = 15.3 };

            Assert.AreEqual(15.3, result.VerticalGap, 0.01);
        }

        [Test]
        [Description("StaggerOffset устанавливается и возвращается")]
        public void StaggerOffset_SetAndGet_ShouldWork()
        {
            var result = new HoleDistributionResult { StaggerOffset = 8.5 };

            Assert.AreEqual(8.5, result.StaggerOffset, 0.01);
        }

        [Test]
        [Description("Можно создать полностью заполненный объект")]
        public void FullObject_ShouldWork()
        {
            var result = new HoleDistributionResult
            {
                Rows = 4,
                Distribution = new List<int> { 5, 6, 5, 6 },
                MaxHolesInRow = 6,
                HorizontalGap = 10.0,
                VerticalGap = 12.0,
                StaggerOffset = 7.5
            };

            Assert.AreEqual(4, result.Rows);
            Assert.AreEqual(4, result.Distribution.Count);
            Assert.AreEqual(6, result.MaxHolesInRow);
        }

        [Test]
        [Description("Distribution можно изменять после создания")]
        public void Distribution_CanBeModified()
        {
            var result = new HoleDistributionResult();
            result.Distribution.Add(5);
            result.Distribution.Add(6);

            Assert.AreEqual(2, result.Distribution.Count);
        }
    }
}