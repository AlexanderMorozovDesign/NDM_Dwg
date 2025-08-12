using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ndm_acad2021
{
    /// <summary>
    /// Информация об арматуре
    /// </summary>
    internal class Reinforcement
    {
        /// <summary>
        /// Создаёт новый набор информации об арматуре
        /// </summary>
        /// <param name="calcProps">Расчетные характеристики материалов</param>
        /// <param name="continuousLoad">Продолжительное действие нагрузки</param>
        /// <param name="rClass">Класс продольной арматуры</param>
        /// <param name="gammaSi">Произведение коэффициентов условий работы γsi</param>
        /// <param name="preStress">Предварительное напряжение арматуры с учетом всех потерь σsp, МПа</param>
        internal Reinforcement(bool calcProps, bool continuousLoad, string rClass = "А500",
            double preStress = 0.0, double gammaSi = 1.0, double gammaSpr = 0.9,
            double gammaSps = 1.1)
        {
            Class = rClass;
            var classes2 = sClasses2.Split();
            double de = 0.0;
            if (Array.IndexOf(classes2, rClass) >= 0) {
                Diagram = "Двухлинейная (физический предел текучести)";
                es2 = 0.025;
            } else {
                Diagram = "Трехлинейная (условный предел текучести)";
                es2 = 0.015;
                de = 0.002;
            }
            Es = rClass.StartsWith("К1") ? 1.95e5 : 2e5;
            initR();
            Rs = Double.Parse(R[rClass][calcProps ? 0 : 1]) * gammaSi;
            Rsc = Double.Parse(R[rClass][(calcProps ? 2 : 3) + (continuousLoad ? 0 : 2)]) * gammaSi;
            es0 = Rs / Es + de;
            PreStress = preStress;
            GammaSpr = gammaSpr;
            GammaSps = gammaSps;
        }

        /// <summary>
        /// Класс арматуры
        /// </summary>
        internal readonly string Class;

        /// <summary>
        /// Диаграмма состояния арматуры
        /// </summary>
        internal readonly string Diagram;

        /// <summary>
        /// Расчетное сопротивление арматуры растяжению, МПа [табл. 6.13, 6.14]
        /// </summary>
        internal readonly double Rs;

        /// <summary>
        /// Расчетное сопротивление арматуры сжатию, МПа [табл. 6.13, 6.14]
        /// </summary>
        internal readonly double Rsc;

        /// <summary>
        /// Величина εs0 [п. 6.2.11]
        /// </summary>
        internal readonly double es0;

        /// <summary>
        /// Величина εs2 [п. 6.2.14, п. 6.2.15]
        /// </summary>
        internal readonly double es2;

        /// <summary>
        /// Модуль упругости арматуры, МПа [п. 6.2.12]
        /// </summary>
        internal readonly double Es;

        /// <summary>
        /// Предварительное напряжение арматуры с учетом всех потерь σsp, МПа
        /// </summary>
        internal readonly double PreStress;

        /// <summary>
        /// Коэффициент для стержней растянутой зоны
        /// </summary>
        internal readonly double GammaSpr;

        /// <summary>
        /// Коэффициент для стержней сжатой зоны
        /// </summary>
        internal readonly double GammaSps;

        void initR()
        {
            tab = new char[] { '\t' };
            R = new Dictionary<string, string[]>();
            R.Add("А500", "435\t500\t435\t500\t400\t500".Split(tab));
            R.Add("А600", "520\t600\t470\t500\t400\t500".Split(tab));
        }

        Dictionary<string, string[]> R;
        char[] tab;

        const string sClasses2 = "А240 А300 А400 А500 В500";
        const string sClasses3 =
            "А600 А800 А1000 Вр500 Вр1200 Вр1300 Вр1400 Вр1500 Вр1600 К1400 К1500 К1600 К1700";

    }
}
