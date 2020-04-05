using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MamaBot.ApiCommunication
{
    public class RecipeApi
    {
        public static async Task<JToken> GetRecipe(string RecipeItem)
        {
            string itemOfRecipe = RecipeItem;
            string _address = "https://api.spoonacular.com/recipes/complexSearch?query=" + itemOfRecipe+"&apiKey=" + Keys.RecipeKey().ToString();

            HttpClient client = new HttpClient();
            var response = await client.GetStringAsync(_address);
            JObject Recipe = JObject.Parse(response);
            var recipe = Recipe["results"];

            return recipe;


        }
    }
}
