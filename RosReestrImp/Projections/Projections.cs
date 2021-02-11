using DotSpatial.Projections;

using System.Collections.Generic;
using System.IO;

namespace RosReestrImp.Projections
{
    /// <summary>Проекции</summary>
    public class Projections
    {
        readonly Dictionary<(string code, int zone), string> _projectionsDescription = new Dictionary<(string code, int zone), string>();

        /// <summary>Загружаем</summary>
        /// <returns></returns>
        public static Projections Load()
        {
            var res = new Projections();
            var lines = File.ReadAllLines("sk_list.txt");
            foreach (var line in lines)
            {
                var args = line.Split(';');
                var key = (args[0], int.Parse(args[1]));
                if (!res._projectionsDescription.ContainsKey(key)) 
                    res._projectionsDescription.Add((args[0], int.Parse(args[1])), args[2]);
            }
            return res;
        }

        /// <summary>Получить описание проекции</summary>
        /// <param name="code"></param>
        /// <param name="zone"></param>
        /// <returns></returns>
        public ProjectionInfo GetProjectionInfo(string code, int zone)
        {
            var key = (code, zone);
            return _projectionsDescription.ContainsKey(key) ? ProjectionInfo.FromProj4String(_projectionsDescription[key]) : null;
        }
    }
}