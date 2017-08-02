using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateService
{
    public partial class UpdateService
    {
        public UpdateService()
        {

        }

        public void Start(string[] args)
        {
            Task.Factory.StartNew(Start, TaskCreationOptions.LongRunning);


        }

        void Start()
        {
            while (true)
            {
                var pipe = new NamedPipeServerStream("IMS_UpdateService", PipeDirection.InOut, -1,
                    PipeTransmissionMode.Message, PipeOptions.Asynchronous | PipeOptions.WriteThrough);
                pipe.WaitForConnection();

                using (pipe)
                {
                    var data = new byte[1000];
                    int bytesRead = pipe.Read(data, 0, data.Length);
                    var message = Encoding.UTF8.GetString(data, 0, bytesRead);
                    Debug.WriteLine("Start. message: {0}", message);

                    var reponse = Encoding.UTF8.GetBytes("Reponse" + DateTime.Now);
                    pipe.Write(reponse, 0, reponse.Length);
                }
            }
        }

        public void StartClient()
        {
            var task = Task.Factory.StartNew(StartClient2, TaskCreationOptions.LongRunning);
        }

        void StartClient2()
        {
            using (var pipe = new NamedPipeClientStream("localhost", "IMS_UpdateService", PipeDirection.InOut, PipeOptions.Asynchronous | PipeOptions.WriteThrough))
            {
                pipe.Connect();
                pipe.ReadMode = PipeTransmissionMode.Message;
                var request = Encoding.UTF8.GetBytes("Hello");
                pipe.Write(request, 0, request.Length);

                var reponse = new byte[1000];
                var bytesRead = pipe.Read(reponse, 0, reponse.Length);
                var reponseMessage = Encoding.UTF8.GetString(reponse, 0, bytesRead);

                Debug.WriteLine("Start Client. responseMessage: {0}", new[] { reponseMessage });
            }
        }

        public void Stop()
        {

        }

    }
}
