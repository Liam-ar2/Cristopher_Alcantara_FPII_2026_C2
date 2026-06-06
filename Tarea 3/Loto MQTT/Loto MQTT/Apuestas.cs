using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loto_MQTT
{
    public class ApuestaMsg

    {
        public string ClienteId { get; set; }
        public int Numero { get; set; }
        public double Monto { get; set; }
    }
}
