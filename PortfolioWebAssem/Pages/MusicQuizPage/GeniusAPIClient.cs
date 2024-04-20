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

		public async Task<string> GetArtistId(string artistName)
		{
			string _fetchURLForArtistID = $"https://api.genius.com/search?q={artistName}&access_token={_apiKey}";
			HttpClient httpClient = new();
			string artistID = "";

			try { 
			//fetch
			var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, _fetchURLForArtistID);
			var response = await httpClient.SendAsync(httpRequestMessage);
			var returnObject = await response.Content.ReadFromJsonAsync<ArtistIDResponse>();

			//check if this artist has any songs
			if (returnObject.response.hits.Count > 0)
			{
				artistID = returnObject.response.hits.FirstOrDefault().result.primary_Artist.id.ToString();
			}

			} catch (Exception e)
			{
				Console.WriteLine(e);
			}
			return artistID;
		}

		public async Task<List<string>> GetSongTitles(string artistId)
		{
			List<Song> songsFromFetch = new();
			List<string> songTitles = new();
			string _fetchURLForSongs = $"https://api.genius.com/artists/{artistId}/songs?sort=popularity&access_token={_apiKey}";
			HttpClient httpClient = new();

			//sometimes the api returns an empty string for artistID since response.hits is empty - see "1StepKloser", so skip fetch
			if (artistId != "")
			{
				try
				{
					//fetch
					var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, _fetchURLForSongs);
					var response = await httpClient.SendAsync(httpRequestMessage);
					var returnObject = await response.Content.ReadFromJsonAsync<SongsResponse>();

					//put song titles in the list and return
					songsFromFetch = returnObject.response.songs;
					foreach (Song song in songsFromFetch)
					{
						songTitles.Add(song.title);
					}
					//songTitles.OrderBy(_ => Guid.NewGuid());
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
			}
			return songTitles;
		}
		
	}
}
