using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace MoCS.BuildService.Business.Tests
{
    [TestFixture]
    public class SubmitSettingsTests
    {
        [Test]
        public void GetDllName_SomeValues_TEAM01_22_20091012_134434777()
        {
            SubmitSettings set = new SubmitSettings();
            set.TeamId = "TEAM01";
            DateTime dt = new DateTime(2009, 10, 12, 13, 44, 34, 777);
            set.TimeStamp = dt;
            string basePath = @"c:\temp\";
            set.BasePath = basePath;
            set.AssignmentId = "22";
                
            Assert.AreEqual("TEAM01", set.TeamId);
            Assert.AreEqual(basePath, set.BasePath);
            Assert.AreEqual(dt, set.TimeStamp);
            Assert.AreEqual("22", set.AssignmentId);

            Assert.AreEqual("TEAM01_22_20091012_134434777.dll", set.GetDllName());




        }
    }
}
