using DeviceProviders;
using RaspberryService.Command;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspberryService.Lightbulb
{
    class LightbulbDevice
    {
        // Service
        public IService Service { get; set; }

        // About data
        public string DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string Manufacturer { get; set; }
        public string ModelNumber { get; set; }

        // Lamp state
        public IProperty OnOff { get; set; }
        public IProperty Brightness { get; set; }
        public IProperty ColorTemperature { get; set; }
        public IProperty Hue { get; set; }
        public IProperty Saturation { get; set; }

        public LightbulbDevice(IService service)
        {
            this.Service = service;

            this.InitializeAboutData();
            this.InitializeLampState();
            this.SendLightbulbSocketUpdate();
        }

        private void InitializeAboutData()
        {
            this.DeviceId = Service.AboutData.DeviceId;
            this.DeviceName = Service.AboutData.DeviceName;
            this.Manufacturer = Service.AboutData.Manufacturer;
            this.ModelNumber = Service.AboutData.ModelNumber;
        }

        private void InitializeLampState()
        {
            var res = (from a in (Service.Objects.SelectMany(i => i.Interfaces))
                       where a.Name.Equals("org.allseen.LSF.LampState")
                       select a).First();
            
            this.OnOff = res.GetProperty("OnOff");
            this.Brightness = res.GetProperty("Brightness");
            this.ColorTemperature = res.GetProperty("ColorTemp");
            this.Hue = res.GetProperty("Hue");
            this.Saturation = res.GetProperty("Saturation");
        }

        public async void SendLightbulbSocketUpdate()
        {
            Request request = new Request();
            request.command = "LightbulbUpdate";

            ReadValueResult onOff = await this.OnOff.ReadValueAsync();
            ReadValueResult brightness = await this.Brightness.ReadValueAsync();
            ReadValueResult colorTemperature = await this.ColorTemperature.ReadValueAsync();
            ReadValueResult hue = await this.Hue.ReadValueAsync();
            ReadValueResult saturation = await this.Saturation.ReadValueAsync();

            request.parameters = new Hashtable();
            request.parameters.Add("IsOnOff", onOff.Value);
            request.parameters.Add("Brightness", brightness.Value);
            request.parameters.Add("ColorTemperature", colorTemperature.Value);
            request.parameters.Add("Hue", hue.Value);
            request.parameters.Add("IsColor", (UInt32)saturation.Value > UInt32.MinValue);

            ControlerService.Instance.SendRequest(request);
        }
    }
}
