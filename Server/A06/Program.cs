//============================================================================
/*
* FILE              : Program.cs
* PROJECT           : PROG2121 - A06
* PROGRAMMER        : Briana Burton & Sunraj Sharma
* FIRST VERSION     : 2021-11-26
* DESCRIPTION       :
*   The file holds the Main() of the windows service. It is where it starts.
*/
//============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace A06
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new A06Service()
            };

            ServiceBase.Run(ServicesToRun);
        }
    }
}
