using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace RaspberryService.Motor
{
    class Service
    {
        // Motor A
        public const int MOTOR_A_PIN1 = 23;
        public const int MOTOR_A_PIN2 = 24;

        public MotorDevice MotorA { get; set; }

        // Motor B
        // TBD

        private GpioController Controller;

        public Service()
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
        }
       
    }
}
