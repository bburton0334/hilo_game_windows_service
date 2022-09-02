//============================================================================
/*
* FILE              : Server.cs
* PROJECT           : PROG2121 - A06
* PROGRAMMER        : Briana Burton & Sunraj Sharma
* FIRST VERSION     : 2021-11-26
* DESCRIPTION       :
*   The function in this file holds the server which communicates
*   with the client high low game. Error messages will be shown
*   if any error occurs. The sever can accept requests from 
*   from mutiple clients. The sever will read the information from the
*   client and the game will be played accordingly. 
*/
//============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Configuration;
using System.Collections;
using A06;

namespace A06
{
    public class Server
    {
        public volatile bool Listening;     // bool which holds if the server should be listening for new clients
        TcpListener server = null;          // holds TcpListener 
        

        // class which holds commands for the
        // server to read and decide which
        // acion to take based on command.
        static class Commands
        {
            public const string NEW_GAME = "NG";
            public const string CONTINUE_GAME = "CG";
            public const string PlAY_AGAIN = "PA";
            public const string DISCONNECT = "D";
            public const string ERROR = "ERR";
        }

        //============================================================================
        // FUNCTION         : Server
        // DESCRIPTION      :
        //      Constructor for the Server class. It sets the bool 
        //      Listening to true so the server can start accepting
        //      clients once the Run function is called.
        // PARAMETERS       : none
        // RETURNS          : none
        //============================================================================
        public Server()
        {
            Listening = true;
        }

        //============================================================================
        // FUNCTION         : Run
        // DESCRIPTION      :
        //      This function recives communication from the cient. It then
        //      starts a thread based on if the client has sent a request to
        //      communicate with the server. It then starts a loop to continue
        //      to listen to requests sent by client in order for multiple
        //      clients to connect to the sever and play the game.
        // PARAMETERS       : none
        // RETURNS          : none
        //============================================================================
        public void Run()
        {
            try
            {
                // Set the TcpListener on port 13000.
                Int32 port = 13000;
                IPAddress localAddr = IPAddress.Any;

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                //Start listening for client requests.
                server.Start();

                //Enter the listening loop.
                while (Listening)
                {
                    // checking if there a are pending clients
                    if(!server.Pending())
                    {
                        // sleeping for 500ms is there are no pending clients
                        Thread.Sleep(500);
                        continue;
                    }
                    else
                    {
                        //Perform a blocking call to accept requests
                        TcpClient client = server.AcceptTcpClient();
                        ParameterizedThreadStart ts_max = new ParameterizedThreadStart(Worker);
                        Thread clientThread_max = new Thread(ts_max);
                        clientThread_max.Start(client);
                        clientThread_max.Join();
                    }
                }
            }
            catch (SocketException e)
            {
                // logging exception
                Logger.Log(e.Message);
            }
            finally
            {
                //Stop listening for new clients.
                server.Stop();
            }
        }

        //============================================================================
        // FUNCTION         : StopServer
        // DESCRIPTION      :
        //      This function is called in order for the server to stop listening for
        //      clients. It will set the Listening bool to false which will stop the 
        //      listening loop. It will also close the sever afterwards.
        // PARAMETERS       : none
        // RETURNS          : none
        //============================================================================
        public void StopServer()
        {
            Listening = false;
            server.Server.Close();
        }

