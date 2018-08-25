using System;
using System.Collections.Generic;

namespace Davis2Mqtt
{
    public class WeatherData
    {
        public List<WeatherDataItem> WeatherDataItems = new List<WeatherDataItem>();

        public WeatherData()
        {
            WeatherDataItems.Add(new WeatherDataItem("BarometricTrend", DataType.OTHER));
            WeatherDataItems.Add(new WeatherDataItem("Barometer", DataType.BAROMETER));
            WeatherDataItems.Add(new WeatherDataItem("InsideTemperature", DataType.TEMP));
            WeatherDataItems.Add(new WeatherDataItem("InsideHumidity", DataType.OTHER));
            WeatherDataItems.Add(new WeatherDataItem("OutsideTemperature", DataType.TEMP));
            WeatherDataItems.Add(new WeatherDataItem("WindSpeed", DataType.WIND));
            WeatherDataItems.Add(new WeatherDataItem("WindSpeed10MinAvg", DataType.WIND));
            WeatherDataItems.Add(new WeatherDataItem("WindDirection", DataType.DIRECTION));
            //WeatherDataItems.Add(new WeatherDataItem("ExtraTemperatures", DataType.TEMP));
            //WeatherDataItems.Add(new WeatherDataItem("SoilTemperatures", DataType.TEMP));
            //WeatherDataItems.Add(new WeatherDataItem("LeafTemperatures", DataType.TEMP));
            WeatherDataItems.Add(new WeatherDataItem("OutsideHumidity", DataType.OTHER));
            //WeatherDataItems.Add(new WeatherDataItem("ExtraHumidities", DataType.OTHER));
            WeatherDataItems.Add(new WeatherDataItem("RainRate", DataType.RAIN));
            WeatherDataItems.Add(new WeatherDataItem("UV", DataType.OTHER));
            WeatherDataItems.Add(new WeatherDataItem("SolarRadiation", DataType.OTHER));
            WeatherDataItems.Add(new WeatherDataItem("StormRain", DataType.RAIN));
            //WeatherDataItems.Add(new WeatherDataItem("StartDateofcurrentStorm", DataType.OTHER));
            WeatherDataItems.Add(new WeatherDataItem("RainDay", DataType.RAIN));
            WeatherDataItems.Add(new WeatherDataItem("RainMonth", DataType.RAIN));
            WeatherDataItems.Add(new WeatherDataItem("RainYear", DataType.RAIN));
            WeatherDataItems.Add(new WeatherDataItem("DayET", DataType.RAIN));
            WeatherDataItems.Add(new WeatherDataItem("MonthET", DataType.RAIN));
            WeatherDataItems.Add(new WeatherDataItem("YearET", DataType.RAIN));
            //WeatherDataItems.Add(new WeatherDataItem("SoilMoistures", DataType.OTHER));
            //WeatherDataItems.Add(new WeatherDataItem("LeafWetnesses", DataType.OTHER));
            //WeatherDataItems.Add(new WeatherDataItem("InsideAlarms", DataType.OTHER));
            //WeatherDataItems.Add(new WeatherDataItem("RainAlarms", DataType.OTHER));
            //WeatherDataItems.Add(new WeatherDataItem("OutsideAlarms", DataType.OTHER));
            //WeatherDataItems.Add(new WeatherDataItem("ExtraTempHumAlarms", DataType.OTHER));
            //WeatherDataItems.Add(new WeatherDataItem("SoilLeafAlarms", DataType.OTHER));
            //WeatherDataItems.Add(new WeatherDataItem("TransmitterBatteryStatus", DataType.OTHER));
            WeatherDataItems.Add(new WeatherDataItem("ConsoleBatteryVoltage", DataType.OTHER));
            //WeatherDataItems.Add(new WeatherDataItem("ForecastIcons", DataType.OTHER));
            //WeatherDataItems.Add(new WeatherDataItem("ForecastRulenumber", DataType.OTHER));
            //WeatherDataItems.Add(new WeatherDataItem("TimeOfSunrise", DataType.OTHER));
            //WeatherDataItems.Add(new WeatherDataItem("TimeOfSunset", DataType.OTHER));
        }

