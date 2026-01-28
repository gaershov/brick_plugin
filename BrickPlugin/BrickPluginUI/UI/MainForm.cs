using BrickPlugin.Services;
using BrickPluginModels.Models;
using BrickPluginModels.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;

namespace BrickPluginUI
{
    /// <summary>
    /// Главная форма плагина для построения кирпичей в КОМПАС-3D.
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// Строитель кирпичей.
        /// </summary>
        private BrickBuilder _builder;

        /// <summary>
        /// Параметры кирпича.
        /// </summary>
        private BrickParameters _parameters;

        /// <summary>
        /// Словарь соответствия типов параметров и текстовых полей.
        /// </summary>
        private Dictionary<ParameterType, TextBox> _textBoxMap;

        /// <summary>
        /// Флаг, указывающий, что обновление параметров происходит программно.
        /// </summary>
        private bool _isUpdatingProgrammatically;

        /// <summary>
        /// Инициализирует новый экземпляр класса MainForm.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            _builder = new BrickBuilder();
            _parameters = new BrickParameters();
            _isUpdatingProgrammatically = false;

            _parameters.ErrorMessage += OnErrorAppeared;
            _parameters.MaxRadiusChanged += OnMaxRadiusChanged;
            _parameters.MaxHolesChanged += OnMaxHolesChanged;

