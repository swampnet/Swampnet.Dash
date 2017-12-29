using Newtonsoft.Json;
using Swampnet.Dash.Common.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Swampnet.Dash
{
    public static class Api
    {
		private static string _apiRoot = "http://localhost:8080/";
		private static string _apiKey = "";

		public static Task<Dashboard> GetDashboard(string dashId)
		{
			return GetAsync<Dashboard>($"dashboards/{dashId}");
		}


		public static Task<IEnumerable<ElementState>> GetDashState(string dashId)
		{
			return GetAsync<IEnumerable<ElementState>>($"dashboards/{dashId}/state");
		}

		public static Task<IEnumerable<Varient>> GetHistory(string itemDefinitionId, string propertyName, TimeSpan history)
		{
			return GetAsync<IEnumerable<Varient>>($"values/{itemDefinitionId}/{propertyName}?seconds={(int)history.TotalSeconds}");
		}

		private static async Task<string> GetAsync(string endpoint)
		{
			using (var client = new HttpClient())
			{
				client.DefaultRequestHeaders.Add("x-api-key", _apiKey);

				var rs = await client.GetAsync(_apiRoot + endpoint);

				rs.EnsureSuccessStatusCode();

				var response = await rs.Content.ReadAsStringAsync();

				return response;
			}
		}


		private static async Task<T> GetAsync<T>(string endpoint)
		{
			var json = await GetAsync(endpoint);

			return JsonConvert.DeserializeObject<T>(json);
		}
	}
}
