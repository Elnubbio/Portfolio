using Newtonsoft.Json.Linq;
using PortfolioWebAssem.Models;
using PortfolioWebAssem.Models2;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace PortfolioWebAssem.Pages.MusicQuizPage
{
	public class GeniusAPIClient
	{
		private readonly string _apiKey = "RpBaUk-iRsnuJJ8ozLN7ZCjSQyd-MU4Vnp2F2HxOllHYPRpVRgooijV63QEToIdT";
		private readonly string _client_id = "nYqyQhaxX-aga5RJaPGD0fsC7Ai-KO_vLotWD8mxwNAt2KqAjsvAmkie4fFG63Mb";
		private readonly string _redirectURI = "localhost:5026";


		public async Task<string> getAccessToken()
		{
			string _fetchURLForAuth = $"https://api.genius.com/oauth/authorize?client_id={_client_id}&response_type=token&redirect_uri={_redirectURI}";
			HttpClient httpClient = new();

			var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, _fetchURLForAuth);
			var response = await httpClient.SendAsync(httpRequestMessage);
			var returnObject = await response.Content.ReadFromJsonAsync<dynamic>();

			return returnObject.access_token;
		}

		public async Task<string> getArtistId(string artistName, string accessToken)
		{
			string _fetchURLForArtistID = $"https://api.genius.com/search?q={artistName}&access_token={_apiKey}";
			HttpClient httpClient = new();

			var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, _fetchURLForArtistID);
			var response = await httpClient.SendAsync(httpRequestMessage);
			var returnObject = await response.Content.ReadFromJsonAsync<ArtistIDResponse>();
			Console.WriteLine(returnObject.response);
			Console.WriteLine(returnObject.response.hits.FirstOrDefault().result.primary_Artist.id.ToString());
			return returnObject.response.hits.FirstOrDefault().result.primary_Artist.id.ToString();
		}

		public async Task<List<string>> getSongTitles(string artistId, string accessToken)
		{
			List<Song> songsFromFetch = new();
			List<string> songTitles = new();
			string _fetchURLForSongs = $"https://api.genius.com/artists/{artistId}/songs?sort=popularity&access_token={_apiKey}";
			//I put /artists/${artistId}/.. because JS :'(
			HttpClient httpClient = new();

			var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, _fetchURLForSongs);
			var response = await httpClient.SendAsync(httpRequestMessage);
			var returnObject = await response.Content.ReadFromJsonAsync<SongsResponse>();

			songsFromFetch = returnObject.response.songs;

			foreach (Song song in songsFromFetch)
			{
				songTitles.Add(song.title);
			}
			return songTitles;
		}
		
	}
}
