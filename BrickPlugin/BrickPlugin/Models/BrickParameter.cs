namespace BrickPlugin.Models
{
    public class BrickParameter
    {
        private double _value;
        private double _minValue;
        private double _maxValue;

        public BrickParameter(double minValue, double maxValue, double defaultValue)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            _value = defaultValue;
        }

        public double Value
        {
            get => _value;
            set => _value = value;
        }

        public double MinValue
        {
            get => _minValue;
            set => _minValue = value;
        }

        public double MaxValue
        {
            get => _maxValue;
            set => _maxValue = value;
        }

        public bool IsValid()
        {
            return _value >= _minValue && _value <= _maxValue;
        }
    }
}