using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ndm_acad2021
{
    internal class Армирование
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="aClass">Класс арматуры сеток косвенного армирования</param>
        /// <param name="nx">число стержней в направлении X</param>
        /// <param name="Asx">площадь сечения стержня в направлении X, см²</param>
        /// <param name="lx">длина стержня сетки в направлении X, см</param>
        /// <param name="ny">число стержней в направлении Y</param>
        /// <param name="Asy">площадь сечения стержня в направлении Y, см²</param>
        /// <param name="ly">длина стержня сетки в направлении Y, см</param>
        /// <param name="Aef">Площадь, заключенная внутри контура сеток косвенного армирования,
        /// считая по их крайним стержням, см²</param>
        /// <param name="s">Шаг сеток косвенного армирования, см</param>
        internal Армирование(
            string aClass = "А400",
            int nx = 5, double Asx = 0.785, double lx = 55.0,
            int ny  =5, double Asy = 0.785, double ly = 55.0,
            double Aef = 3600.0, double s = 10.0)
        {
            Mus = (nx * Asx * lx + ny * Asy * ly) / (Aef * s);
            InitR();
            var allR = r[aClass].Split(tab);
            Rs = Double.Parse(allR[0]);
            Rsn = Double.Parse(allR[1]);
        }

        /// <summary>
        /// Расчетное сопротивление арматуры сеток косвенного армирования Rs,xy, МПа [табл. 6.13, 6.14]
        /// </summary>
        internal readonly double Rs;

        /// <summary>
        /// Нормативное сопротивление арматуры сеток косвенного армирования Rs,xy, МПа [табл. 6.13, 6.14]
        /// </summary>
        internal readonly double Rsn;

        /// <summary>
        /// Коэффициент косвенного армирования μs,xy [(К.10)]
        /// </summary>
        internal readonly double Mus;

        /// <summary>
        /// Коэффициент αred [(К.12)]
        /// </summary>
        /// <param name="Rb">Справочное значение для заданного класса бетона</param>
        /// <returns>Коэффициент</returns>
        internal double AlphaRed(double Rb)
        {
            return (Mus * Rs) / (Rb + 10);
        }

        /// <summary>
        /// Коэффициент ϕ [(К.11)]
        /// </summary>
        internal double Phi(double Rb)
        {
            return 1 / (0.23 + AlphaRed(Rb));
        }



        /// <summary>
        /// Словарь "класс арматуры" - сопротивления арматуры (Rs, Rsn), разделенные табуляцией
        /// </summary>
        Dictionary<string, string> r;
        char[] tab;

        void InitR()
        {
            tab = new char[] { '\t' };
            r = new Dictionary<string, string>();
            r.Add("А400", "350\t400\t350\t400\t350\t400\t0,025\t0\t2,00E+05");
        }
    }
}
