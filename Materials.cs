using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ndm_acad2021
{
    internal class Materials
    {
        internal Materials(Concrete c, Reinforcement r, Reinforcement pr, UserMaterial u)
        {
            C = c;
            R = r;
            P = pr;
            U = u;
        }

        internal readonly Concrete C;
        internal readonly Reinforcement R;
        internal readonly Reinforcement P;
        internal readonly UserMaterial U;
    }
}
