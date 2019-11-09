using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Porter2StemmerStandard.UnitTest
{
    public class StemBatchTestCaseSource
    {
        public static IEnumerable<BatchTest> GetTestCaseData()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "data.json");
            var json = File.ReadAllText(path);
            var models = JsonConvert.DeserializeObject<BatchTestDataModel>(json);

            return models.TestData;
        }
    }
}