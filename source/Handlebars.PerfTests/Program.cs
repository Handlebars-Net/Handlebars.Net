using System;
using System.Diagnostics;
using System.IO;
using Handlebars.PerfTests.Models;

namespace Handlebars.PerfTests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            TestBigTemplate(1); 
            TestBigTemplate(100);    
            TestBigTemplate(500);    
            TestBigTemplate(1000);   
            TestBigTemplate(5000);   
        }

        private static Action<TextWriter, object> GetTemplate(string templatePath)
        {
            var handlebars = HandlebarsDotNet.Handlebars.Create();
            Action<TextWriter, object> template;

            using (var stream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(stream))
                {
                    template = handlebars.Compile(reader);
                }
            }

            return template;
        }

        private static void TestBigTemplate(int times)
        {
            var template = GetTemplate("./Templates/BigTemplate.hbs");

            using (var memStream = new MemoryStream())
            {
                using (var writer = new StreamWriter(memStream))
                {
                    var model = DataModel.GetRandom(times);

                    var stopWatch = new Stopwatch();
                    stopWatch.Start();

                    template(writer, model);

                    stopWatch.Stop();
                    Console.WriteLine("Rendered the data {0} time in {1:g}", times, stopWatch.Elapsed);
                }
            }
        }

        //private static void TestBigTemplateMulti()
        //{
        //    var template = GetTemplate("./Templates/BigTemplate.hbs");
        //    var model = Model.CreateRandom();

        //    Stopwatch sw;

        //    using (var memStream = new MemoryStream())
        //    {
        //        using (var writer = new StreamWriter(memStream))
        //        {
        //            sw = new Stopwatch();
        //            sw.Start();

        //            for (int i = 0; i < times; i++)
        //            {
        //                template(writer, model);
        //            }
        //        }
        //    }

        //    sw.Stop();

        //    Console.WriteLine("Rendered the template {0} times in {1:g}", times, sw.Elapsed);
        //}
    }
}
