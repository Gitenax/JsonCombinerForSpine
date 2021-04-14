using System.Text.Json.Serialization;

namespace Сombine.Components
{
    /// <summary>
    /// Хранит метаданные о скелете.
    /// </summary>
    public class Skeleton
    {
        public Skeleton(){ }

        
        
        [JsonConstructor]
        public Skeleton(string hash, string spine, float x, float y, 
            float width, float height, int fps, string images, string audio)
        {
            Hash = hash;
            Spine = spine;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Fps = fps;
            Images = images;
            Audio = audio;
        }
        
        /// <summary>
        /// Хэш всех данных скелета.
        /// </summary>
        public string Hash   { get; set; }
        /// <summary>
        /// Версия Spine, которая экспортировала данные.
        /// </summary>
        public string Spine  { get; set; }
        /// <summary>
        /// X-координата нижнего левого угла AABB для прикреплений скелета, как это было в установочной позе в Spine.
        /// </summary>
        public float  X      { get; set; }
        /// <summary>
        /// Y-координата нижнего левого угла AABB для прикреплений скелета, как это было в установочной позе в Spine.
        /// </summary>
        public float  Y      { get; set; }
        /// <summary>
        /// Ширина AABB для прикреплений скелета, как это было в установочной позе в Spine.
        /// Можно использовать как общий размер скелета, хотя AABB скелета зависит от того, как он размещен.
        /// </summary>
        public float  Width  { get; set; }
        /// <summary>
        /// Высота AABB для прикреплений скелета, как это было в установочной позе в Spine.
        /// </summary>
        public float  Height { get; set; }
        /// <summary>
        /// Частота кадров допинг-листа в кадрах в секунду, как это было в Spine.
        /// <remarks>Предположим, что 30, если опущено. Несущественное.</remarks>
        /// </summary>
        public int    Fps    { get; set; }
        /// <summary>
        /// Путь для изображений, как это было в Spine.
        /// <remarks>Несущественное.</remarks>
        /// </summary>
        public string Images { get; set; }
        /// <summary>
        /// Путь для аудио, как это было в Spine.
        /// <remarks>Несущественное.</remarks>
        /// </summary>
        public string Audio  { get; set; }


        
        public override string ToString()
        {
            return "Skeleton(" +
                   $"hash:{Hash}, " +
                   $"spine:{Spine}, " +
                   $"x:{X}, " +
                   $"y:{Y}, " +
                   $"width:{Width}, " +
                   $"height:{Height}, " +
                   $"fps:{Fps}, " +
                   $"images:{Images}, " +
                   $"audio:{Audio})";
        }
    }
}