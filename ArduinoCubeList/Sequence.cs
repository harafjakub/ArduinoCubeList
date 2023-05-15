using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoCubeList
{
    public class Sequence
    {
        public string Diody { get; set; }
        public int Delay { get; set; }
        public Sequence(string diody, int delay)
        {
            this.Diody = diody;
            this.Delay = delay;
        }
        public Sequence()
        {
        }
    }
}