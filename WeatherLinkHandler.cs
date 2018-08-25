using System;
using System.IO.Ports;
namespace Davis2Mqtt
{
    class WeatherLinkHandler
    {
        public WeatherLoopData Data;

        public void GetWeatherLinkData(string PortName)
        {
            byte[] RawWeatherData;
            SerialPort serialPort;

            serialPort = OpenSerialPort(PortName);

            if (serialPort != null)
            {
                if (WakeSerialVantage(serialPort))
                {
                    Data = new WeatherLoopData();
                    RawWeatherData = RetrieveSerialData(serialPort, "LOOP 1", 95);
                    if (RawWeatherData != null)
                    {
                        Data.LoopDataArray = RawWeatherData;
                    }
                    else
                        Show_Message("Lost Connection to Vantage Weatherstation");
                }
                else
                    Show_Message("Cannot Connect to Vantage Weatherstation");

                serialPort.Close();
            }
        }

        // Open the serial port for communication
        private SerialPort OpenSerialPort(string PortName)
        {
            try
            {
                SerialPort thePort = new SerialPort(PortName, 19200, Parity.None, 8, StopBits.One);
                thePort.ErrorReceived += new SerialErrorReceivedEventHandler(SerialPort_ErrorReceived);
                thePort.ReadTimeout = 2500;
                thePort.WriteTimeout = 2500;
                thePort.DtrEnable = true; // Set Data Terminal Ready to true - can't transmit without DTR turned on
                thePort.Open();
                return (thePort);
            }
            catch (Exception ex)
            {
                Show_Message(ex.ToString());
                return (null);
            }
        }

        private void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs ex)
        {
            Show_Message(ex.ToString());
        }

        // The Vantage weather station sleeps when it can to save power.  In order to get it to respond to commands, it 
        // needs to "wake up."  To wake it up, it needs to receive a '\n' (newline character).  It responds with a '\n\r'.
        // If no response arrives after 1.2 seconds (the max delay according to the Davis documentation), we try again.
        private bool WakeSerialVantage(SerialPort thePort)
        {
            int currentWaitTime = 0;
            int retryWaitTime = 100;
            int maxWaitTime = 1200;

            try
            {
                // Clear out both input and output buffers just in case something is in there already
                thePort.DiscardInBuffer();
                thePort.DiscardOutBuffer();

                do
                {
                    // Put a newline character ('\n') out the serial port - the Writeline method terminates with a '\n' of its own
                    thePort.WriteLine("");
                    // The Vantage documentation states that 1.2 seconds is the maximum delay - wait for that amount of time
                    System.Threading.Thread.Sleep(retryWaitTime);
                    currentWaitTime += retryWaitTime;
                } while (thePort.BytesToRead == 0 && currentWaitTime < maxWaitTime);

                // Vantage found and awakened
                if (currentWaitTime < maxWaitTime)
                {
                    // Now that the Vantage is awake, clean out the input buffer again for good measure.
                    thePort.DiscardInBuffer();
                    return (true);
                }
                else
                    return (false);
            }
            catch (Exception ex)
            {
                Show_Message(ex.ToString());
                return (false);
            }
        }

        // Retrieve_Command retrieves data from the Vantage weather station using the specified command
        private byte[] RetrieveSerialData(SerialPort thePort, string commandString, int returnLength)
        {
            bool FoundACK = false;
            int ACK = 6; // ASCII 6
            int passCount = 0;
            int maxPasses = 4;
            int currChar;

            try
            {
                // Clean out the input (receive) buffer just in case something showed up in it
                thePort.DiscardInBuffer();
                // . . . and clean out the output buffer while we're at it for good measure
                thePort.DiscardOutBuffer();

                // Try the command until we get a clean ACKnowledge from the Vantage.  We count the number of passes since
                // a timeout will never occur reading from the sockets buffer.  If we try a bunch of times (maxPasses) and
                // we get nothing back, we assume that the connection is busted
                while (!FoundACK && passCount < maxPasses)
                {
                    thePort.WriteLine(commandString);
                    // I'm using the LOOP command as the baseline here because many its parameters are a superset of
                    // those for other commands.  The most important part of this is that the LOOP command is iterative
                    // and the station waits 2 seconds between its responses.  Although it's not clear from the documentation, 
                    // I'm assuming that the first packet isn't sent for 2 seconds.  In any event, the conservative nature
                    // of waiting this amount of time probably makes sense to deal with serial IO in this manner anyway.
                    System.Threading.Thread.Sleep(2000);

                    // Wait for the Vantage to acknowledge the the receipt of the command - sometimes we get a '\n\r'
                    // in the buffer first or nor response is given.  If all else fails, try again.
                    while (thePort.BytesToRead > 0 && !FoundACK)
                    {
                        // Read the current character
                        currChar = thePort.ReadChar();
                        if (currChar == ACK)
                            FoundACK = true;
                    }

                    passCount++;
                }

                // We've tried a bunch of times and have heard nothing back from the port (nothing's in the buffer).  Let's 
                // bounce outta here
                if (passCount == maxPasses)
                    return null;
                else
                {
                    // Allocate a byte array to hold the return data that we care about - up to, but not including the '\n'
                    // Size the array according to the data passed to the procedure
                    byte[] loopString = new byte[returnLength];

                    // Wait until the buffer is full - we've received returnLength characters from the LOOP response, 
                    // including the final '\n' 
                    while (thePort.BytesToRead <= loopString.Length)
                    {
                        // Wait a short period to let more data load into the buffer
                        System.Threading.Thread.Sleep(200);
                    }

                    // Read the first returnLength bytes of the buffer into the array
                    thePort.Read(loopString, 0, returnLength);

                    return loopString;
                }
            }
            catch (Exception ex)
            {
                Show_Message(ex.ToString());
                return null;
            }
        }

        private void Show_Message(string msgString)
        {
            Console.WriteLine(msgString);
        }
    }
}
