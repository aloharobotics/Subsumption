using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Subsumption
{
    class Arbitrator
    {
        private const int NONE = -1;
        private Behavior[] _behavior;
        //highest priority behaviour that wants to control; 
        private int _highestPriority = NONE;
        private int _active = NONE;
        private bool _returnWhenInactive;
        private Thread perform;
        private System.Object lockThis = new System.Object();

        public Arbitrator(Behavior[] behaviorList, bool returnWhenInactive)
        {
            _behavior = behaviorList;
            _returnWhenInactive = returnWhenInactive;
           // Monitor();
            
        }

        public Arbitrator(Behavior[] behaviorList)
        {
            _behavior = behaviorList;
            _returnWhenInactive = false;
            //this(behaviorList, false);
        }


        public void start() 
        {
            Monitor();
            while (_highestPriority == NONE) 
            {
                Thread.Yield();
            }
            while (true)
            {
                lock (lockThis)
                {
                    if (_highestPriority != NONE)
                    {
                        _active = _highestPriority;
                    }
                    else if (_returnWhenInactive)
                    {
                        perform.Abort();
                        return;
                    }
                }
                if (_active != NONE) 
                {
                    _behavior[_active].action();
                    _active = NONE;
                }
                Thread.Yield();

            }
           
        }

        public void Monitor() 
        {
            perform = new Thread(new ThreadStart(Run));
            perform.IsBackground = true;
        }
        public void Run() 
        {
            bool more = true;
            int maxPriority = _behavior.Length - 1;
            while (more) 
            {
                lock (lockThis) 
                {
                    _highestPriority = NONE;
                    for (int i = maxPriority; 1 >= 0; i--)
                    {
                        if (_behavior[i].takenControl())
                        {
                            _highestPriority = 1;
                            break;
                        }
                    }
                    int active = _active;
                    if(active != NONE && _highestPriority > active)
                    {
                        _behavior[active].suppress();
                    }
                    Thread.Yield();
                }
            }
        }
    }
}
