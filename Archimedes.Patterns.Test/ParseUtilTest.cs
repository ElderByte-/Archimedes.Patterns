using System;
using Archimedes.Patterns.Utils;
using NUnit.Framework;

namespace Archimedes.Patterns.Test
{
    [TestFixture]
    public class ParseUtilTest
    {
        [Test]
        public void TestSimple()
        {
            Assert.AreEqual("hello world", ParseUtil.Parse<string>("hello world"));
            Assert.AreEqual(12, ParseUtil.Parse<int>("12"));
            Assert.AreEqual(321333423433, ParseUtil.Parse<long>("321333423433"));
            Assert.AreEqual(3213.33423433f, ParseUtil.Parse<float>("3213.33423433"));
            Assert.AreEqual(3213.33423433, ParseUtil.Parse<double>("3213.33423433"));
            Assert.AreEqual(true, ParseUtil.Parse<bool>("true"));
        }

        [Test]
        public void TestParseComplex()
        {
            Assert.AreEqual(MessageMatch.Regex, ParseUtil.Parse<MessageMatch>("Regex"));
            Assert.AreEqual(Guid.Parse("9A87A428-5E16-485F-833C-AAAE7B2CF203"), ParseUtil.Parse<Guid>("9A87A428-5E16-485F-833C-AAAE7B2CF203"));
        }


        [Test]
        public void TestParseObjects()
        {
            object simpleString = "Hello world";
            object simpleNumber = 12;
            object simpleNumberStr = "12";

            Assert.AreEqual("Hello world", ParseUtil.Parse<string>(simpleString));
            Assert.AreEqual(12, ParseUtil.Parse<int>(simpleNumber));
            Assert.AreEqual(12, ParseUtil.Parse<int>(simpleNumberStr));
        }


        [Test]
        public void TestParseObjectsOptional()
        {
            object simpleString = "Hello world";
            Assert.AreEqual("Hello world", ParseUtil.ParseSave<string>(simpleString).Value);
        }


        [Test]
        public void TestParseObjectsOptionalNUmber()
        {
            object simpleNumber = 12;
            object simpleNumberStr = "12";

            Assert.AreEqual(12, ParseUtil.ParseSave<int>(simpleNumber).Value);
            Assert.AreEqual(12, ParseUtil.ParseSave<int>(simpleNumberStr).Value);
        }

        [Test]
        public void TestParseObjectsOptionalFail()
        {
            object simpleNull = null;
            object testObj = new object();
            object testStr = "xyz";

            Assert.AreEqual(false, ParseUtil.ParseSave<int>(simpleNull).IsPresent); 
            Assert.AreEqual(false, ParseUtil.ParseSave<int>(testObj).IsPresent);
            Assert.AreEqual(false, ParseUtil.ParseSave<int>(testStr).IsPresent);


        }
    }
}
