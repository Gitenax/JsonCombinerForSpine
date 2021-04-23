using System;
using System.IO;
using System.Text;
using System.Windows;
using Сombine;
using Сombine.Components;
using Сombine.Exceptions;
using MessageBox = System.Windows.MessageBox;

namespace JSON_Spine_Combine
{
    public class Combiner
    {
        private SpineDocument _originalJson;
        private SpineDocument _targerJson;
        
        
        public event Action CorrectLoaded;
        public event Action<Slot[]> SlotsLoaded;
        
        
        public string FirstJsonPath { get; set; }

        public string SecondJsonPath { get; set; }

        public string DestinationPath { get; set; }

        public bool CopyAnimation { get; set; } = true;
        
        public bool CompressJson { get; set; } = true;



        public void LoadOriginalJson(string path)
        {
            _originalJson = FileManager.ReadJsonFile(path);

        }

        public void LoadTargetJson(string path)
        {
            _targerJson = FileManager.ReadJsonFile(path);

        }

        public bool CheckDocuments()
        {
            try
            {
                if (_originalJson == null || _targerJson == null) return false;
                if (DocumentConsistencyCheck(_originalJson, _targerJson))
                {
                    CorrectLoaded?.Invoke();
                    return true;
                }
            }
            catch (ArgumentNullException e)        { ShowError("Путь к файлу не определен!"); }
            catch (MissingTiedVerticesException e) { ShowError(e.Message); }
            catch (VertexConnectionException e)    { ShowError(e.Message); }
            catch (SlotConsistencyException e)     { ShowError(e.Message); }
            catch (Exception e)                    { ShowError(e.Message + "\n" + e.StackTrace, e.Source); }

            return false;
        }
        
        public void Combine(Slot[] selectedSlots)
        {
            try
            {
                AssignDataToOtherJson(_originalJson, _targerJson, selectedSlots);

                SaveFile(_targerJson);

                MessageBox.Show("Готово!", "Операция завершена", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (ArgumentNullException e)        { ShowError("Путь к файлу не определен!"); }
            catch (MissingTiedVerticesException e) { ShowError(e.Message); }
            catch (NoModifySlotException e)        { ShowError(e.Message); }
            catch (VertexConnectionException e)    { ShowError(e.Message); }
            catch (SlotConsistencyException e)     { ShowError(e.Message); }
            catch (Exception e)                    { ShowError(e.Message + "\n" + e.StackTrace, e.Source); }
        }

        
        private void CheckPathStrings()
        {
            if (string.IsNullOrEmpty(DestinationPath) 
                || string.IsNullOrEmpty(FirstJsonPath) 
                || string.IsNullOrEmpty(SecondJsonPath))
            {
                throw new ArgumentNullException();
            }
        }
        


        private bool DocumentConsistencyCheck(SpineDocument first, SpineDocument second)
        {
            // Проверка на наличие костей и связанных вершин в первом json
            if (!first.HasTiedBones)
                throw new MissingTiedVerticesException($"Данный Json не имеет связанных вершин с костями");
            
            // Проверка на отсутствие связанных вершин во втором json
            if (second.HasTiedBones)
                throw new VertexConnectionException("Целевой Json уже имеет связанные вершины!");
            
            // Проверка наличия одинаковых слотов и соответствующих атачментов
            if (first.CompareSlots(second.Slots, out Slot[] identical))
                SlotsLoaded?.Invoke(identical);
            else
                throw new SlotConsistencyException("Отсутствуют одинаковые слоты между Json файлами!");
            
            return true;
        }

        private void AssignDataToOtherJson(SpineDocument from, SpineDocument to, Slot[] selectedSlots)
        {
            if(selectedSlots.Length == 0) 
                throw new NoModifySlotException();
            
            if(CopyAnimation) 
                to.Animations = from.Animations;
            
            from.AssignSlotDataToOther(to, selectedSlots);
        }
        
        private void SaveFile(SpineDocument document)
        {
            JsonWriter writer = new JsonWriter(CompressJson);
            var jsonData = writer.Serialize(document);

            using (var fileStream = new FileStream(FileManager.DestinationPath, FileMode.Create))
            {
                byte[] data = new UTF8Encoding(true).GetBytes(jsonData);
                fileStream.Write(data, 0, data.Length);
            }
        }

        private void ShowError(string message)
        {
            ShowError(message, "Ошибка");
        }

        private void ShowError(string message, string title)
        {
            MessageBox.Show(message, $"Ошибка: {title}", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}