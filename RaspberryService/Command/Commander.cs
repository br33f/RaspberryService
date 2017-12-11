using Newtonsoft.Json;
using RaspberryService.Lightbulb;
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
    class Commander : IDisposable
    {
        public const string INPUT_SOCK_PORT = "8724";
        public const string REQUEST_INVOKE_PREFIX = "Request_";

        private BackgroundTaskDeferral deferral;

        private bool running = true;

        // Controler Service
        private ControlerService OutputControlerService; 

        // Request Invoker
        private Invoker RequestInvoker;

        // Input Socket
        private StreamSocketListener InputListener;

        // Lightbulb
        private Lightbulb.Service LightbulbService;

        // Motors
        private Motor.Service MotorService;

        // Light emitting diode
        private Rgb.Service RgbService;

        // TemperatureSensor
        private TemperatureSensor.Service TemperatureSensorService;

        public Commander(BackgroundTaskDeferral deferral)
        {
            this.deferral = deferral;
        }

        private void Dispose()
        {
            this.InputListener.Dispose();
            deferral.Complete();
        }

        private void Initialize()
        {
            this.InitializeInputSocket();
            this.InitializeControlerService();
            this.InitializeLightbulbService();
            this.InitializeMotorService();
            this.InitializeRgbService();
            this.InitializeTemperatureSensorService();
            this.InitializeInvoker();
        }

        private void InitializeInvoker()
        {
            this.RequestInvoker = new Invoker(this.LightbulbService, this.MotorService, this.RgbService);
        }

        private async void InitializeInputSocket()
        {
            this.InputListener = new StreamSocketListener();
            this.InputListener.ConnectionReceived += OnInputStreamSocketConnectionReceived;
            await this.InputListener.BindServiceNameAsync(INPUT_SOCK_PORT);

            Utils.LogLine("Nasłuchiwanie InputStreamSocket na porcie " + INPUT_SOCK_PORT);
        }

        private void InitializeControlerService()
        {
            this.OutputControlerService = ControlerService.Instance;
        }

        private void InitializeLightbulbService()
        {
            this.LightbulbService = new Lightbulb.Service(this.OutputControlerService);
        }

        private void InitializeMotorService()
        {
            this.MotorService = new Motor.Service(this.OutputControlerService);
        }

        private void InitializeRgbService()
        {
            this.RgbService = new Rgb.Service(this.OutputControlerService);
        }

        private void InitializeTemperatureSensorService()
        {
            this.TemperatureSensorService = new TemperatureSensor.Service(this.OutputControlerService);
        }

        private async void OnInputStreamSocketConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            Utils.LogLine("Otrzymano połączenie InputStreamSocket.");
            try
            {
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
            }
            catch (Exception e)
            {
                Utils.LogLine("Zakończono połączenie InputStreamSocket.");
                Utils.LogLine(e.Message);
            }
        }       

        private void ProcessRequest(Request request)
        {
            string commandName = REQUEST_INVOKE_PREFIX + request.command;
            Type commanderType = RequestInvoker.GetType();
            MethodInfo method = commanderType.GetMethod(commandName);

            if (method != null)
            {
                Utils.LogLine("Wywołuje metodę: " + commandName);
                Object[] parameters = new Object[] { request.parameters };
                method.Invoke(RequestInvoker, parameters);
            } 
            else
            {
                Utils.LogLine("Metoda: " + commandName + " nie istnieje");
            }
        }

       

        /* Entry point */
        public void Start()
        {
            this.Initialize();
        }

        void IDisposable.Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
