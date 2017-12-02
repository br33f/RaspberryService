using RaspberryService.Command;
using RaspberryService.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace RaspberryService.Rgb
{
    public enum DiodeColor : int
    {
        RED = 17,
        GREEN = 27,
        BLUE = 22
    }

    class Service : AbstractDeviceService
    {
        private const GpioPinValue DEFAULT_DIODE_PIN_VALUE = GpioPinValue.Low;

        private GpioController Controller;

        private Dictionary<DiodeColor, GpioPin> Diode;

        public Service(ControlerService outputControlerService) : base(outputControlerService)
        {
            this.InitializeGPIO();
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
                this.InitializeLightEmittingDiode();
            }
        }

        private void InitializeLightEmittingDiode()
        {
            this.Diode = new Dictionary<DiodeColor, GpioPin>();

            foreach(DiodeColor pin in Enum.GetValues(typeof(DiodeColor)))
            {
                String pinName = Enum.GetName(typeof(DiodeColor), pin);

                GpioOpenStatus dpStatus;
                GpioPin diodePin;

                this.Controller.TryOpenPin((int)pin, GpioSharingMode.Exclusive, out diodePin, out dpStatus);
                if (dpStatus!= GpioOpenStatus.PinOpened)
                {
                    Utils.LogLine("Nie udało się otworzyć pinu diody " + pinName);
                }
                else
                {
                    diodePin.Write(DEFAULT_DIODE_PIN_VALUE);
                    diodePin.SetDriveMode(GpioPinDriveMode.Output);

                    this.Diode.Add(pin, diodePin);

                    NotifyServiceUp("IsLedEnabled");

                    Utils.LogLine("Pomyślnie zainicjalizowano diodę " + pinName);
                }
            }
        }

        public void lightUp(DiodeColor diodeColor)
        {
            this.Diode[diodeColor].Write(GpioPinValue.Low);
        }

        public void turnOff(DiodeColor diodeColor)
        {
            this.Diode[diodeColor].Write(GpioPinValue.High);
        }

    }
}
