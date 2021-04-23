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
using Сombine.Units;
using Path = Сombine.Components.Attachments.Path;

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
                Skeleton skeletonObj = JsonSerializer.Deserialize<Skeleton>(GetRaw(_skeletonElement),   _options);
                Bone[]   boneArray   = JsonSerializer.Deserialize<Bone[]>  (GetRaw(_bonesElement),      _options);
                Slot[]   slots       = JsonSerializer.Deserialize<Slot[]>  (GetRaw(_slotsElement),      _options);
                Skin[]   skins       = JsonSerializer.Deserialize<Skin[]>  (GetRaw(_skinsElement),      _options);
                object   animations  = JsonSerializer.Deserialize<object>  (GetRaw(_animationsElement), _options);

                // Подстройка компонентов
                BoneResolver.AssignParentToBones(boneArray);
                SlotResolver.AssignBonesToSlots(boneArray, slots);
                CompileAttachmentForSlotsFromSkins(skins, slots);
                VerifyAttachmentInSlots(slots, boneArray);
                
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
            
            List<Slot> slotsForSingleSkin = new List<Slot>();
            List<Slot> slotsWithoutExplicitAttachments = new List<Slot>();
            
            for (int i = 0; i < skinsLenght; i++)
            {
                var currentSkinElement = _skinsElement[i];
                var attachmentsElement = currentSkinElement.GetProperty("attachments");
                
                // Перебор всех слотов в текущем скине
                for (int j = 0; j < slotsLenght; j++)
                {
                    var slotName = _slotsElement[j].GetProperty("name").GetString(); // Имя слота из скина
                    var slot = slots.First(x => x.Name == slotName); // Имя слота из "слотов" с проверкой имени из скина
                    slotsForSingleSkin.Add(slot); 
                    
                    // Проверка на наличия у слота атрибута "attachment"
                    if(_slotsElement[j].TryGetProperty("attachment", out JsonElement value))
                    {
                        var attachmentName = value.GetString();
                        var slotElement = attachmentsElement.GetProperty(slotName);
                        var attachmentElement = slotElement.GetProperty(attachmentName);
                        var compiled = CompileAttachment(attachmentElement);
                       
                        if (compiled != null)
                        {
                            compiled.Name = attachmentName;
                            AssignAttachmentToSlot(slot, compiled);
                        }
                    }
                    else
                        slotsWithoutExplicitAttachments.Add(slot); // Слот без явно указанного атачмента(для последующего поиска)
                }
                
                // Поиск слотов без явно указанных атачментов
                foreach (var slot in slotsWithoutExplicitAttachments)
                {
                    var rawAttachment = currentSkinElement.GetProperty("attachments").GetProperty(slot.Name);
                    var name = GetAttachmentName(rawAttachment.GetRawText());
                    var attachmentElement = rawAttachment.GetProperty(name);
                    var attachment = CompileAttachment(attachmentElement);
                    attachment.Name = name;
                    AssignAttachmentToSlot(slot, attachment);
                }
                
                slotsForSingleSkin.AddRange(slotsWithoutExplicitAttachments);
                skins[i].Attachments = slotsForSingleSkin.ToArray();
                slotsForSingleSkin.Clear();
            }
        }

        private Attachment CompileAttachment(JsonElement attachmentElement)
        {
            var raw = attachmentElement.GetRawText();

            if (attachmentElement.TryGetProperty("type", out JsonElement value))
            { 
                switch (value.GetString())
                {
                    case "mesh":        return JsonSerializer.Deserialize<Mesh>       (raw, _options);
                    case "boundingbox": return JsonSerializer.Deserialize<BoundingBox>(raw, _options);
                    case "point":       return JsonSerializer.Deserialize<Point>      (raw, _options);
                    case "clipping":    return JsonSerializer.Deserialize<Clipping>   (raw, _options);
                    case "linkedmesh":  return JsonSerializer.Deserialize<LinkedMesh> (raw, _options);
                    case "path":        return JsonSerializer.Deserialize<Path>       (raw, _options);
                }
            }
            return JsonSerializer.Deserialize<Region>(raw, _options);
        }
        
        private void AssignAttachmentToSlot(Slot slot, Attachment attachment)
        {
            AttachmentResolver.VerifyAttachmentProperties(attachment);
            slot.Attachment = attachment;
        }
        
        private string GetAttachmentName(string rawString)
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
        
        private void VerifyAttachmentInSlots(Slot[] slots, Bone[] bones)
        {
            foreach (var slot in slots)
            {
                if (slot.Attachment is IVertexIncludingAttachment mesh)
                    AssignParentToWeightedVertices(mesh, bones);
            }
        }

        private void AssignParentToWeightedVertices(IVertexIncludingAttachment attachment, Bone[] bones)
        {
            foreach (VertexNode node in attachment.VertexCollection)
            {
                for (var i = 0; i < node.Count; i++)
                    if (node[i].Connected)
                    {
                        node[i] = new Vertex(node[i].BoneIndex, node[i].X, node[i].Y, node[i].Weight,
                            BoneResolver.FindByIndex(bones, node[i].BoneIndex));
                    }
            }
        }
    }
}
