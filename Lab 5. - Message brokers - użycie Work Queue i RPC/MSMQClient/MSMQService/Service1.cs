using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IO;
using System.Diagnostics;


namespace MSMQService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class Service1 : IService1
    {
        public void SendToOutputWindow(string msg, DateTime sendDate)
        {
            Debug.WriteLine(msg + " Otrzymana: " + System.DateTime.Now + " Wysłana: " + sendDate);
        }

        private static readonly object SyncObject = new object();
        private static readonly TextWriter W = new StreamWriter("log.txt", true);

        public void SendLockMessage(string msg, DateTime sendDate)
        {
            lock (SyncObject)
            {
                W.WriteLine("Otrzymana: {0} {1}", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString());
                W.WriteLine("Wysłana {0} {1}", sendDate.ToLongTimeString(),
                    sendDate.ToLongDateString());
                W.WriteLine(" :");
                W.WriteLine(" :{0}", msg);
                W.WriteLine("-------------------------------");
                W.Flush();
            }
        }



    }
}