        //============================================================================
        // FUNCTION         : Worker
        // DESCRIPTION      :
        //      This function connects to the client and reads a message 
        //      passed from the cleint. It then passes a message back to
        //      the cleint. The message contains information on the players
        //      current game and how if should be played. The function will
        //      then read the message from the client and act according to
        //      the command passed in the message.
        // PARAMETERS       : Object o
        // RETURNS          : none
        //============================================================================
        void Worker(Object o)
        {
            // getting max and min from configueration file
            var maximum = ConfigurationManager.AppSettings["maximum"];
            var minimum = ConfigurationManager.AppSettings["minimum"];

            TcpClient client = (TcpClient)o;
            // Buffer for reading data
            Byte[] bytes = new Byte[256];
            String data = null;

            // Get a stream object for reading and writing
            NetworkStream stream = client.GetStream();

            int i;

            try
            {
                // Loop to receive all the data sent by the client.
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    //Console.WriteLine("Reading messaeg from client");

                    // Translate data bytes to a ASCII string.
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                    string sendMessage = null;
                    string[] words = null;
                    words = data.Split(',');

                    // checking to ensure the message recived contains
                    // 7 messages, if not, return will message indicating
                    // an error has taken pace in the message.
                    if (words == null || words.Length != 7)
                    {
                        // error in format, return indicating such.
                        sendMessage = ("0,0,0,0,0,0," + Commands.ERROR);
                    }
                    else
                    {
                        // splitting string into values.
                        string name = words[0];
                        string min = words[1];
                        string max = words[2];
                        string random = words[3];
                        string guess = words[4];
                        string status = words[5];
                        string command = words[6];

                        // Process the data sent by the client.
                        data = data.ToUpper();

                        // holds the random number for guessing game
                        string strRandom = null;

                        // checking the command
                        if (command == Commands.NEW_GAME || command == Commands.PlAY_AGAIN)
                        {
                            // client specified new game.
                            // update values accordingly.
                            int newRandom = GenerateRandom();
                            strRandom = newRandom.ToString();

                            min = minimum;
                            max = maximum;
                        }
                        else if (command == Commands.CONTINUE_GAME)
                        {
                            // client specified to continue game.
                            // updating values accordingly.
                            strRandom = random;

                            // parsing all values to int for comparison
                            int intGuess = int.Parse(guess);
                            int intRand = int.Parse(random);
                            int orgMax = int.Parse(maximum);
                            int orgMin = int.Parse(minimum);

                            // updating min and max value based on the current
                            // guess that the user has submitted.
                            if ((intGuess <= orgMax) && (intGuess >= orgMin))
                            {
                                if ((intGuess < intRand))
                                {
                                    min = guess;
                                }
                                else if (intGuess > intRand)
                                {
                                    max = guess;
                                }
                            }

                            // updating the game status with a
                            // message indicating if the users
                            // guess was too high, too low, or
                            // if the user guess was correct.
                            if (intGuess < intRand)
                            {
                                status = "Too Low";
                            }
                            else if (intGuess > intRand)
                            {
                                status = "Too High";
                            }
                            else if (intGuess == intRand)
                            {
                                status = "Correct";
                            }
                        }
                        else if (command == Commands.DISCONNECT)
                        {
                            // message to indicate accepting request to disconnect
                            command = "Accepted";
                        }

                        sendMessage = (name + "," + min + "," + max + "," + strRandom + "," + guess + "," + status + "," + command);
                    }

                    byte[] bytStrRange = System.Text.Encoding.ASCII.GetBytes(sendMessage);

                    // Send back a response.
                    stream.Write(bytStrRange, 0, bytStrRange.Length);
                }
            }
            catch (Exception ex)
            {
                // logging excpetion
                Logger.Log(ex.Message);
            }

            // Shutdown and end connection
            client.Close();
        }

        //============================================================================
        // FUNCTION         : GenerateRandom
        // DESCRIPTION      :
        //      This function used to generate a random number based on
        //      the values within the configuration. Will return the
        //      generated int value.
        // PARAMETERS       : int containg random number
        // RETURNS          : none
        //============================================================================
        int GenerateRandom()
        {
            // getting maximum and minimum
            var maximum = ConfigurationManager.AppSettings["maximum"];
            var minimum = ConfigurationManager.AppSettings["minimum"];

            // parsing the values to int
            int max = int.Parse(maximum);
            int min = int.Parse(minimum);

            // generating random number
            Random rnd = new Random();
            int rand = rnd.Next(min, max);

            // returning value
            return rand;
        }
    }
}
