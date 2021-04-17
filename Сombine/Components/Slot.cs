using System;
using System.Text.Json.Serialization;

namespace Сombine.Components
{
    /// <summary>
    /// Описывает порядок отрисовки и доступные слоты, в которых могут быть назначены вложения.
    /// <remarks>Слоты упорядочиваются в порядке отрисовки.
    /// Изображения в более высоких индексных слотах рисуются поверх изображений в более низких индексных слотах.</remarks>
    /// </summary>
    public class Slot
    {
        private string _boneName;
        private string _attachmentName;
        
        [JsonConstructor]
        [Obsolete("Конструтор который используется только для работы с JSON", true)]
        public Slot(string name, string boneName, string attachmentName)
        {
            Name = name;
            _boneName = boneName;
            _attachmentName = attachmentName;
        }
        
        
        
        public Slot(string name, Bone boneAttached, Attachment attachment)
        {
            Name = name;
            Bone = boneAttached;
            Attachment = attachment;
        }
        
        
        public enum BlendMode { Normal, Additive, Multiply, Screen }


        /// <summary>
        ///     Название слота. Это уникально для скелета.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Название кости, к которой прикреплен этот слот.
        /// </summary>
        [JsonIgnore] 
        public Bone Bone { get; set; }
        
        [JsonPropertyName("bone")]
        public string BoneName => Bone?.Name ?? _boneName;

        /// <summary>
        ///     Цвет слота для установки позы. Это 8-символьная строка, содержащая 4 двузначных шестнадцатеричных числа в порядке
        ///     RGBA.
        ///     <remarks>Если Альфа-канал не задан, по умолчанию он "FF".</remarks>
        ///     <remarks>По умолчанию "FFFFFFFF"</remarks>
        /// </summary>
        public object Color { get; set; }

        /// <summary>
        ///     Темный цвет слота для установки позы, используется для двухцветной тонировки. Это 6-символьная строка, содержащая 3
        ///     двузначных шестнадцатеричных числа в порядке RGB.
        /// </summary>
        public object Dark { get; set; }

        /// <summary>
        ///     Препления слота для установки позы.
        ///     <remarks>Не используется если нет привязки</remarks>
        /// </summary>
        [JsonIgnore]
        public Attachment Attachment { get; set; }
        
        [JsonPropertyName("attachment")]
        public string AttachmentName => Attachment?.Name ?? _attachmentName;
        
        /// <summary>
        ///     Тип смешивания, используемый при рисовании слота: обычный, аддитивный, умноженный или экранный.
        /// </summary>
        public BlendMode Blend { get; set; }


        public override bool Equals(object obj)
        {
            if (obj is Slot slot)
            {
                return slot.AttachmentName == AttachmentName 
                       && slot.Name == Name 
                       && slot.BoneName == BoneName;
            }
            return false;
        }


        public override string ToString()
        {
            return $"Slot(" +
                   $"name:{Name}, " +
                   $"bone:{Bone.Name}, " +
                   $"color:{Color}, " +
                   $"dark:{Dark}, " +
                   $"attachment:{Attachment}, " +
                   $"blend:{Blend})";
        }
    }
}