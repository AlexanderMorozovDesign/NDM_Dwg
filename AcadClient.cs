using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace ndm_acad2021
{
    /// <summary>
    /// Клиент приложения AutoCAD
    /// </summary>
    internal class AcadClient
    {
        internal AcadClient()
        {
            //try {
            //    app = Activator.CreateInstance(Type.GetTypeFromProgID("AutoCAD.Application"));
            //    doc = app.ActiveDocument;
                
            //    status = "Успешное подключение к AutoCAD";
            //} catch (ArgumentException) {
            //    status = "Не удалось подключиться к AutoCAD. Будет использован эмулятор";
            //}
            
        }

        /// <summary>
        /// Получает геометрию из активного/текущего чертежа AutoCAD.
        /// </summary>
        internal AcadGeometry GetGeometry(Concrete concrete)
        {
            //var g = new AcadGeometry(concrete);
            //if (doc != null) {
            //    const string CMDECHO = "CMDECHO";
            //    int echo= doc.GetVariable(CMDECHO);
            //    doc.SetVariable(CMDECHO, echoOff);
            //} else {
                return GetConstGeometry(concrete);
            //}
            //return g;
        }

        /// <summary>
        /// Отображает элементы сжатой зоны голубым цветом на слое Compressed
        /// </summary>
        internal void ShowCompressed()
        {

        }

        /// <summary>
        /// Показывает элементы, относительные деформации сжатия в которых
        /// превышают предельные, красным цветом на слое Destroyed_Compression
        /// </summary>
        internal void ShowDestroyedCompression()
        { }

        /// <summary>
        /// Показывает элементы, относительные деформации растяжения в которых
        /// превышают предельные, желтым цветом на слое Destroyed_Tension
        /// </summary>
        internal void ShowDestroyedTension()
        { }

        /// <summary>
        /// Отображает "столбики" напряжений в бетоне.
        /// Напряжения в бетоне отображаются объектами на слоях Columns_Stress_Comp
        /// и Columns_Stress_Tens
        /// </summary>
        internal void ShowColumnsStressConcrete()
        { }

        /// <summary>
        /// Отобразить "столбики" напряжений в пользовательском материале.
        /// Напряжения в пользовательском материале отображаются объектами
        /// на слоях Columns_Stress_Comp и Columns_Stress_Tens
        /// </summary>
        internal void ShowColumnsStressUser()
        { }

        /// <summary>
        /// Отобразить "столбики" отн. деформаций.
        /// Относительные деформации отображаются объектами на слое Columns_strain
        /// </summary>
        internal void ShowColumnsStrain()
        { }

        /// <summary>
        /// Очистить чертеж
        /// </summary>
        internal void ClearDWG()
        { }

        /// <summary>
        /// Отобразить напряжения в арматуре, МПа
        /// </summary>
        internal void ShowReinfStress()
        { }

        /// <summary>
        /// Отобразить напряжения в бетоне, МПа
        /// </summary>
        internal void ShowConcreteStress()
        { }

        /// <summary>
        /// Отобразить напряжения в пользовательском материале, МПа
        /// </summary>
        internal void ShowUserStress()
        { }

        /// <summary>
        /// Отобразить напряжения в точках, МПа
        /// </summary>
        internal void ShowPointStress()
        { }

        /// <summary>
        /// Отобразить относительные деформации в элементах
        /// </summary>
        internal void ShowDeformation()
        { }

        /// <summary>
        /// Отобразить относительные деформации в точках
        /// </summary>
        internal void ShowPointDeformation()
        { 
        }

        /// <summary>
        /// Отобразить усилия в арматуре, кН
        /// </summary>
        internal void ShowReinfForce()
        { }

        /// <summary>
        /// Отобразить усилия в пользовательском материале, кН
        /// </summary>
        internal void ShowUserForce()
        { }

        /// <summary>
        /// Возвращает геометрию из исходной книги Excel
        /// </summary>
        /// <returns></returns>
        AcadGeometry GetConstGeometry(Concrete concrete)
        {
            var g = new AcadGeometry(concrete);
            var lines = File.ReadAllLines("geometry.tsv");
            var sep = new char[] { '\t' };
            string[] fields;
            const int handleCol = 0;
            const int areaCol = 1;
            const int typeCol = 2;
            const int xCol = 3;
            const int yCol = 4;
            const int weightCol = 24;
            var format = new NumberFormatInfo();
            format.NumberDecimalSeparator = ",";
            const int startRow = 1;
            for (int i = startRow; i < lines.Length; i++) {
                fields = lines[i].Split(sep);
                g.AddElement(new AcadElement(
                    fields[handleCol],
                    fields[typeCol],
                    Double.Parse(fields[xCol], format),
                    Double.Parse(fields[yCol], format),
                    Double.Parse(fields[weightCol],format),
                    Double.Parse(fields[areaCol], format)
                ));
            }
            return g;
        }

        /// <summary>
        /// Статус 
        /// </summary>
        public string Status
        {
            get { return status; }
        }

        /// <summary>
        /// Приложение AutoCAD
        /// </summary>
        readonly dynamic app;

        /// <summary>
        /// Чертёж AutoCAD
        /// </summary>
        readonly dynamic doc;
        
        string status;

        /// <summary>
        /// Режим отображения в командной строке AutoCAD отключён
        /// </summary>
        const int echoOff = 0;
    }
}
