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
            HttpClient client = new HttpClient();
            string req = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/3e10c9ba-e501-488a-829f-27f36511b80f?verbose=true&timezoneOffset=0&subscription-key=02987c1c72cc4ffba847d9adb353589d&q=" + input;
            var res = await client.GetStringAsync(req);
            var LuisRespone = JObject.Parse(res);
            var Intent = LuisRespone["topScoringIntent"]["intent"].ToString();
            return Intent;
           
        }
    }
}
