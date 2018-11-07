using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RosReestrImp.Geometry
{
    public class TMBR
    {

        public double minx { get; private set; }
        public double miny { get; private set; }
        public double maxx { get; private set; }
        public double maxy { get; private set; }

        public TMBR(double x1, double y1, double x2, double y2)
        {
            this.minx = Math.Min(x1, x2);
            this.miny = Math.Min(y1, y2);
            this.maxx = Math.Max(x1, x2);
            this.maxy = Math.Max(y1, y2);
        }

        public TMBR(MyPoint p1)
        {
            this.minx = p1.X;
            this.miny = p1.Y;
            this.maxx = p1.X;
            this.maxy = p1.Y;
        }

        public TMBR(MyPoint p1, MyPoint p2)
        {
            this.minx = Math.Min(p1.X, p2.X);
            this.miny = Math.Min(p1.Y, p2.Y);
            this.maxx = Math.Max(p1.X, p2.X);
            this.maxy = Math.Max(p1.Y, p2.Y);
        }

        public void AddPoint(MyPoint p)
        {
            this.minx = Math.Min(this.minx, p.X);
            this.miny = Math.Min(this.miny, p.Y);
            this.maxx = Math.Max(this.maxx, p.X);
            this.maxy = Math.Max(this.maxy, p.Y);
        }

        public void AddMBR(TMBR nmbr)
        {
            if (nmbr != null)
            {
                this.minx = Math.Min(this.minx, nmbr.minx);
                this.miny = Math.Min(this.miny, nmbr.miny);
                this.maxx = Math.Max(this.maxx, nmbr.maxx);
                this.maxy = Math.Max(this.maxy, nmbr.maxy);
            }
        }

    }
}
