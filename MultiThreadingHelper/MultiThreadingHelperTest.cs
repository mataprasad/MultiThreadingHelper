﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiThreadingHelper
{
    public class MultiThreadingHelperTest
    {
        public void Test()
        {
            MultiThreadingHelper.GetInstance(4).QueueNewThread("", (s) =>
            {
                System.IO.File.WriteAllText(DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + "__" + System.Threading.Thread.CurrentThread.ManagedThreadId + ".txt", "X");
                System.Threading.Thread.Sleep(10000);
            });
        }
    }
}
