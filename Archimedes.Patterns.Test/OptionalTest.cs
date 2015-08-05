using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Archimedes.Patterns.Test
{
    [TestFixture]
    public class OptionalTest
    {
        [Test]
        public void TestEmpty()
        {
            var emptyStr = Optional.Empty<string>();
            var emptyInt = Optional.Empty<int>();
            var emptyObj = Optional.Empty<object>();

            Assert.AreEqual(false, emptyStr.IsPresent);
            Assert.AreEqual(false, emptyInt.IsPresent);
            Assert.AreEqual(false, emptyObj.IsPresent);
        }

        [Test]
        public void TestValue()
        {
            var emptyStr = Optional.Of("Hello");
            var emptyInt = Optional.Of(12);

            var obj = new object();

            var emptyObj = Optional.Of(obj);

            Assert.AreEqual(true, emptyStr.IsPresent);
            Assert.AreEqual(true, emptyInt.IsPresent);
            Assert.AreEqual(true, emptyObj.IsPresent);

            Assert.AreEqual("Hello", emptyStr.Value);
            Assert.AreEqual(12, emptyInt.Value);
            Assert.AreEqual(obj, emptyObj.Value);
        }

    }
}
