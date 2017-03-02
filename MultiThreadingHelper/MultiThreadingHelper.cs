using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MultiThreadingHelper
{
    public class MultiThreadingHelper
    {
        private static MultiThreadingHelper instance = null;

        private static object MUTEX = new object();
        private static int RunningThreadCount = 0;
        private static int MaxThreadCount = -1;

        private MultiThreadingHelper() { }

        public static MultiThreadingHelper GetInstance(int maxThreadCount = -1)
        {
            if (instance == null)
            {
                MultiThreadingHelper.MaxThreadCount = maxThreadCount;
                instance = new MultiThreadingHelper();
            }

            return instance;
        }

        private void DoIncrement()
        {
            lock (MultiThreadingHelper.MUTEX)
            {
                MultiThreadingHelper.RunningThreadCount++;
            }
        }

        private void DoDecrement()
        {
            lock (MultiThreadingHelper.MUTEX)
            {
                MultiThreadingHelper.RunningThreadCount--;
            }
        }

        private bool CanCreateNewThread()
        {
            lock (MultiThreadingHelper.MUTEX)
            {
                if (MultiThreadingHelper.MaxThreadCount == -1)
                {
                    return true;
                }

                if (MultiThreadingHelper.RunningThreadCount <= MultiThreadingHelper.MaxThreadCount)
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
