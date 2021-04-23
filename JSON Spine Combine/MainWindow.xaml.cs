using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using Сombine;
using Сombine.Components;
using MessageBox = System.Windows.MessageBox;
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
			_combiner.SlotsLoaded += OnFitstJsonSlotsLoaded;
			
			CopyAnimationCheckBox.Checked   += (sender, args) =>  _combiner.CopyAnimation = true; 
			CopyAnimationCheckBox.Unchecked += (sender, args) =>  _combiner.CopyAnimation = false; 
			CompressJsonCheckBox.Checked    += (sender, args) =>  _combiner.CompressJson  = true; 
			CompressJsonCheckBox.Unchecked  += (sender, args) =>  _combiner.CompressJson  = false;

			
			// FirstJsonTextBox.Text = _combiner.FirstJsonPath = FileManager.FirstPath = @"C:\Users\Gitenax\Desktop\ТЗ\Фикс размеров\Boned.json";
			// SecondJsonTextBox.Text = _combiner.SecondJsonPath = FileManager.SecondPath = @"C:\Users\Gitenax\Desktop\ТЗ\Фикс размеров\Not.json";
			// SaveDirectoryJsonTextBox.Text = _combiner.DestinationPath = FileManager.DestinationPath = @"C:\Users\Gitenax\Desktop\ТЗ\Фикс размеров\Combined.json";
		}

		private void OnFitstJsonSlotsLoaded(Slot[] obj)
		{
			JsonAttachmentsListView.ItemsSource = obj;
		}
		
		private void OnFirstJsonLoadButtonClick(object sender, RoutedEventArgs e)
		{
			FileInfo file = FileManager.ChooseJsonFile();
			if (file == null) return;
			
			FirstJsonTextBox.Text = FileManager.FirstPath = file.FullName;
			ClearListView();
			_combiner.LoadOriginalJson(file.FullName);
			CombineJsonButton.IsEnabled = _combiner.CheckDocuments();
		}
		
		private void OnSecondJsonLoadButtonClick(object sender, RoutedEventArgs e)
		{
			FileInfo file = FileManager.ChooseJsonFile();
			if (file == null) return;
			
			SecondJsonTextBox.Text = FileManager.SecondPath = file.FullName;
			ClearListView();
			_combiner.LoadTargetJson(file.FullName);
			CombineJsonButton.IsEnabled = _combiner.CheckDocuments();
		}

		private void ClearListView()
		{
			JsonAttachmentsListView.ItemsSource = null;
		}
		
		private void OnSelectSaveDirectioryJsonButtonClick(object sender, RoutedEventArgs e)
		{
			var path = FileManager.SetDestinationFile();
			SaveDirectoryJsonTextBox.Text = path;
			FileManager.DestinationPath = path;
		}

		
		private void SelectAllAttachmentsButton_OnClick(object sender, RoutedEventArgs e)
		{
			JsonAttachmentsListView.SelectAll();
		}

		private void DeselectAllAttachmentsButton_OnClick(object sender, RoutedEventArgs e)
		{
			JsonAttachmentsListView.UnselectAll();
		}
		
		private void OnJsonCombineButtonClick(object sender, RoutedEventArgs e)
		{
			var slots = new List<Slot>();
			foreach (Slot slot in JsonAttachmentsListView.SelectedItems)
				slots.Add(slot);
			
			_combiner.Combine(slots.ToArray());
		}

		private void ShowSelectedItems_OnClick(object sender, RoutedEventArgs e)
		{
			var names = new List<string>();
			foreach (Slot slot in JsonAttachmentsListView.SelectedItems)
			{
				names.Add(slot.Name + " | " + slot.Attachment.Type + " | " + slot.Attachment.Name);
			}
			
			var s = string.Join("\n", names);

			MessageBox.Show(s);
		}
	}
}