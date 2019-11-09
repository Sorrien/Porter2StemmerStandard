using System.Collections.Generic;

namespace Porter2StemmerStandard.UnitTest
{
    public class BatchTest
    {
        public string Unstemmed { get; set; }
        public string Expected { get; set; }
    }

    public class BatchTestDataModel
    {
        public List<BatchTest> TestData { get; set; }
    }
}