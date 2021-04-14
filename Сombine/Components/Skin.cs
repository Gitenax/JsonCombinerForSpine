/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 *  Раздел "скины" может быть опущен, если нет скинов или вложений.
 *  
 *  Каждый скин - это, по сути, карта с составным ключом, состоящим из слота и имени вложения, 
 *  а значение-это вложение. Имя вложения, используемое в ключе, одинаково для каждого скина, 
 *  но фактическое имя вложения для вложения может отличаться.
 *  
 *  Например, изображение "таза" для слота "таз" может иметь фактическое имя вложения 
 *  "красный/таз" для "красной" кожи и "синий/таз" для "синей" кожи. Если скелет имеет 
 *  активную "синюю" кожу и ему говорят показать изображение "таза" для слота "таз", 
 *  то найденное им вложение будет иметь фактическое имя вложения "синий/таз".
 *  
 *  Когда скелету нужно найти крепление для щели, он сначала проверяет свою кожу.
 *  Если он не найден, то скелет проверяет свою кожу по умолчанию. Кожа по умолчанию содержит вложения, 
 *  которые не определяются кожей в Позвоночнике, что позволяет смешивать 
 *  использование вложений кожи и не-кожи. Скин по умолчанию всегда имеет имя "default".
 *  
 *  Скины, отличные от скина по умолчанию, могут иметь кости и/или ограничения:
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Сombine.Components
{
    /// <summary>
    /// Описывает вложения, которые могут быть назначены каждому слоту.
    /// </summary>
    public class Skin
    {
        private string _attachmentsName;
        private object _attachmentObject;
        
        [JsonConstructor]
        public Skin(string name)
        {
            Name = name;
            //_attachmentsName = attachments;
        }
        
        public Skin(string name, Slot[] attachments = null, object attachmentsDictionary = null)
        {
            Name = name;
            Attachments = attachments;
            AttachmentsDictionary = attachmentsDictionary;
        }
        
        public string Name { get; set; }
        
        [JsonIgnore]
        public Slot[] Attachments { get; set; }
        
        [JsonPropertyName("attachments")]
        public object AttachmentsDictionary 
        {
            get => AssignSlotAttachment();
            set => _attachmentObject = value;
        }

        private object AssignSlotAttachment()
        {
            var slotObject = new Dictionary<string, object>();
            foreach (var slot in Attachments)
            {
                var attachmentObject = new Dictionary<string, object>();
                attachmentObject.Add(slot.Attachment.Name, slot.Attachment);
                slotObject.Add(slot.Name, attachmentObject);
            }

            return slotObject;
        }
    }
}