        internal void Update(WeatherLinkHandler weather)
        {
            WeatherLoopData weatherLoopData = weather.Data;

            if (weatherLoopData == null)
                return;

            WeatherDataItems.Find(x => x.Name == "BarometricTrend").RawValueUpdate(weatherLoopData.BarometerTrend);
            WeatherDataItems.Find(x => x.Name == "Barometer").RawValueUpdate(weatherLoopData.Barometer);
            WeatherDataItems.Find(x => x.Name == "InsideTemperature").RawValueUpdate(weatherLoopData.InsideTemperature);
            WeatherDataItems.Find(x => x.Name == "InsideHumidity").RawValueUpdate(weatherLoopData.InsideHumidity);
            WeatherDataItems.Find(x => x.Name == "OutsideTemperature").RawValueUpdate(weatherLoopData.OutsideTemperature);
            WeatherDataItems.Find(x => x.Name == "WindSpeed").RawValueUpdate(weatherLoopData.WindSpeed);
            WeatherDataItems.Find(x => x.Name == "WindSpeed10MinAvg").RawValueUpdate(weatherLoopData.WindSpeed10MinAvg);
            WeatherDataItems.Find(x => x.Name == "WindDirection").RawValueUpdate(weatherLoopData.WindDirection);
            WeatherDataItems.Find(x => x.Name == "OutsideHumidity").RawValueUpdate(weatherLoopData.OutsideHumidity);
            WeatherDataItems.Find(x => x.Name == "RainRate").RawValueUpdate(weatherLoopData.RainRate);
            WeatherDataItems.Find(x => x.Name == "UV").RawValueUpdate(weatherLoopData.UV);
            WeatherDataItems.Find(x => x.Name == "SolarRadiation").RawValueUpdate(weatherLoopData.SolarRadiation);
            WeatherDataItems.Find(x => x.Name == "StormRain").RawValueUpdate(weatherLoopData.StormRain);
            WeatherDataItems.Find(x => x.Name == "RainDay").RawValueUpdate(weatherLoopData.RainDay);
            WeatherDataItems.Find(x => x.Name == "RainMonth").RawValueUpdate(weatherLoopData.RainMonth);
            WeatherDataItems.Find(x => x.Name == "RainYear").RawValueUpdate(weatherLoopData.RainYear);
            WeatherDataItems.Find(x => x.Name == "DayET").RawValueUpdate(weatherLoopData.DayET);
            WeatherDataItems.Find(x => x.Name == "MonthET").RawValueUpdate(weatherLoopData.MonthET);
            WeatherDataItems.Find(x => x.Name == "YearET").RawValueUpdate(weatherLoopData.YearET);
            WeatherDataItems.Find(x => x.Name == "ConsoleBatteryVoltage").RawValueUpdate(weatherLoopData.ConsoleBatteryVoltage);
        }
    }

    public enum DataType { TEMP, WIND, BAROMETER, DIRECTION, RAIN, OTHER }

    public class WeatherDataItem
    {
        public string Name { get; set; }
        public DataType Type { get; set; }
        public object RawValue { get; set; }
        public bool Published { get; set; }
        public DateTime Updated { get; set; }

        public WeatherDataItem(string Name, DataType Type)
        {
            this.Name = Name;
            this.Type = Type;

            RawValue = string.Empty;
            Published = false;
            Updated = DateTime.Now;
        }

        internal void RawValueUpdate(object newRawValue)
        {
            if (!RawValue.Equals(newRawValue))
            {
                RawValue = newRawValue;
                Updated = DateTime.Now;
                Published = false;
            }
        }

        public string Value
        {
            get
            {
                string value = "";

                if (Type == DataType.BAROMETER)
                    value = ConvertToHectopascal;
                else if (Type == DataType.DIRECTION)
                    value = ConvertToDirectionName;
                else if (Type == DataType.TEMP)
                    value = ConvertToDegreeCelcius;
                else if (Type == DataType.WIND)
                    value = ConvertToMetersPrSecond;
                else if (Type == DataType.RAIN)
                    value = ConvertToMillimeters;
                else
                    value = RawValue.ToString();

                return value.Replace(",", ".");
            }
        }

        private string ConvertToDirectionName
        {
            get
            {
                if (RawValue.ToString() == string.Empty)
                    return "--";

                // The wind direction is given in degrees - 0-359 - convert to string representing the direction
                if (Convert.ToSingle(RawValue) < 11.25f) return "N";
                else if (Convert.ToSingle(RawValue) < 11.25f) return "N";
                else if (Convert.ToSingle(RawValue) < 33.75f) return "NNE";
                else if (Convert.ToSingle(RawValue) < 56.25f) return "NE";
                else if (Convert.ToSingle(RawValue) < 78.75f) return "ENE";
                else if (Convert.ToSingle(RawValue) < 101.25f) return "E";
                else if (Convert.ToSingle(RawValue) < 123.75f) return "ESE";
                else if (Convert.ToSingle(RawValue) < 146.25f) return "SE";
                else if (Convert.ToSingle(RawValue) < 168.75f) return "SSE";
                else if (Convert.ToSingle(RawValue) < 191.25f) return "S";
                else if (Convert.ToSingle(RawValue) < 213.75f) return "SSW";
                else if (Convert.ToSingle(RawValue) < 236.25f) return "SW";
                else if (Convert.ToSingle(RawValue) < 258.75f) return "WSW";
                else if (Convert.ToSingle(RawValue) < 281.25f) return "W";
                else if (Convert.ToSingle(RawValue) < 303.75f) return "WNW";
                else if (Convert.ToSingle(RawValue) < 326.25f) return "NW";
                else if (Convert.ToSingle(RawValue) < 348.75f) return "NNW";
                else return "N";
            }
        }

        private string ConvertToDegreeCelcius
        {
            get
            {
                if (RawValue.ToString() == string.Empty)
                    return "0";

                return Math.Round(((Convert.ToSingle(RawValue) - 32) * 5 / 9), 2).ToString();
            }
        }

        private string ConvertToMetersPrSecond
        {
            get
            {
                if (RawValue.ToString() == string.Empty)
                    return "0";

                return Math.Round((Convert.ToSingle(RawValue) * 0.44704f), 2).ToString();
            }
        }

        private string ConvertToHectopascal
        {
            get
            {
                if (RawValue.ToString() == string.Empty)
                    return "";

                return Math.Round((Convert.ToSingle(RawValue) * 33.8639f), 2).ToString();
            }
        }

        private string ConvertToMillimeters
        {
            get
            {
                if (RawValue.ToString() == string.Empty)
                    return "";

                return Math.Round((Convert.ToSingle(RawValue) * .2f), 2).ToString(); // Not inches but "clicks"
            }
        }
    }
}
