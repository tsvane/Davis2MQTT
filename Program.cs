using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading;

namespace Davis2Mqtt
{
    class Program
    {
        static void Main(string[] args)
        {
            // Read config
            string MqttBroker = ConfigurationManager.AppSettings["MqttBroker"].ToString();
            string MqttBasePath = ConfigurationManager.AppSettings["MqttBasePath"].ToString();
            string SerialPortName = ConfigurationManager.AppSettings["SerialPortName"].ToString();
            int UpdateIntervalSeconds = Convert.ToInt32(ConfigurationManager.AppSettings["UpdateIntervalSeconds"]);
            bool OnlyPublishIfUpdated = Convert.ToBoolean(ConfigurationManager.AppSettings["OnlyPublishIfUpdated"]);

            // Create MQTT object
            MqttHandler mqttHandler = null;
            bool retry = true;
            do
            {
                try
                {
                    Console.WriteLine("[{0}] INFO: Connecting to MQTT broker {1}, Path {2}", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"), MqttBroker, MqttBasePath);
                    mqttHandler = new MqttHandler(MqttBroker, MqttBasePath);
                    Console.WriteLine("[{0}] INFO: Connected!", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                    retry = false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[{0}] ERROR: {1}", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"), ex.Message);
                    Thread.Sleep(1000);
                }
            } while (retry);

            WeatherLinkHandler weather = new WeatherLinkHandler();
            WeatherData weatherData = new WeatherData();

            while (true)
            {
                Stopwatch Watch = Stopwatch.StartNew();

                Console.WriteLine("[{0}] INFO: Executing", DateTime.Now);

                try
                {
                    Console.WriteLine("[{0}] INFO: Connecting to Davis on port {1}", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"), SerialPortName);
                    weather.GetWeatherLinkData(SerialPortName);
                    weatherData.Update(weather);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[{0}] ERROR: {1}", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"), ex.Message);
                }

                foreach(WeatherDataItem weatherDataItem in weatherData.WeatherDataItems)
                {
                    if (weatherDataItem.Published && OnlyPublishIfUpdated)
                        continue;

                    weatherDataItem.Published = true;
                    Console.WriteLine("[{0}] INFO:    Publishing {1} Value {2} (RawValue {3})", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"), weatherDataItem.Name, weatherDataItem.Value, weatherDataItem.RawValue);
                    mqttHandler.Publish(weatherDataItem.Name, weatherDataItem.Value);
                }

                Watch.Stop();
                int ElapsedMs = Convert.ToInt32(Watch.ElapsedMilliseconds);
                int TimeToSleepMs = (UpdateIntervalSeconds * 1000) - ElapsedMs;

                Console.WriteLine("[{0}] INFO: Sleeping for {1} milliseconds", DateTime.Now, TimeToSleepMs);
                Thread.Sleep(TimeToSleepMs);
            }
        }
    }
}
