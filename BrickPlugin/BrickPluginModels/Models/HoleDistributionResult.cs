namespace BrickPluginModels.Models
{
    /// <summary>
    /// Результат расчета размещения отверстий.
    /// </summary>
    public class HoleDistributionResult
    {
        /// <summary>
        /// Количество рядов.
        /// </summary>
        public int Rows { get; set; }

        /// <summary>
        /// Распределение отверстий по рядам.
        /// </summary>
        public List<int> Distribution { get; set; }

        /// <summary>
        /// Максимальное количество отверстий в одном ряду.
        /// </summary>
        public int MaxHolesInRow { get; set; }

        /// <summary>
        /// Горизонтальный зазор между отверстиями.
        /// </summary>
        public double HorizontalGap { get; set; }

        /// <summary>
        /// Вертикальный зазор между рядами.
        /// </summary>
        public double VerticalGap { get; set; }

        /// <summary>
        /// Горизонтальное смещение для шахматного распределения.
        /// </summary>
        public double StaggerOffset { get; set; }

        /// <summary>
        /// Инициализирует новый экземпляр класса HoleDistributionResult.
        /// </summary>
        public HoleDistributionResult()
        {
            Distribution = new List<int>();
        }
    }
}