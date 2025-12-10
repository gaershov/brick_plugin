namespace BrickPlugin.Models
{
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
}