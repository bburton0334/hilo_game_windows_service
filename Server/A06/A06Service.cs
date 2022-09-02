//============================================================================
/*
* FILE              : A06Service.cs
* PROJECT           : PROG2121 - A06
* PROGRAMMER        : Briana Burton & Sunraj Sharma
* FIRST VERSION     : 2021-11-26
* DESCRIPTION       :
*   The file holds the funtionality of the A06Service. It holds the functions
*   that are called when the service is initialized, when it starts, and 
*   when the service is stopped. OnStart will create a thread which will
*   start the server, and OnStop will wait for the thread to end before
*   terminated the thread to close the server.
*/
//============================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Configuration;


namespace A06
{
    public partial class A06Service : ServiceBase
    {
        private Server svr;     // holds svr of type Server
        private Thread t;       // holds thread

        public A06Service()
        {
            InitializeComponent();
        }

        //============================================================================
        // FUNCTION         : OnStart
        // DESCRIPTION      :
        //      Starts the server for the service.
        // PARAMETERS       : string[] args
        // RETURNS          : none
        //============================================================================
        protected override void OnStart(string[] args)
        {
            // writting to the log that the service has started
            Logger.Log("Service Started");

            // starting new Server and thread
            svr = new Server();
            t = new Thread(new ThreadStart(svr.Run));
            t.Start();
        }

        //============================================================================
        // FUNCTION         : onStop
        // DESCRIPTION      :
        //      Stops the server when the service is called to be stopped.
        // PARAMETERS       : none
        // RETURNS          : none
        //============================================================================
        protected override void OnStop()
        {
            svr.StopServer();   // calling function to close server and stop listening for clients
            t.Join();           // waiting for thread to end
            svr = null;         // setting server to null
            t = null;           // setting thread to null

            // writting to the log that the service has stopped
            Logger.Log("Service Stopped");
        }

    }
}
