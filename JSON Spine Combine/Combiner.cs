using System;
using System.IO;
using System.Text;
using System.Windows;
using Сombine;
using Сombine.Exceptions;
using MessageBox = System.Windows.MessageBox;

namespace JSON_Spine_Combine
{
    public class Combiner
    {
        public string FirstJsonPath { get; set; } = @"C:\Users\Gitenax\Desktop\ТЗ\Фикс размеров\Boned.json";

        public string SecondJsonPath { get; set; } = @"C:\Users\Gitenax\Desktop\ТЗ\Фикс размеров\Not.json";

        public string DestinationPath { get; set; } = @"C:\Users\Gitenax\Desktop\ТЗ\Фикс размеров\combined.json";

        public bool CopyAnimation { get; set; } = true;
        
        public bool CompressJson { get; set; } = true;

        
        public void Combine()
        {
            try
            {
                CheckPathStrings();
                
                var firstJson = ReadJsonFile(FirstJsonPath);
                var secondJson = ReadJsonFile(SecondJsonPath);

                
                DocumentConsistencyCheck(firstJson, secondJson);

                AssignDataToOtherJson(firstJson, secondJson);

                SaveFile(secondJson);

                MessageBox.Show("Готово!", "Операция завершена", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (ArgumentNullException e)
            {
                ShowError("Путь к файлу не определен!");
            }
            catch (MissingTiedVerticesException e)
            {
                ShowError(e.Message);
            }
            catch (VertexConnectionException e)
            {
                ShowError(e.Message);
            }
            catch(SlotConsistencyException e)
            {
                ShowError(e.Message);
            }
            catch (Exception e)
            {
                ShowError(e.Message + "\n" + e.StackTrace, e.Source);
            }
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
        
        private SpineDocument ReadJsonFile(string path)
        {
            SpineDocument document = default;
            try
            {
                document = new JsonReader(path).Deserialize();
            }
            catch (FileNotFoundException e)
            {
                MessageBox.Show("Указанного Json файла не существует по указанному пути!\n" +
                                $"{path}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return document;
        }

        private void DocumentConsistencyCheck(SpineDocument first, SpineDocument second)
        {
            // Проверка на наличие костей и связанных вершин в первом json
            if (!first.HasTiedBones)
                throw new MissingTiedVerticesException($"Данный Json не имеет связанных вершин с костями");
            
            // Проверка на отсутствие связанных вершин во втором json
            if (second.HasTiedBones)
                throw new VertexConnectionException("Целевой Json уже имеет связанные вершины!");
            
            // Проверка наличия одинаковых слотов и соответствующих атачментов
            if (first.CompareSlots(second.Slots) == false)
                throw new SlotConsistencyException("Не соответствие слотов(и атачментов) между Json файлами!");
        }

        private void AssignDataToOtherJson(SpineDocument from, SpineDocument to)
        {
            if(CopyAnimation) 
                to.Animations = from.Animations;
            
            from.AssignSlotDataToOther(to);
        }
        
        private void SaveFile(SpineDocument document)
        {
            JsonWriter writer = new JsonWriter(CompressJson);
            var jsonData = writer.Serialize(document);

            using (var fileStream = new FileStream(DestinationPath, FileMode.Create))
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