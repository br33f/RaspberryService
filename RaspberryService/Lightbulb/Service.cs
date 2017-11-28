using DeviceProviders;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace RaspberryService.Lightbulb
{
    class Service
    {
        public const string DEFAULT_LIGHTBULB_DEVICE_ID = "1feb85536970ceb754c167f409b7f4a5";

        private AllJoynProvider Provider;
        private ObservableCollection<LightbulbDevice> Lightbulbs { get; set; }

        public Service()
        {
            this.Lightbulbs = new ObservableCollection<LightbulbDevice>();

            this.InitializeAlljoyn();
        }

        private void InitializeAlljoyn()
        {
            Provider = new AllJoynProvider();
            Provider.ServiceJoined += AlljoynServiceJoined;
            Provider.ServiceDropped += AlljoynServiceDropped;
            Provider.Start();

            System.Diagnostics.Debug.WriteLine("Nasłuchiwanie urządzeń AllJoyn.");
        }

        private void AlljoynServiceJoined(IProvider sender, ServiceJoinedEventArgs args)
        {
            Utils.LogLine("Podłączono urządzenie AllJoyn: " + args.Service.AboutData.DeviceName);

            LightbulbDevice joinedDevice = new LightbulbDevice(args.Service);
            Lightbulbs.Add(joinedDevice);
        }

        private void AlljoynServiceDropped(IProvider sender, ServiceDroppedEventArgs args)
        {
            Utils.LogLine("Urządzenie " + args.Service.AboutData.DeviceName + " zostało odłączone.");

            LightbulbDevice droppedItem = Lightbulbs.Where(deviceItem => deviceItem.DeviceId == args.Service.AboutData.DeviceId).First();
            Lightbulbs.Remove(droppedItem);
        }

        private LightbulbDevice GetLightbulb()
        {
            // Note: Możemy tutaj dodać rozpoznawanie żarówki po ID, jeżeli chcielibyśmy podłączyć więcej niż 1
            string deviceId = DEFAULT_LIGHTBULB_DEVICE_ID;
       
            LightbulbDevice lightbulb = Lightbulbs.Where(deviceItem => deviceItem.DeviceId == deviceId).First();

            return lightbulb;
        }

        public async void TurnLightOn()
        {
            await GetLightbulb().OnOff.SetValueAsync(true);
            Utils.LogLine("Włączono żarówkę");
        }

        public async void TurnLightOff()
        {
            await GetLightbulb().OnOff.SetValueAsync(false);
            Utils.LogLine("Włączono żarówkę");
        }

        public async void SetColorWhite(UInt32 colorTemperature = UInt32.MaxValue)
        {
            await GetLightbulb().ColorTemperature.SetValueAsync(colorTemperature);
            await GetLightbulb().Saturation.SetValueAsync((UInt32) UInt32.MinValue);

            Utils.LogLine("Ustawiono kolor biały na: " + colorTemperature);
        }

        public async void SetColorHue(UInt32 hue)
        {
            await GetLightbulb().Hue.SetValueAsync(hue);
            await GetLightbulb().Saturation.SetValueAsync((UInt32) UInt32.MaxValue);

            Utils.LogLine("Ustawiono kolor na: " + hue);
        }

        public async void SetBrightness(UInt32 brightness = UInt32.MaxValue)
        {
            await GetLightbulb().Brightness.SetValueAsync(brightness);
            Utils.LogLine("Ustawiono jasność żarówki na " + brightness);
        }
    }
}
