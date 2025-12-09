using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using BrickPlugin.Models;
using BrickPlugin.Services;

namespace BrickPlugin
{
    /// <summary>
    /// Главная форма плагина для построения кирпичей в КОМПАС-3D.
    /// </summary>
    public partial class MainForm : Form
    {
        private BrickBuilder _builder;
        private BrickParameters _parameters;
        private Dictionary<ParameterType, TextBox> _textBoxMap;

        /// <summary>
        /// Инициализирует новый экземпляр класса MainForm.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            _builder = new BrickBuilder();
            _parameters = new BrickParameters();
            _parameters.ErrorMessage += OnErrorAppeared;
            _parameters.MaxRadiusChanged += OnMaxRadiusChanged;
            _parameters.MaxHolesChanged += OnMaxHolesChanged;

            InitializeTextBoxMapping();
            InitializeDefaultValues();
            AttachEventHandlers();
            UpdateAllFieldColors();
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
            textBox_LengthValue.Text =
                _parameters[ParameterType.Length]
                    .ToString(CultureInfo.InvariantCulture);
            textBox_WidthValue.Text =
                _parameters[ParameterType.Width]
                    .ToString(CultureInfo.InvariantCulture);
            textBox_HeightValue.Text =
                _parameters[ParameterType.Height]
                    .ToString(CultureInfo.InvariantCulture);
            textBox_HoleRadiusValue.Text =
                _parameters[ParameterType.HoleRadius]
                    .ToString(CultureInfo.InvariantCulture);
            textBox_HolesCountValue.Text =
                _parameters[ParameterType.HolesCount]
                    .ToString(CultureInfo.InvariantCulture);

            label_MaxRadiusHint.Text = _parameters.GetMaxRadiusHint();
            label_MaxHolesHint.Text = _parameters.GetMaxHolesHint();
        }

        /// <summary>
        /// Подключает обработчики событий к текстовым полям.
        /// </summary>
        private void AttachEventHandlers()
        {
            textBox_LengthValue.TextChanged +=
                (s, e) => OnParameterChanged(
                    ParameterType.Length,
                    textBox_LengthValue);
            textBox_WidthValue.TextChanged +=
                (s, e) => OnParameterChanged(
                    ParameterType.Width,
                    textBox_WidthValue);
            textBox_HeightValue.TextChanged +=
                (s, e) => OnParameterChanged(
                    ParameterType.Height,
                    textBox_HeightValue);
            textBox_HoleRadiusValue.TextChanged +=
                (s, e) => OnParameterChanged(
                    ParameterType.HoleRadius,
                    textBox_HoleRadiusValue);
            textBox_HolesCountValue.TextChanged +=
                (s, e) => OnParameterChanged(
                    ParameterType.HolesCount,
                    textBox_HolesCountValue);
        }

        /// <summary>
        /// Обрабатывает изменение значения параметра.
        /// </summary>
        /// <param name="paramType">Тип параметра.</param>
        /// <param name="textBox">Текстовое поле.</param>
        private void OnParameterChanged(
            ParameterType paramType,
            TextBox textBox)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.BackColor = Color.White;
                return;
            }

            if (double.TryParse(
                textBox.Text,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out double value))
            {
                _parameters[paramType] = value;
                UpdateAllFieldColors();
            }
            else
            {
                textBox.BackColor = Color.LightCoral;
            }
        }

        /// <summary>
        /// Обновляет цвет фона всех текстовых полей 
        /// в зависимости от валидности значений.
        /// </summary>
        private void UpdateAllFieldColors()
        {
            foreach (var kvp in _textBoxMap)
            {
                var param = _parameters.GetParameter(kvp.Key);
                if (param.IsValid)
                    kvp.Value.BackColor = Color.White;
                else
                    kvp.Value.BackColor = Color.LightCoral;
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
        /// Обрабатывает событие изменения максимального 
        /// количества отверстий.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="hint">Текст подсказки.</param>
        private void OnMaxHolesChanged(object sender, string hint)
        {
            label_MaxHolesHint.Text = hint;
            UpdateFieldColor(ParameterType.HolesCount, textBox_HolesCountValue);
        }

        /// <summary>
        /// Обновляет цвет фона текстового поля в зависимости 
        /// от валидности значения.
        /// </summary>
        /// <param name="paramType">Тип параметра.</param>
        /// <param name="textBox">Текстовое поле.</param>
        private void UpdateFieldColor(ParameterType paramType, TextBox textBox)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.BackColor = Color.White;
                return;
            }

            var param = _parameters.GetParameter(paramType);
            if (param.IsValid)
                textBox.BackColor = Color.White;
            else
                textBox.BackColor = Color.LightCoral;
        }

        /// <summary>
        /// Обрабатывает событие появления ошибки валидации.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="errorMessage">Сообщение об ошибке.</param>
        private void OnErrorAppeared(object sender, string errorMessage)
        {
            MessageBox.Show(
                errorMessage,
                "Ошибка валидации",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Обрабатывает нажатие кнопки построения модели.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void button_Build_Click(object sender, EventArgs e)
        {
            BuildModel();
        }

        /// <summary>
        /// Выполняет построение модели кирпича.
        /// </summary>
        private void BuildModel()
        {
            try
            {
                foreach (var textBox in _textBoxMap.Values)
                {
                    textBox.BackColor = Color.White;
                }

                button_Build.Enabled = false;
                button_Build.Text = "Построение...";
                Application.DoEvents();

                if (!_parameters.Validate())
                {
                    button_Build.Enabled = true;
                    button_Build.Text = "Построить кирпич";
                    return;
                }

                _builder.Build(_parameters);

                MessageBox.Show(
                    "✓ Модель успешно построена в КОМПАС-3D!",
                    "Успех",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"✗ Ошибка при построении:\n\n{ex.Message}",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                button_Build.Enabled = true;
                button_Build.Text = "Построить кирпич";
            }
        }
    }
}