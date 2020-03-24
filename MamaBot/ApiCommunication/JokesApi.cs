using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MamaBot.ApiCommunication
{
    public class JokesApi
    {
        public static async Task<string> GetJoke()
        {

            string _address = "http://api.icndb.com/jokes/random";

            HttpClient client = new HttpClient();
            var response = await client.GetStringAsync(_address);
            JObject joke = JObject.Parse(response);
            string Joke = joke["value"]["joke"].ToString();

            return Joke;
        
        
        }


            
    }
}
