using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;


namespace MamaBot.ApiCommunication
{
    public class WeatherApi
    {
        public static async Task<JToken> GetWeather(string userInput)
        {
            string input = userInput;
            string _address = "https://api.openweathermap.org/data/2.5/weather?q=" + input + "&appid=" + Keys.WeatherKey().ToString() + "&units=metric";

            HttpClient client = new HttpClient();
            var response = await client.GetStringAsync(_address);
            JObject weather = JObject.Parse(response);
            string Temperature = weather["main"]["temp"].ToString();
            string Location = weather["name"].ToString();


            return "Location: " + Location + " Temperature: " + Temperature + " Sky: ";


        }
    }
}
