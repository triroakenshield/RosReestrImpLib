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
            minx = Math.Min(x1, x2);
            miny = Math.Min(y1, y2);
            maxx = Math.Max(x1, x2);
            maxy = Math.Max(y1, y2);
        }

        /// <summary>Конструктор из точки</summary>
        /// <param name="p1"></param>
        public TMBR(MyPoint p1)
        {
            minx = p1.X;
            miny = p1.Y;
            maxx = p1.X;
            maxy = p1.Y;
        }

        /// <summary>Конструктор по двум точкам</summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        public TMBR(MyPoint p1, MyPoint p2)
        {
            minx = Math.Min(p1.X, p2.X);
            miny = Math.Min(p1.Y, p2.Y);
            maxx = Math.Max(p1.X, p2.X);
            maxy = Math.Max(p1.Y, p2.Y);
        }

        /// <summary>Добавить точку</summary>
        /// <param name="p"></param>
        public void AddPoint(MyPoint p)
        {
            minx = Math.Min(minx, p.X);
            miny = Math.Min(miny, p.Y);
            maxx = Math.Max(maxx, p.X);
            maxy = Math.Max(maxy, p.Y);
        }

        /// <summary>Добавить MBR</summary>
        /// <param name="nmbr"></param>
        public void AddMBR(TMBR nmbr)
        {
            if (nmbr == null) return;
            minx = Math.Min(minx, nmbr.minx);
            miny = Math.Min(miny, nmbr.miny);
            maxx = Math.Max(maxx, nmbr.maxx);
            maxy = Math.Max(maxy, nmbr.maxy);
        }

        public bool Contains(TMBR r) => !(maxx < r.maxx) && !(minx > r.minx) && !(maxy < r.maxy) && !(miny > r.miny);
    }
}