using System.Text.RegularExpressions;

namespace PortfolioWebAssem.Pages.MusicQuizPage
{
	//API Client for fetching lyrics
	public static class LyricsAPIClient
	{
		//TODO - replace this api with custom web scraping some random lyric website, this api has too many BIG songs without lyrics
		 public static async Task<List<string>> FetchLyricsAsync(string artistName, string songName) {
			//LYRICS API FETCH - gives the lyrics for a given artist and song
			//Have to use this API since Genius won't let you get lyrics without manually scraping
			//possible problem with mismatch between geniusAPI song name and this lyric's api song name
			try
			{
				//Add check to replace apostrophe with backtick for songName because API did that for some reason
				if (songName.Contains('’'))
				{
					songName = songName.Replace("’", "%60");
				}
				var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"https://api.lyrics.ovh/v1/{artistName}/{songName}");
				var httpClient = new HttpClient();
				var response = await httpClient.SendAsync(httpRequestMessage);
		 		return getLyricSnippet(await response.Content.ReadAsStringAsync());
			} catch (Exception e)
			{
				Console.WriteLine($"Can not find lyrics for {artistName} {songName}");
				return new List<string>();
			}
		 }

		private static List<string> getLyricSnippet(string response)
		{
			//Give me wordCount amount of words from response. Has to start at the beginning of a sentence.

			int wordCount = 10;

			//remove start info + all [SINGER NAME] occurences
			string pattern = "\"lyrics\".*?\\n|\\[[^\\]]*\\]";
			Regex regex = new Regex(pattern);
			string result = regex.Replace(response, "");
			
			//remove start - could redo regex filter to remove this part
			int returnIndex = result.IndexOf("\\r") + 2;
			string stringWithoutStart = result.Substring(returnIndex);
			//remove end - remove "}
			string stringWithoutEnd = stringWithoutStart.Substring(0, stringWithoutStart.Length - 2);

			//remove Chorus: (Chorus) Singername: etc.
			string pattern2 = @"\{[^}]*\}|[A-Z]\w*:|\(.+?\+.+?\)|\((?:[Cc]horus|[Vv]erse|[Rr]efrain|[Ii]ntermediate|[Bb]ridge|[Rr]epeat)(?:,?\s+\w+)?\)";
			Regex regex2 = new Regex(pattern2);
			string result2 = regex2.Replace(stringWithoutEnd, "");
			
			//Tidy up empty parentheses
			string pattern3 = @"\(\)";
			Regex regex3 = new Regex(pattern3);
			string result3 = regex3.Replace(result2, "");
			
			//split strings to list
			List<string> lyricLinesWithNewLines = result3.Split("\\n").ToList();
			List<string> songLyricsSentences = lyricLinesWithNewLines.Where((s) => s.Length > 1).ToList();

			//Console.WriteLine(response);

			int maxStartingIndex = getMaxStartingIndex(songLyricsSentences, wordCount);
			Random rand = new Random();
			int startingIndex = rand.Next(0, maxStartingIndex);

			List<string> finalLyricLineList = new();
			int currentWordCount = 0;
			for (int i = startingIndex; i < songLyricsSentences.Count; i++)
			{
				string sentence = songLyricsSentences[i];
				List<string> sentenceWords = sentence.Split(" ").ToList();
				foreach (string word in sentenceWords)
				{
					finalLyricLineList.Add(word);
					currentWordCount++;
					if (currentWordCount >= wordCount)
					{
						return finalLyricLineList;
					}
				}
			}
			return ["oops"];
		}

		private static int getMaxStartingIndex(List<string> songLyrics, int maxWordCount)
		{
			//What is the last possible sentence I can choose to make sure that I have at least maxWordCount amount of words to display

			//reverse list
			songLyrics.Reverse();

			//count words
			int currentWordCount = 0;
			int loopCounter = 0;

			//add words one sentence at a time until maxWordCount
			foreach (string sentence in songLyrics)
			{
				int numOfWords = sentence.Split(" ").Count();
				currentWordCount += numOfWords;
				if (currentWordCount > maxWordCount)
				{
					songLyrics.Reverse();
					return songLyrics.Count - loopCounter - 1;
				}
				loopCounter++;
			}
			return -1;
		}
	}
}
