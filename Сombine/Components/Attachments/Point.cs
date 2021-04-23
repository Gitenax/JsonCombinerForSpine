using System.Text.Json.Serialization;

namespace Сombine.Components.Attachments
{
    public class Point : Attachment, IVertexExcludingAttachment
    {
        [JsonConstructor]
        public Point(float x, float y, float rotation, object color)
        {
            Type = "point";
            X = x;
            Y = y;
            Rotation = rotation;
            Color = color;
        }
        
        /// <summary>
        ///     X-положение кости относительно родителя для установочной позы.
        ///     <remarks>По умолчанию 0</remarks>
        /// </summary>
        public float X { get; set; }

        /// <summary>
        ///     Y-положение кости относительно родителя для установочной позы.
        ///     <remarks>По умолчанию 0</remarks>
        /// </summary>
        public float Y { get; set; }
        
        /// <summary>
        ///     Поворот в градусах кости относительно родительской для установки позы.
        ///     <remarks>По умолчанию 0</remarks>
        /// </summary>
        public float Rotation { get; set; }
        
        /// <summary>
        ///     Цвет тонирования.
        /// </summary>
        public object Color { get; set; }
        
        
        public void Adjust(float x, float y)
        {
            X *= x;
            Y *= y;
        }
    }
}