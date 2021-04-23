using System.Text.Json.Serialization;

namespace Сombine.Components.Attachments
{
    public class Region : Attachment, IVertexExcludingAttachment
    {
        [JsonConstructor]
        public Region(float x, float y, float width, float height, float rotation, float scaleX, float scaleY)
        {
	        Type = "region";
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Rotation = rotation;
            ScaleX = scaleX;
            ScaleY = scaleY;
        }
        
        /// <summary>
        ///     Если указан, то это значение используется вместо имени вложения для поиска области текстуры.
        /// </summary>
        public string Path { get; set; }
        
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
        ///     Масштабирование кости по X-оси  для установки позы.
        ///     <remarks>По умолчанию 1</remarks>
        /// </summary>
        public float ScaleX { get; set; }

        /// <summary>
        ///     Масштабирование кости по Y-оси кости для установки позы.
        ///     <remarks>По умолчанию 1</remarks>
        /// </summary>
        public float ScaleY { get; set; }
        
        /// <summary>
        ///     Поворот в градусах кости относительно родительской для установки позы.
        ///     <remarks>По умолчанию 0</remarks>
        /// </summary>
        public float Rotation { get; set; }
        
        /// <summary>
        /// Ширина изображения.
        /// </summary>
        public float  Width  { get; set; }
        
        /// <summary>
        /// Высота изображения.
        /// </summary>
        public float  Height { get; set; }
        
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