using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subsumption
{
    interface Behavior
    {
        bool takenControl();
        void action();
        void suppress();
    }
}
