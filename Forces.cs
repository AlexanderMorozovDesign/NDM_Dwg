using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ndm_acad2021
{
    /// <summary>
    /// Информация об усилиях в сечении элемента
    /// </summary>
    internal class Forces
    {
        internal Forces(double N = -15000.0, double Mx = 176.0, double My = 176.0)
        {
            this.N = N;
            this.Mx = Mx;
            this.My = My;
        }

        internal readonly double N;

        internal readonly double Mx;

        internal readonly double My;
    }
}
