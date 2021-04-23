using System.IO;
using System.Windows;
using System.Windows.Forms;
using Сombine;
using MessageBox = System.Windows.MessageBox;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;
using XDialogResult = System.Windows.Forms.DialogResult;



namespace JSON_Spine_Combine
{
    public static class FileManager
    {
        public static string FirstPath {get; set;} 
        public static string SecondPath {get; set;} 
        public static string DestinationPath {get; set;} 
        
        public static void SetPaths(string p1, string p2, string p3)
        {
            FirstPath = p1;
            SecondPath = p2;
            DestinationPath = p3;
        }
        
        public static FileInfo ChooseJsonFile()
        {
            var dialog = new OpenFileDialog();
            var path1 = string.IsNullOrEmpty(FirstPath) ? null : new FileInfo(FirstPath).DirectoryName;
            var path2 = string.IsNullOrEmpty(SecondPath) ? null : new FileInfo(SecondPath).DirectoryName;
            
            dialog.InitialDirectory = path1 ?? path2 ?? Directory.GetCurrentDirectory();
            dialog.Filter = "Файл Json(*.json)|*.json";
			
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return new FileInfo(dialog.FileName);
            }
            return null;
        }

        public static string SetDestinationFile()
        {
            using (var dialog = new SaveFileDialog())
            {
                var path1 = string.IsNullOrEmpty(FirstPath) ? null : new FileInfo(FirstPath).DirectoryName;
                var path2 = string.IsNullOrEmpty(SecondPath) ? null : new FileInfo(SecondPath).DirectoryName;
                dialog.InitialDirectory = path2 ?? path1 ?? Directory.GetCurrentDirectory();
                dialog.Filter = "Файл Json(*.json)|*.json";
                dialog.FileName = "combined.json";
				
                DialogResult result = dialog.ShowDialog();

                if (result == XDialogResult.OK)
                    return dialog.FileName;
            }

            return null;
        }
        
        public static SpineDocument ReadJsonFile(string path)
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
    }
}