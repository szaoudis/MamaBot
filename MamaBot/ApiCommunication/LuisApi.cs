using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MamaBot.ApiCommunication
{
    public class LuisApi
    {
        public static async Task<string> SendUserInputToLuis(string userInput)
        {
            string input = userInput;
            string req = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/" + Keys.LuisFullApi().ToString() + input;
            HttpClient client = new HttpClient();
            var res = await client.GetStringAsync(req);
            var LuisRespone = JObject.Parse(res);
            var Intent = LuisRespone["topScoringIntent"]["intent"].ToString();
            return Intent;
           
        }
    }
}
