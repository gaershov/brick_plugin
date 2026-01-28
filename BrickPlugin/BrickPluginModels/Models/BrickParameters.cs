using BrickPluginModels.Services;

namespace BrickPluginModels.Models
{
    /// <summary>
    /// Управляет параметрами кирпича и их валидацией.
    /// </summary>
    public class BrickParameters
    {
        /// <summary>
        /// Минимальный отступ от края кирпича до отверстия, используемый
        /// при расчёте максимального допустимого радиуса.
        /// </summary>
        private const double MinimumEdgeMargin = 15.0;

        /// <summary>
        /// Множитель радиуса отверстия для вычисления отступа от края.
        /// </summary>
        private const double EdgeMarginMultiplier = 0.75;

        /// <summary>
        /// Дополнительное смещение, добавляемое к отступу от края.
        /// </summary>
        private const double EdgeMarginOffset = 5.0;

        /// <summary>
        /// Минимально допустимый отступ от края кирпича до отверстия.
        /// </summary>
        private const double EdgeMarginMinimum = 10.0;

        /// <summary>
        /// Множитель радиуса отверстия для расчёта минимального зазора между отверстиями.
        /// </summary>
        private const double MinGapMultiplier = 0.75;

        /// <summary>
        /// Минимально допустимый зазор между отверстиями.
        /// </summary>
        private const double MinGapMinimum = 5.0;

        /// <summary>
        /// Коллекция параметров кирпича, индексируемая по типу параметра.
        /// </summary>
        private readonly Dictionary<ParameterType, BrickParameter> _parameters;

        /// <summary>
        /// Калькулятор распределения отверстий.
        /// </summary>
        private readonly HoleDistributionCalculator _distributionCalculator;

        /// <summary>
        /// Калькулятор пустотности кирпича.
        /// </summary>
        private readonly VoidnessCalculator _voidnessCalculator;

        /// <summary>
        /// Текущий тип распределения отверстий.
        /// </summary>
        private HoleDistributionType _distributionType;

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

