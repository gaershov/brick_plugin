namespace BrickPluginModels.Models
{
    //TODO: RSDN
    /// <summary>
    /// Определяет типы параметров кирпича.
    /// </summary>
    public enum ParameterType
    {
        /// <summary>
        /// Длина кирпича.
        /// </summary>
        Length,

        /// <summary>
        /// Ширина кирпича.
        /// </summary>
        Width,

        /// <summary>
        /// Высота кирпича.
        /// </summary>
        Height,

        /// <summary>
        /// Радиус отверстий.
        /// </summary>
        HoleRadius,

        /// <summary>
        /// Количество отверстий.
        /// </summary>
        HolesCount
    }

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