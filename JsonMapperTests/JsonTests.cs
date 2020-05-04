using JsonMapper.Services;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Collections.Generic;

namespace JsonMapperTests
{
    [TestFixture]
    public class JsonTests
    {
        ValueMapper mapper { get; set; }

        [SetUp]
        public void SetUp()
        {
            mapper = new ValueMapper();
        }

        [Test]
        public void TryPropertyMap()
        {
            List<string> testInstructions = new List<string> { "PropertyMap(target=\"basicParent.joo.mursu\", source=\"PropertySource\")" };
            string testValuesToMap = @"C:\\testFile.csv";
            var testTemplate = new
            {
                basicParent = new
                {
                    joo = new
                    {
                        mursu = ""
                    }
                }
            };

            mapper.SetInstructions(testInstructions);
            mapper.SetValues(testValuesToMap, ',');
            mapper.SetTemplate(JObject.FromObject(testTemplate));
            List<JObject> resultList = mapper.MapValues();

            Assert.IsTrue(resultList.Count == 8);
        }

        [Test]
        public void TryPropertyValueInserter()
        {
            List<string> testInstructions = new List<string> { "PropertyValueInserter(target=\"basicParent.joo.mursu\", source=\"PropertySource\", baseText=\"This animal is {replace} and it's awesome!\", replaceKey=\"{replace}\")" };
            string testValuesToMap = @"C:\\testFile.csv";
            var testTemplate = new
            {
                basicParent = new
                {
                    joo = new
                    {
                        mursu = ""
                    }
                }
            };

            mapper.SetInstructions(testInstructions);
            mapper.SetValues(testValuesToMap, ',');
            mapper.SetTemplate(JObject.FromObject(testTemplate));
            List<JObject> resultList = mapper.MapValues();

            Assert.IsTrue(resultList.Count == 8);
            Assert.IsTrue((string)resultList[0].SelectToken("basicParent.joo.mursu") == "This animal is joku and it's awesome!");
        }

        [Test]
        public void TryPropertyReader()
        {
            var testTemplate = new
            {
                basicParent = new
                {
                    joo = new
                    {
                        mursu = ""
                    }
                }
            };
            mapper.SetTemplate(JObject.FromObject(testTemplate));
            List<string> resultList = mapper.NodeValues;
            Assert.IsTrue(resultList.Count == 3);
            string moreComplexJson = "{\"array\":[1,2,3],\"boolean\":true,\"color\":\"#82b92c\",\"null\":null,\"number\":123,\"object\":{\"a\":\"b\",\"c\":\"d\",\"e\":\"f\"},\"string\":\"Hello World\"}";
            mapper.SetTemplate(JObject.Parse(moreComplexJson));
            resultList = mapper.NodeValues;
            Assert.IsTrue(resultList.Count == 10);
            moreComplexJson = "{\"id\":\"0001\",\"type\":\"donut\",\"name\":\"Cake\",\"ppu\":0.55,\"batters\":{\"batter\":[{\"id\":\"1001\",\"type\":\"Regular\"},{\"id\":\"1002\",\"type\":\"Chocolate\"},{\"id\":\"1003\",\"type\":\"Blueberry\"},{\"id\":\"1004\",\"type\":\"Devil's Food\"}]},\"topping\":[{\"id\":\"5001\",\"type\":\"None\"},{\"id\":\"5002\",\"type\":\"Glazed\"},{\"id\":\"5005\",\"type\":\"Sugar\"},{\"id\":\"5007\",\"type\":\"Powdered Sugar\"},{\"id\":\"5006\",\"type\":\"Chocolate with Sprinkles\"},{\"id\":\"5003\",\"type\":\"Chocolate\"},{\"id\":\"5004\",\"type\":\"Maple\"}]}";
            mapper.SetTemplate(JObject.Parse(moreComplexJson));
            resultList = mapper.NodeValues;
            Assert.IsTrue(resultList.Count == 29);
        }

        [Test]
        public void TrySiblingPropertyMap()
        {
            List<string> testInstructions = new List<string> { "SiblingPropertyMap(target=\"object.a\", source=\"PropertySource\"), siblingName=\"e\", siblingValue=\"f\"" };
            string testValuesToMap = @"C:\\testFile.csv";
            string testTemplate = "{\"array\":[1,2,3],\"boolean\":true,\"color\":\"#82b92c\",\"null\":null,\"number\":123,\"object\":{\"a\":\"b\",\"c\":\"d\",\"e\":\"f\"},\"string\":\"Hello World\"}";

            mapper.SetInstructions(testInstructions);
            mapper.SetValues(testValuesToMap, ',');
            mapper.SetTemplate(JObject.Parse(testTemplate));
            List<JObject> resultList = mapper.MapValues();

            Assert.IsTrue(resultList.Count == 8);
            Assert.IsTrue((string)resultList[0].SelectToken("object.a") == "joku");
        }

        [Test]
        public void TrySiblingPropertyMapSiblingValueWrong()
        {
            List<string> testInstructions = new List<string> { "SiblingPropertyMap(target=\"object.a\", source=\"PropertySource\"), siblingName=\"e\", siblingValue=\"b\"" };
            string testValuesToMap = @"C:\\testFile.csv";
            string testTemplate = "{\"array\":[1,2,3],\"boolean\":true,\"color\":\"#82b92c\",\"null\":null,\"number\":123,\"object\":{\"a\":\"b\",\"c\":\"d\",\"e\":\"f\"},\"string\":\"Hello World\"}";

            mapper.SetInstructions(testInstructions);
            mapper.SetValues(testValuesToMap, ',');
            mapper.SetTemplate(JObject.Parse(testTemplate));
            List<JObject> resultList = mapper.MapValues();

            Assert.IsTrue(resultList.Count == 8);
            Assert.IsFalse((string)resultList[0].SelectToken("object.a") == "joku");
            Assert.IsTrue((string)resultList[0].SelectToken("object.a") == "b");
        }

        [Test]
        public void TrySiblingPropertyMapAndValueInserter()
        {
            List<string> testInstructions = new List<string> { "SiblingPropertyMap(target=\"object.a\", source=\"PropertySource\", siblingName=\"e\", siblingValue=\"f\")", "PropertyValueInserter(target=\"object.c\", source=\"PropertySource\", baseText=\"This animal is {replace} and it's awesome!\", replaceKey=\"{replace}\")" };
            string testValuesToMap = @"C:\\testFile.csv";
            string testTemplate = "{\"array\":[1,2,3],\"boolean\":true,\"color\":\"#82b92c\",\"null\":null,\"number\":123,\"object\":{\"a\":\"b\",\"c\":\"d\",\"e\":\"f\"},\"string\":\"Hello World\"}";

            mapper.SetInstructions(testInstructions);
            mapper.SetValues(testValuesToMap, ',');
            mapper.SetTemplate(JObject.Parse(testTemplate));
            List<JObject> resultList = mapper.MapValues();

            Assert.IsTrue(resultList.Count == 8);
            Assert.IsTrue((string)resultList[0].SelectToken("object.a") == "joku");
            Assert.IsTrue((string)resultList[0].SelectToken("object.c") == "This animal is joku and it's awesome!");

        }
    }
}

