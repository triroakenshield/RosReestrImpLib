using System;
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace RosReestrImp.Geometry
{
    /// <summary>Минимальный ограничивающий прямоугольник</summary>
    public class TMBR
    {
        /// <summary>нижний левый угол (x)</summary>
        public double minx { get; private set; }

        /// <summary>нижний левый угол (y)</summary>
        public double miny { get; private set; }

        /// <summary>верхний правый угол (x)</summary>
        public double maxx { get; private set; }

        /// <summary>верхний правый угол (y)</summary>
        public double maxy { get; private set; }

        /// <summary>Конструктор из координат</summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public TMBR(double x1, double y1, double x2, double y2)
        {
            this.minx = Math.Min(x1, x2);
            this.miny = Math.Min(y1, y2);
            this.maxx = Math.Max(x1, x2);
            this.maxy = Math.Max(y1, y2);
        }

        /// <summary>Конструктор из точки</summary>
        /// <param name="p1"></param>
        public TMBR(MyPoint p1)
        {
            this.minx = p1.X;
            this.miny = p1.Y;
            this.maxx = p1.X;
            this.maxy = p1.Y;
        }

        /// <summary>Конструктор по двум точкам</summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        public TMBR(MyPoint p1, MyPoint p2)
        {
            this.minx = Math.Min(p1.X, p2.X);
            this.miny = Math.Min(p1.Y, p2.Y);
            this.maxx = Math.Max(p1.X, p2.X);
            this.maxy = Math.Max(p1.Y, p2.Y);
        }

        /// <summary>Добавить точку</summary>
        /// <param name="p"></param>
        public void AddPoint(MyPoint p)
        {
            this.minx = Math.Min(this.minx, p.X);
            this.miny = Math.Min(this.miny, p.Y);
            this.maxx = Math.Max(this.maxx, p.X);
            this.maxy = Math.Max(this.maxy, p.Y);
        }

        /// <summary>Добавить MBR</summary>
        /// <param name="nmbr"></param>
        public void AddMBR(TMBR nmbr)
        {
            if (nmbr == null) return;
            this.minx = Math.Min(this.minx, nmbr.minx);
            this.miny = Math.Min(this.miny, nmbr.miny);
            this.maxx = Math.Max(this.maxx, nmbr.maxx);
            this.maxy = Math.Max(this.maxy, nmbr.maxy);
        }

        public bool Contains(TMBR r)
        {
            if (maxx < r.maxx || minx > r.minx || maxy < r.maxy || miny > r.miny) return false;
            return true;
        }
    }
}