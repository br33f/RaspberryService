using RaspberryService.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspberryService.Common
{
    abstract class AbstractDeviceService
    {
        private ControlerService OutputControlerService;

        public AbstractDeviceService(ControlerService outputControlerService)
        {
            this.OutputControlerService = outputControlerService;
        }

        protected void NotifyServiceUp(string serviceName) {
            this.OutputControlerService.NotifyServiceUp(serviceName);
        }

        protected void NotifyServiceDown(string serviceName)
        {
            this.OutputControlerService.NotifyServiceDown(serviceName);
        }
    }
}