            InitializeTextBoxMapping();
            InitializeDefaultValues();
            AttachEventHandlers();
            UpdateAllFieldColors();
            UpdateCurrentVoidness();
        }

        /// <summary>
        /// Инициализирует маппинг между типами параметров и текстовыми полями.
        /// </summary>
        private void InitializeTextBoxMapping()
        {
            _textBoxMap = new Dictionary<ParameterType, TextBox>
            {
                { ParameterType.Length, textBox_LengthValue },
                { ParameterType.Width, textBox_WidthValue },
                { ParameterType.Height, textBox_HeightValue },
                { ParameterType.HoleRadius, textBox_HoleRadiusValue },
                { ParameterType.HolesCount, textBox_HolesCountValue }
            };
        }

        /// <summary>
        /// Устанавливает значения по умолчанию в текстовые поля.
        /// </summary>
        private void InitializeDefaultValues()
        {
            textBox_LengthValue.Text = _parameters[ParameterType.Length]
                .ToString(CultureInfo.InvariantCulture);
            textBox_WidthValue.Text = _parameters[ParameterType.Width]
                .ToString(CultureInfo.InvariantCulture);
            textBox_HeightValue.Text = _parameters[ParameterType.Height]
                .ToString(CultureInfo.InvariantCulture);
            textBox_HoleRadiusValue.Text = _parameters[ParameterType.HoleRadius]
                .ToString(CultureInfo.InvariantCulture);
            textBox_HolesCountValue.Text = _parameters[ParameterType.HolesCount]
                .ToString(CultureInfo.InvariantCulture);

            label_MaxRadiusHint.Text = _parameters.GetMaxRadiusHint();
            label_MaxHolesHint.Text = _parameters.GetMaxHolesHint();

            radioButton_Straight.Checked = true;
            _parameters.DistributionType = HoleDistributionType.Straight;
        }

        /// <summary>
        /// Подключает обработчики событий к элементам управления.
        /// </summary>
        private void AttachEventHandlers()
        {
            textBox_LengthValue.TextChanged += (s, e) =>
                OnParameterChanged(ParameterType.Length, textBox_LengthValue);
            textBox_WidthValue.TextChanged += (s, e) =>
                OnParameterChanged(ParameterType.Width, textBox_WidthValue);
            textBox_HeightValue.TextChanged += (s, e) =>
                OnParameterChanged(ParameterType.Height, textBox_HeightValue);
            textBox_HoleRadiusValue.TextChanged += (s, e) =>
                OnParameterChanged(ParameterType.HoleRadius, textBox_HoleRadiusValue);
            textBox_HolesCountValue.TextChanged += (s, e) =>
                OnParameterChanged(ParameterType.HolesCount, textBox_HolesCountValue);

            button_CalculateVoidness.Click += OnCalculateVoidnessClick;

            radioButton_Straight.CheckedChanged += OnDistributionTypeChanged;
            radioButton_Staggered.CheckedChanged += OnDistributionTypeChanged;
        }

        /// <summary>
        /// Обрабатывает изменение типа распределения отверстий.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void OnDistributionTypeChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton radioButton && radioButton.Checked)
            {
                HoleDistributionType distributionType 
                    = radioButton == radioButton_Straight
                    ? HoleDistributionType.Straight
                    : HoleDistributionType.Staggered;

                _parameters.DistributionType = distributionType;
                UpdateAllFieldColors();
                UpdateCurrentVoidness();
            }
        }

        /// <summary>
        /// Обрабатывает изменение значения параметра.
        /// </summary>
        /// <param name="paramType">Тип параметра.</param>
        /// <param name="textBox">Текстовое поле.</param>
        private void OnParameterChanged(ParameterType paramType, TextBox textBox)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.BackColor = Color.LightCoral;
                return;
            }

            if (double.TryParse(textBox.Text, NumberStyles.Any,
                CultureInfo.InvariantCulture, out double value))
            {
                _parameters[paramType] = value;
                UpdateAllFieldColors();

                if (!_isUpdatingProgrammatically)
                {
                    UpdateCurrentVoidness();
                }
            }
            else
            {
                textBox.BackColor = Color.LightCoral;
            }
        }

        /// <summary>
        /// Обновляет цвет фона всех текстовых полей в зависимости от валидности значений.
        /// </summary>
        private void UpdateAllFieldColors()
        {
            foreach (var kvp in _textBoxMap)
            {
                var param = _parameters.GetParameter(kvp.Key);

                // ИСПРАВЛЕНИЕ: проверяем также, что поле не пустое
                if (string.IsNullOrWhiteSpace(kvp.Value.Text))
                {
                    kvp.Value.BackColor = Color.LightCoral;
                }
                else if (param.IsValid)
                {
                    kvp.Value.BackColor = Color.White;
                }
                else
                {
                    kvp.Value.BackColor = Color.LightCoral;
                }
            }
        }

        /// <summary>
        /// Обрабатывает событие изменения максимального радиуса.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="hint">Текст подсказки.</param>
        private void OnMaxRadiusChanged(object sender, string hint)
        {
            label_MaxRadiusHint.Text = hint;
            UpdateFieldColor(ParameterType.HoleRadius, textBox_HoleRadiusValue);
            UpdateFieldColor(ParameterType.HolesCount, textBox_HolesCountValue);
        }

        /// <summary>
        /// Обрабатывает событие изменения максимального количества отверстий.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="hint">Текст подсказки.</param>
        private void OnMaxHolesChanged(object sender, string hint)
        {
            label_MaxHolesHint.Text = hint;
            UpdateFieldColor(ParameterType.HolesCount, textBox_HolesCountValue);
        }

        /// <summary>
        /// Обновляет цвет фона конкретного поля.
        /// </summary>
        /// <param name="paramType">Тип параметра.</param>
        /// <param name="textBox">Текстовое поле.</param>
        private void UpdateFieldColor(ParameterType paramType, TextBox textBox)
        {
            var param = _parameters.GetParameter(paramType);

            // ИСПРАВЛЕНИЕ: проверяем также, что поле не пустое
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.BackColor = Color.LightCoral;
            }
            else if (param.IsValid)
            {
                textBox.BackColor = Color.White;
            }
            else
            {
                textBox.BackColor = Color.LightCoral;
            }
        }

        /// <summary>
        /// Обрабатывает появление ошибки валидации.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="errorMessage">Сообщение об ошибке.</param>
        private void OnErrorAppeared(object sender, string errorMessage)
        {
            MessageBox.Show(
                errorMessage,
                "Ошибка валидации",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        /// <summary>
        /// Обрабатывает нажатие кнопки построения модели.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void OnBuildClick(object sender, EventArgs e)
        {
            if (!ValidateAllFieldsFilled())
            {
                MessageBox.Show(
                    "Пожалуйста, заполните все поля корректными " +
                    "числовыми значениями перед построением модели.\n\n" +
                    "Поля с ошибками подсвечены красным цветом.",
                    "Ошибка ввода данных",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (_parameters.Validate())
            {
                try
                {
                    _builder.Build(_parameters);

                    MessageBox.Show(
                        "Построение завершено успешно!",
                        "Успех",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Произошла ошибка при " +
                        $"построении модели:\n{ex.Message}",
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Проверяет, что все текстовые поля заполнены и содержат корректные числовые значения.
        /// </summary>
        /// <returns>True, если все поля заполнены и корректны; иначе False.</returns>
        private bool ValidateAllFieldsFilled()
        {
            bool allValid = true;

            foreach (var kvp in _textBoxMap)
            {
                // Проверка на пустое поле
                if (string.IsNullOrWhiteSpace(kvp.Value.Text))
                {
                    kvp.Value.BackColor = Color.LightCoral;
                    allValid = false;
                    continue;
                }
                if (!double.TryParse(kvp.Value.Text, NumberStyles.Any,
                    CultureInfo.InvariantCulture, out double _))
                {
                    kvp.Value.BackColor = Color.LightCoral;
                    allValid = false;
                }
            }

            return allValid;
        }

        /// <summary>
        /// Обрабатывает нажатие кнопки расчёта по пустотности.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void OnCalculateVoidnessClick(object sender, EventArgs e)
        {
            if (!ValidateVoidnessInput(out double targetVoidness))
            {
                return;
            }

            double holeRadius = _parameters[ParameterType.HoleRadius];

            if (holeRadius >= 2)
            {
                CalculateWithFixedRadius(targetVoidness, holeRadius);
            }
            else
            {
                double length = _parameters[ParameterType.Length];
                double width = _parameters[ParameterType.Width];
                double height = _parameters[ParameterType.Height];
                CalculateOptimalParameters
                    (targetVoidness, length, width, height);
            }
        }

        /// <summary>
        /// Проверяет введенное значение пустотности на корректность.
        /// </summary>
        /// <param name="targetVoidness">Выходное значение пустотности.</param>
        /// <returns>True, если значение корректно; иначе False.</returns>
        private bool ValidateVoidnessInput(out double targetVoidness)
        {
            targetVoidness = 0;

            if (string.IsNullOrWhiteSpace(textBox_VoidnessValue.Text))
            {
                MessageBox.Show(
                    "Введите желаемую пустотность в процентах (от 0 до 45).",
                    "Пустое значение",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return false;
            }

            if (!double.TryParse(textBox_VoidnessValue.Text, NumberStyles.Any,
                CultureInfo.InvariantCulture, out targetVoidness))
            {
                MessageBox.Show(
                    "Введено некорректное значение пустотности." +
                    "\nИспользуйте числовой формат, например: 25 или 12.5",
                    "Ошибка формата",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                textBox_VoidnessValue.BackColor = Color.LightCoral;
                return false;
            }

            if (targetVoidness <= 0 || targetVoidness > 45)
            {
                MessageBox.Show(
                    $"Пустотность должна быть в диапазоне " +
                    $"от 0 до 45%.\nВведено: {targetVoidness:F2}%",
                    "Недопустимое значение",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                textBox_VoidnessValue.BackColor = Color.LightCoral;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Выполняет расчет количества отверстий при фиксированном радиусе.
        /// </summary>
        /// <param name="targetVoidness">Целевая пустотность.</param>
        /// <param name="holeRadius">Фиксированный радиус отверстия.</param>
        private void CalculateWithFixedRadius(double targetVoidness, double holeRadius)
        {
            var range = _parameters.GetVoidnessRange();

            if (targetVoidness < range.min || targetVoidness > range.max)
            {
                HandleUnachievableVoidness(targetVoidness, holeRadius, range);
                return;
            }

            var calculationResult =
                _parameters.CalculateHolesCountForVoidness(targetVoidness);

            if (calculationResult.Success)
            {
                ApplyCalculationResult(calculationResult, holeRadius);
            }
            else
            {
                ShowCalculationError(calculationResult.ErrorMessage);
            }
        }

        /// <summary>
        /// Обрабатывает случай, когда пустотность недостижима для текущего радиуса.
        /// </summary>
        /// <param name="targetVoidness">Целевая пустотность.</param>
        /// <param name="holeRadius">Текущий радиус отверстия.</param>
        /// <param name="range">Доступный диапазон пустотности.</param>
        private void HandleUnachievableVoidness(
            double targetVoidness,
            double holeRadius,
            (double min, double max) range)
        {
            string distributionName = _parameters.DistributionType
                == HoleDistributionType.Straight
                ? "прямого"
                : "шахматного";

            DialogResult dialogResult = MessageBox.Show(
                $"Для радиуса {holeRadius:F1} мм при " +
                $"{distributionName} распределении " +
                $"доступна пустотность от {range.min:F2}% до {range.max:F2}%.\n\n" +
                $"Введённое значение: {targetVoidness:F2}%\n\n" +
                $"Пересчитать радиус автоматически для достижения " +
                $"{targetVoidness:F2}%?",
                "Пустотность недостижима",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                double length = _parameters[ParameterType.Length];
                double width = _parameters[ParameterType.Width];
                double height = _parameters[ParameterType.Height];
                CalculateOptimalParameters(targetVoidness, length, width, height);
            }
            else
            {
                textBox_VoidnessValue.BackColor = Color.LightCoral;
            }
        }

        /// <summary>
        /// Применяет результат успешного расчета к параметрам.
        /// </summary>
        /// <param name="result">Результат расчета.</param>
        /// <param name="holeRadius">Радиус отверстия.</param>
        private void ApplyCalculationResult(
            VoidnessCalculationResult result,
            double holeRadius)
        {
            _isUpdatingProgrammatically = true;
            textBox_HolesCountValue.Text =
                result.HolesCount.ToString(CultureInfo.InvariantCulture);
            _parameters[ParameterType.HolesCount] = result.HolesCount;
            textBox_VoidnessValue.BackColor = Color.LightGreen;
            _isUpdatingProgrammatically = false;

            UpdateCurrentVoidness();

            MessageBox.Show(
                $"✓ Параметры успешно рассчитаны!\n\n" +
                $"Радиус отверстий: {holeRadius:F1} мм\n" +
                $"Количество отверстий: {result.HolesCount} шт\n" +
                $"Фактическая пустотность: {result.ActualVoidness:F2}%",
                "Расчёт завершён",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        /// <summary>
        /// Отображает сообщение об ошибке расчета.
        /// </summary>
        /// <param name="errorMessage">Сообщение об ошибке.</param>
        private void ShowCalculationError(string errorMessage)
        {
            MessageBox.Show(
                errorMessage,
                "Ошибка расчёта",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            textBox_VoidnessValue.BackColor = Color.LightCoral;
        }

        /// <summary>
        /// Рассчитывает оптимальные параметры отверстий для заданной пустотности.
        /// </summary>
        /// <param name="targetVoidness">Целевая пустотность.</param>
        /// <param name="length">Длина кирпича.</param>
        /// <param name="width">Ширина кирпича.</param>
        /// <param name="height">Высота кирпича.</param>
        private void CalculateOptimalParameters(double targetVoidness,
            double length, double width, double height)
        {
            var optimalResult = _parameters.CalculateOptimalParameters(targetVoidness);

            if (optimalResult.Success)
            {
                _isUpdatingProgrammatically = true;
                textBox_HoleRadiusValue.Text =
                    optimalResult.HoleRadius.
                    ToString("F1", CultureInfo.InvariantCulture);
                textBox_HolesCountValue.Text =
                    optimalResult.HolesCount.ToString(CultureInfo.InvariantCulture);
                _parameters[ParameterType.HoleRadius]
                    = optimalResult.HoleRadius;
                _parameters[ParameterType.HolesCount]
                    = optimalResult.HolesCount;
                textBox_VoidnessValue.BackColor = Color.LightGreen;
                _isUpdatingProgrammatically = false;

                UpdateCurrentVoidness();

                MessageBox.Show(
                    $"✓ Параметры успешно рассчитаны!\n\n" +
                    $"Радиус отверстий: {optimalResult.HoleRadius:F1} мм\n" +
                    $"Количество отверстий: {optimalResult.HolesCount} шт\n" +
                    $"Фактическая пустотность: {optimalResult.ActualVoidness:F2}%",
                    "Расчёт завершён",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(
                    optimalResult.ErrorMessage ??
                    "Не удалось подобрать параметры для заданной пустотности.",
                    "Ошибка расчёта",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                textBox_VoidnessValue.BackColor = Color.LightCoral;
            }
        }

        /// <summary>
        /// Обновляет отображение текущей пустотности.
        /// </summary>
        private void UpdateCurrentVoidness()
        {
            double holeRadius
                = _parameters[ParameterType.HoleRadius];
            int holesCount
                = (int)_parameters[ParameterType.HolesCount];

            if (holesCount > 0 && holeRadius >= 2)
            {
                double currentVoidness = _parameters.CalculateCurrentVoidness();
                label_CurrentVoidnessValue.Text = $"{currentVoidness:F2}%";

                if (currentVoidness > 45.0)
                {
                    label_CurrentVoidnessValue.ForeColor = Color.Red;
                }
                else if (currentVoidness > 40.0)
                {
                    label_CurrentVoidnessValue.ForeColor = Color.Orange;
                }
                else
                {
                    label_CurrentVoidnessValue.ForeColor = Color.DarkGreen;
                }
            }
            else
            {
                label_CurrentVoidnessValue.Text = "0.0%";
                label_CurrentVoidnessValue.ForeColor = Color.DarkGreen;
            }
        }
    }
}