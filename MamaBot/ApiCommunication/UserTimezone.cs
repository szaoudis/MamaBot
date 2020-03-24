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
    public class ApiCommunication
    {
        public static async Task<string> GetCountryName(string CountryName)
        {
            string CountryFlag = "0";

            string _address = "https://restcountries.eu/rest/v2/name/" + CountryName;
            try
            {
                HttpClient client = new HttpClient();
                var response = await client.GetStringAsync(_address);
                JArray a = (JArray)JsonConvert.DeserializeObject(response);

                var countryName = a[0].First.First.ToString();
                var l = a[0].Children();
                var p = a[0].Children().Children();
                var pl = a[0].Select(x => x).ToList();
                var timezone = pl[14].First.ToList();
                List<string> ListofTimezones = timezone.Values<string>().ToList();
                CountryInfo.Timezones = ListofTimezones;
                CountryInfo.CountryName = countryName;
                CountryFlag = "1";


            }
            catch (Exception e)
            {
                if (e.Message == "Response status code does not indicate success: 404 ().")
                {

                    CountryFlag = "2";
                }

            }

            return CountryFlag;
        }

    }
    
    public class UserCurrentTime
    {
        public static DateTime UserTime { get; set; }
    }

    public class CountryInfo
    {
        public static List<string> Timezones { get; set; }
        public static string CountryName { get; set; }
    }
 
}
