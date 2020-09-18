using Microsoft.VisualBasic;
using Newtonsoft.Json;
using NUnit.Framework;
using Salvation.Core;
using Salvation.Core.Constants;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Salvation.CoreTests
{
    public class ConstantManagerTests
    {
        private ConstantsManager GetCM()
        {
            return new ConstantsManager();
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

            Assert.IsNotEmpty(cm.DefaultFileName);
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

            cm.SetDefaultFileName(newFile);

            Assert.AreEqual(newFile, cm.DefaultFileName);
        }
    }
}
