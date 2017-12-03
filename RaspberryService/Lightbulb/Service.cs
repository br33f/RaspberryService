using DeviceProviders;
using RaspberryService.Command;
using RaspberryService.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace RaspberryService.Lightbulb
{
    class Service : AbstractDeviceService
    {
        private const string DEFAULT_LIGHTBULB_DEVICE_ID = "1feb85536970ceb754c167f409b7f4a5";

        private AllJoynProvider Provider;
        private ObservableCollection<LightbulbDevice> Lightbulbs { get; set; }

        public Service(ControlerService outputControlerService) : base(outputControlerService)
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

            NotifyServiceUp("IsAllJoynEnabled");
        }

        private void AlljoynServiceDropped(IProvider sender, ServiceDroppedEventArgs args)
        {
            Utils.LogLine("Urządzenie " + args.Service.AboutData.DeviceName + " zostało odłączone.");

            LightbulbDevice droppedItem = Lightbulbs.Where(deviceItem => deviceItem.DeviceId == args.Service.AboutData.DeviceId).First();
            Lightbulbs.Remove(droppedItem);

            if (Lightbulbs.Count == 0)
            {
                NotifyServiceDown("IsAllJoynEnabled");
            }
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

        public async void SetColorTemperature(UInt32 colorTemperature)
        {
            await GetLightbulb().ColorTemperature.SetValueAsync(colorTemperature);

            Utils.LogLine("Ustawiono temperaturę światła na : " + colorTemperature);
        }

        public async void SetHue(UInt32 hue)
        {
            await GetLightbulb().Hue.SetValueAsync(hue);

            Utils.LogLine("Ustawiono hue na: " + hue);
        }

        public async void SetIsColor(bool isColor)
        {
            UInt32 saturationValue = isColor ? UInt32.MaxValue : UInt32.MinValue;
            await GetLightbulb().Saturation.SetValueAsync(saturationValue);

            Utils.LogLine("Ustawiono isColor na: " + isColor);
        }

        public async void SetBrightness(UInt32 brightness)
        {
            await GetLightbulb().Brightness.SetValueAsync(brightness);
            Utils.LogLine("Ustawiono jasność żarówki na " + brightness);
        }
    }
}
