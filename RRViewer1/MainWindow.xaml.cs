using System;
using System.IO;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
//
using oForms = System.Windows.Forms;
using RosReestrImp.Rule;
using RosReestrImp.Data;

namespace RRViewer1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        RuleManager wRM;
        List<Layer> wData = null;

        public MainWindow()
        {
            InitializeComponent();
            wRM = new RuleManager("rule.xml");
        }

        private void FillTbl(DataGrid wDTable, Layer l)
        {
            wDTable.ItemsSource = new MyLayerDataView(l);
            wDTable.ColumnWidth = 100;
        }

        private void MenuItemOpenFile_Click(object sender, RoutedEventArgs e)
        {
            oForms.OpenFileDialog wOpenFileDialog = new oForms.OpenFileDialog();
            TabItem wTab;
            DataGrid wDTable;
            if (wOpenFileDialog.ShowDialog() == oForms.DialogResult.OK)
            {
                try
                {
                    wData = wRM.LoadData(wOpenFileDialog.FileName);
                    if (wData != null)
                    {
                        foreach (Layer l in wData)
                        {
                            wTab = new TabItem();
                            wTab.Header = l.Name;
                            wDTable = new DataGrid();
                            //wDTable.ItemsSource = l.Table;
                            FillTbl(wDTable, l);
                            wTab.Content = wDTable;
                            TabControl1.Items.Add(wTab);
                        }
                        TabControl1.Items.Refresh();
                    }

                }
                catch (Exception exc)
                {
                    oForms.MessageBox.Show(exc.Message);
                }
            }
        }

        private void MenuItemSaveCSV_Click(object sender, RoutedEventArgs e)
        {
            if (wData != null)
            {
                oForms.SaveFileDialog wSFD = new oForms.SaveFileDialog();
                if (wSFD.ShowDialog() == oForms.DialogResult.OK)
                {
                    string ph = wSFD.FileName;
                    //ph = System.IO.Path.GetFullPath(ph) + "\\" + System.IO.Path.GetFileNameWithoutExtension(ph);
                    wData.ForEach(l => File.WriteAllText(ph + "_" + l.Name + ".csv", l.GetCSV()));
                }
            }
        }

    }
}
