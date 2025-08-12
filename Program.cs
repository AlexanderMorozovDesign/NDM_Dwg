using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ndm_acad2021
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var ac = new AcadClient();
            var c = new Concrete(
                diagram: ConcreteDiagram.Linear2,
                humidity: HumidityLevel.High,
                continuousLoad: true);
            var pr = new Reinforcement(c.CalcProps, c.ContinuousLoad, "А600", 346.0);
            c.preReinf = pr;
            c.user = new UserMaterial();
            c.forces = new Forces();
            var g = ac.GetGeometry(c);
            Console.WriteLine("Получено объектов: {0}", g.Elements.Count);
            Console.WriteLine("Характеристики материалов: {0}", c.CalcProps ? "Расчетные" : "Нормативные");
            Console.WriteLine("Класс тяжелого бетона: {0}", c.Class);
            Console.WriteLine("Относительная влажность воздуха окружающей среды,%: {0}", c.Humidity);
            Console.WriteLine("Действие нагрузки: {0}",
                c.ContinuousLoad ? "Продолжительное" : "Непродолжительное");
            Console.WriteLine("Диаграмма состояния бетона: {0}", c.Diagram);
            Console.WriteLine("Класс продольной ненапрягаемой арматуры: {0}", c.reinf.Class);
            Console.WriteLine("Класс продольной напрягаемой арматуры: {0}", pr.Class);
            Console.WriteLine(
                "Предварительное напряжение арматуры с учетом всех потерь, МПа: {0}",
                pr.PreStress);
            Console.WriteLine("Заданные нагрузки");
            Console.WriteLine("Нормальная сила, N, кН: {0}", c.forces.N);
            Console.WriteLine("Изгибающий момент Mx, кНм: {0}", c.forces.Mx);
            Console.WriteLine("Изгибающий момент My, кНм: {0}", c.forces.My);
            displayUpperTable(c, pr);
            displayResults(g);
            displayFooterTable(g);
        }

        static void displayUpperTable(Concrete c, Reinforcement pr)
        {
            Console.WriteLine();
            Console.WriteLine("Данные для графиков (верх листа Excel):");
            Console.WriteLine("{0,3}{1,12}{2,12}{3,12}{4,12}{5,12}{6,12}{7,5}",
                "", "e бетона", "sigma бет.", "e арматуры", "sigma арм.", "sigma мат.",
                "sigma арм.", "gamma");
            double e, er, gammaSP;
            for (int i = -40; i <= 40; i++) {
                e = (1.5 * (i >= 0 ? c.Ebt2 : i < -1 ? c.Eb2 : 0) / 40) * i;
                er = 1.1 * c.reinf.es2 / 40 * i;
                gammaSP = er > 0 ? pr.GammaSpr : er < 0 ? pr.GammaSps : 1.0;
                Console.WriteLine(
                    "{0,3}{1,12:g3}{2,12:g6}{3,12}{4,12}{5,12:g6}{6,12:g6}{7,5}",
                    i,
                    e,
                    Functions.StressStrainConcrete(
                        e, false, c.Diagram, c.Class, c.eb0, c.eb1, c.eb1red, c.Ebt0, c.ebt1,
                        c.ebt1red, c.Ebt2, c.Ebt, c.Eb, c.Rb, c.Rbt, 900),
                    er,
                    Functions.StressStrainReinf(
                        er, c.reinf.Diagram, c.reinf.es0, c.reinf.es2, c.reinf.Es, c.reinf.Rs,
                        c.reinf.Rsc),
                    Functions.User2Linear(er, c.user.E, c.user.Rs, c.user.Rsc, c.user.PreStress,
                        c.user.GammaUp),
                    Functions.StressStrainPreReinf(
                        er, pr.Diagram, pr.es0, pr.es2, pr.Es, pr.Rs, pr.Rsc, pr.PreStress, gammaSP),
                    gammaSP);
            }
        }

        static void displayResults(AcadGeometry g)
        {
            Console.WriteLine();
            Console.WriteLine("Результаты расчета");
            Console.WriteLine("N = {0} кН", g.N());
            Console.WriteLine("Координата центра тяжести Xc = {0:g6} мм", g.Xc);
            Console.WriteLine("Координата центра тяжести Yc = {0:g6} мм", g.Yc);
            Console.WriteLine("Габарит сечения по направлению оси X, мм: {0}", g.DimX);
            Console.WriteLine("Габарит сечения по направлению оси Y, мм: {0}", g.DimY);
            Console.WriteLine("Момент инерции бетона относительно оси Y: Ix, см4: {0:g8}", g.Ix());
            Console.WriteLine("Момент инерции бетона относительно оси Y: Iy, см4: {0:g8}", g.Iy());
            Console.WriteLine(
                "Момент инерции ненапрягаемой арматуры относительно оси X: Isx, см4: {0:g8}",
                g.Isx());
            Console.WriteLine(
                "Момент инерции ненапрягаемой арматуры относительно оси Y: Isy, см4: {0:g8}",
                g.Isy());
            Console.WriteLine(
                "Момент инерции напрягаемой арматуры относительно оси X: Ispx, см4: {0:g8}",
                g.Ispx());
            Console.WriteLine(
                "Момент инерции напрягаемой арматуры относительно оси Y: Ispy, см4: {0:g8}",
                g.Ispy());
            Console.WriteLine(
                "Момент инерции пользовательского материала относительно оси X: Iux, см4: {0:g8}",
                g.Iux());
            Console.WriteLine(
                "Момент инерции пользовательского материала относительно оси Y: Iuy, см4: {0:g8}",
                g.Iuy());
            Console.WriteLine("Учет прогиба элемента");
            Console.WriteLine("Коэффициент deltae в плоскости момента Mx: {0}", g.deltaeMx());
            Console.WriteLine("Коэффициент deltae в плоскости момента My: {0}", g.deltaeMy());
            Console.WriteLine("Коэффициент kb в плоскости момента Mx: {0}", g.kbMx());
            Console.WriteLine("Коэффициент kb в плоскости момента My: {0}", g.kbMy());
            Console.WriteLine("Жесткость D в плоскости момента Mx, кН×м2: {0:g8}", g.DMx());
            Console.WriteLine("Жесткость D в плоскости момента My, кН×м2: {0:g8}", g.DMy());
            Console.WriteLine("Условная критическая сила Ncr в плоскости момента Mx, кН: {0:g8}", g.NcrMx());
            Console.WriteLine("Условная критическая сила Ncr в плоскости момента My, кН: {0:g8}", g.NcrMy());
            Console.WriteLine("Жесткости");
            Console.WriteLine("D11 = {0:g6} кН*кв.м", g.D11());
            Console.WriteLine("D22 = {0:g6} кН*кв.м", g.D22());
            Console.WriteLine("D12 = {0:g6} кН*кв.м", g.D12());
            Console.WriteLine("D13 = {0:g6} кН*м", g.D13());
            Console.WriteLine("D23 = {0:g6} кН*м", g.D23());
            Console.WriteLine("D33 = {0:g6} кН*м", g.D33());
        }

        static void displayFooterTable(AcadGeometry g)
        {
            Console.WriteLine();
            Console.WriteLine("Данные нижней части листа Excel:");
            const string format =
                "{0,12:g6}{1,5}{2,13:g6}{3,13:g6}{4,13:g6}{5,13:g6}{6,13:g6}{7,6}{8,6}{9,6}";
            Console.WriteLine(format,
                "Area", "Type", "X", "Y", "Zx", "Zy", "E", "vbi=1", "vsi=1", "g");
            foreach (CalcElement element in g.Elements) {
                Console.WriteLine(format, element.Area, element.Type, element.X,
                    element.Y, (element.X - g.Xc) / 1000, (element.Y - g.Yc) / 1000,
                    element.E, element.Vbi, element.Vsi, element.Weight);
            }
        }
    }
}
