using NUnit.Framework;

[TestFixture]
public class HelloWorldTestsServer
{
    [Test]
    public void SayIt_GreetWithSecretName_NameAttached()
    {
        IHelloWorld w = new HelloWorld();
        string name = "Yoda";
        string result = w.SayIt(name);

        bool ok = result.Equals("Hello Yoda");
        Assert.IsTrue(ok, "result is not ok.");
    }
    
    
}