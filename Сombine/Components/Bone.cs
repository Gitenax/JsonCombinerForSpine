using System;
using System.Text.Json.Serialization;
using Сombine.Units;

namespace Сombine.Components
{
    public class Bone
    {
        private string _parentName;
        
        public Bone(Bone copy, float x, float y)
        {
            Name = copy.Name;
            Parent = copy.Parent;
            Length = copy.Length;
            X = x;
            Y = y;
            Rotation = copy.Rotation;
            ScaleX = copy.ScaleX;
            ScaleY = copy.ScaleY;
            ShearX = copy.ShearX;
            ShearY = copy.ShearY;
        }
        
        [JsonConstructor]
        [Obsolete("Конструтор который используется только для работы с JSON", true)]
        public Bone(string name, string parentName, float length, float x, float y, 
            float rotation, float scaleX, float scaleY, float shearX, float shearY) 
            : this(name, (Bone)null, length, x, y, rotation, scaleX, scaleY, shearX, shearY)
        {
            _parentName = parentName;
        }
        
        public Bone(string name, Bone parent = null, float length = 0, float x = 0, float y = 0, 
            float rotation = 0, float scaleX = 0, float scaleY = 0, float shearX = 0, float shearY = 0)
        {
            Name = name;
            Parent = parent;
            Length = length;
            X = x;
            Y = y;
            Rotation = rotation;
            ScaleX = scaleX;
            ScaleY = scaleY;
            ShearX = shearX;
            ShearY = shearY;
        }
        
        
        
        [Flags]
        public enum TransformMode
        {
            //0000 0 Flip Scale Rotation
            Normal                 = 0, // 0000
            OnlyTranslation        = 7, // 0111
            NoRotationOrReflection = 1, // 0001
            NoScale                = 2, // 0010
            NoScaleOrReflection    = 6, // 0110
        }
        
        [JsonIgnore]
        public int Index { get; set; }

        /// <summary>
        ///     Имя кости. Уникально для скелета.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Кость-родитель для данной кости.
        ///     <remarks>Кости упорядочены таким образом, что родитель всегда идет перед дочерней костью.</remarks>
        /// </summary>
        [JsonIgnore] 
        public Bone Parent { get; set; }

        [JsonPropertyName("parent")]
        public string ParentName => Parent?.Name ?? _parentName;

        /// <summary>
        ///     Длина кости. Длина кости обычно не используется во время выполнения, за исключением рисования отладочных линий для
        ///     костей
        ///     <remarks>По умолчанию 0</remarks>
        /// </summary>
        public float Length { get; set; }

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
        ///     Сдвиг кости по X-оси кости для установки позы.
        ///     <remarks>По умолчанию 0</remarks>
        /// </summary>
        public float ShearX { get; set; }

        /// <summary>
        ///     Сдвиг кости по Y-оси кости для установки позы.
        ///     <remarks>По умолчанию 0</remarks>
        /// </summary>
        public float ShearY { get; set; }

        /// <summary>
        ///     Цвет кости, как это было в Spine.
        ///     <example>Например #989898FF RGBA</example>
        /// </summary>
        public object Color { get; set; }

        /// <summary>
        ///     Определяет, как наследуются трансформации родительской кости:
        ///     <remarks>нормальная</remarks>
        ///     <remarks>только Трансляция</remarks>
        ///     <remarks>отсутствие Вращения Или Отражения</remarks>
        ///     <remarks>отсутствие Масштаба</remarks>
        ///     <remarks>noScaleOrReflection</remarks>
        /// </summary>
        public TransformMode Transform { get; set; }


        public void SetParent(Bone bone)
        {
            Parent = bone;
        }

        public void Adjust(float scaleX, float scaleY)
        {
            X *= scaleX;
            Y *= scaleY;
        }
        
        public void AdjustLength(float scaleX, float scaleY)
        {
            float x = X + (float)Math.Cos(Rotation) * Length * scaleX;
            float y = Y + (float)Math.Sin(Rotation) * Length * scaleY;
            
            Length = Magnitude(x, y, X, Y);
        }
        
        public override string ToString()
        {
            return $"Bone(" +
                   $"name:{Name}, " +
                   $"parent:{Parent}, " +
                   $"lenght:{Length}, " +
                   $"x:{X}, " +
                   $"y:{Y}, " +
                   $"rotation:{Rotation}, " +
                   $"scaleX:{ScaleX}, " +
                   $"scaleY:{ScaleY}, " +
                   $"shearX:{ShearX}, " +
                   $"shearY:{ShearY}, " +
                   $"color:{Color}, " +
                   $"transform:{Transform})";
        }


        private float Magnitude(float aX, float aY, float bX, float bY)
        {
            var x = aX - bX;
            var y = aY - bY;
            return (float)Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
        }
    }
}