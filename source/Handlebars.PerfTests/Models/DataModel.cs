using System.Collections.Generic;

namespace Handlebars.PerfTests.Models
{
    public class DataModel
    {
        public List<Model> Data { get; set; }

        public static DataModel GetRandom(int times)
        {
            var obj = new DataModel()
            {
                Data = new List<Model>()
            };

            for (int i = 0; i < times; i++)
            {
                obj.Data.Add(Model.CreateRandom());
            }

            return obj;
        }
    }
}
