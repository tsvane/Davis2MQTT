using System;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Davis2Mqtt
{
    class MqttHandler
    {
        private MqttClient mqttClient;
        private string BaseTopic;

        public MqttHandler(string Server, string BaseTopic)
        {
            this.BaseTopic = BaseTopic;
            mqttClient = new MqttClient(Server);
            mqttClient.Connect(Guid.NewGuid().ToString());
        }

        public void Publish(string ItemName, string Value)
        {
            string Topic = string.Format("{0}/{1}", BaseTopic, ItemName);
            mqttClient.Publish(Topic, Encoding.UTF8.GetBytes(Value), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        }
    }
}
