using DeviceProviders;
using System;
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
    }
}
