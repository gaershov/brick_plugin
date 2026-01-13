namespace BrickPluginModels.Models
{
    /// <summary>
    /// Определяет типы распределения отверстий в кирпиче.
    /// </summary>
    public enum HoleDistributionType
    {
        /// <summary>
        /// Прямое распределение без смещения.
        /// </summary>
        Straight,

        /// <summary>
        /// Шахматное распределение со смещением.
        /// </summary>
        Staggered
    }
}