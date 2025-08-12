using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ndm_acad2021
{
    internal class CalcElement
    {
        internal CalcElement(AcadElement ae, Concrete c)
        {
            this.ae = ae;
            switch (ae.Type) {
                case ElementType.Concrete:
                    E = c.Diagram.Equals(ConcreteDiagram.Linear2) ? c.Rb / c.eb1red : c.Ebt;
                    Vbi = 1;
                    break;
                case ElementType.Reinforcement:
                    E = c.reinf.Es;
                    Vsi = 1;
                    break;
                case ElementType.Prereinforcement:
                    E = c.preReinf.Es;
                    Vspm = 1;
                    break;
                default:
                    E = c.user.E;
                    Vuk = 1;
                    break;
            }
            X = ae.X;
            Y = ae.Y;
            Area = ae.Area;
            Type = ae.Type;
            Weight = ae.Weight;
        }

        internal readonly double Area;
        internal readonly double X;
        internal readonly double Y;
        internal readonly string Type;
        internal readonly double Weight;
        internal readonly int Vbi;
        internal readonly int Vsi;
        internal readonly int Vuk;
        internal readonly int Vspm;

        internal readonly double E;

        /// <summary>
        /// Признак бетона
        /// </summary>
        internal bool Concrete
        {
            get { return Type.Equals(ElementType.Concrete); }
        }

        /// <summary>
        /// Признак пользовательского материала
        /// </summary>
        internal bool User
        {
            get { return Type.Equals(ElementType.User); }
        }

        /// <summary>
        /// Признак ненапрягаемой арматуры
        /// </summary>
        internal bool Reinforcement
        {
            get { return Type.Equals(ElementType.Reinforcement); }
        }

        /// <summary>
        /// Признак напрягаемой арматуры
        /// </summary>
        internal bool Prereinforcement
        {
            get { return Type.Equals(ElementType.Prereinforcement); }
        }

        AcadElement ae;
        
        /// <summary>
        /// Тип элемента
        /// </summary>
        static class ElementType
        {
            /// <summary>
            /// Бетон
            /// </summary>
            internal const string Concrete = "c";

            /// <summary>
            /// Ненапрягаемая арматура
            /// </summary>
            internal const string Reinforcement = "r";

            /// <summary>
            /// Напрягаемая арматура
            /// </summary>
            internal const string Prereinforcement = "p";

            /// <summary>
            /// Пользовательский материал
            /// </summary>
            internal const string User = "u";
        }
    }
}
