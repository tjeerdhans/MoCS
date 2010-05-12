using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace MoCS.BuildService.Business.Tests
{
    [TestFixture]
    public class SubmitResultTests
    {
        [Test]
        public void Constructor_NoParameters_TestProperties()
        {
            SubmitResult r = new SubmitResult();
            Assert.AreEqual(SubmitStatusCode.Unknown, r.Status);
            Assert.AreEqual(0, r.Messages.Count);
        }
    }
}
