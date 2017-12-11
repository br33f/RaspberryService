using RaspberryService.Command;
using RaspberryService.Common;
using Sensors.Dht;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.System.Threading;

namespace RaspberryService.TemperatureSensor
{
    class Service : AbstractDeviceService
    {
        private const int DATA_PIN = 4;

        private GpioController Controller;
        private GpioPin DataPin;

        private Dht11 Sensor { get; set; }

        public DhtReading SensorReading { get; set; }

        private ThreadPoolTimer ReadTimer;

        public Service(ControlerService outputControlerService) : base(outputControlerService)
        {
            this.InitializeGPIO();
            this.InitializeSensor();
            this.StartReadingTimer();
        }

        private void InitializeGPIO()
        {
            this.Controller = GpioController.GetDefault();
            if (this.Controller == null)
            {
                Utils.LogLine("Urządzenie nie posiada kontrolera GPIO");
            }
            else
            {
                Utils.LogLine("Pomyślnie zainicjalizowano kontroler GPIO");
                this.InitializeDataPin();
            }
        }

        private void InitializeDataPin()
        {
            GpioOpenStatus dpStatus;

            this.Controller.TryOpenPin(DATA_PIN, GpioSharingMode.Exclusive, out DataPin, out dpStatus);
            if (dpStatus != GpioOpenStatus.PinOpened)
            {
                Utils.LogLine("Nie udało się otworzyć pinu data sensora temperatury");
            }
            else
            {
                Utils.LogLine("Pomyślnie zainicjalizowano pin data sensora temperatury");
            }
        }

        private void InitializeSensor()
        {
            if (DataPin == null) return;

            Sensor = new Dht11(DataPin, GpioPinDriveMode.Input);
            ControlerService.Instance.NotifyServiceUp("IsTemperatureSensorEnabled");
        }

        private void StartReadingTimer()
        {
            ReadTimer = ThreadPoolTimer.CreatePeriodicTimer((t) => {
                this.ReadSensor();
            }, TimeSpan.FromSeconds(5));
        }

        public async void ReadSensor()
        {
            SensorReading = await Sensor.GetReadingAsync().AsTask();
            if (!SensorReading.IsValid)
            {
                Utils.LogLine("Błędny odczyt sensora.");
            } 

            this.SendTemperatureSensorSocketUpdate();
        }

        public void SendTemperatureSensorSocketUpdate()
        {
            Request request = new Request();
            request.command = "TemperatureSensorUpdate";

            request.parameters = new Hashtable();
            request.parameters.Add("Temperature", SensorReading.Temperature);
            request.parameters.Add("Humidity", SensorReading.Humidity);

            ControlerService.Instance.SendRequest(request);
        }

    }
}
