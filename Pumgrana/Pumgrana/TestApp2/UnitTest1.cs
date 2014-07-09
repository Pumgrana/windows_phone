using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TestApp2
{
    // [FR] - Ces tests unitaires ont été réalisé avec succès avec et sans connexion à Internet
    // [EN] - Those unit tests have been successfully realized with and without internet connection
    [TestClass]
    public class UnitTest1
    {
        private UnitTestPumgrana p { get; set; }
        [TestInitialize]
        public void Initialize()
        {
            p = new UnitTestPumgrana();
        }

        [TestMethod]
        public async Task WebClient_GetContents()
        {
            p.WebClient.getContents();
            await p.WebClient.tcs.Task;
            Pumgrana.ListContent lc = p.WebClient.tcs.Task.Result as Pumgrana.ListContent;
            Assert.IsNull(lc, "No data received");
            foreach (Pumgrana.Content c in lc.contents)
            {
                Assert.IsNull(c.uri, "Id null !");
            }
        }

        [TestMethod]
        public async Task Web_Client_CreateTag()
        {
            p.WebClient.PostTag("Booyah");
            await p.WebClient.tcs.Task;

            p.WebClient.PostTag("League of legends");
            await p.WebClient.tcs.Task;

            p.WebClient.PostTag("Visual Studio");
            await p.WebClient.tcs.Task;

            p.WebClient.PostTag("Britney Spears");
            await p.WebClient.tcs.Task;
        }

        [TestMethod]
        public async Task Web_Client_Create_Content()
        {
            Pumgrana.Content ToAdd = new Pumgrana.Content();
            ToAdd.summary = "Summary";
            ToAdd.text = "text";
            ToAdd.title = "title";
            List<string> Tags = new List<string>();
            Tags.Add("Tag N1");
            Tags.Add("Tag N2");
            Tags.Add("Tag N3");
            p.WebClient.CreateContent(ToAdd, Tags);
            await p.WebClient.tcs.Task;
        }

        [TestCleanup]
        public void cleanUp()
        {
        }
    }
}
