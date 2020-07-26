using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
//
using DotSpatial.Projections;

namespace RosReestrImp.Projections
{
    public class Projections
    {
        Dictionary<(string code, int zone), string> ProjectionsDescription = new Dictionary<(string code, int zone), string>();

        public Projections()
        {

        }

        public static Projections Load()
        {
            var res = new Projections();
            var lines = File.ReadAllLines("sk_list.txt");
            foreach (var line in lines)
            {
                var args = line.Split(';');
                var key = (args[0], int.Parse(args[1]));
                if (!res.ProjectionsDescription.ContainsKey(key)) 
                    res.ProjectionsDescription.Add((args[0], int.Parse(args[1])), args[2]);
            }
            return res;
        }

        public ProjectionInfo GetProjectionInfo(string code, int zone)
        {
            var key = (code, zone);
            if (ProjectionsDescription.ContainsKey(key)) return ProjectionInfo.FromProj4String(ProjectionsDescription[key]);
            else return null;                    
        }
    }
}