using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using Сombine;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using XDialogResult = System.Windows.Forms.DialogResult;

namespace JSON_Spine_Combine
{
	public partial class MainWindow
	{
		private Combiner _combiner;
		
		
		public MainWindow()
		{
			InitializeComponent();
			_combiner = new Combiner();
			CopyAnimationCheckBox.Checked += (sender, args) =>
			{
				_combiner.CopyAnimation = true;
			};
			CopyAnimationCheckBox.Unchecked += (sender, args) =>
			{
				_combiner.CopyAnimation = false;
			};
			CompressJsonCheckBox.Checked += (sender, args) =>
			{
				_combiner.CompressJson = true;
			};
			CompressJsonCheckBox.Unchecked += (sender, args) =>
			{
				_combiner.CompressJson = false;
			};

			SelectAllAttachmentsButton.Click += (sender, args) =>
			{

			};
		}
		


		private void OnFirstJsonLoadButtonClick(object sender, RoutedEventArgs e)
		{
			FileInfo file = ChooseJsonFile();
			
			if (file == null) return;
			
			_combiner.FirstJsonPath = file.FullName;
			FirstJsonTextBox.Text = file.FullName;
		}
		
		private void OnSecondJsonLoadButtonClick(object sender, RoutedEventArgs e)
		{
			FileInfo file = ChooseJsonFile();
			
			if (file == null) return;
			
			_combiner.SecondJsonPath = file.FullName;
			SecondJsonTextBox.Text = file.FullName;
		}
		
		private void OnSelectSaveDirectioryJsonButtonClick(object sender, RoutedEventArgs e)
		{
			using (var dialog = new SaveFileDialog())
			{
				dialog.InitialDirectory = _combiner.SecondJsonPath ?? _combiner.FirstJsonPath ?? Directory.GetCurrentDirectory();
				dialog.Filter = "Файл Json(*.json)|*.json";
				dialog.FileName = "combined.json";
				
				DialogResult result = dialog.ShowDialog();

				if (result == XDialogResult.OK)
				{
					_combiner.DestinationPath = dialog.FileName;
					SaveDirectoryJsonTextBox.Text = _combiner.DestinationPath;
				}
			}
		}
		
		private void OnJsonCombineButtonClick(object sender, RoutedEventArgs e)
		{
			_combiner.Combine();
		}

		private FileInfo ChooseJsonFile()
		{
			var dialog = new OpenFileDialog();
			dialog.InitialDirectory = _combiner.FirstJsonPath ?? _combiner.SecondJsonPath ?? Directory.GetCurrentDirectory();
			dialog.Filter = "Файл Json(*.json)|*.json";
			
			if (dialog.ShowDialog() == true)
			{
				return new FileInfo(dialog.FileName);
			}
			return null;
		}

		private string RemoveExtension(string fileName, string extension)
		{
			return fileName.Remove(fileName.Length - extension.Length, extension.Length);
		}

		private void LoadTreeView_OnClick(object sender, RoutedEventArgs e)
		{
			var firstJson = ReadJsonFile(FirstJsonTextBox.Text);


			ObservableCollection<SpineDocument> documents = new ObservableCollection<SpineDocument>
			{
				firstJson
			};


			Dictionary<string, int[]> d = new Dictionary<string, int[]>();
			d.Add("digits", new []{0,1,2,3,4,5,6});


			JsonAttachmentsTreeView.ItemsSource = firstJson.GetTableData();
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
	}
}