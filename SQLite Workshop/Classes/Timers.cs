using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteWorkshop
{
    class Timers
    {
        
        #region Instrumentation
        /*
        *  Performance Measurement Routines
        */
        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        public static extern bool QueryPerformanceCounter(ref Int64 perfcount);

        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        public static extern bool QueryPerformanceFrequency(ref Int64 freq);
        internal static Int64 QueryPerformanceCounter()
        {
            Int64 perfcount = 0;
            QueryPerformanceCounter(ref perfcount);
            return perfcount;
        }

        internal static Int64 QueryPerformanceFrequency()
        {
            Int64 freq = 0;
            QueryPerformanceFrequency(ref freq);
            return freq;
        }

        public static Double QueryLapsedTime(Int64 startClock)
        {
            Int64 stopClock = QueryPerformanceCounter();
            return (stopClock - startClock) * 1.0 / QueryPerformanceFrequency();

        }

        public static string DisplayTime(double milliTime)
        {
            long millisecs = Convert.ToInt64(milliTime);
            int millimin = (int)millisecs % 3600;
            long ihours = (millisecs - millimin) / 3600;
            long millisec = millimin % 60;
            int iminutes = (int)(millimin - millisec) / 60;
            int iseconds = (int)millisec;
            return string.Format("{0}:{1}:{2}", ihours.ToString().PadLeft(2, '0'), iminutes.ToString().PadLeft(2, '0'), iseconds.ToString().PadLeft(2, '0'));
        }
        #endregion

    }
}
