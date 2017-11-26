using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Networking.Sockets;


namespace RaspberryService.Command
{
    class Commander
    {
        public const string SOCK_PORT = "8723";

        private BackgroundTaskDeferral deferral;

        private bool running = true;

        private StreamSocketListener Listener;

        public Commander(BackgroundTaskDeferral deferral)
        {
            this.deferral = deferral;
        }

        private void Initialize()
        {
            this.InitializeSocket();
        }

        private async void InitializeSocket()
        {
            this.Listener = new StreamSocketListener();
            this.Listener.ConnectionReceived += OnStreamSocketConnectionReceived;
            await this.Listener.BindServiceNameAsync(SOCK_PORT);

            Utils.LogLine("Nasłuchiwanie StreamSocket na porcie " + SOCK_PORT);
        }

        private async void OnStreamSocketConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            Utils.LogLine("Otrzymano połączenie StreamSocket.");

            Stream inStream = args.Socket.InputStream.AsStreamForRead();
            StreamReader reader = new StreamReader(inStream);

            while (running)
            {
                string rawRequest = await reader.ReadLineAsync();
                string decodedRequest = Utils.Base64Decode(rawRequest);
                Utils.LogLine("Odebrano: " + decodedRequest);

                Request request = JsonConvert.DeserializeObject<Request>(decodedRequest);
                ProcessRequest(request);
            }

            deferral.Complete();
        }

        private void ProcessRequest(Request request)
        {
            switch (request.command)
            {
                case "turnLightOn":
                    break;
                case "turnLightOff":
                    break;
                case "stop":
                    running = false;
                    break;
            }
        }

        /* Entry point */
        public void Start()
        {
            this.Initialize();
        }

    }
}
