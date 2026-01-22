namespace BrickPlugin.Models
{
    /// <summary>
    /// Представляет параметр кирпича с ограничениями по минимальному и максимальному значению.
    /// </summary>
    public class BrickParameter
    {
        private double _value;
        private double _minValue;
        private double _maxValue;

        /// <summary>
        /// Инициализирует новый экземпляр класса BrickParameter.
        /// </summary>
        /// <param name="minValue">Минимальное допустимое значение.</param>
        /// <param name="maxValue">Максимальное допустимое значение.</param>
        /// <param name="defaultValue">Значение по умолчанию.</param>
        public BrickParameter(double minValue, double maxValue, double defaultValue)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            _value = defaultValue;
        }

        /// <summary>
        /// Получает или задает текущее значение параметра.
        /// </summary>
        public double Value
        {
            get => _value;
            set
            {
                if (value < _minValue || value > _maxValue)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"Значение {value} вне допустимого диапазона [{_minValue}, {_maxValue}]");
                }
                _value = value;
            }
        }

        /// <summary>
        /// Получает или задает минимальное допустимое значение параметра.
        /// </summary>
        public double MinValue
        {
            get => _minValue;
            set
            {
                if (value > _maxValue)
                {
                    throw new ArgumentException(
                        $"Минимальное значение {value} не может быть больше максимального {_maxValue}");
                }
                _minValue = value;
            }
        }

        /// <summary>
        /// Получает или задает максимальное допустимое значение параметра.
        /// </summary>
        public double MaxValue
        {
            get => _maxValue;
            set
            {
                if (value < _minValue)
                {
                    throw new ArgumentException(
                        $"Максимальное значение {value} не может быть меньше минимального {_minValue}");
                }
                _maxValue = value;
            }
        }

        /// <summary>
        /// Проверяет, находится ли текущее значение в допустимых пределах.
        /// </summary>
        /// <returns>True, если значение валидно; иначе False.</returns>
        public bool IsValid()
        {
            return _value >= _minValue && _value <= _maxValue;
        }
    }
}