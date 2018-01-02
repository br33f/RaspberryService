using RaspberryService.Command;
using RaspberryService.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace RaspberryService.Motor
{
    class Service : AbstractDeviceService
    {
        // Motor A
        private const int MOTOR_A_PIN1 = 17;
        private const int MOTOR_A_PIN2 = 27;

        public MotorDevice MotorA { get; set; }

        // Motor B
        private const int MOTOR_B_PIN1 = 10;
        private const int MOTOR_B_PIN2 = 9;

        public MotorDevice MotorB { get; set; }

        private GpioController Controller;


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
                this.InitializeMotors();
            }
        }

        private void InitializeMotors()
        {
            this.MotorA = new MotorDevice(this.Controller, MOTOR_A_PIN1, MOTOR_A_PIN2);
            this.MotorB= new MotorDevice(this.Controller, MOTOR_B_PIN1, MOTOR_B_PIN2);
            NotifyServiceUp("IsMotorEnabled");
        }
       
    }
}
