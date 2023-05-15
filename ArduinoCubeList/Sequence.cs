using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoCubeList
{
    internal class Sequence
    {
        string diody;
        int delay;
        public Sequence(string diody, int delay)
        {
            this.diody = diody;
            this.delay = delay;
        }
    }
}
