using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspberryService.Command
{
    class Invoker
    {
        // Lightbulb
        private Lightbulb.Service LightbulbService;

        // Motors
        private Motor.Service MotorService;

        // Light emitting diode
        private Rgb.Service RgbService;
        
        public Invoker(Lightbulb.Service lightbulbService, Motor.Service motorService, Rgb.Service rgbService)
        {
            this.LightbulbService = lightbulbService;
            this.MotorService = motorService;
            this.RgbService = rgbService;
        }

        public void Request_TurnLightOn(Hashtable parameters)
        {
            LightbulbService.TurnLightOn();
        }

        public void Request_TurnLightOff(Hashtable parameters)
        {
            LightbulbService.TurnLightOff();
        }

        public void Request_SetColorHue(Hashtable parameters)
        {
            LightbulbService.SetColorHue(0x88FFFFFF);
        }

        public void Request_SetColorWhite(Hashtable parameters)
        {
            LightbulbService.SetColorWhite(UInt32.MaxValue);
        }

        public void Request_SetBrightness(Hashtable parameters)
        {
            UInt32 brightnessValue = (UInt32)(long)parameters["brightness"];
            LightbulbService.SetBrightness(brightnessValue);
        }

        public void Request_SpinClockwise(Hashtable parameters)
        {
            MotorService.MotorA.SpinClockwise();
        }

        public void Request_SpinCounterClockwise(Hashtable parameters)
        {
            MotorService.MotorA.SpinCounterClockwise();
        }

        public void Request_Stop(Hashtable parameters)
        {
            MotorService.MotorA.Stop();
        }
    }
}
