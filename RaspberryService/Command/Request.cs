using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspberryService.Command
{
    class Request
    {
        public string command { get; set; }
        public Object parameters { get; set; }
    }
}
