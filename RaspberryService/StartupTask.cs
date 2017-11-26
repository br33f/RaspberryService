using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using RaspberryService.Command;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace RaspberryService
{
    public sealed class StartupTask : IBackgroundTask
    {
        private BackgroundTaskDeferral deferral;
        private Commander Cmd;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            this.deferral = taskInstance.GetDeferral();

            this.Cmd = new Commander(this.deferral);
            Cmd.Start();
        }
    }
}
