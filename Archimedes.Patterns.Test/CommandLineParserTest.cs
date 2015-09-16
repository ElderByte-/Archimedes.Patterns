
using Archimedes.Patterns.CommandLine;
using NUnit.Framework;

namespace Archimedes.Patterns.Test
{
    [TestFixture]
    class CommandLineParserTest
    {

        [Test]
        public void ParseTest()
        {
            string[] args = {"/simple.prop", "hellowurld"};
            var options = CommandLineParser.ParseCommandLineArgs(args);
            Assert.AreEqual("hellowurld", options.GetParameterValue("simple.prop"));
        }

        [Test]
        public void ParseTestFlag2()
        {
            string[] args = { "-simple.prop", "hellowurld" };
            var options = CommandLineParser.ParseCommandLineArgs(args);
            Assert.AreEqual("hellowurld", options.GetParameterValue("simple.prop"));
        }

        /*
        [Test]
        public void ParseTestFlag3()
        {
            string[] args = { "--simple.prop", "hellowurld" };
            var options = CommandLineParser.ParseCommandLineArgs(args);
            Assert.AreEqual("hellowurld", options.GetParameterValue("simple.prop"));
        }*/


        [Test]
        public void ParseTestfAIL()
        {
            string[] args = { "\\simple.prop", "hellowurld" };  // Not working, since flag is wrong way.
            var options = CommandLineParser.ParseCommandLineArgs(args);
            Assert.AreEqual(null, options.GetParameterValue("simple.prop"));
        }

        [Test]
        public void ParseTestSpaces()
        {
            string[] args = { "/simple.filepath", @"C:\folder with spaces\and file with.txt" };
            var options = CommandLineParser.ParseCommandLineArgs(args);
            Assert.AreEqual(@"C:\folder with spaces\and file with.txt", options.GetParameterValue("simple.filepath"));
        }

        [Test]
        public void ParseBooleanFlag()
        {
            string[] args = { "/quiet", "True" };
            var options = CommandLineParser.ParseCommandLineArgs(args);
            Assert.AreEqual(true.ToString(), options.GetParameterValue("quiet"));
        }

        [Test]
        public void ParseBooleanFlagShorthand()
        {
            string[] args = { "/quiet" };
            var options = CommandLineParser.ParseCommandLineArgs(args);
            Assert.AreEqual(true.ToString(), options.GetParameterValue("quiet"));
        }

        [Test]
        public void ParseTestSpecail()
        {
            string[] args = { "/erp.connection", @"C:\Temp\Planung\DATA-007_Zu" };
            var options = CommandLineParser.ParseCommandLineArgs(args);
            Assert.AreEqual(@"C:\Temp\Planung\DATA-007_Zu", options.GetParameterValue("erp.connection"));
        }
    }
}
