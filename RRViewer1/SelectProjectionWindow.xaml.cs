using System.Windows;
//
using DotSpatial.Projections;

using RosReestrImp.Projections;

namespace RRViewer1
{
    /// <summary>Логика взаимодействия для Window1.xaml</summary>
    public partial class SelectProjectionWindow : Window
    {
        Projections Projections;
        public ProjectionInfo SelectProjection = null;

        public SelectProjectionWindow()
        {
            InitializeComponent();
            Projections = Projections.Load();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var SKCode = TB_SKName.Text;
            int Zone;
            if (int.TryParse(TB_Zone.Text, out Zone))
            {
                SelectProjection = Projections.GetProjectionInfo(SKCode, Zone);
                TB_SelectProjection.Text = SelectProjection.ToProj4String();
            }
        }
    }
}