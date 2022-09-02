/*
* FILE              : MainWindow.xaml.cs
* PROJECT           : PROG2121 - A05
* PROGRAMMER        : Briana Burton & Sunraj Sharma
* FIRST VERSION     : 2021-11-12
* DESCRIPTION       :
*   The functions in this file are used with the xaml in order to
*   show case the functions of the program to the user. The function
*   in this file will show the user textboxes to enter in data to
*   connect to the sever. Error messages will be shown if any error
*   occurs. The user can than connect to the sever with a button
*   click and the client will the send information to the server
*   about the game. The client will read the information from the
*   server and the game will be played accordingly. The user can
*   guess any numer and the number will be sent to the server.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace A05_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string usrIpAddress = null;     // holds user ip address value
        int usrPortNumber = 0;          // holds user port number
        string usrName = null;          // holds user name

        string responseMessage = null;  // holds response from server
        string serverResponse = null;   // holds response from server

        string usrGuess = "0";          // holds guess value
        string usrRandom = "0";         // holds the random value
        string min = "0";               // holds the min value
        string max = "0";               // holds the max value

        string gameStatus = "0";        // holds game range status 
        string command = "NG";          // holds command value

        bool connected = false;         // holds whether or not user is connected

        
        public MainWindow()
        {
            InitializeComponent();
        }

        // FUNCTION         : btnConnect_Click
        // DESCRIPTION      :
        //      This function validates the user input into the
        //      ipaddress, name, and port number fields. If an
        //      error is found, the program wil notify the user
        //      that an error is present.
        // PARAMETERS       : object sender, routedeventargs e
        // RETURNS          : none
        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            bool fieldsFilled = false;

            // checking if all values are filled and not blank
            if ((txtIPAddress.Text != "") || (txtPortNumber.Text != "") || (txtName.Text != ""))
            {
                // checking if port number is in fact numeric
                if (!txtPortNumber.Text.Any(Char.IsLetter))
                {
                    fieldsFilled = true;
                }
                else
                {
                    // error message if port number contains letters
                    MessageBox.Show("Port Number Must be Numeric", "Connect Error");
                }
            }
            else
            {
                // error message that all values must be entered
                MessageBox.Show("Please Enter all Fields in Valid Format", "Connect Error");
            }

            // checking if all fields have been filled
            if (fieldsFilled == true)
            {
                // setting the users name
                usrName = txtName.Text;

                // checking to see if the user inputs are numeric and in IP address format
                bool isNumeric = int.TryParse(txtPortNumber.Text, out int port);
                bool isIp = IPAddress.TryParse(txtIPAddress.Text, out IPAddress ip);

                // checking if port is numeric and
                // if the ipAddress is valid.
                if (isNumeric && isIp)
                {
                    // setting values
                    usrIpAddress = ip.ToString();
                    usrPortNumber = port;

                    // fields valid. user can now connect
                    connected = true;
                }
                else
                {
                    // prompting error message that there was an incorrect format
                    MessageBox.Show("Please Enter all Fields in Valid Format", "Connect Error");
                }

                // checking if user can connect
                if (connected)
                {
                    // changing text boxes and buttons
                    // to disable and enable to prepare
                    // for the actual game to take place.
                    btnConnect.IsEnabled = false;
                    txtIPAddress.IsEnabled = false;
                    txtName.IsEnabled = false;
                    txtPortNumber.IsEnabled = false;

                    txtGuess.IsEnabled = true;
                    btnGuess.IsEnabled = true;
                    btnDisconnect.IsEnabled = true;

                    // calling function to connect to server
                    connect();
                }
            }
        }

        // FUNCTION         : connect
        // DESCRIPTION      :
        //      This function calls the ConnectClient function to send a
        //      message to the server. The function will split the string
        //      the response is stored in and will set the values accordingly.
        //      that an error is present.
        // PARAMETERS       : none
        // RETURNS          : none
        private void connect()
        {
            // calling function to send message to server
            ConnectClient(usrIpAddress, usrName);

            try
            {
                // splitting string
                string[] words = responseMessage.Split(',');

                // if the server did not respond with 7 messages,
                // prompt the user with an server error message.
                if (words == null || words.Length != 7)
                {
                    // resetting values due to server error.
                    resetValues();
                    MessageBox.Show("Error In Message from Server");
                }
                else
                {
                    // setting values
                    min = words[1];
                    max = words[2];
                    usrRandom = words[3];
                    gameStatus = words[5];

                    // updating range label.
                    lblRange.Content = ("Your Range is Between : " + min + " - " + max);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // FUNCTION         : ConnectClient
        // DESCRIPTION      :
        //      This function connects to the sever and passes a message to
        //      the sever. The message contains information on the players
        //      current game and how if should be played. The function will
        //      then read the message and store it within repsonseMessage.
        //      that an error is present.
        // PARAMETERS       : string server, string message
        // RETURNS          : none
        private void ConnectClient(String server, String message)
        {
            try
            {
                // Create a TcpClient
                Int32 port = usrPortNumber;
                TcpClient client = new TcpClient(server, port);

                string sendMessage = (message + "," + min + "," + max + "," + usrRandom + "," + usrGuess + "," + gameStatus + "," + command);

                Byte[] data = System.Text.Encoding.ASCII.GetBytes(sendMessage);     // Translate passed message into ASCII. Store as Byte array.             
                NetworkStream stream = client.GetStream();                          // Get a client stream for reading and writing.
                stream.Write(data, 0, data.Length);                                 // Send the message to the connected TcpServer. 
                data = new Byte[256];                                               // Receive TcpServer.response. Buffer to store response bytes.
                String responseData = String.Empty;                                 // String to store the response ASCII representation.

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                responseMessage = responseData;

                // Close everything.
                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e)
            {
                resetValues();
                MessageBox.Show("ArgumentNullException: " + e.Message);
            }
            catch (SocketException e)
            {
                resetValues();
                MessageBox.Show("SocketException: " + e.Message);
            }
        }

        // FUNCTION         : btnGuess_Click
        // DESCRIPTION      :
        //      This function validates the user input into the
        //      guess number field. If an error is found, the 
        //      program wil notify the user that an error is present.
        // PARAMETERS       : object sender, routedEventArgs e
        // RETURNS          : none
        private void btnGuess_Click(object sender, RoutedEventArgs e)
        {
            // checking if textbox is not blank
            if (txtGuess.Text != "")
            {
                // checking if textbox does not contain letters
                if ((!txtGuess.Text.Any(Char.IsLetter)))
                {
                    // ensuring guess number is a positive integer value.
                    // shwoing error of guss number is something negative.
                    bool res = int.TryParse(txtGuess.Text, out int guessNum);
                    if ((res) && (guessNum > 0))
                    {
                        usrGuess = txtGuess.Text;
                        command = "CG";

                        connectGuess(usrIpAddress, usrName);
                        splitResponse();
                    }
                    else
                    {
                        // error message
                        MessageBox.Show("Please Enter Positive Numbers Only");
                    }
                }
                else
                {
                    // error message
                    MessageBox.Show("Please Enter Numbers Only");
                }
            }
            else
            {
                // error message
                MessageBox.Show("Please Enter a Valid Guess");
            }

        }

        // FUNCTION         : splitResponse
        // DESCRIPTION      :
        //      This function is used to look at the reponse message
        //      of the sever and split it into appropriate fields. If
        //      there are not certain amount of strings that are split,
        //      the fields will not be set and an error will be shown.
        // PARAMETERS       : object sender, routedEventArgs e
        // RETURNS          : none
        private void splitResponse()
        {
            // splitting message
            string[] words = serverResponse.Split(',');

            // checking if string contains 7 messages
            if (words == null || words.Length != 7)
            {
                // resetting values to default and prompting
                // error message saying there is a server error.
                resetValues();
                MessageBox.Show("Error In Message from Server");
            }
            else
            {
                // setting values
                min = words[1];
                max = words[2];
                gameStatus = words[5];

                // checking if user guessed correctly
                if (gameStatus == "Correct")
                {
                    // disabling guessing functions and prompting user that they have won
                    lblRange.Content = ("You Win! You Guessed the Number!");
                    txtGuess.IsEnabled = false;
                    btnGuess.IsEnabled = false;
                    btnPlayAgain.IsEnabled = true;
                }
                else
                {
                    // updating the range for the user to guess in
                    lblRange.Content = ("Your Range is Between : " + min + " - " + max);
                }

                // updaing label with whether or not their guess
                // is too high or if their guess is too low.
                lblStatus.Content = ("[ " + gameStatus + " ]");
            }
        }

        // FUNCTION         : ConnectGuess
        // DESCRIPTION      :
        //      This function connects to the sever and passes a message to
        //      the sever. The message contains information on the players
        //      current game and how if should be played. The function will
        //      then read the message and store it within repsonseMessage.
        //      that an error is present.
        // PARAMETERS       : string server, string message
        // RETURNS          : none
        private void connectGuess(String server, String message)
        {
            try
            {
                //Create a TcpClient
                Int32 port = usrPortNumber;
                TcpClient client = new TcpClient(server, port);

                //string guessMessage = (message + "," + usrRandom);
                string sendMessage = (message + "," + min + "," + max + "," + usrRandom + "," + usrGuess + "," + gameStatus + "," + command);

                Byte[] data = System.Text.Encoding.ASCII.GetBytes(sendMessage);     // Translate the passed message into ASCII and store it as a Byte array.             
                NetworkStream stream = client.GetStream();                          // Get a client stream for reading and writing.
                stream.Write(data, 0, data.Length);                                 // Send the message to the connected TcpServer. 
                data = new Byte[256];                                               // Receive the TcpServer.response. Buffer to store the response bytes.
                String responseData = String.Empty;                                 // String to store the response ASCII representation.

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                serverResponse = responseData;

                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e)
            {
                resetValues();
                MessageBox.Show("ArgumentNullException: " + e.Message);
            }
            catch (SocketException e)
            {
                resetValues();
                MessageBox.Show("SocketException: " + e.Message);
            }
        }

        // FUNCTION         : btnPlayAgain_Click
        // DESCRIPTION      :
        //      This function is used to reset all of the values and
        //      tell the server thats the user wants to reststart the
        //      game and play again. The game will essentall be reset
        //      so that the user may play the game again.
        // PARAMETERS       : object sender, routedEventArgs e
        // RETURNS          : none
        private void btnPlayAgain_Click(object sender, RoutedEventArgs e)
        {
            // resetting all values for user and game
            usrGuess = "0";        
            usrRandom = "0";
            min = "0";
            max = "0";
            gameStatus = "0";

            // command that the user wishes to play again
            command = "PA";     

            // resetting lables in WPF
            txtGuess.Text = "";
            lblStatus.Content = "[ Guess Hint ]";
            lblRange.Content = "Your Range is Between: ";

            // changing the status of buttons and textboxes
            btnGuess.IsEnabled = true;
            btnPlayAgain.IsEnabled = false;
            txtGuess.IsEnabled = true;

            // sending message to client 
            ConnectClient(usrIpAddress, usrName);

            // spliting string and setting values
            string[] words = responseMessage.Split(',');
            min = words[1];
            max = words[2];
            usrRandom = words[3];
            gameStatus = words[5];

            // updating label in WPF with new range
            lblRange.Content = ("Your range is between " + min + " - " + max);
        }

        // FUNCTION         : btnDisconnect_Click
        // DESCRIPTION      :
        //      This function is called when the user clicks the disconnect button.
        //      The function will then send a message to the server saying that the
        //      user would like to disconnect. If the server accepts, the user will
        //      then be disconnectd from the game and will need to re-connect if they
        //      want to play the game again.
        // PARAMETERS       : object sender, routedEventArgs e
        // RETURNS          : none
        private void btnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            command = "D";      // command that the user wishes to disconnect

            // calling function to send message to server
            ConnectClient(usrIpAddress, usrName);

            // splitting and reading string to check if
            // sever allows for disconnection.
            string[] words = responseMessage.Split(',');
            string returnCommand = words[6];
            if (returnCommand == "Accepted")
            {
                // prompting user to confirm wish to disconnect
                MessageBoxResult choice = MessageBox.Show("Are You Sure You Want to Disconnect?", "Disconnect Confirmation", MessageBoxButton.YesNoCancel);
                if (choice == MessageBoxResult.Yes)
                {
                    // calling function to reset game to default values
                    resetValues();
                }
            }
            else
            {
                // server did not accept disconnect
                MessageBox.Show("Cannot Disconnect From Server. Try Again.");
            }
        }

        // FUNCTION         : btnPlayAgain_Click
        // DESCRIPTION      :
        //      This function is called to reset all of the fields and
        //      reset the labels and reset all of the button and textboxes
        //      to their default value.
        // PARAMETERS       : object sender, routedEventArgs e
        // RETURNS          : none
        private void resetValues()
        {
            // resetting all of the values and lables 
            usrGuess = "0";
            usrRandom = "0";
            min = "0";
            max = "0";
            gameStatus = "0";
            command = "NG";
            txtGuess.Text = "";
            lblStatus.Content = "[ Guess Hint ]";
            lblRange.Content = "Your Range is Between: ";

            // resetting the button and textboxes
            btnPlayAgain.IsEnabled = false;
            btnConnect.IsEnabled = true;
            txtIPAddress.IsEnabled = true;
            txtName.IsEnabled = true;
            txtPortNumber.IsEnabled = true;
            txtGuess.IsEnabled = false;
            btnGuess.IsEnabled = false;
            btnDisconnect.IsEnabled = false;

            // resetting the value to state user is disconnected 
            connected = false;
        }

        // FUNCTION         :  Window_Closing
        // DESCRIPTION      :
        //      This function is called when the user wants to exit the program.
        //      The user will be able to exit normally as long as they are not
        //      connected. If the user is connected, the program will send a 
        //      a message to the server saying the user wishes to disconnect,
        //      if accepeted, the user will disonnect and the window will close.
        // PARAMETERS       : object sender, routedEventArgs e
        // RETURNS          : none
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // checking is user is connected
            if (connected)
            {
                command = "D";  // command that the user wishes to disconnect

                // calling function to contact server
                ConnectClient(usrIpAddress, usrName);

                // if the server accpects the disconnect, the user will
                //  be prompted to confirm their wish to exit program.
                string[] words = responseMessage.Split(',');
                string returnCommand = words[6];
                if (returnCommand == "Accepted")
                {
                    // prompting user to confirm wish to disconnect.
                    MessageBoxResult choice = MessageBox.Show("Are You Sure You Want to Disconnect and Exit?", "Disconnect Confirmation", MessageBoxButton.YesNoCancel);
                    if (choice == MessageBoxResult.No || choice == MessageBoxResult.Cancel)
                    {
                        e.Cancel = true;
                    }
                }
                else
                {
                    // server did not accept disconnect
                    MessageBoxResult choice = MessageBox.Show("Issue communicating with server in order to disconnect. Close Anyway?", "Force Disconnect Confirmation", MessageBoxButton.YesNoCancel);
                    if (choice == MessageBoxResult.No || choice == MessageBoxResult.Cancel)
                    {
                        e.Cancel = true;
                    }
                }
            }
        }
    }
}
