using System;
using System.IO;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
//
using DotSpatial.Projections;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;
//
using oForms = System.Windows.Forms;
using RosReestrImp.Rule;
using RosReestrImp.Data;
using MITAB;
using RRViewer1.kml;

namespace RRViewer1
{
    /// <summary>Логика взаимодействия для MainWindow.xaml</summary>
    public partial class MainWindow : Window
    {
        ProjectionInfo SelectProjection = null;
        RuleManager wRM;
        List<DataLayer> wData = null;

        public MainWindow()
        {
            InitializeComponent();
            wRM = new RuleManager("rule.xml");
        }

        private void dgrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            var table = sender as DataGrid;
            var itemsSource = ((MyLayerDataView)table.ItemsSource);

            if (itemsSource.Count > 0)
            {
                var instance = (MyRecordView)((MyLayerDataView)table.ItemsSource)[0];
                MyRecordTypeDescriptionProvider.SetProperties(instance);
            }
        }

        private void FillTbl(DataGrid wDTable, DataLayer l)
        {
            wDTable.ItemsSource = new MyLayerDataView(l);
            wDTable.ColumnWidth = 100;
            wDTable.Sorting += dgrid_Sorting;
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
                        foreach (DataLayer l in wData)
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

        private void MenuItemSaveTAB_Click(object sender, RoutedEventArgs e)
        {
            if (wData != null)
            {
                oForms.SaveFileDialog wSFD = new oForms.SaveFileDialog();
                if (wSFD.ShowDialog() == oForms.DialogResult.OK)
                {
                    string ph = wSFD.FileName;
                    foreach (DataLayer l in wData)
                    {
                        MiLayer.CreateTab(ph + "_" + l.Name + ".tab", l);
                    }
                }
            }
        }

        private void MenuItemSaveMIF_Click(object sender, RoutedEventArgs e)
        {
            if (wData != null)
            {
                oForms.SaveFileDialog wSFD = new oForms.SaveFileDialog();
                if (wSFD.ShowDialog() == oForms.DialogResult.OK)
                {
                    string ph = wSFD.FileName;
                    foreach (DataLayer l in wData)
                    {
                        MiLayer.CreateMif(ph + "_" + l.Name + ".mif", l);
                    }
                }
            }
        }

        private void MI_Projection_Click(object sender, RoutedEventArgs e)
        {
            var form = new SelectProjectionWindow();
            form.ShowDialog();
            SelectProjection = form.SelectProjection;
            if (SelectProjection != null) MI_Kml.Visibility = Visibility.Visible;
        }

        private void MI_Kml_Click(object sender, RoutedEventArgs e)
        {
            if (SelectProjection == null) return;
            if (wData != null)
            {
                oForms.SaveFileDialog wSFD = new oForms.SaveFileDialog();
                if (wSFD.ShowDialog() == oForms.DialogResult.OK)
                {
                    string ph = wSFD.FileName;
                    foreach (DataLayer l in wData)
                    {
                        //var doc = l.GetKmlDocument(SelectProjection);
                        var kmlc = new KmlConverter(l, SelectProjection);
                        KmlFile kml = KmlFile.Create(kmlc.GetKmlDocument(), false);
                        using (var stream = File.OpenWrite(ph + "_" + l.Name + ".kml"))
                        {
                            kml.Save(stream);
                        }
                    }
                }
            }
        }
    }
}