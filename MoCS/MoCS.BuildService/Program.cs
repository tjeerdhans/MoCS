using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using MoCS.Business.Facade;
using MoCS.Business.Objects;

namespace MoCS.BuildService
{
    class Program
    {
        private static SubmitWatcher _watcher;

        static void Main(string[] args)
        {
            _watcher = new SubmitWatcher();
            Console.WriteLine("Start watching for submits to process.. Press enter to quit.");
            _watcher.StartWatching();

            Console.ReadLine();
        }
    }
}
