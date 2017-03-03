using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MultiThreadingHelper
{
    public class MultiThreadingNamedHelper
    {
        private static Dictionary<string, MultiThreadingNamedHelper> instances = new Dictionary<string, MultiThreadingNamedHelper>();

        private object mutex = new object();
        private int runningThreadCount = 0;
        private int maxThreadCount = -1;

        private MultiThreadingNamedHelper() { }

        public static MultiThreadingNamedHelper GetInstance(int maxThreadCount = -1, string name = "")
        {
            if (!instances.ContainsKey(name) || instances[name] == null)
            {
                if (!instances.ContainsKey(name))
                {
                    instances.Add(name, null);
                }

                var obj = new MultiThreadingNamedHelper();
                obj.maxThreadCount = maxThreadCount;
                instances[name] = obj;
            }

            return instances[name];
        }

        private void DoIncrement()
        {
            lock (this.mutex)
            {
                this.runningThreadCount++;
            }
        }

        private void DoDecrement()
        {
            lock (this.mutex)
            {
                this.runningThreadCount--;
            }
        }

        private bool CanCreateNewThread()
        {
            lock (this.mutex)
            {
                if (this.maxThreadCount == -1)
                {
                    return true;
                }

                if (this.runningThreadCount <= this.maxThreadCount)
                {
                    return true;
                }

                return false;
            }
        }

        public void QueueNewThread<T>(T input, Action<T> task)
        {
            try
            {
                bool keepChecking = true;
                DoIncrement();
                while (keepChecking)
                {
                    if (!CanCreateNewThread())
                    {
                        continue;
                    }

                    keepChecking = false;
                    new Thread(new ParameterizedThreadStart((o) =>
                    {
                        if (task != null)
                        {
                            task((T)o);
                        }
                        DoDecrement();
                    })).Start(input);
                }
            }
            finally { }
        }

        public void QueueNewThread(string input, Action<string> task)
        {
            QueueNewThread<string>(input, task);
        }
    }
}
