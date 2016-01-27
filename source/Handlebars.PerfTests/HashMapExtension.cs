using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Exceptionless;
using Handlebars.PerfTests.Models;

namespace Handlebars.PerfTests
{
    public class HashMapExtension
    {
        public static Dictionary<string, object> CreateRandomHashMap()
        {
            var obj = new Dictionary<string, object>
            {
                { "A", RandomData.GetInt() },
                { "B", RandomData.GetAlphaNumericString() },
                { "C", RandomData.GetDateTime() },
                { "D", RandomData.GetDouble() },
                { "E", RandomData.GetAlphaString() },
                { "F", RandomData.GetBool() },
                { "G", RandomData.GetCoordinate() },
                { "H", RandomData.GetDateTimeOffset() },
                { "I", RandomData.GetDecimal() },
                { "J", null },
                { "K", new Guid() },
                { "L", RandomData.GetParagraphs() },
                { "M", RandomData.GetTimeSpan() },
                { "N", Obj.CreateRandom() },
                { "O", RandomData.GetLong() },
                { "P", RandomData.GetIp4Address() },
                { "Q", RandomData.GetDecimal() },
                { "R", RandomData.GetAlphaString() },
                { "S", RandomData.GetInt() },
                { "T", RandomData.GetCoordinate() },
                { "U", RandomData.GetTitleWords() },
                { "V", RandomData.GetSentence() },
                { "W", "" },
                { "X", Guid.Empty },
                { "Y", new object() },
                { "Z", DateTime.UtcNow },
            };

            return obj;
        } 
    }
}
