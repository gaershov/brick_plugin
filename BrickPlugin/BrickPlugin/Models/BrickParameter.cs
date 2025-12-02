namespace BrickPlugin.Models
{
    //TODO: XML
    public class BrickParameter
    {
        //TODO: XML
        private double _value;

        //TODO: XML
        private double _minValue;

        //TODO: XML
        private double _maxValue;

        //TODO: XML
        public BrickParameter(double minValue, double maxValue, double defaultValue)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            _value = defaultValue;
        }

        //TODO: validation
        public double Value
        {
            get => _value;
            set => _value = value;
        }

        //TODO: validation
        public double MinValue
        {
            get => _minValue;
            set => _minValue = value;
        }
        //TODO: validation

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