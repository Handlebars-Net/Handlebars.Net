using System;
using Exceptionless;

namespace Handlebars.PerfTests.Models
{
    public class Obj
    {
        public string Field1 { get; set; }
        public int Field2 { get; set; }
        public double Field3 { get; set; }
        public DateTime Field4 { get; set; }
        public Guid Field5 { get; set; }

        public int? Field6 { get; set; }
        public double? Field7 { get; set; }
        public float? Field8 { get; set; }
        public Guid? Field9 { get; set; }

        public string Field10 { get; set; }
        public int Field11 { get; set; }
        public double Field12 { get; set; }
        public DateTime Field13 { get; set; }
        public Guid Field14 { get; set; }

        public int? Field15 { get; set; }
        public double? Field16 { get; set; }
        public DateTime? Field17 { get; set; }
        public Guid? Field18 { get; set; }

        public string Field19 { get; set; }
        public string Field20 { get; set; }

        public static Obj CreateRandom()
        {
            var obj = new Obj
            {
                Field1 = RandomData.GetAlphaNumericString(),
                Field2 = RandomData.GetInt(),
                Field3 = RandomData.GetDouble(),
                Field4 = RandomData.GetDateTime(),
                Field5 = new Guid(),

                Field6 = null,
                Field7 = null,
                Field8 = null,
                Field9 = null,

                Field10 = RandomData.GetAlphaNumericString(),
                Field11 = RandomData.GetInt(),
                Field12 = RandomData.GetDouble(),
                Field13 = RandomData.GetDateTime(),
                Field14 = new Guid(),

                Field15 = RandomData.GetInt(),
                Field16 = RandomData.GetDouble(),
                Field17 = RandomData.GetDateTime(),
                Field18 = new Guid(),

                Field19 = RandomData.GetAlphaNumericString(),
                Field20 = RandomData.GetAlphaNumericString(),
            };

            return obj;
        }

    }
}
