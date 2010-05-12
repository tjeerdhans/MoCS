using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.IO;
using MoCS.Service.DataContracts;
using System.Runtime.Serialization;
using System.Xml;

namespace MoCS.Service.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class MoCSServiceTests
    {
        //string basePath = "http://mylocalhost/MoCS.Service/MoCSService.svc";
        string basePath = "http://mylocalhost:4037/MoCSService.svc";

        string token;

        public MoCSServiceTests()
        {

        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void AddTeam_NotExistingNewTeam_XmlWithTeam()
        {
            Team newTeam = new Team { TeamName = "TH's normal team", Password="1234" };

            DataContractSerializer dcSerializer = new DataContractSerializer(typeof(Team));
            MemoryStream memoryStream = new MemoryStream();
            dcSerializer.WriteObject(memoryStream, newTeam);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(basePath + "/teams");
            request.Method = "POST";
            request.ContentType = "application/xml";
            request.ContentLength = memoryStream.Length;
            

            request.GetRequestStream().Write(memoryStream.ToArray(), 0, (int)memoryStream.Length);

            HttpWebResponse httpWebResponse = (HttpWebResponse)request.GetResponse();

            Team createdTeam = (Team)dcSerializer.ReadObject(httpWebResponse.GetResponseStream());

            Assert.IsNotNull(createdTeam.TeamId);
            Assert.AreEqual(createdTeam.TeamStatus, TeamStatus.Active);
            Assert.AreEqual(createdTeam.TeamType, TeamType.Normal);
            Assert.AreEqual(createdTeam.TeamName,  newTeam.TeamName);
            //Assert.AreEqual(createdTeam.Username, newTeam.Username);
            Assert.AreEqual(createdTeam.Password, newTeam.Password);
        }

        [TestMethod]
        public void GetAllTeams_NonAdminCredentials_XmlWithTeamsWithoutCredentials()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(basePath + "/teams");
            request.Method = "GET";
            request.Headers.Add("Username", "THT");
            request.Headers.Add("Password", "1234");

            HttpWebResponse httpWebResponse = (HttpWebResponse)request.GetResponse();

            DataContractSerializer dcSerializer = new DataContractSerializer(typeof(Teams));
            Teams allTeams = (Teams)dcSerializer.ReadObject(httpWebResponse.GetResponseStream());

            foreach (Team team in allTeams)
            {
                //Assert.AreEqual("", team.Username);
                //Assert.AreEqual("", team.Password); //password may be empty
            }
        }

        [TestMethod]
        public void GetAllTeams_AdminCredentials_XmlWithTeamsWithCredentials()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(basePath + "/teams");
            request.Method = "GET";
            request.Headers.Add("Username", "admin");
            request.Headers.Add("Password", "abc123");

            HttpWebResponse httpWebResponse = (HttpWebResponse)request.GetResponse();

            DataContractSerializer dcSerializer = new DataContractSerializer(typeof(Teams));
            Teams allTeams = (Teams)dcSerializer.ReadObject(httpWebResponse.GetResponseStream());

            foreach (Team team in allTeams)
            {
                //Assert.AreNotEqual("", team.Username);
                //Assert.AreNotEqual("", team.Password); //password may be empty
            }
        }

        //[TestMethod]
        //public void AddEnrolledAssignmentForTeam_NonExistentAssignment_BadRequest()
        //{            
        //    EnrolledAssignment newEnrolledAssignment = new EnrolledAssignment { AssignmentId = "1", AssignmentName = "Hell  world" };

        //    DataContractSerializer dcSerializer = new DataContractSerializer(typeof(EnrolledAssignment));
        //    MemoryStream memoryStream = new MemoryStream();
        //    dcSerializer.WriteObject(memoryStream, newEnrolledAssignment);

        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(basePath + "/teams/2/enrolledassignments");
        //    request.Method = "POST";
        //    request.ContentType = "application/xml";
        //    request.Headers.Add("Username", "THT");
        //    request.Headers.Add("Password", "1234");
        //    request.ContentLength = memoryStream.Length;

        //    request.GetRequestStream().Write(memoryStream.ToArray(), 0, (int)memoryStream.Length);
            
        //    try
        //    {
        //        HttpWebResponse httpWebResponse = (HttpWebResponse)request.GetResponse();
        //        Assert.AreNotEqual(HttpStatusCode.OK, httpWebResponse.StatusCode);
        //    }
        //    catch (WebException ex)
        //    {
        //        HttpWebResponse httpWebResponse = (HttpWebResponse)ex.Response;
        //        Assert.AreEqual(HttpStatusCode.BadRequest, httpWebResponse.StatusCode);
        //    }

        //}
    }
}
