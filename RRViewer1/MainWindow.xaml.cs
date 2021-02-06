using System;
using System.IO;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
//
using DotSpatial.Projections;
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
    public partial class MainWindow
    {
        ProjectionInfo _selectProjection;
        readonly RuleManager _workRuleManager;
        List<DataLayer> _workData;

        public MainWindow()
        {
            InitializeComponent();
            _workRuleManager = new RuleManager("rule.xml");
        }

        private void dgrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            var table = sender as DataGrid;
            var itemsSource = ((MyLayerDataView)table?.ItemsSource);

            if (itemsSource == null || itemsSource.Count <= 0) return;
            var instance = (MyRecordView)((MyLayerDataView)table.ItemsSource)[0];
            MyRecordTypeDescriptionProvider.SetProperties(instance);
        }

        private void FillTbl(DataGrid wDTable, DataLayer l)
        {
            wDTable.ItemsSource = new MyLayerDataView(l);
            wDTable.ColumnWidth = 100;
            wDTable.Sorting += dgrid_Sorting;
        }

        private void MenuItemOpenFile_Click(object sender, RoutedEventArgs e)
        {
            var wOpenFileDialog = new oForms.OpenFileDialog();
            if (wOpenFileDialog.ShowDialog() != oForms.DialogResult.OK) return;
            try
            {
                _workData = _workRuleManager.LoadData(wOpenFileDialog.FileName);
                if (_workData == null) return;
                foreach (var l in _workData)
                {
                    var wTab = new TabItem();
                    wTab.Header = l.Name;
                    var wDTable = new DataGrid();
                    //wDTable.ItemsSource = l.Table;
                    FillTbl(wDTable, l);
                    wTab.Content = wDTable;
                    TabControl1.Items.Add(wTab);
                }
                TabControl1.Items.Refresh();
            }
            catch (Exception exc)
            {
                oForms.MessageBox.Show(exc.Message);
            }
        }

        private void MenuItemSaveCSV_Click(object sender, RoutedEventArgs e)
        {
            if (_workData == null) return;
            oForms.SaveFileDialog workSaveFileDialog = new oForms.SaveFileDialog();
            if (workSaveFileDialog.ShowDialog() == oForms.DialogResult.OK)
            {
                var ph = workSaveFileDialog.FileName;
                //ph = System.IO.Path.GetFullPath(ph) + "\\" + System.IO.Path.GetFileNameWithoutExtension(ph);
                _workData.ForEach(l => File.WriteAllText(ph + "_" + l.Name + ".csv", l.GetCSV()));
            }
        }

        private void MenuItemSaveTAB_Click(object sender, RoutedEventArgs e)
        {
            if (_workData == null) return;
            oForms.SaveFileDialog workSaveFileDialog = new oForms.SaveFileDialog();
            if (workSaveFileDialog.ShowDialog() == oForms.DialogResult.OK)
            {
                var ph = workSaveFileDialog.FileName;
                foreach (DataLayer l in _workData)
                {
                    MiLayer.CreateTab(ph + "_" + l.Name + ".tab", l);
                }
            }
        }

        private void MenuItemSaveMIF_Click(object sender, RoutedEventArgs e)
        {
            if (_workData == null) return;
            oForms.SaveFileDialog workSaveFileDialog = new oForms.SaveFileDialog();
            if (workSaveFileDialog.ShowDialog() == oForms.DialogResult.OK)
            {
                var ph = workSaveFileDialog.FileName;
                foreach (DataLayer l in _workData)
                {
                    MiLayer.CreateMif(ph + "_" + l.Name + ".mif", l);
                }
            }
        }

        private void MI_Projection_Click(object sender, RoutedEventArgs e)
        {
            var form = new SelectProjectionWindow();
            form.ShowDialog();
            _selectProjection = form.SelectProjection;
            if (_selectProjection != null) MI_Kml.Visibility = Visibility.Visible;
        }

        private void MI_Kml_Click(object sender, RoutedEventArgs e)
        {
            if (_selectProjection == null) return;
            if (_workData == null) return;
            var workSaveFileDialog = new oForms.SaveFileDialog();
            if (workSaveFileDialog.ShowDialog() == oForms.DialogResult.OK)
            {
                var ph = workSaveFileDialog.FileName;
                foreach (DataLayer l in _workData)
                {
                    //var doc = l.GetKmlDocument(SelectProjection);
                    var kmlConverter = new KmlConverter(l, _selectProjection);
                    var kml = KmlFile.Create(kmlConverter.GetKmlDocument(), false);
                    using (var stream = File.OpenWrite(ph + "_" + l.Name + ".kml"))
                    {
                        kml.Save(stream);
                    }
                }
            }
        }
    }
}