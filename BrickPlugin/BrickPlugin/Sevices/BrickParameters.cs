using System;
using System.Collections.Generic;
using BrickPlugin.Models;

namespace BrickPlugin.Services
{
    /// <summary>
    /// Управляет параметрами кирпича и их валидацией.
    /// </summary>
    public class BrickParameters
    {
        private const double MinimumEdgeMargin = 15.0;
        private const double EdgeMarginMultiplier = 0.75;
        private const double EdgeMarginOffset = 5.0;
        private const double EdgeMarginMinimum = 10.0;
        private const double MinGapMultiplier = 0.75;
        private const double MinGapMinimum = 5.0;

        private readonly Dictionary<ParameterType, BrickParameter> _parameters;

        /// <summary>
        /// Событие, возникающее при появлении ошибки валидации.
        /// </summary>
        public event EventHandler<string> ErrorMessage;

        /// <summary>
        /// Событие, возникающее при изменении максимального радиуса.
        /// </summary>
        public event EventHandler<string> MaxRadiusChanged;

        /// <summary>
        /// Событие, возникающее при изменении максимального количества отверстий.
        /// </summary>
        public event EventHandler<string> MaxHolesChanged;

        /// <summary>
        /// Инициализирует новый экземпляр класса BrickParameters с параметрами по умолчанию.
        /// </summary>
        public BrickParameters()
        {
            _parameters = new Dictionary<ParameterType, BrickParameter>
            {
                { ParameterType.Length, new BrickParameter(100, 1000, 250) },
                { ParameterType.Width, new BrickParameter(50, 500, 120) },
                { ParameterType.Height, new BrickParameter(30, 300, 65) },
                { ParameterType.HoleRadius, new BrickParameter(2, 30, 8) },
                { ParameterType.HolesCount, new BrickParameter(0, 5777, 19) }
            };

            CalculateDependent();
        }

        /// <summary>
        /// Получает или задает значение параметра по типу.
        /// </summary>
        /// <param name="name">Тип параметра.</param>
        /// <returns>Значение параметра.</returns>
        public double this[ParameterType name]
        {
            get => _parameters[name].Value;
            set
            {
                _parameters[name].Value = value;
                CalculateDependent();
            }
        }

        /// <summary>
        /// Получает параметр по типу.
        /// </summary>
        /// <param name="name">Тип параметра.</param>
        /// <returns>Объект параметра.</returns>
        public BrickParameter GetParameter(ParameterType name)
        {
            return _parameters[name];
        }

        /// <summary>
        /// Получает текстовую подсказку о максимальном радиусе отверстий.
        /// </summary>
        /// <returns>Строка с максимальным радиусом.</returns>
        public string GetMaxRadiusHint()
        {
            double maximumRadius = _parameters[ParameterType.HoleRadius].MaxValue;
            return $"Макс: {maximumRadius:F1} мм";
        }

        /// <summary>
        /// Получает текстовую подсказку о максимальном количестве отверстий.
        /// </summary>
        /// <returns>Строка с максимальным количеством.</returns>
        public string GetMaxHolesHint()
        {
            double maximumHoles = _parameters[ParameterType.HolesCount].MaxValue;
            return $"Макс: {(int)maximumHoles} шт";
        }

        /// <summary>
        /// Пересчитывает зависимые параметры (максимальный радиус и количество отверстий).
        /// </summary>
        public void CalculateDependent()
        {
            double length = _parameters[ParameterType.Length].Value;
            double width = _parameters[ParameterType.Width].Value;
            double holeRadius = _parameters[ParameterType.HoleRadius].Value;

            double maximumRadius = (width - 2 * MinimumEdgeMargin) / 2.0;
            _parameters[ParameterType.HoleRadius].MaxValue = Math.Max(2, maximumRadius);

            MaxRadiusChanged?.Invoke(this, GetMaxRadiusHint());

            if (holeRadius >= 2)
            {
                double maximumHoles = CalculateMaxHoles(length, width, holeRadius);
                _parameters[ParameterType.HolesCount].MaxValue = Math.Max(0, maximumHoles);
            }
            else
            {
                _parameters[ParameterType.HolesCount].MaxValue = 0;
            }

            MaxHolesChanged?.Invoke(this, GetMaxHolesHint());
        }

