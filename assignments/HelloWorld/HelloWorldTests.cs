using NUnit.Framework;

[TestFixture]
public class HelloWorldTests
{
    [Test]
    public void SayIt_GreetWithName_NameAttached()
    {
        IHelloWorld w = new HelloWorld();
        string result = w.SayIt("E.T.");
        Assert.AreEqual("Hello E.T.", result);
    }
    
    [Test]
    public void SayIt_GreetWithNull_NoNameAttached()
    {
        IHelloWorld w = new HelloWorld();
        string result = w.SayIt(null);
        Assert.AreEqual("Hello", result);
    }
    
    [Test]
    public void SayIt_GreetWithEmptyString_NoNameAttached()
    {
        IHelloWorld w = new HelloWorld();
        string result = w.SayIt("");
        Assert.AreEqual("Hello", result);
    }
}