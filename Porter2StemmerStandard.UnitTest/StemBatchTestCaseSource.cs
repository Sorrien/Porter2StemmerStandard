using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace Porter2StemmerStandard.UnitTest
{
    public class StemBatchTestCaseSource
    {
        public static IConfigurationRoot Configuration { get; set; }
        public static IEnumerable<TestCaseData> GetTestCaseData()
        {
            var builder = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("data.json", false, true);

            Configuration = builder.Build();

            var models = new List<BatchTestDataModel>();
            Configuration.Bind("TestData", models);

            foreach (var model in models)
            {
                yield return new TestCaseData(new object[] { model });
            }
        }
    }
}