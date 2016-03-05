/*Copyright(c) 2016 Steven Sheffey
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Net;
using System.Collections.Specialized;
using System.Diagnostics;
namespace Remote
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Stores whether the controller is connected or not
        private bool m_IsConnected;
        //Stores the ip address of the server
        private String m_IPAddress;
        //Used for sending requests to the server
        private WebClient m_RequestSender;
        public MainWindow()
        {
            InitializeComponent();
            m_RequestSender = new WebClient();
            m_IsConnected = false;
        }
        /// <summary>
        /// Updates the ip address when this textbox is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ipAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            m_IPAddress = ipAddress.Text;
        }
        /// <summary>
        /// Sends a button action to the STB
        /// </summary>
        /// <param name="keyName">The button to send to the STB</param>
        private bool sendButton(String keyName)
        {
            //Check if the server is connected
            if (!m_IsConnected)
            {
                //TODO: Some sort of error message
                return false;
            }
            //Construct the URI
            String uri = "http://" + m_IPAddress + ":8080/remote/processKey";
            //Create a dictionary for constructing the request
            NameValueCollection query = new NameValueCollection();
            //Add the key parameter
            query.Add("key", keyName);
            m_RequestSender.QueryString = query;
            byte[] response = m_RequestSender.DownloadData(uri);
            debugBox.Text = Encoding.UTF8.GetString(response);
            m_RequestSender.QueryString = new NameValueCollection();
            return true;
        }

        private void key_Click(object sender, RoutedEventArgs e)
        {
            sendButton((sender as Button).Tag.ToString());
        }

        private bool attemptConnection()
        {

            //Get IP address from box
            m_IPAddress = ipAddress.Text;
            //Construct URI
            String uri = "http://" + m_IPAddress + ":8080/";
            //Objects for handling http request
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            try
            {
                //Construct request
                request = (HttpWebRequest)(WebRequest.Create(uri));
                //Execute request
                response = (HttpWebResponse)(request.GetResponse());
            }
            //This is the expected case: STB returns a 403
            catch(WebException ex)
            {
                //Response is captured from the exception
                response = (HttpWebResponse)ex.Response;
            }
            //This is for when the connection fails, probably due to it being wrong
            catch(UriFormatException)
            {
                MessageBox.Show("Invalid ip address entered.");
                return false;
            }
            if (response == null)
            {
                MessageBox.Show("No response recieved.");
            }
            //If errors have not occured, connection was successful
            return true;
        }

        private void connect_Click(object sender, RoutedEventArgs e)
        {
            m_IsConnected = attemptConnection();
            if(m_IsConnected)
            {
                connectionStatus.Text = "CONNECTED";
            }
            else
            {
                connectionStatus.Text = "DISCONNECTED";
            }
        }
    }
}
