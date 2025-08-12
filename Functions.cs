using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ndm_acad2021
{
    /// <summary>
    /// Функции расчёта железобетонных элементов
    /// </summary>
    internal static class Functions
    {
        internal static double StressStrainConcrete(double e, bool tension, string diagram,
            string concreteClass, double eb0, double eb1, double eb1red, double ebt0,
            double ebt1, double ebt1red, double ebt2, double Eb, double Ebint, double Rb,
            double Rbt, double h)
        {
            switch (diagram) {
                case ConcreteDiagram.Linear2:
                    return Concrete2Linear(e, tension, Eb, Rb, Rbt, eb1red, ebt1red, ebt2);
                case "Трехлинейная":
                    return Concrete3Linear(e, tension, Eb, Rb, Rbt, eb0, eb1, ebt0, ebt1, ebt2);
                case "Криволинейная":
                    return ConcreteCurved(e, tension, Ebint, Eb, Rb, Rbt, concreteClass, h);
                default:
                    return 0.0;
            }
            
        }

        internal static double StressStrainReinf(double e, string diagram, double es0, double es2,
            double Es, double Rs, double Rsc)
        {
            switch (diagram) {
                case linear2:
                    return Reinf2Linear(e, Es, Rs, Rsc, 0.0, 1.0);
                case linear3:
                    return Reinf3Linear(e, Es, Rs, Rsc, es0, 0.0, 1.0);
                default:
                    return 0.0;
            }
        }

        internal static double StressStrainPreReinf(double e, string diagram, double es0,
            double es2, double Es, double Rs, double Rsc, double preStress, double gammaSP)
        {
            switch (diagram) {
                case linear2:
                    return Reinf2Linear(e, Es, Rs, Rsc, preStress, gammaSP);
                case linear3:
                    return Reinf3Linear(e, Es, Rs, Rsc, es0, preStress, gammaSP);
                default:
                    return 0.0;
            }
        }

        internal static double User2Linear(double e, double Es, double Rs, double Rsc,
            double preStress, double gammaUp)
        {
            double e_calc = e + (preStress * gammaUp) / Es;
            double es0;
            if (e_calc < 0) {
                es0 = Rsc / Es;
                return Math.Abs(e_calc) < Math.Abs(es0) ? e_calc * Es : -Rsc;
            } else {
                es0 = Rs / Es;
                return e_calc < es0 ? e_calc * Es : Rs;
            }
        }

        internal static double Rx(double N, double Mx, double My, double D11, double D22,
            double D12, double D13, double D23, double D33)
        {
            return (Math.Pow(D23, 2) * Mx + D12 * D33 * My - D13 * D23 * My - D22 * D33 * Mx -
                D12 * D23 * N + D13 * D22 * N) / (D33 * Math.Pow(D12, 2) - 2 * D12 * D13 * D23 +
                D22 * Math.Pow(D13, 2) + D11 * Math.Pow(D23, 2) - D11 * D22 * D33);
        }

        internal static double Ry(double N, double Mx, double My, double D11, double D22,
            double D12, double D13, double D23, double D33)
        {
            return (Math.Pow(D13, 2) * My - D13 * D33 * My + D12 * D33 * Mx - D13 * D23 * Mx +
                D11 * D23 * N - D12 * D13 * N) / (D33 * Math.Pow(D12, 2) - 2 * D12 * D13 * D23 +
                D22 * Math.Pow(D13, 2) + D11 * Math.Pow(D23, 2) - D11 * D22 * D33);
        }

        internal static double E0(double N, double Mx, double My, double D11, double D22,
            double D12, double D13, double D23, double D33)
        {
            return (Math.Pow(D12, 2) * N + D11 * D23 * My - D12 * D13 * My - D12 * D23 * Mx +
                D13 * D22 * Mx - D11 * D22 * N) / (D33 * Math.Pow(D12, 2) - 2 * D12 * D13 * D23 +
                D22 * Math.Pow(D13, 2) + D11 * Math.Pow(D23, 2) - D11 * D22 * D33);
        }

        const string linear2 = "Двухлинейная (физический предел текучести)";
        const string linear3 = "Трехлинейная (условный предел текучести)";

        static double Concrete2Linear(double e, bool tension, double Eb, double Rb, double Rbt,
            double eb1red, double ebt1red, double ebt2)
        {
            double Ebred = Rb / eb1red;
            double Ebtred = Rbt / ebt1red;
            if (e > 0) {
                if (!tension) {
                    return 0.0;
                } 
                if (Math.Abs(e) <= ebt1red) {
                    return Ebred * e;
                } else {
                    return Math.Abs(e) <= ebt2 ? Rbt : 0.0;
                }
            } else {
                return Math.Abs(e) <= eb1red ? Ebred * e : -Rb;
            }
        }

        static double Concrete3Linear(double e, bool tension, double Eb, double Rb, double Rbt,
            double eb0, double eb1, double ebt0, double ebt1, double ebt2)
        {
            if (e > 0) {
                if (!tension) {
                    return 0.0;
                }
                if (Math.Abs(e) <= ebt1) {
                    return Eb * e;
                } else {
                    if (Math.Abs(e) <= ebt0) {
                        return (0.4 * (Math.Abs(e) - ebt1) / (ebt0 - ebt1) + 0.6) * Rbt;
                    } else {
                        return Math.Abs(e) <= ebt2 ? Rbt : 0.0;
                    }
                }
            } else {
                if (Math.Abs(e) <= eb1) {
                    return Eb * e;
                } else {
                    return Math.Abs(e) <= eb0 ?
                        -(0.4 * (Math.Abs(e) - eb1) / (eb0 - eb1) + 0.6) * Rb :
                        -Rb;
                }
            }
        }

        static double ConcreteCurved(double e, bool tension, double E0, double Eb, double Rb,
            double Rbt, string concreteClass, double h)
        {
            const double h_e = 30.0;
            double dClass = Double.Parse(concreteClass.Substring(1));
            double lambda, e_, sigma, v, eE, v0, w1, w2, m1, m2, m3, result;
            bool check = false;
            if (e < 0) {
                lambda = 1;
                e_ = -(dClass / E0) * lambda *
                    (1 + 0.75 * lambda * dClass / 60 + 0.2 * lambda / dClass) /
                    (0.12 + dClass / 60.0 + 0.2 / dClass);
                sigma = -Rb;
                v = sigma / (e_ * E0);
                eE = e * E0;
                if (Math.Abs(e) <= Math.Abs(e_)) {
                    v0 = 1;
                    w1 = 2 - 2.5 * v;
                } else {
                    v0 = 2.05 * v;
                    w1 = 1.95 * v - 0.138;
                    check = true;
                }
                w2 = 1 - w1;
                m1 = M1(sigma, v, eE, v0, w1);
                m2 = M2(sigma, v, eE, v0, w1, w2);
                m3 = M3(sigma, v, eE, v0, w2);
                result = -1 * Math.Pow(sigma, 2) * (m1 + m2) / m3;
                if (check && (result / sigma < 0.85)) {
                    result = 0.0;
                }
                return result;
            } else {
                if (!tension) {
                    return 0.0;
                }
                h /= 10.0;
                double gamma_btq = 2.07 - Math.Pow(h / h_e, 1.0 / 5.0);
                if (gamma_btq < 0.9) {
                    gamma_btq = 0.9;
                }
                sigma = Rbt * gamma_btq;
                v = (0.6 + (0.15 * (Rbt / 2.5))) / gamma_btq;
                e_ = sigma / (E0 * v);
                eE = e * E0;
                if (e <= e_) {
                    v0 = 1;
                    w1 = 2 - 2.5 * v;
                } else {
                    v0 = 2.05 * v;
                    w1 = 1.95 * v - 0.138;
                    check = true;
                }
                w2 = 1 - w1;
                m1 = M1(sigma, v, eE, v0, w1);
                m2 = M2(sigma, v, eE, v0, w1, w2);
                m3 = M3(sigma, v, eE, v0, w2);
                result = -1 * Math.Pow(sigma, 2) * (m1 - m2) / m3;
                if (check && (result / sigma < 0.85)) {
                    result = 0.0;
                }
                return result;
            }
        }

        private static double M3(double sigma, double v, double eE, double v0, double w2)
        {
            return w2 * Math.Pow(eE, 2) * Math.Pow(v0, 2) -
                2 * w2 * Math.Pow(eE, 2) * v0 * v +
                w2 * Math.Pow(eE, 2) * Math.Pow(v, 2) +
                Math.Pow(sigma, 2);
        }

        private static double M2(double sigma, double v, double eE, double v0, double w1, double w2)
        {
            return (eE * (v0 - v) * Math.Sqrt(
                Math.Pow(eE, 2) * Math.Pow(v0, 2) * Math.Pow(w1, 2) +
                4 * w2 * Math.Pow(eE, 2) * Math.Pow(v0, 2) -
                2 * Math.Pow(eE, 2) * v0 * v * Math.Pow(w1, 2) -
                8 * w2 * Math.Pow(eE, 2) * v0 * v +
                Math.Pow(eE, 2) * Math.Pow(v, 2) * Math.Pow(w1, 2) -
                4 * eE * sigma * v * w1 +
                4 * Math.Pow(sigma, 2))) / (2 * sigma);
        }

        private static double M1(double sigma, double v, double eE, double v0, double w1)
        {
            return (w1 * Math.Pow(eE, 2) * Math.Pow(v0, 2) -
                2 * w1 * Math.Pow(eE, 2) * v0 * v +
                w1 * Math.Pow(eE, 2) * Math.Pow(v, 2) -
                2 * sigma * eE * v) / (2 * sigma);
        }

        static double Reinf2Linear(double e, double Es, double Rs, double Rsc,
            double preStress, double gammaSP)
        {
            double e_calc = e + preStress * gammaSP / Es;
            double es0;
            if (e_calc < 0) {
                es0 = Rsc / Es;
                return Math.Abs(e_calc) < Math.Abs(es0) ? e_calc * Es : -Rsc;
            } else {
                es0 = Rs / Es;
                return e_calc < es0 ? e_calc * Es : Rs;
            }
        }

        static double Reinf3Linear(double e, double Es, double Rs, double Rsc, double es0,
            double preStress, double gammaSP)
        {
            double e_calc = e + preStress * gammaSP / Es;
            double es1, sigma_s1, eRed, result;
            if (e_calc < 0) {
                es1 = 0.9 * Rsc / Es;
                if (Math.Abs(e_calc) < Math.Abs(es1)) {
                    return e_calc * Es;
                } else {
                    sigma_s1 = 0.9 * Rsc;
                    eRed = (1 - sigma_s1 / Rsc) * (Math.Abs(e_calc) - es1) / (es0 - es1) +
                        sigma_s1 / Rsc;
                    result = -eRed * Rsc;
                    return Math.Abs(result) > Math.Abs(1.1 * Rsc) ? -1.1 * Rsc : result;
                }
            } else {
                es1 = 0.9 * Rs / Es;
                if (e_calc < es1) {
                    return e_calc * Es;
                } else {
                    sigma_s1 = 0.9 * Rs;
                    eRed = (1 - sigma_s1 / Rs) * (e_calc - es1) / (es0 - es1) + sigma_s1 / Rs;
                    result = eRed * Rs;
                    return result > 1.1 * Rs ? 1.1 * Rs : result;
                }
            }
        }
    }
}
