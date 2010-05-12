using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ServiceModel.Web;
using System.Configuration;
using System.Reflection;
using System.Xml;
using System.ServiceModel.Description;

namespace MoCS.Service.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {

            //System.ServiceModel.WebHttpBinding binding = new System.ServiceModel.WebHttpBinding();
            //binding.ReaderQuotas = new XmlDictionaryReaderQuotas();
            //binding.ReaderQuotas.MaxArrayLength = XmlDictionaryReaderQuotas.Max.MaxArrayLength;
            //binding.ReaderQuotas.MaxBytesPerRead = XmlDictionaryReaderQuotas.Max.MaxBytesPerRead;
            //binding.ReaderQuotas.MaxDepth = XmlDictionaryReaderQuotas.Max.MaxDepth;
            //binding.ReaderQuotas.MaxNameTableCharCount = XmlDictionaryReaderQuotas.Max.MaxNameTableCharCount;
            //binding.ReaderQuotas.MaxStringContentLength = XmlDictionaryReaderQuotas.Max.MaxStringContentLength;
            //binding.MaxReceivedMessageSize = 50000000;

            WebServiceHost2 host = new WebServiceHost2(typeof(MoCSService), true, new Uri(ConfigurationManager.AppSettings["ServiceBaseAddress"]))
            {
                EnableAutomaticHelpPage = false,
                MaxMessageSize = int.MaxValue
            };
            //            host.AddServiceEndpoint(typeof(IMoCSService), binding, new Uri(ConfigurationManager.AppSettings["ServiceBaseAddress"])); 


            ////setup debug behaviour for getting the errors in the client.
            //    ServiceDebugBehavior debugBehaviour;
            //    debugBehaviour = host.Description.Behaviors.Find<ServiceDebugBehavior>();
            //    if (debugBehaviour == null)
            //    {
            //        debugBehaviour = new ServiceDebugBehavior();
            //        debugBehaviour.IncludeExceptionDetailInFaults = true;
            //        host.Description.Behaviors.Add(debugBehaviour);
            //    }
            //    else
            //    {
            //        debugBehaviour.IncludeExceptionDetailInFaults = true;
            //    }


            //if (host.ReaderQuotas != null)
            //{
            //    host.ReaderQuotas.MaxArrayLength = XmlDictionaryReaderQuotas.Max.MaxArrayLength;
            //}


            host.Open();

            //if (host.ReaderQuotas == null)
            //{
            //    host.ReaderQuotas = new XmlDictionaryReaderQuotas();
            //}

            //host.ReaderQuotas.MaxArrayLength = XmlDictionaryReaderQuotas.Max.MaxArrayLength;
            //host.ReaderQuotas.MaxBytesPerRead = XmlDictionaryReaderQuotas.Max.MaxBytesPerRead;
            //host.ReaderQuotas.MaxDepth = XmlDictionaryReaderQuotas.Max.MaxDepth;
            //host.ReaderQuotas.MaxNameTableCharCount = XmlDictionaryReaderQuotas.Max.MaxNameTableCharCount;
            //host.ReaderQuotas.MaxStringContentLength = XmlDictionaryReaderQuotas.Max.MaxStringContentLength;

            string version = Assembly.GetAssembly(typeof(MoCSService)).GetName().Version.ToString();

            Console.WriteLine("MoCSService version " + version + " started.");
            Console.WriteLine("Base address: " + ConfigurationManager.AppSettings["ServiceBaseAddress"]);
            Console.WriteLine("Press Enter to stop the service.");
            Console.ReadLine();
            host.Close();
        }
    }
}
