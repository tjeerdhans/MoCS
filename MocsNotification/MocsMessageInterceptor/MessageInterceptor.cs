using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace MocsMessageInterceptor
{
    public class MessageInterceptor
    {
        private List<IMessageInterceptor> _messageInterceptorList = new List<IMessageInterceptor>();

        public void Initialize()
        {
            try
            {
                string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string interceptorPath = Path.Combine(assemblyPath, "Interceptor");

                if (!Directory.Exists(interceptorPath))
                {
                    Directory.CreateDirectory(interceptorPath);
                }

                DirectoryInfo directoryInfo = new DirectoryInfo(interceptorPath);

                FileInfo[] files = directoryInfo.GetFiles("*.dll");

                for (int i = 0; i < files.Length; i++)
                {

                    Assembly assembly = Assembly.LoadFrom(files[i].FullName);
                    Type[] classes = assembly.GetTypes();
                    for (int j = 0; j < classes.Length; j++)
                    {
                        Type interceptorInterface = classes[j].GetInterface("IMessageInterceptor", true);
                        if (interceptorInterface != null)
                        {
                            IMessageInterceptor messageInterceptor = Activator.CreateInstance(classes[j]) as IMessageInterceptor;
                            _messageInterceptorList.Add(messageInterceptor);
                        }
                    }

                }
             
            }
            catch
            {
                MessageBox.Show("Exception: Initialize MessageInterceptor");
            }
        }

        public void ProcessMessage(string message)
        {
            foreach (IMessageInterceptor messageInterceptor in _messageInterceptorList)
            {
                try
                {
                    ParameterizedThreadStart threadStart = new ParameterizedThreadStart(ProcessInterceptor);
                    Thread thread = new Thread(threadStart);
                    thread.Start(new InterceptorJob { MessageInterceptor = messageInterceptor, Message = message });                                    
                }
                catch
                {
                    MessageBox.Show("Exception: ProcessMessage");
                }
            }
        }


        private void ProcessInterceptor(object stateInfo)
        {
            InterceptorJob job = stateInfo as InterceptorJob;
            if(job!=null)
            {
                job.MessageInterceptor.ProcessMessage(job.Message);
            }
        }
    }
}
