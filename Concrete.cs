using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ndm_acad2021
{
    /// <summary>
    /// Информация о бетоне
    /// </summary>
    internal class Concrete
    {
        /// <summary>
        /// Создаёт новый набор информации о бетоне
        /// </summary>
        /// <param name="calcProps">Расчетные характеристики материалов</param>
        /// <param name="concClass">Класс тяжелого бетона</param>
        /// <param name="humidity">Относительная влажность воздуха окружающей среды, %</param>
        /// <param name="continuousLoad">Продолжительное действие нагрузки</param>
        /// <param name="Ybi">Произведение коэффициентов условий работы γbi [п. 6.1.12]</param>
        /// <param name="Ybti">Произведение коэффициентов условий работы γbti [п. 6.1.12]</param>
        /// <param name="diagram">Диаграмма состояния бетона</param>
        /// <param name="indirect">Учет косвенного армирования</param>
        internal Concrete(
            bool calcProps = true,
            string concClass = "В45",
            string humidity = HumidityLevel.Middle,
            bool continuousLoad = false,
            double Ybi = 0.85,
            double Ybti = 0.85,
            string diagram = "Криволинейная",
            Армирование indirect = null)
        {
            CalcProps = calcProps;
            Class = concClass;
            Humidity = humidity;
            ContinuousLoad = continuousLoad;
            Diagram = diagram;
            r = new Reference(concClass);
            classes = sClass.Split();
            format = new NumberFormatInfo();
            format.NumberDecimalSeparator = ",";
            tab = new char[] { '\t' };
            Eb = r.Eb;
            Rb = (calcProps ? r.Rb : r.Rbn) * Ybi;
            Rbt = (calcProps ? r.Rbt : r.Rbtn) * Ybti;
            if (indirect != null) {
                Rb += indirect.Phi(r.Rb) * indirect.Mus * (calcProps ? indirect.Rs : indirect.Rsn);
            }
            initEpsilon();
            var e = epsilon[Humidity].Split(tab);
            eb0 = ContinuousLoad ? Double.Parse(e[0], format) / 1000 : 0.002;
            if (indirect != null) {
                eb0 += 0.02 * indirect.AlphaRed(r.Rb);
            }
            Eb2 = ContinuousLoad ? Double.Parse(e[1], format) / 1000 : 0.0035;
            eb1red = continuousLoad ? Double.Parse(e[2], format) / 1000 : 0.0015;
            Ebt0 = continuousLoad ? Double.Parse(e[3], format) / 1000 : 0.0001;
            Ebt2 = continuousLoad ? Double.Parse(e[4], format) / 1000 : 0.00015;
            ebt1red = continuousLoad ? Double.Parse(e[5], format) / 1000 : 0.00008;
            Ebt = Eb;
            if (continuousLoad) {
                Ebt /= (1 + r.PhiB(humidity));
            }
            eb1 = 0.6 * Rb / Ebt;
            ebt1 = 0.6 * Rbt / Ebt;
            reinf = new Reinforcement(calcProps, continuousLoad);
        }

        internal readonly bool CalcProps;

        /// <summary>
        /// Класс тяжелого бетона
        /// </summary>
        internal readonly string Class;

        internal readonly string Humidity;

        /// <summary>
        /// Диаграмма состояния бетона
        /// </summary>
        internal readonly string Diagram;

        /// <summary>
        /// Начальный модуль упругости бетона, Eb, Ebt, МПа [табл. 6.11]
        /// </summary>
        internal readonly double Eb;

        /// <summary>
        /// Модуль деформации бетона, EbT, EbTt, МПа [табл. 6.11]
        /// </summary>
        internal double Ebt;

        /// <summary>
        /// Расчетное сопротивление осевому сжатию Rb, МПа [табл. 6.7, 6.8]
        /// </summary>
        internal readonly double Rb;

        /// <summary>
        /// Расчетное сопротивление осевому растяжению Rbt, МПа [табл. 6.7, 6.8]
        /// </summary>
        internal readonly double Rbt;

        /// <summary>
        /// Продолжительная нагрузка
        /// </summary>
        internal readonly bool ContinuousLoad;

        /// <summary>
        /// Величина εb0. Сжатие с возможностью учета косвенного армирования [п 6.1.14, Приложение К]
        /// </summary>
        internal readonly double eb0;

        /// <summary>
        /// Величина εb1. Сжатие с возможностью учета косвенного армирования [п 6.1.14, Приложение К]
        /// </summary>
        internal readonly double eb1;

        /// <summary>
        /// Величина εb1,red.
        /// Сжатие с возможностью учета косвенного армирования [п 6.1.14, Приложение К]
        /// </summary>
        internal readonly double eb1red;

        /// <summary>
        /// Относительная деформация при сжатии
        /// </summary>
        internal readonly double Eb2;

        internal readonly double Ebt0;

        /// <summary>
        /// Величина εbt1. Растяжение [п 6.1.14]
        /// </summary>
        internal readonly double ebt1;

        /// <summary>
        /// Величина εbt1,red. Растяжение [п 6.1.14]
        /// </summary>
        internal readonly double ebt1red;

        /// <summary>
        /// Относительная деформация при растяжении
        /// </summary>
        internal readonly double Ebt2;

        internal readonly Армирование Indirect;

        /// <summary>
        /// Ненапрягаемая арматура
        /// </summary>
        internal Reinforcement reinf;

        /// <summary>
        /// Напрягаемая арматура
        /// </summary>
        internal Reinforcement preReinf;

        /// <summary>
        /// Пользовательский материал
        /// </summary>
        internal UserMaterial user;

        internal Forces forces;

        const string sClass = "В10 В15 В20 В25 В30 В35 В40 В45 В50 В55 В60";
        const string sEb = "19\t24\t27,5\t30\t32,5\t34,5\t36\t37\t38\t39\t39,5";
        const string sRb = "6\t8,5\t11,5\t14,5\t17\t19,5\t22\t25\t27,5\t30\t33";
        const string sRbn = "7,5\t11\t15\t18,5\t22\t25,5\t29\t32\t36\t39,5\t43";
        const string sRbt = "0,56\t0,75\t0,9\t1,05\t1,15\t1,3\t1,4\t1,5\t1,6\t1,7\t1,8";
        const string sRbtn = "0,85\t1,1\t1,35\t1,55\t1,75\t1,95\t2,1\t2,25\t2,45\t2,6\t2,75";

        string[] classes;
        NumberFormatInfo format;
        Dictionary<string, string> epsilon;
        char[] tab;
        Reference r;

        void initEpsilon()
        {
            epsilon = new Dictionary<string, string>();
            epsilon.Add(HumidityLevel.High, "3,00\t4,20\t2,40\t0,21\t0,27\t0,19");
            epsilon.Add(HumidityLevel.Middle, "3,40\t4,80\t2,80\t0,24\t0,31\t0,22");
            epsilon.Add(HumidityLevel.Low, "4,00\t5,60\t3,40\t0,28\t0,36\t0,26");
        }
    }
}
