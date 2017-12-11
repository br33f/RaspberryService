using DeviceProviders;
using Newtonsoft.Json;
using RaspberryService.Command;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking.Sockets;

namespace RaspberryService.Command
{
    class ControlerService : IDisposable
    {
        private const string OUTPUT_SOCK_PORT = "8725";

        public static ControlerService _Instance = null;

        private StreamSocketListener OutputListener;
        private StreamWriter OutputStreamWriter;

        private ObservableCollection<Request> RequestQueue; 

        public static ControlerService Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new ControlerService();
                }

                return _Instance;
            }
        }

        private ControlerService()
        {
            this.RequestQueue = new ObservableCollection<Request>();
            this.InitializeOutputSocket();
        }

        public void Dispose()
        {
            this.OutputListener.Dispose();
        }

        public async void InitializeOutputSocket()
        {
            this.OutputListener = new StreamSocketListener();
            this.OutputListener.ConnectionReceived += OnOutputStreamSocketConnectionReceived;
            await this.OutputListener.BindServiceNameAsync(OUTPUT_SOCK_PORT);

            Utils.LogLine("Nasłuchiwanie OutputStreamSocket na porcie " + OUTPUT_SOCK_PORT);
        }

        private void OnOutputStreamSocketConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            Utils.LogLine("Otrzymano połączenie OutputStreamSocket.");

            Stream outStream = args.Socket.OutputStream.AsStreamForWrite();
            OutputStreamWriter = new StreamWriter(outStream);

            ProcessRequests();
            this.RequestQueue.CollectionChanged += OnRequestQueueChange;

            this.NotifyServiceUp("IsTcpEnabled");
        }

        public void SendRequest(Request request)
        {
            RequestQueue.Add(request);
        }

        private void OnRequestQueueChange(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                ProcessRequests();
            }
        }

        private void ProcessRequests()
        {
            foreach (Request request in RequestQueue.ToArray())
            {
                string serializedRequest = JsonConvert.SerializeObject(request);

                this.OutputStreamWriter.WriteLine(Utils.Base64Encode(serializedRequest));
                this.OutputStreamWriter.Flush();

                RequestQueue.Remove(request);
            }
        }

        public void NotifyServiceUp(String serviceName)
        {
            Request request = new Request();
            request.command = "ServiceIsUp";
            request.parameters = new Hashtable();
            request.parameters.Add("serviceName", serviceName);
            SendRequest(request);
        }

        public void NotifyServiceDown(String serviceName)
        {
            Request request = new Request();
            request.command = "ServiceIsDown";
            request.parameters = new Hashtable();
            request.parameters.Add("serviceName", serviceName);
            SendRequest(request);
        }
    }
}