            _distributionCalculator = new HoleDistributionCalculator();
            _voidnessCalculator = new VoidnessCalculator();
            _distributionType = HoleDistributionType.Straight;

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
        /// Получает или задает тип распределения отверстий.
        /// </summary>
        public HoleDistributionType DistributionType
        {
            get => _distributionType;
            set
            {
                _distributionType = value;
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
            string distributionInfo = _distributionType == HoleDistributionType.Straight
                ? "прямое"
                : "шахматное";
            return $"Макс: {(int)maximumHoles} шт ({distributionInfo})";
        }

        /// <summary>
        /// Рассчитывает текущую пустотность кирпича в процентах.
        /// </summary>
        /// <returns>Пустотность в процентах.</returns>
        public double CalculateCurrentVoidness()
        {
            double length = _parameters[ParameterType.Length].Value;
            double width = _parameters[ParameterType.Width].Value;
            double height = _parameters[ParameterType.Height].Value;
            double holeRadius = _parameters[ParameterType.HoleRadius].Value;
            int holesCount = (int)_parameters[ParameterType.HolesCount].Value;

            return _voidnessCalculator.CalculateCurrentVoidness(
                length, width, height, holeRadius, holesCount);
        }

        /// <summary>
        /// Рассчитывает оптимальные параметры отверстий для достижения заданной пустотности.
        /// </summary>
        /// <param name="targetVoidness">Желаемая пустотность в процентах.</param>
        /// <returns>Результат расчёта с оптимальными параметрами.</returns>
        public VoidnessCalculationResult CalculateOptimalParameters(double targetVoidness)
        {
            double length = _parameters[ParameterType.Length].Value;
            double width = _parameters[ParameterType.Width].Value;
            double height = _parameters[ParameterType.Height].Value;

            return _voidnessCalculator.CalculateOptimalParameters(
                length, width, height, targetVoidness, _distributionType);
        }

        /// <summary>
        /// Рассчитывает количество отверстий для достижения заданной пустотности
        /// при фиксированном радиусе.
        /// </summary>
        /// <param name="targetVoidness">Желаемая пустотность в процентах.</param>
        /// <returns>Результат расчёта с количеством отверстий.</returns>
        public VoidnessCalculationResult CalculateHolesCountForVoidness(
            double targetVoidness)
        {
            double length = _parameters[ParameterType.Length].Value;
            double width = _parameters[ParameterType.Width].Value;
            double height = _parameters[ParameterType.Height].Value;
            double holeRadius = _parameters[ParameterType.HoleRadius].Value;

            return _voidnessCalculator.CalculateHolesCountForVoidness(
                length, width, height, holeRadius, targetVoidness, _distributionType);
        }

        /// <summary>
        /// Получает диапазон возможной пустотности для текущих параметров.
        /// </summary>
        /// <returns>Кортеж с минимальной и максимальной пустотностью.</returns>
        public (double min, double max) GetVoidnessRange()
        {
            double length = _parameters[ParameterType.Length].Value;
            double width = _parameters[ParameterType.Width].Value;
            double height = _parameters[ParameterType.Height].Value;
            double holeRadius = _parameters[ParameterType.HoleRadius].Value;

            return _voidnessCalculator.GetVoidnessRange(
                length, width, height, holeRadius, _distributionType);
        }

        /// <summary>
        /// Пересчитывает зависимые параметры (максимальный радиус и количество отверстий).
        /// </summary>
        public void CalculateDependent()
        {
            double length = _parameters[ParameterType.Length].Value;
            double width = _parameters[ParameterType.Width].Value;
            double holeRadius = _parameters[ParameterType.HoleRadius].Value;

            // ИСПРАВЛЕНИЕ: Максимальный радиус зависит только от геометрии кирпича
            // Радиус ограничен шириной кирпича с учётом минимальных отступов от краёв
            double geometricMaxRadius = (width - 2 * MinimumEdgeMargin) / 2.0;

            // Устанавливаем максимальный радиус (минимум 2 мм)
            _parameters[ParameterType.HoleRadius].MaxValue = Math.Max(2, geometricMaxRadius);

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
                    errorMessages.Add($"• {displayName}: " +
                        $"должно быть в диапазоне [{minValue}, {maxValue}]");
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
                string message = "Обнаружены ошибки:\n\n"
                    + string.Join("\n", errorMessages);
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
        /// <returns>
        /// Кортеж с diameter, edgeMargin, minGap, availableLength, availableWidth.
        /// </returns>
        public static (double diameter, double edgeMargin,
            double minGap, double availableLength,
            double availableWidth) CalculateAvailableArea(
            double length, double width, double holeRadius
            )
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
        /// <param name="errorMessage">
        /// Сообщение об ошибке, если валидация не прошла.
        /// </param>
        /// <returns>True, если все правила соблюдены; иначе False.</returns>
        private bool ValidateBusinessRules(out string errorMessage)
        {
            errorMessage = null;
            double width = _parameters[ParameterType.Width].Value;
            double length = _parameters[ParameterType.Length].Value;
            double height = _parameters[ParameterType.Height].Value;
            double holeRadius = _parameters[ParameterType.HoleRadius].Value;
            int holesCount = (int)_parameters[ParameterType.HolesCount].Value;

            double maximumRadius = (width - 2 * MinimumEdgeMargin) / 2.0;
            if (holeRadius > maximumRadius)
            {
                errorMessage =
                    "• Радиус отверстий слишком большой для данной ширины кирпича";
                return false;
            }

            if (holesCount > 0 && holeRadius >= 2)
            {
                // Проверка пустотности (не более 45%)
                double brickVolume = length * width * height;
                double holeVolume = Math.PI * holeRadius * holeRadius * height;
                double totalHolesVolume = holeVolume * holesCount;
                double voidness = (totalHolesVolume / brickVolume) * 100.0;

                if (voidness > 45.0)
                {
                    errorMessage = $"• Пустотность ({voidness:F2}%) " +
                        $"превышает максимально допустимую (45%)";
                    return false;
                }

                double maximumHoles = CalculateMaxHoles(length, width, holeRadius);
                if (holesCount > maximumHoles)
                {
                    string distributionType =
                        _distributionType == HoleDistributionType.Straight
                        ? "прямого"
                        : "шахматного";
                    errorMessage = $"• Количество отверстий превышает возможное для " +
                        $"{distributionType} распределения";
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
            return _distributionType == HoleDistributionType.Straight
                ? _distributionCalculator.CalculateMaxHolesStraight(
                    length, width, holeRadius)
                : _distributionCalculator.CalculateMaxHolesStaggered(
                    length, width, holeRadius);
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
            };
        }
    }
}