using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ndm_acad2021
{
    /// <summary>
    /// Геометрия чертежа AutoCAD
    /// </summary>
    internal class AcadGeometry
    {
        /// <summary>
        /// Габарит сечения по направлению оси X, мм
        /// </summary>
        internal readonly double DimX;

        /// <summary>
        /// Габарит сечения по направлению оси Y, мм
        /// </summary>
        internal readonly double DimY;

        /// <summary>
        /// Создаёт новую геометрию чертежа AutoCAD
        /// </summary>
        /// <param name="dimX">Габарит сечения по направлению оси X, мм</param>
        /// <param name="dimY">Габарит сечения по направлению оси X, мм</param>
        internal AcadGeometry(Concrete c, double dimX = 900, double dimY = 900, bool weight = false)
        {
            concrete = c;
            DimX = dimX;
            DimY = dimY;
            Weight = weight;
            elements = new List<CalcElement>();
            areas = new List<double>();
            x = new List<double>();
            y = new List<double>();
            ax = 0.0;
            aw = 0.0;
            ay = 0.0;
        }

        internal readonly bool Weight;

        internal void AddElement(AcadElement element)
        {
            elements.Add(new CalcElement(element, concrete));
            areas.Add(element.Area);
            x.Add(element.X);
            y.Add(element.Y);
            if (Weight) {
                ax += element.Area * element.X * element.Weight;
                ay += element.Area * element.Y * element.Weight;
                aw += element.Area * element.Weight;
            } else {
                ax += element.Area * element.X;
                ay += element.Area * element.Y;
                aw += element.Area;
            }

        }

        /// <summary>
        /// Координата центра тяжести Xc, мм
        /// </summary>
        /// <returns></returns>
        internal double Xc
        {
            get { return ax / aw; }
        }

        /// <summary>
        /// Координата центра тяжести Yc, мм
        /// </summary>
        internal double Yc
        {
            get { return ay / aw; }
        }

        /// <summary>
        /// Возвращает начальную жесткость D11, кН×м2
        /// </summary>
        /// <returns></returns>
        internal double D11()
        {
            double result = 0.0;
            double xc = Xc;
            double Zx;
            foreach(CalcElement ce in elements) {
                Zx = (ce.X - xc) / kilo;
                result += ce.Area * Zx * Zx * ce.E * ce.Vbi;
                result += ce.Area * Zx * Zx * ce.E * ce.Vsi;
                result += ce.Area * Zx * Zx * ce.E * ce.Vuk;
                result += ce.Area * Zx * Zx * ce.E * ce.Vspm;
            }
            return result * kilo;
        }

        /// <summary>
        /// Возвращает начальную жесткость D22, кН×м2
        /// </summary>
        /// <returns></returns>
        internal double D22()
        {
            double result = 0.0;
            double yc = Yc;
            double Zy;
            foreach(CalcElement ce in elements) {
                Zy = (ce.Y - yc) / kilo;
                result += ce.Area * Zy * Zy * ce.E * ce.Vbi;
                result += ce.Area * Zy * Zy * ce.E * ce.Vsi;
                result += ce.Area * Zy * Zy * ce.E * ce.Vuk;
                result += ce.Area * Zy * Zy * ce.E * ce.Vspm;
            }
            return result * kilo;
        }

        /// <summary>
        /// Возвращает начальную жесткость D12, кН×м2
        /// </summary>
        /// <returns></returns>
        internal double D12()
        {
            double result = 0.0;
            double xc = Xc;
            double yc = Yc;
            foreach(CalcElement ce in elements) {
                result += ce.Area * (ce.X - xc) / kilo * (ce.Y - yc) / kilo * ce.E * ce.Vbi;
                result += ce.Area * (ce.X - xc) / kilo * (ce.Y - yc) / kilo * ce.E * ce.Vsi;
                result += ce.Area * (ce.X - xc) / kilo * (ce.Y - yc) / kilo * ce.E * ce.Vuk;
                result += ce.Area * (ce.X - xc) / kilo * (ce.Y - yc) / kilo * ce.E * ce.Vspm;
            }
            return result * kilo;
        }

        /// <summary>
        /// Возвращает начальную жесткость D13, кН×м
        /// </summary>
        /// <returns></returns>
        internal double D13()
        {
            double result = 0.0;
            double xc = Xc;
            double Zx;
            foreach (CalcElement ce in elements) {
                Zx = (ce.X - xc) / kilo;
                result += ce.Area * Zx * ce.E * ce.Vbi;
                result += ce.Area * Zx * ce.E * ce.Vsi;
                result += ce.Area * Zx * ce.E * ce.Vuk;
                result += ce.Area * Zx * ce.E * ce.Vspm;
            }
            return result * kilo;
        }

        /// <summary>
        /// Возвращает начальную жесткость D23, кН×м
        /// </summary>
        /// <returns></returns>
        internal double D23()
        {
            double result = 0.0;
            double yc = Yc;
            double Zy;
            foreach (CalcElement ce in elements) {
                Zy = (ce.Y - yc) / kilo;
                result += ce.Area * Zy * ce.E * ce.Vbi;
                result += ce.Area * Zy * ce.E * ce.Vsi;
                result += ce.Area * Zy * ce.E * ce.Vuk;
                result += ce.Area * Zy * ce.E * ce.Vspm;
            }
            return result * kilo;
        }

        /// <summary>
        /// Возвращает начальную жесткость D33, кН
        /// </summary>
        /// <returns></returns>
        internal double D33()
        {
            double result = 0.0;
            foreach (CalcElement ce in elements) {
                result += ce.Area * ce.E * ce.Vbi;
                result += ce.Area * ce.E * ce.Vsi;
                result += ce.Area * ce.E * ce.Vuk;
                result += ce.Area * ce.E * ce.Vspm;
            }
            return result * kilo;
        }

        /// <summary>
        /// Вычисляет нормальную силу с учетом прогиба и преднапряжения
        /// </summary>
        /// <returns>N, кН</returns>
        internal double N(double N0 = -15000.0)
        {
            double result = N0;
            foreach(CalcElement ce in elements) {
                if (ce.User) {
                    //учесть vuk, GammaUp!
                    result += ce.Area * concrete.user.PreStress * kilo;
                } else if (ce.Prereinforcement) {
                    //учесть vspm, GammaSp!
                    result += ce.Area * concrete.preReinf.PreStress * kilo;
                }
            }
            return result;
        }

        internal List<CalcElement> Elements
        {
            get { return elements; }
        }

        internal double deltaeMx()
        {
            var f = concrete.forces;
            if (f.N < 0) {
                return Math.Min(Math.Max(Math.Abs(f.Mx / f.N) / (DimX / kilo), 0.15), 1.5);
            } else {
                return 0.0;
            }
        }

        internal double deltaeMy()
        {
            var f = concrete.forces;
            if (f.N < 0) {
                return Math.Min(Math.Max(Math.Abs(f.My / f.N) / (DimY / kilo), 0.15), 1.5);
            } else {
                return 0.0;
            }
        }

        internal double kbMx(double phiL = 1.7)
        {
            return concrete.forces.N < 0 ? 0.15 / (phiL * (0.3 + deltaeMx())) : 0.0;
        }

        internal double kbMy(double phiL = 1.7)
        {
            return concrete.forces.N < 0 ? 0.15 / (phiL * (0.3 + deltaeMy())) : 0.0;
        }

        /// <summary>
        /// Вычисляет момент инерции бетона относительно оси X
        /// </summary>
        /// <returns>Ix, см4</returns>
        internal double Ix()
        {
            double result = 0.0;
            double yc = Yc;
            double Zy;
            foreach (CalcElement ce in elements) {
                if (ce.Concrete) {
                    Zy = (ce.Y - yc) / kilo;
                    result += ce.Area * Zy * Zy;
                }
            }
            return result * 1.0e8;
        }

        /// <summary>
        /// Вычисляет момент инерции бетона относительно оси Y
        /// </summary>
        /// <returns>Iy, см4</returns>
        internal double Iy()
        {
            double result = 0.0;
            double xc = Xc;
            double Zx;
            foreach (CalcElement ce in elements) {
                if (ce.Concrete) {
                    Zx = (ce.X - xc) / kilo;
                    result += ce.Area * Zx * Zx;
                }
            }
            return result * 1.0e8;
        }

        /// <summary>
        /// Вычисляет момент инерции ненапрягаемой арматуры относительно оси X
        /// </summary>
        /// <returns>Isx, см4</returns>
        internal double Isx()
        {
            double result = 0.0;
            double yc = Yc;
            double Zy;
            foreach (CalcElement ce in elements) {
                if (ce.Reinforcement) {
                    Zy = (ce.Y - yc) / kilo;
                    result += ce.Area * Zy * Zy;
                }
            }
            return result * 1.0e8;
        }

        /// <summary>
        /// Вычисляет момент инерции ненапрягаемой арматуры относительно оси Y
        /// </summary>
        /// <returns>Isy, см4</returns>
        internal double Isy()
        {
            double result = 0.0;
            double xc = Xc;
            double Zx;
            foreach (CalcElement ce in elements) {
                if (ce.Reinforcement) {
                    Zx = (ce.X - xc) / kilo;
                    result += ce.Area * Zx * Zx;
                }
            }
            return result * 1.0e8;
        }

        /// <summary>
        /// Вычисляет момент инерции напрягаемой арматуры относительно оси X
        /// </summary>
        /// <returns>Ispx, см4</returns>
        internal double Ispx()
        {
            double result = 0.0;
            double yc = Yc;
            double Zy;
            foreach (CalcElement ce in elements) {
                if (ce.Prereinforcement) {
                    Zy = (ce.Y - yc) / kilo;
                    result += ce.Area * Zy * Zy;
                }
            }
            return result * 1.0e8;
        }

        /// <summary>
        /// Вычисляет момент инерции напрягаемой арматуры относительно оси Y
        /// </summary>
        /// <returns>Ispy, см4</returns>
        internal double Ispy()
        {
            double result = 0.0;
            double xc = Xc;
            double Zx;
            foreach (CalcElement ce in elements) {
                if (ce.Prereinforcement) {
                    Zx = (ce.X - xc) / kilo;
                    result += ce.Area * Zx * Zx;
                }
            }
            return result * 1.0e8;
        }

        /// <summary>
        /// Вычисляет момент инерции пользовательского материала относительно оси X
        /// </summary>
        /// <returns>Iux, см4</returns>
        internal double Iux()
        {
            double result = 0.0;
            double yc = Yc;
            double Zy;
            foreach (CalcElement ce in elements) {
                if (ce.User) {
                    Zy = (ce.Y - yc) / kilo;
                    result += ce.Area * Zy * Zy;
                }
            }
            return result * 1.0e8;
        }

        /// <summary>
        /// Вычисляет момент инерции пользовательского материала относительно оси Y
        /// </summary>
        /// <returns>Iuy, см4</returns>
        internal double Iuy()
        {
            double result = 0.0;
            double xc = Xc;
            double Zx;
            foreach (CalcElement ce in elements) {
                if (ce.User) {
                    Zx = (ce.X - xc) / kilo;
                    result += ce.Area * Zx * Zx;
                }
            }
            return result * 1.0e8;
        }

        /// <summary>
        /// Вычисляет жесткость в плоскости момента Mx
        /// </summary>
        /// <returns>D,кН×м2</returns>
        internal double DMx()
        {
            if (concrete.forces.N < 0) {
                return (kbMx() * concrete.Eb * Iy() + 0.7 * concrete.reinf.Es * Isy() +
                    0.7 * concrete.preReinf.Es * Ispy()) * 1.0e-5;
            } else {
                return 0.0;
            }
        }

        /// <summary>
        /// Вычисляет жесткость в плоскости момента My
        /// </summary>
        /// <returns>D,кН×м2</returns>
        internal double DMy()
        {
            if (concrete.forces.N < 0) {
                return (kbMy() * concrete.Eb * Ix() + 0.7 * concrete.reinf.Es * Isx() +
                    0.7 * concrete.preReinf.Es * Ispx()) * 1.0e-5;
            } else {
                return 0.0;
            }
        }

        /// <summary>
        /// Вычисляет условную критическую силу в плоскости момента Mx
        /// </summary>
        /// <param name="l0x"></param>
        /// <returns>Ncr, кН</returns>
        internal double NcrMx(double l0x = 3.3)
        {
            if (concrete.forces.N < 0) {
                return Math.Pow(Math.PI, 2) * DMx() / Math.Pow(l0x, 2);
            } else {
                return 0.0;
            }

        }

        /// <summary>
        /// Вычисляет условную критическую силу в плоскости момента My
        /// </summary>
        /// <param name="l0y"></param>
        /// <returns>Ncr, кН</returns>
        internal double NcrMy(double l0y = 3.3)
        {
            if (concrete.forces.N < 0) {
                return Math.Pow(Math.PI, 2) * DMy() / Math.Pow(l0y, 2);
            } else {
                return 0.0;
            }

        }

        List<CalcElement> elements;
        List<double> areas, x, y;
        double ax, aw, ay;
        Concrete concrete;
        const double kilo = 1000.0;
    }
}
