using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace RaspberryService.Motor
{
    public sealed class MotorDevice
    {
        private const GpioPinValue DEFAULT_MOTOR_PIN_VALUE = GpioPinValue.Low;

        private GpioController Controller;

        private GpioPin MotorPin1;
        private GpioPin MotorPin2;

        public MotorDevice(GpioController controller, int PIN1, int PIN2)
        {
            this.Controller = controller;
            this.InitializeMotor(PIN1, PIN2);
        }

        private void InitializeMotor(int PIN1, int PIN2)
        {
            GpioOpenStatus m1Status;
            GpioOpenStatus m2Status;

            this.Controller.TryOpenPin(PIN1, GpioSharingMode.Exclusive, out this.MotorPin1, out m1Status);
            this.Controller.TryOpenPin(PIN2, GpioSharingMode.Exclusive, out this.MotorPin2, out m2Status);

            if (m1Status != GpioOpenStatus.PinOpened && m2Status != GpioOpenStatus.PinOpened)
            {
                Utils.LogLine("Nie udało się otworzyć pinów silnika");
            }
            else
            {
                this.MotorPin1.Write(DEFAULT_MOTOR_PIN_VALUE);
                this.MotorPin1.SetDriveMode(GpioPinDriveMode.Output);

                this.MotorPin2.Write(DEFAULT_MOTOR_PIN_VALUE);
                this.MotorPin2.SetDriveMode(GpioPinDriveMode.Output);

                Utils.LogLine("Pomyślnie zainicjalizowano silnik");
            }
        }

        public void SpinClockwise()
        {
            this.MotorPin1.Write(GpioPinValue.High);
            this.MotorPin2.Write(GpioPinValue.Low);
            Utils.LogLine("Uruchomiono silnik zgodnie z ruchem wskazówek zegara");
        }

        public void SpinCounterClockwise()
        {
            this.MotorPin1.Write(GpioPinValue.Low);
            this.MotorPin2.Write(GpioPinValue.High);
            Utils.LogLine("Uruchomiono silnik przeciwnie do ruchu wskazówek zegara");
        }

        public void Stop()
        {
            this.MotorPin1.Write(GpioPinValue.High);
            this.MotorPin2.Write(GpioPinValue.High);
            Utils.LogLine("Zatrzymano silnik");
        }
    }
}
