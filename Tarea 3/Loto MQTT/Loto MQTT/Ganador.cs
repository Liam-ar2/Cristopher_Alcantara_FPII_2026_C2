using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loto_MQTT
{
    public class GanadorMsg
    {
        public string ClientId { get; set; }
        public int Numero { get; set; }
        public double Monto { get; set; }
        public double Premio { get; set; }
        public string Lugar { get; set; }
        public string Mensaje { get; set; }
    }
}
