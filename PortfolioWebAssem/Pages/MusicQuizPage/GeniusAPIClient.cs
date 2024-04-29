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
using FuzzySharp;

namespace PortfolioWebAssem.Pages.MusicQuizPage
{
	public class GeniusAPIClient
	{
		private readonly string _apiKey = "RpBaUk-iRsnuJJ8ozLN7ZCjSQyd-MU4Vnp2F2HxOllHYPRpVRgooijV63QEToIdT";
		public string NewArtistName = "";

		public async Task<string> GetArtistIdUsingArtistName(string artistName)
		//returns Genius API's Artist ID for a selected artist.
		{
			string _fetchURLForArtistID = $"https://api.genius.com/search?q={artistName}&access_token={_apiKey}";
			HttpClient httpClient = new();
			string artistID = "";
			Console.WriteLine("inside function");
			try { 
			//fetch
			var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, _fetchURLForArtistID);
			var response = await httpClient.SendAsync(httpRequestMessage);
			var returnObject = await response.Content.ReadFromJsonAsync<ArtistIDResponse>();
				Console.WriteLine();
			double artistSimilarity;
			//check if this artist has any songs
			//if (returnObject.response.hits.Count > 0)
			//{
			//	artistID = returnObject.response.hits.FirstOrDefault().result.primary_Artist.id.ToString();
			//	NewArtistName = returnObject.response.hits.FirstOrDefault().result.primary_Artist.name.ToString();
			//}

			foreach (Hit hit in returnObject.response.hits)
				{
					artistSimilarity = Fuzz.Ratio(hit.result.primary_Artist.name, artistName) ;
					//check if SELECTED artistName matches this artist name
					Console.WriteLine($"Checking {hit.result.primary_Artist.name} against {artistName}, score: {artistSimilarity}");
					if (artistSimilarity > 90)
					{
						//found match - 90 in case of different spacing etc.
						artistID = hit.result.primary_Artist.id.ToString();
						NewArtistName = hit.result.primary_Artist.name.ToString();
						break;
					}
				}
			} catch (Exception e)
			{
				Console.WriteLine($"Could not find artistID for {artistName}. Exception: {e}");
			}
			return artistID;
		}

		public async Task<string> GetArtistIdUsingSongTitle(Recording recording)
		//returns Genius API's Artist ID for a selected song title + artist combo.
		{
			string _fetchURLForArtistID = $"https://api.genius.com/search?q={recording.title}&access_token={_apiKey}";
			HttpClient httpClient = new();
			string artistID = "";

			try
			{
				//fetch
				var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, _fetchURLForArtistID);
				var response = await httpClient.SendAsync(httpRequestMessage);
				var returnObject = await response.Content.ReadFromJsonAsync<ArtistIDResponse>();
				double songTitleSimilarity;

				foreach (Hit hit in returnObject.response.hits)
				{
					songTitleSimilarity = Fuzz.Ratio(hit.result.primary_Artist.name.ToLower(), recording.artistcredit.First().name.ToLower());
					//check if SELECTED artistName matches this song's artist name
					Console.WriteLine($"Checking {hit.result.title} against {recording.title}, score: {songTitleSimilarity}");
					if (songTitleSimilarity > 90)
					{
						//found match - 90 in case of different spacing etc.
						artistID = hit.result.primary_Artist.id.ToString();
						NewArtistName = hit.result.primary_Artist.name.ToString();
						break;
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine($"Could not find artistID for {recording.title}. Exception: ", e);
			}
			return artistID;
		}

		public async Task<List<string>> GetSongTitles(string artistId, int numberOfSongs)
		//returns Genius API's Song Titles using Genius API's Artist ID. Returns numberOfSongs song titles - can be increased
		{
			List<Song> songsFromFetch = new();
			List<string> songTitles = new();
			HttpClient httpClient = new();

			//base case
			//numberOfSongs = 20;

			string _fetchURLForSongs = $"https://api.genius.com/artists/{artistId}/songs?sort=popularity&per_page={numberOfSongs}&access_token={_apiKey}";
			//sometimes the api returns an empty string for artistId since response.hits is empty - see "1StepKloser", so skip fetch
			if (artistId != "")
			{
				try
				{
					//fetch
					var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, _fetchURLForSongs);
					var response = await httpClient.SendAsync(httpRequestMessage);
					var returnObject = await response.Content.ReadFromJsonAsync<SongsResponse>();

					songsFromFetch = returnObject.response.songs;
					//put song titles in my list
					
						foreach (Song song in songsFromFetch)
						{
							songTitles.Add(song.title);
						}
					
				}
				catch (Exception e)
				{
					Console.WriteLine($"Could not find song titles for {artistId}. Exception: ", e);
				}
			}
			//numberOfSongs song titles
			return songTitles;
		}
		
	}
}
