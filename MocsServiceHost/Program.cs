using System;
using System.ServiceModel;

namespace MocsServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ServiceHost host = new ServiceHost(typeof(Notify)))
            {
                host.Open();
                Console.WriteLine("Mocs ServiceHost Started");
                Console.WriteLine("Press Enter to quit");
                Console.ReadLine();
            }
        }
    }
}
