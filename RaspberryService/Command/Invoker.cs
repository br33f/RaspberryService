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

        public void Request_SetHue(Hashtable parameters)
        {
            UInt32 hueValue = Convert.ToUInt32(parameters["hue"]);
            LightbulbService.SetHue(hueValue);
        }

        public void Request_SetColorTemperature(Hashtable parameters)
        {
            UInt32 colorTemperatureValue = Convert.ToUInt32(parameters["colorTemperature"]);
            LightbulbService.SetColorTemperature(colorTemperatureValue);
        }

        public void Request_SetIsColor(Hashtable parameters)
        {
            bool isColorValue = (bool)parameters["isColor"];
            LightbulbService.SetIsColor(isColorValue);
        }

        public void Request_SetBrightness(Hashtable parameters)
        {
            UInt32 brightnessValue = Convert.ToUInt32(parameters["brightness"]);
            LightbulbService.SetBrightness(brightnessValue);
        }

        public void Request_SpinClockwise(Hashtable parameters)
        {
            string motorName = parameters["motorName"] as string;
            if (motorName == "A")
            {
                MotorService.MotorA.SpinClockwise();
            } 
            else
            {

            }

        }

        public void Request_SpinCounterClockwise(Hashtable parameters)
        {
            string motorName = parameters["motorName"] as string;
            if (motorName == "A")
            {
                MotorService.MotorA.SpinCounterClockwise();
            }
            else
            {

            }
        }

        public void Request_Stop(Hashtable parameters)
        {
            string motorName = parameters["motorName"] as string;
            if (motorName == "A")
            {
                MotorService.MotorA.Stop();
            }
            else
            {

            }
        }
    }
}
