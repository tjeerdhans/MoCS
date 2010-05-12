using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace MoCS.BuildService.Business.Tests
{
    [TestFixture]
    public class UtilsTests
    {
        [Test]
        public void DateToString_MinimumDate_00010101()
        {
            DateTime d = new DateTime();
            string result = Utils.DateToString(d);
            Assert.AreEqual("00010101", result);
       }

        [Test]
        public void DateToString_12dec2009_20091231()
        {
            DateTime d = new DateTime(2009,12,31);
            string result = Utils.DateToString(d);
            Assert.AreEqual("20091231", result);
        }

        [Test]
        public void TimeToString_minimal_000000000()
        {
            DateTime d = new DateTime();
            string result = Utils.TimeToString(d);
            Assert.AreEqual("000000000", result);
        }


        [Test]
        public void TimeToString_sometime_220304088()
        {
            DateTime d = new DateTime(2009, 12,31,22,3,4,88);
            string result = Utils.TimeToString(d);
            Assert.AreEqual("220304088", result);
        }


    }
}