        /// <summary>
        /// Выполняет полную валидацию всех параметров.
        /// </summary>
        /// <returns>True, если все параметры валидны; иначе False.</returns>
        public bool Validate()
        {
            List<string> errorMessages = new List<string>();

            foreach (KeyValuePair<ParameterType, BrickParameter> kvp in _parameters)
            {
                if (!kvp.Value.IsValid)
                {
                    string displayName = GetParameterDisplayName(kvp.Key);
                    string minValue = kvp.Value.MinValue.ToString("F0");
                    string maxValue = kvp.Value.MaxValue.ToString("F0");
                    errorMessages.Add($"• {displayName}: должно быть в диапазоне [{minValue}, {maxValue}]");
                }
            }

            if (!ValidateBusinessRules(out string businessError))
            {
                if (!string.IsNullOrEmpty(businessError))
                {
                    errorMessages.Add(businessError);
                }
            }

            if (errorMessages.Count > 0)
            {
                string message = "Обнаружены ошибки:\n\n" + string.Join("\n", errorMessages);
                ErrorMessage?.Invoke(this, message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Рассчитывает параметры доступной области для размещения отверстий.
        /// </summary>
        /// <param name="length">Длина кирпича.</param>
        /// <param name="width">Ширина кирпича.</param>
        /// <param name="holeRadius">Радиус отверстия.</param>
        /// <returns>Кортеж с diameter, edgeMargin, minGap, availableLength, availableWidth.</returns>
        public static (double diameter, double edgeMargin, double minGap, double availableLength,
            double availableWidth) CalculateAvailableArea(double length, double width, double holeRadius)
        {
            double diameter = 2 * holeRadius;
            double edgeMargin = Math.Max(
                EdgeMarginMultiplier * holeRadius + EdgeMarginOffset,
                EdgeMarginMinimum);
            double minGap = Math.Max(MinGapMultiplier * holeRadius, MinGapMinimum);
            double availableLength = length - 2 * edgeMargin;
            double availableWidth = width - 2 * edgeMargin;

            return (diameter, edgeMargin, minGap, availableLength, availableWidth);
        }

        /// <summary>
        /// Проверяет бизнес-правила валидации параметров.
        /// </summary>
        /// <param name="errorMessage">Сообщение об ошибке, если валидация не прошла.</param>
        /// <returns>True, если все правила соблюдены; иначе False.</returns>
        private bool ValidateBusinessRules(out string errorMessage)
        {
            errorMessage = null;
            double width = _parameters[ParameterType.Width].Value;
            double length = _parameters[ParameterType.Length].Value;
            double holeRadius = _parameters[ParameterType.HoleRadius].Value;
            int holesCount = (int)_parameters[ParameterType.HolesCount].Value;

            double maximumRadius = (width - 2 * MinimumEdgeMargin) / 2.0;
            if (holeRadius > maximumRadius)
            {
                errorMessage = "• Радиус отверстий слишком большой для данной ширины кирпича";
                return false;
            }

            if (holesCount > 0 && holeRadius >= 2)
            {
                double maximumHoles = CalculateMaxHoles(length, width, holeRadius);
                if (holesCount > maximumHoles)
                {
                    errorMessage = "• Количество отверстий превышает возможное";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Рассчитывает максимально возможное количество отверстий для заданных параметров.
        /// </summary>
        /// <param name="length">Длина кирпича.</param>
        /// <param name="width">Ширина кирпича.</param>
        /// <param name="holeRadius">Радиус отверстия.</param>
        /// <returns>Максимальное количество отверстий.</returns>
        private double CalculateMaxHoles(double length, double width, double holeRadius)
        {
            var area = CalculateAvailableArea(length, width, holeRadius);

            if (area.availableLength < 0 || area.availableWidth < 0)
            {
                return 0;
            }

            if (area.availableLength < area.diameter || area.availableWidth < area.diameter)
            {
                if (length >= area.diameter && width >= area.diameter)
                {
                    return 1;
                }
                return 0;
            }

            int maxHorizontal = (int)Math.Floor(
                (area.availableLength + area.minGap) / (area.diameter + area.minGap));
            int maxVertical = (int)Math.Floor(
                (area.availableWidth + area.minGap) / (area.diameter + area.minGap));

            maxHorizontal = Math.Max(1, maxHorizontal);
            maxVertical = Math.Max(1, maxVertical);

            return maxHorizontal * maxVertical;
        }

        /// <summary>
        /// Получает отображаемое имя параметра для пользовательского интерфейса.
        /// </summary>
        /// <param name="parameter">Тип параметра.</param>
        /// <returns>Локализованное имя параметра.</returns>
        private string GetParameterDisplayName(ParameterType parameter)
        {
            return parameter switch
            {
                ParameterType.Length => "Длина",
                ParameterType.Width => "Ширина",
                ParameterType.Height => "Высота",
                ParameterType.HoleRadius => "Радиус отверстий",
                ParameterType.HolesCount => "Количество отверстий",
                _ => parameter.ToString()
            };
        }
    }
}