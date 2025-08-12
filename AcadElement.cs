using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ndm_acad2021
{
    /// <summary>
    /// Элемент чертежа AutoCAD (область или точка)
    /// </summary>
    internal class AcadElement
    {
        internal AcadElement(
            string handle, string type, double x, double y, double weight = 0.0, double area = 0.0)
        {
            Handle = handle;
            Type = type;
            X = x;
            Y = y;
            Weight = weight;
            Area = area;
        }

        internal readonly string Handle;
        internal readonly string Type;
        internal readonly double X;
        internal readonly double Y;
        internal readonly double Weight;
        internal readonly double Area;
    }
}
