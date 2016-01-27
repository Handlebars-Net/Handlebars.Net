using System;
using System.Collections.Generic;
using System.Dynamic;
using Exceptionless;

namespace Handlebars.PerfTests.Models
{
    public class Model
    {
        public Obj Obj { get; set; }

        public Dictionary<string, object> HashMap { get; set; }

        public dynamic DynamicObject { get; set; }

        public Dictionary<int, double> DoubleDict { get; set; }

        public List<string> Coll { get; set; }

        public static Model CreateRandom()
        {
            var obj = new Model()
            {
                Obj = Obj.CreateRandom(),
                HashMap = HashMapExtension.CreateRandomHashMap(),
                DynamicObject = new ExpandoObject(),
                DoubleDict = new Dictionary<int, double>
                {
                    { 0, double.Epsilon },
                    { 1, Math.PI },
                    { 2, 0.0 }
                },
                Coll = new List<string>()
                {
                    RandomData.GetAlphaString(),
                    RandomData.GetAlphaString(),
                    RandomData.GetAlphaString(),
                    RandomData.GetAlphaString(),
                    RandomData.GetAlphaString(),
                    RandomData.GetAlphaString(),
                    RandomData.GetAlphaString()
                }
            };

            obj.DynamicObject.Alpha = RandomData.GetAlphaString();
            obj.DynamicObject.Beta = RandomData.GetInt();
            obj.DynamicObject.Gamma = new Guid();
            obj.DynamicObject.Delta = new object();
            obj.DynamicObject.Epsilon = RandomData.GetLong();

            return obj;
        }


    }
}
