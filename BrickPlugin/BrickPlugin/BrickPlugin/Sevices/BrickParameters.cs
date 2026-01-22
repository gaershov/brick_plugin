using System;
using System.Collections.Generic;
using System.Linq;
using BrickPlugin.Models;

namespace BrickPlugin.Services
{
    public class BrickParameters
    {
        private readonly Dictionary<ParameterType, BrickParameter> _parameters;

        public event EventHandler<string> ErrorMessage;
        public event EventHandler<string> MaxRadiusChanged;
        public event EventHandler<string> MaxHolesChanged;

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

        public double this[ParameterType name]
        {
            get => _parameters[name].Value;
            set
            {
                _parameters[name].Value = value;
                CalculateDependent();
            }
        }

        public BrickParameter GetParameter(ParameterType name)
        {
            return _parameters[name];
        }

        public string GetMaxRadiusHint()
        {
            var width = _parameters[ParameterType.Width].Value;
            double maxRadius = width * 0.25;
            return $"Макс: {maxRadius:F1} мм";
        }

        public string GetMaxHolesHint()
        {
            var holeRadius = _parameters[ParameterType.HoleRadius].Value;
            double maxHoles = _parameters[ParameterType.HolesCount].MaxValue;
            return $"Макс: {(int)maxHoles} шт";
        }

        public void CalculateDependent()
        {
            var length = _parameters[ParameterType.Length].Value;
            var width = _parameters[ParameterType.Width].Value;
            var holeRadius = _parameters[ParameterType.HoleRadius].Value;

            double maxRadius = width * 0.25;
            _parameters[ParameterType.HoleRadius].MaxValue = maxRadius;

            MaxRadiusChanged?.Invoke(this, GetMaxRadiusHint());

            if (holeRadius >= 2)
            {
                double maxHoles = CalculateMaxHoles(length, width, holeRadius);
                _parameters[ParameterType.HolesCount].MaxValue = Math.Max(0, maxHoles);
            }
            else
            {
                _parameters[ParameterType.HolesCount].MaxValue = 0;
            }

            MaxHolesChanged?.Invoke(this, GetMaxHolesHint());
        }

        public bool Validate()
        {
            List<string> errorMessages = new List<string>();

            foreach (var kvp in _parameters)
            {
                if (!kvp.Value.IsValid())
                {
                    errorMessages.Add($"• {GetParameterDisplayName(kvp.Key)}: " +
                        $"должно быть в диапазоне [{kvp.Value.MinValue:F0}, {kvp.Value.MaxValue:F0}]");
                }
            }

            if (!ValidateBusinessRules(out var businessError))
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

        private bool ValidateBusinessRules(out string errorMessage)
        {
            errorMessage = null;
            var width = _parameters[ParameterType.Width].Value;
            var length = _parameters[ParameterType.Length].Value;
            var holeRadius = _parameters[ParameterType.HoleRadius].Value;
            var holesCount = (int)_parameters[ParameterType.HolesCount].Value;

            if (holeRadius > width * 0.25)
            {
                errorMessage = "• Радиус отверстий превышает 25% от ширины";
                return false;
            }

            if (holesCount > 0 && holeRadius >= 2)
            {
                double maxHoles = CalculateMaxHoles(length, width, holeRadius);
                if (holesCount > maxHoles)
                {
                    errorMessage = "• Количество отверстий превышает возможное";
                    return false;
                }
            }

            return true;
        }

        private double CalculateMaxHoles(double length, double width, double holeRadius)
        {
            double diameter = 2 * holeRadius;
            double edgeMargin = Math.Max(2 * holeRadius, 10);
            double minGap = Math.Max(1.5 * holeRadius, 5);

            double availableLength = length - 2 * edgeMargin;
            double availableWidth = width - 2 * edgeMargin;

            if (availableLength < diameter || availableWidth < diameter)
            {
                return 0;
            }

            int maxHorizontal = (int)Math.Floor((availableLength + minGap) / (diameter + minGap));
            int maxVertical = (int)Math.Floor((availableWidth + minGap) / (diameter + minGap));

            if (maxHorizontal <= 0 || maxVertical <= 0)
            {
                return 0;
            }

            return maxHorizontal * maxVertical;
        }

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