using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Networking.Sockets;


namespace RaspberryService.Command
{
    class Commander
    {
        public const string SOCK_PORT = "8724";
        public const string REQUEST_INVOKE_PREFIX = "Request_";

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

        private void Dispose()
        {
            this.Listener.Dispose();
            deferral.Complete();
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

            Dispose();
        }

        private void ProcessRequest(Request request)
        {
            // Note: Możemy pobrać również typ danego kontrolera
            string commandName = REQUEST_INVOKE_PREFIX + request.command;
            Type commanderType = this.GetType();
            MethodInfo method = commanderType.GetMethod(commandName);

            if (method != null)
            {
                Utils.LogLine("Wywołuje metodę: " + commandName);
                Object[] parameters = new Object[] { request.parameters };
                method.Invoke(this, parameters);
            } 
            else
            {
                Utils.LogLine("Metoda: " + commandName + " nie istnieje");
            }
        }

        public void Request_TurnLightOn(Dictionary<string, dynamic> parameters)
        {
            Utils.LogLine("turn light on!");
        }

        /* Entry point */
        public void Start()
        {
            this.Initialize();
        }

    }
}
