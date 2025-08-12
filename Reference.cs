using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ndm_acad2021
{
    /// <summary>
    /// Справочник
    /// </summary>
    internal class Reference
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="concreteClass">Класс тяжелого бетона</param>
        /// <param name="aClass">Класс арматуры сеток косвенного армирования</param>
        internal Reference(string concreteClass)
        { 
            var classes = sClass.Split();
            index = Array.IndexOf(classes, concreteClass);
            format = new NumberFormatInfo();
            format.NumberDecimalSeparator = ",";
            tab = new char[] { '\t' };
            var allEb = sEb.Split(tab);
            var allRb = sRb.Split(tab);
            var allRbn = sRbn.Split(tab);
            var allRbt = sRbt.Split(tab);
            var allRbtn = sRbtn.Split(tab);
            Eb = Double.Parse(allEb[index], format) * 1000;
            Rb = Double.Parse(allRb[index], format);
            Rbn = Double.Parse(allRbn[index], format);
            Rbt = Double.Parse(allRbt[index], format);
            Rbtn = Double.Parse(allRbtn[index], format);
            initPhi();
        }

        internal readonly double Eb;
        internal readonly double Rb;
        internal readonly double Rbn;
        internal readonly double Rbt;
        internal readonly double Rbtn;

        /// <summary>
        /// Возвращает коэффициент ползучести бетона ϕb
        /// </summary>
        /// <param name="humidity">Относительная влажность воздуха окружающей среды, %</param>
        /// <returns>коэффициент ползучести бетона</returns>
        internal double PhiB(string humidity)
        {
            return Double.Parse(phi[humidity][index], format);
        }

        const string sClass = "В10 В15 В20 В25 В30 В35 В40 В45 В50 В55 В60";
        const string sEb = "19\t24\t27,5\t30\t32,5\t34,5\t36\t37\t38\t39\t39,5";
        const string sRb = "6\t8,5\t11,5\t14,5\t17\t19,5\t22\t25\t27,5\t30\t33";
        const string sRbn = "7,5\t11\t15\t18,5\t22\t25,5\t29\t32\t36\t39,5\t43";
        const string sRbt = "0,56\t0,75\t0,9\t1,05\t1,15\t1,3\t1,4\t1,5\t1,6\t1,7\t1,8";
        const string sRbtn = "0,85\t1,1\t1,35\t1,55\t1,75\t1,95\t2,1\t2,25\t2,45\t2,6\t2,75";

        int index;
        Dictionary<string, string[]> phi;
        char[] tab;
        NumberFormatInfo format;

        void initPhi()
        {
            phi = new Dictionary<string, string[]>();
            phi.Add(
                HumidityLevel.High,
                "2,8\t2,4\t2\t1,8\t1,6\t1,5\t1,4\t1,3\t1,2\t1,1\t1".Split(tab));
            phi.Add(
                HumidityLevel.Middle,
                "3,9\t3,4\t2,8\t2,5\t2,3\t2,1\t1,9\t1,8\t1,6\t1,5\t1,4".Split(tab));
            phi.Add(
                HumidityLevel.Low,
                "5,6\t4,8\t4\t3,6\t3,2\t3\t2,8\t2,6\t2,4\t2,2\t2".Split(tab));
        }
        
    }
}
