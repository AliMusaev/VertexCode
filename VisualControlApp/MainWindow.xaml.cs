using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using VertexCodeMakerDomain;
using System.Windows.Data;
using System.Windows.Media;

namespace VisualControlApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window , INotifyPropertyChanged
    {
        private OutputBuilder _oBuilder;
        private List<PresentVertexSection> _sections;
        public event PropertyChangedEventHandler PropertyChanged;
        private string _cabinName;
        private string _frameName;
        public MainWindow(VertexFrame frame, List<ErrorMessage> errors)
        {
            InitializeComponent();
            VertexSectionAdapter adapter = new VertexSectionAdapter();
            _sections = adapter.AdaptData(frame, errors);
            _frameName = frame.FrameName;
            _cabinName = frame.CabinName;

            AdditionalRules ar = new AdditionalRules();
            _sections = ar.AddAddintionalRulesInSections(_sections);


            SetHighestAndLowerSections();
            FrameNameField.Text = _frameName;
            CabinNameField.Text = _cabinName;

            PlotModel.Model = new OxyChartBuilder().BuildOxyModel(_sections);
            
            sectionTable.ItemsSource = _sections;
            sectionTable.AutoGenerateColumns = false;
          
            _oBuilder = new OutputBuilder();
        }
        private void SetHighestAndLowerSections()
        {
            // Set flag in section with max height and min height. Ignore sections with width 50 - FIL section
            _sections.Find(x => x.Width == _sections.Max(y => y.Width)).Type = "Higher";
            var val = _sections.Where(z => z.Width > 50).ToList();
            val.Find(x => x.Width == val.Min(y => y.Width)).Type = "Lower";
        }
        private void SectionTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sectionTable.SelectedItem != null)
            {
                var source = sectionTable.SelectedItem as PresentVertexSection;
                if (source!= null)
                {
                    commandTable.ItemsSource = source.CommandsCollection;
                    commandTable.AutoGenerateColumns = false;
                }
                
            }
            
        }
            private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = "txt";
            saveFileDialog.FileName = "TEST";
            saveFileDialog.Filter = "Text files(*.txt)| *.txt | All files(*.*) | *.* ";
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, _oBuilder.Build(_sections, _cabinName, _frameName));
            }
        }
         
        private void CabinNameField_SourceUpdated(object sender, DataTransferEventArgs e)
        {

        }
        protected void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private void FrameNameField_TextChanged(object sender, TextChangedEventArgs e)
        {
            _frameName = FrameNameField.Text;
        }

        private void CabinNameField_TextChanged(object sender, TextChangedEventArgs e)
        {
            _cabinName = CabinNameField.Text;
        }

        private void unlockButton_Click(object sender, RoutedEventArgs e)
        {
            if (sectionTable.IsReadOnly && commandTable.IsReadOnly)
            {
                unlockButton.Content = "Lock Tables";
                sectionTable.IsReadOnly = false;
                commandTable.IsReadOnly = false;
                
            }
            else
            {
                unlockButton.Content = "Unlock Tables";
                sectionTable.IsReadOnly = true;
                commandTable.IsReadOnly = true;
            }
        }
    }
}
