using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ConsoleApp1
{
    class JsonFeed
    {
        static string _url = "";
        private readonly int _results;

        public JsonFeed() { }
        public JsonFeed(string endpoint, int results)
        {
            _url = endpoint;
            _results = results;
        }
        
		public IEnumerable<string> GetRandomJokes(string firstname, string lastname, string category)
		{
			HttpClient client = new HttpClient();
			client.BaseAddress = new Uri(_url);
			string url = "jokes/random";
			if (category != null)
			{
				if (url.Contains('?'))
					url += "&";
				else url += "?";
				url += "category=";
				url += category;
			}

            for (int i = 0; i < _results; i++)
            {
                string joke = Task.FromResult(client.GetStringAsync(url).Result).Result;

                if (firstname != null && lastname != null)
                {
                    joke = joke.Replace("Chuck Norris", $"{firstname} {lastname}");
                }

                yield return JsonConvert.DeserializeObject<dynamic>(joke).value;
            }
        }

        /// <summary>
        /// returns an object that contains name and surname
        /// </summary>
        /// <param name="client2"></param>
        /// <returns></returns>
		public static dynamic Getnames()
		{
			HttpClient client = new HttpClient();
			client.BaseAddress = new Uri(_url);
			var result = client.GetStringAsync("").Result;
			return JsonConvert.DeserializeObject<dynamic>(result);
		}

		public static string[] GetCategories()
		{
			HttpClient client = new HttpClient();
			client.BaseAddress = new Uri(_url);

			return new string[] { Task.FromResult(client.GetStringAsync("jokes/categories").Result).Result };
		}
    }
}
