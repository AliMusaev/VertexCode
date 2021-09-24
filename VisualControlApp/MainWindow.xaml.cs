using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using VertexCodeMakerDomain;

namespace VisualControlApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OutputBuilder _oBuilder;
        private VertexFrame _frame;
        public MainWindow(VertexFrame frame)
        {
            InitializeComponent();
            _frame = frame;
            SectionList.ItemsSource = _frame.GetSections();
            PlotModel.Model = new OxyChartBuilder().BuildOxyModel(frame);
            _oBuilder = new OutputBuilder();
        }

        private void SectionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SectionList.SelectedItem != null)
            {
                var source = SectionList.SelectedItem as VertexSection;
                CommandList.ItemsSource = source.CommandsCollection;
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
                File.WriteAllText(saveFileDialog.FileName, _oBuilder.Build(_frame));
            }
        }
    }
}
