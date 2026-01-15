namespace BrickPluginModels.Services
{
    /// <summary>
    /// Результат расчета пустотности.
    /// </summary>
    public class VoidnessCalculationResult
    {
        /// <summary>
        /// Рассчитанный радиус отверстия.
        /// </summary>
        public double HoleRadius { get; set; }

        /// <summary>
        /// Рассчитанное количество отверстий.
        /// </summary>
        public int HolesCount { get; set; }

        /// <summary>
        /// Фактическая достигнутая пустотность в процентах.
        /// </summary>
        public double ActualVoidness { get; set; }

        /// <summary>
        /// Успешность расчета.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Сообщение об ошибке, если расчет не удался.
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}