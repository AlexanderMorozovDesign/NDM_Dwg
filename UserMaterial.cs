using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ndm_acad2021
{
    /// <summary>
    /// Информация о пользовательском материале (диаграмма Прандля)
    /// </summary>
    internal class UserMaterial
    {
        /// <summary>
        /// Коэффициент, вводимый при расчете на прочность и учитывающий
        /// возможные отклонения предварительного напряжения γup
        /// </summary>
        internal readonly double GammaUp;

        /// <summary>
        /// Расчетное сопротивление материала растяжению с учетом γui R+, МПа
        /// </summary>
        internal readonly double Rs;

        /// <summary>
        /// Расчетное сопротивление материала сжатию с учетом γui  R-, МПа
        /// </summary>
        internal readonly double Rsc;

        /// <summary>
        /// Модуль упругости материала E, МПа
        /// </summary>
        internal double E;

        /// <summary>
        /// Предварительное напряжение материала с учетом всех потерь σup, МПа
        /// </summary>
        internal readonly double PreStress;

        /// <summary>
        /// Удельный вес материала, кН/м³
        /// </summary>
        internal double Weight;

        internal UserMaterial(double Rs = 145.0, double Rsc = 145.0,
            double preStress = 0.0, double gammaUp = 1.0)
        {
            GammaUp = gammaUp;
            this.Rs = Rs;
            this.Rsc = Rsc;
            E = 2.10E+05;
            PreStress = preStress;
            Weight = 78.5;
        }
    }
}
