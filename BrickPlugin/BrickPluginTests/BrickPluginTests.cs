using BrickPlugin.Models;
using BrickPlugin.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BrickPlugin.Tests
{
    [TestClass]
    public class BrickParameterTests
    {
        //TODO: description
        [TestMethod]
        public void Constructor_SetsValuesCorrectly()
        {
            var param = new BrickParameter(10, 100, 50);
            Assert.AreEqual(10, param.MinValue);
            Assert.AreEqual(100, param.MaxValue);
            Assert.AreEqual(50, param.Value);
        }

        //TODO: description
        [TestMethod]
        public void IsValid_ReturnsTrueForValueInRange()
        {
            var param = new BrickParameter(10, 100, 50);
            Assert.IsTrue(param.IsValid());
        }

        //TODO: description
        [TestMethod]
        public void IsValid_ReturnsFalseForValueBelowMin()
        {
            var param = new BrickParameter(10, 100, 5);
            Assert.IsFalse(param.IsValid());
        }

        [TestMethod]
        public void IsValid_ReturnsFalseForValueAboveMax()
        {
            var param = new BrickParameter(10, 100, 150);
            Assert.IsFalse(param.IsValid());
        }

        //TODO: description
        [TestMethod]
        public void IsValid_ReturnsTrueForValueEqualToMin()
        {
            var param = new BrickParameter(10, 100, 10);
            Assert.IsTrue(param.IsValid());
        }

        //TODO: description
        [TestMethod]
        public void IsValid_ReturnsTrueForValueEqualToMax()
        {
            var param = new BrickParameter(10, 100, 100);
            Assert.IsTrue(param.IsValid());
        }

        [TestMethod]
        public void Value_CanBeChanged()
        {
            var param = new BrickParameter(10, 100, 50);
            param.Value = 75;
            Assert.AreEqual(75, param.Value);
        }

        //TODO: description
        [TestMethod]
        public void MaxValue_CanBeChanged()
        {
            var param = new BrickParameter(10, 100, 50);
            param.MaxValue = 200;
            Assert.AreEqual(200, param.MaxValue);
        }
    }

    //TODO: RSDN
    [TestClass]
    public class BrickParametersTests
    {
        private BrickParameters _parameters;

        [TestInitialize]
        public void SetUp() => _parameters = new BrickParameters();

        [TestMethod]
        public void Constructor_SetsDefaultValues()
        {
            Assert.AreEqual(250, _parameters[ParameterType.Length]);
            Assert.AreEqual(120, _parameters[ParameterType.Width]);
            Assert.AreEqual(65, _parameters[ParameterType.Height]);
            Assert.AreEqual(8, _parameters[ParameterType.HoleRadius]);
            Assert.AreEqual(19, _parameters[ParameterType.HolesCount]);
        }

        [TestMethod]
        public void Constructor_CalculatesDependentParameters()
        {
            Assert.AreEqual(30, _parameters.GetParameter(ParameterType.HoleRadius).MaxValue);
        }

        [TestMethod]
        public void Indexer_GetReturnsCorrectValue() => Assert.AreEqual(250, _parameters[ParameterType.Length]);

        [TestMethod]
        public void Indexer_SetChangesValue()
        {
            _parameters[ParameterType.Length] = 500;
            Assert.AreEqual(500, _parameters[ParameterType.Length]);
        }

        [TestMethod]
        public void Indexer_SetRecalculatesDependentParameters()
        {
            _parameters[ParameterType.Width] = 200;
            Assert.AreEqual(50, _parameters.GetParameter(ParameterType.HoleRadius).MaxValue);
        }

        [TestMethod]
        public void CalculateDependent_UpdatesMaxRadius()
        {
            _parameters[ParameterType.Width] = 400;
            _parameters.CalculateDependent();
            Assert.AreEqual(100, _parameters.GetParameter(ParameterType.HoleRadius).MaxValue);
        }

        [TestMethod]
        public void CalculateDependent_UpdatesMaxHolesCount()
        {
            _parameters[ParameterType.Length] = 200;
            _parameters[ParameterType.Width] = 100;
            _parameters[ParameterType.HoleRadius] = 5;
            _parameters.CalculateDependent();
            Assert.AreEqual(50, _parameters.GetParameter(ParameterType.HolesCount).MaxValue);
        }

        [TestMethod]
        public void CalculateDependent_SetsMaxHolesToZeroWhenRadiusTooSmall()
        {
            _parameters[ParameterType.HoleRadius] = 1;
            _parameters.CalculateDependent();
            Assert.AreEqual(0, _parameters.GetParameter(ParameterType.HolesCount).MaxValue);
        }

        [TestMethod]
        public void CalculateDependent_RaisesMaxHolesChangedEvent()
        {
            bool eventRaised = false;
            _parameters.MaxHolesChanged += (s, h) => { eventRaised = true; };
            _parameters[ParameterType.Length] = 300;
            Assert.IsTrue(eventRaised);
        }

        [TestMethod]
        public void Validate_ReturnsTrueForValidParameters()
        {
            _parameters[ParameterType.Length] = 250;
            _parameters[ParameterType.Width] = 120;
            _parameters[ParameterType.Height] = 65;
            _parameters[ParameterType.HoleRadius] = 8;
            _parameters[ParameterType.HolesCount] = 10;
            Assert.IsTrue(_parameters.Validate());
        }

        [TestMethod]
        public void Validate_ReturnsFalseWhenLengthTooSmall()
        {
            _parameters[ParameterType.Length] = 50;
            Assert.IsFalse(_parameters.Validate());
        }

        [TestMethod]
        public void Validate_ReturnsFalseWhenLengthTooLarge()
        {
            _parameters[ParameterType.Length] = 1500;
            Assert.IsFalse(_parameters.Validate());
        }

        [TestMethod]
        public void Validate_ReturnsFalseWhenRadiusExceeds25PercentOfWidth()
        {
            _parameters[ParameterType.Width] = 100;
            _parameters[ParameterType.HoleRadius] = 30;
            Assert.IsFalse(_parameters.Validate());
        }

        [TestMethod]
        public void Validate_ReturnsFalseWhenHolesCountExceedsMaximum()
        {
            _parameters[ParameterType.Length] = 100;
            _parameters[ParameterType.Width] = 50;
            _parameters[ParameterType.HoleRadius] = 10;
            _parameters.CalculateDependent();
            _parameters[ParameterType.HolesCount] = _parameters.GetParameter(ParameterType.HolesCount).MaxValue + 10;
            Assert.IsFalse(_parameters.Validate());
        }

        [TestMethod]
        public void Validate_RaisesErrorMessageEventOnFailure()
        {
            bool eventRaised = false;
            string errorMessage = null;
            _parameters.ErrorMessage += (s, m) => { eventRaised = true; errorMessage = m; };
            _parameters[ParameterType.Length] = 50;
            _parameters.Validate();
            Assert.IsTrue(eventRaised);
            Assert.IsNotNull(errorMessage);
            Assert.IsTrue(errorMessage.Contains("Длина"));
        }

        [TestMethod]
        public void GetMaxRadiusHint_ReturnsCorrectFormat()
        {
            string temp1 = _parameters.GetMaxRadiusHint();
            _parameters[ParameterType.Width] = 200;

            Assert.IsFalse(temp1 == _parameters.GetMaxRadiusHint());
        }

        [TestMethod]
        public void GetMaxHolesHint_ReturnsCorrectFormat()
        {
            _parameters[ParameterType.Length] = 200;
            _parameters[ParameterType.Width] = 100;
            _parameters[ParameterType.HoleRadius] = 5;
            _parameters.CalculateDependent();
            Assert.AreEqual("Макс: 50 шт", _parameters.GetMaxHolesHint());
        }

        [TestMethod]
        public void CalculateMaxHoles_ReturnsZeroWhenSpaceInsufficient()
        {
            _parameters[ParameterType.Length] = 100;
            _parameters[ParameterType.Width] = 50;
            _parameters[ParameterType.HoleRadius] = 20;
            _parameters.CalculateDependent();
            Assert.AreEqual(0, _parameters.GetParameter(ParameterType.HolesCount).MaxValue);
        }

        [TestMethod]
        public void Validate_AllowsZeroHoles()
        {
            _parameters[ParameterType.HolesCount] = 0;
            Assert.IsTrue(_parameters.Validate());
        }

        [TestMethod]
        public void Validate_AllowsMinimumRadius()
        {
            _parameters[ParameterType.HoleRadius] = 2;
            _parameters[ParameterType.HolesCount] = 5;
            Assert.IsTrue(_parameters.Validate());
        }
    }
}
