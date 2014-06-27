

namespace TestApp2
{
    public class UnitTestPumgrana
    {
        //private MainPage App {get;set;}
        public Pumgrana.PumgranaWebClient WebClient { get; set; }
        public UnitTestPumgrana()
        {
            //this.App = new MainPage();
            this.WebClient = new Pumgrana.PumgranaWebClient();
        }
    }
}

