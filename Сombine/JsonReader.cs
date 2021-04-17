using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Сombine.Components;
using Сombine.Components.Attachments;
using Сombine.Resolvers;

namespace Сombine
{
    /// <summary>
    /// Класс для чтения Json файла данных Spine
    /// </summary>
    public class JsonReader
    {
        private string _filePath;
        private JsonSerializerOptions _options;
        private JsonDocument _jsonDocument;
        private JsonElement _root;
        private JsonElement _skeletonElement;
        private JsonElement _bonesElement;
        private JsonElement _slotsElement;
        private JsonElement _skinsElement;
        private JsonElement _animationsElement;
        
        
        public JsonReader(string path)
        {
            _filePath = path;
            _options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                },
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            };
        }
        
        public SpineDocument Deserialize()
        {
            using (_jsonDocument = JsonDocument.Parse(ReadFile()))
            {
                AssignJsonElements();
                

                // Десериализация
                Skeleton skeletonObj = JsonSerializer.Deserialize<Skeleton>(GetRaw(_skeletonElement), _options);
                Bone[] boneArray     = JsonSerializer.Deserialize<Bone[]>(GetRaw(_bonesElement), _options);
                Slot[] slots         = JsonSerializer.Deserialize<Slot[]>(GetRaw(_slotsElement), _options);
                Skin[] skins         = JsonSerializer.Deserialize<Skin[]>(GetRaw(_skinsElement), _options);
                object animations    = JsonSerializer.Deserialize<object>(GetRaw(_animationsElement), _options);

                // Подстройка компонентов
                BoneResolver.AssignParentToBones(boneArray);
                SlotResolver.AssignBonesToSlots(boneArray, slots);
                CompileAttachmentForSlotsFromSkins(skins, slots);

                return new SpineDocument(skeletonObj, boneArray, slots, skins, animations);;
            }
        }



        private string ReadFile()
        {
            return File.ReadAllText(_filePath);
        }

        private void AssignJsonElements()
        {
            if(_jsonDocument == null) return;
            
            _root              = _jsonDocument.RootElement;
            _skeletonElement   = _root.GetProperty("skeleton");
            _bonesElement      = _root.GetProperty("bones");
            _slotsElement      = _root.GetProperty("slots");
            _skinsElement      = _root.GetProperty("skins");
            _animationsElement = _root.TryGetProperty("animations", out JsonElement value) ? value : default;
        }

        private string GetRaw(JsonElement element)
        {
            return element.GetRawText();
        }

        private void CompileAttachmentForSlotsFromSkins(Skin[] skins, Slot[] slots)
        {
            int skinsLenght = skins.Length;
            int slotsLenght = slots.Length;

            List<Attachment> attachments = new List<Attachment>();
            List<Slot> slotsForSingleSkin = new List<Slot>();
            List<Slot> slotsWithoutExplicitAttachments = new List<Slot>();
            
            for (int i = 0; i < skinsLenght; i++)
            {
                var currentSkin = _skinsElement[i];
                var attachmentsElement = currentSkin.GetProperty("attachments");
                
                
                for (int j = 0; j < slotsLenght; j++)
                {
                    var slotName = _slotsElement[j].GetProperty("name").GetString();
                    var slot = slots.First(x => x.Name == slotName);
                    slotsForSingleSkin.Add(slot);
                    
                    if(_slotsElement[j].TryGetProperty("attachment", out JsonElement value))
                    {
                        var attachmentName = value.GetString();
                        
                        var slotElement = attachmentsElement.GetProperty(slotName);
                        var attachmentElement = slotElement.GetProperty(attachmentName);
                        
                        
                        var compiled = CompileAttachment(attachmentElement);
                        if (compiled != null)
                        {
                            compiled.Name = attachmentName;
                        
                            if(attachments.Exists((x) => x.Name == compiled.Name) == false)
                            {
                                AssignAttachmentToSlot(slot, compiled);
                                attachments.Add(compiled);
                            }
                        }
                    }
                    else
                        slotsWithoutExplicitAttachments.Add(slot);
                }
                AssignSlotsToSkin(skins[i], slotsForSingleSkin.ToArray());
                slotsForSingleSkin.Clear();
            }
            
            // Find Implicit attachments
            foreach (var slot in slotsWithoutExplicitAttachments)
            {
                var rawAttachment = _skinsElement[0].GetProperty("attachments").GetProperty(slot.Name);
                var name = SelectAttachmentName(rawAttachment.GetRawText());
                var attachmentElement = rawAttachment.GetProperty(name);
                var attachment = CompileAttachment(attachmentElement);
                AssignAttachmentToSlot(slot, attachment);
            }
        }

        private string SelectAttachmentName(string rawString)
        {
            var (startIndex, lenght) = (0 , 0);
            bool startCounting = false;
            
            
            for (int i = 0; i < rawString.Length; i++)
            {
                if (rawString[i].Equals('"'))
                {
                    if(startIndex == 0)
                    {
                        startIndex = i + 1;
                        startCounting = true;
                    }
                    else break;
                }
                
                if (startCounting) lenght++;
            }

            return rawString.Substring(startIndex, lenght - 1);
        }
        
        
        private Attachment CompileAttachment(JsonElement attachmentElement)
        {
            var raw = attachmentElement.GetRawText();

            if (attachmentElement.TryGetProperty("type", out JsonElement value))
            {
                var type = value.GetString();
                
                switch (type)
                {
                    case "mesh": 
                        return JsonSerializer.Deserialize<Mesh>(raw, _options);
                    case "boundingbox": 
                        return JsonSerializer.Deserialize<BoundingBox>(raw, _options);
                    default: 
                        return JsonSerializer.Deserialize<Region>(raw, _options);
                }
            }
         
            return JsonSerializer.Deserialize<Region>(raw, _options);
        }

        private void AssignAttachmentToSlot(Slot slot, Attachment attachment)
        {
            AttachmentResolver.VerifyAttachmentProperties(attachment);
            slot.Attachment = attachment;
        }

        private void AssignSlotsToSkin(Skin skin, Slot[] slots)
        {
            skin.Attachments = slots;
        }
    }
}
