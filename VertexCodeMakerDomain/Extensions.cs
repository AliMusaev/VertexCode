using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VertexCodeMakerDomain
{
    public static class Extensions
    {
        public static void Swap<T>(this IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }
        public static Point2d Round(this Point2d point, int digits)
        {
            return new Point2d()
            {
                X = Math.Round(point.X, digits),
                Y = Math.Round(point.Y, digits)
            };
        }
        public static Point3d Round(this Point3d point, int digits)
        {
            return new Point3d()
            {
                X = Math.Round(point.X, digits),
                Y = Math.Round(point.Y, digits),
                Z = Math.Round(point.Z, digits)
            };
        }
    }
}
