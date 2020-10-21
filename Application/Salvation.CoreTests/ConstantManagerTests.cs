using NUnit.Framework;
using Salvation.Core.Constants;
using System;

namespace Salvation.CoreTests
{
    public class ConstantManagerTests
    {
        private ConstantsService GetCM()
        {
            return new ConstantsService();
        }

        [Test]
        public void CMLoadsConstantsFile()
        {
            var cm = GetCM();
            var result = cm.LoadConstantsFromFile();

            Console.Write(result);
        }

        [Test]
        public void CMHasDefaults()
        {
            var cm = GetCM();

            Assert.IsNotEmpty(cm.DefaultFilename);
            Assert.IsNotNull(cm.DefaultDirectory);
        }

        [Test]
        public void CMDirectoryUpdates()
        {
            var cm = GetCM();

            var newDirectory = "test";

            cm.SetDefaultDirectory(newDirectory);

            Assert.AreEqual(newDirectory, cm.DefaultDirectory);
        }

        [Test]
        public void CMFileUpdates()
        {
            var cm = GetCM();

            var newFile = "test.json";

            cm.SetDefaultFilename(newFile);

            Assert.AreEqual(newFile, cm.DefaultFilename);
        }
    }
}
