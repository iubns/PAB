using Newtonsoft.Json.Linq;
using PAB.Objects;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;

namespace PAB.Model
{
    public static class Web
    {
        const string header_UA = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Whale/3.25.232.19 Safari/537.36";
        const string header_Accept = "application/json";
        const string header_ConType = "application/x-www-form-urlencoded";
        const string serverDomain = "http://iubns.com/PPTMaker";
            
        public static ObservableCollection<ShowingObject> GetMusicList(string searchWord)
        {
            ObservableCollection<ShowingObject> musicList = new ObservableCollection<ShowingObject>();
            string url = $"https://apis.naver.com/vibeWeb/musicapiweb/v4/search/track?query={searchWord}&start=1&display=100&sort=RELEVANCE&cact=ogn";
            JObject jObject = GetJObjectToHttpWebRequest(url);
            var musicJObjectList = jObject["response"]["result"]["tracks"];
            if(musicJObjectList is null)
            {
                return musicList;
            }
            foreach (JObject musicJObject in musicJObjectList)
            {
                Music music = new Music();
                music.musicTitle = musicJObject.Value<string>("trackTitle");
                string artist = "";
                foreach (JObject artistJObject in musicJObject["artists"])
                {
                    if (artist.Length == 0)
                    {
                        artist += artistJObject["artistName"];
                    }
                    else
                    {
                        artist += ", " + artistJObject["artistName"];
                    }
                }
                music.artist = artist;
                music.code = musicJObject.Value<int>("trackId");
                musicList.Add(music);
            }
            return musicList;
        }

        public static void GetLyrics(int musicIndex)
        {
            if (!(ListManager.GetSearchMusic(musicIndex) is Music music) || music.lyrics != null)
            {
                return;
            }
            string url = $"https://apis.naver.com/vibeWeb/musicapiweb/vibe/v4/lyric/{ music.code }";
            var JObject = GetJObjectToHttpWebRequest(url);
            if(JObject is null)
            {
                music.lyrics = "가사를 찾을 수 없음";
                return;
            }
            music.lyrics = JObject["response"]["result"]["lyric"]["normalLyric"].Value<string>("text");
        }

        public static bool GetStatuProductionKey(string macAdress, string version)
        {
            string url = $"{ serverDomain }/?macAdress={macAdress}&version={version}";
            return bool.Parse(GetResultToHttpWebRequest(url));
        }

        public static bool GetCanUseChurchName(string macAdress, string churchName)
        {
            string url = $"{ serverDomain }/churchNameCheck.php?macAdress={macAdress}&churchName={churchName}";
            return bool.Parse(GetResultToHttpWebRequest(url));
        }

        public static string GetNewVersion()
        {
            string url = $"{serverDomain}/update/version.php";
            return GetResultToHttpWebRequest(url);
        }

        public static string GetNewVersionInIubnsNet()
        {
            string url = $"http://iubns.net/PPTMaker/update/version.php";
            return GetResultToHttpWebRequest(url);
        }

        public static JObject GetBible(string searchWord)
        {
            searchWord = searchWord.Remove(0, 1);
            string startBook = searchWord.Split(' ')[0];
            int startChapters = int.Parse(searchWord.Split(' ')[1].Split(':')[0]);
            int startVerses = int.Parse(searchWord.Split(' ')[1].Split('~')[0].Split(':')[1]);
            int endChapters = int.Parse(searchWord.Split('~')[1].Split(':')[0]);
            int endVerses = 0;
            try
            {
                endVerses = int.Parse(searchWord.Split('~')[1].Split(':')[1]);
            }
            catch
            {
                endVerses = endChapters;
                endChapters = startChapters;
            }

            for (int bookIndex = 0; bookIndex < 66; bookIndex++)
            {
                if(Bible.bookNames[bookIndex,0] == startBook || Bible.bookNames[bookIndex, 1] == startBook)
                {
                    string url = $"{ serverDomain }/GetBible.php?startBook={bookIndex}&startChapters={--startChapters}&startVerses={--startVerses}&endChapters={--endChapters}&endVerses={--endVerses}";
                    return JObject.Parse(GetResultToHttpWebRequest(url));
                }
            }
            return null;
        }

        public static void GetUpdate()
        {
            FileInfo fileInfo = new FileInfo(Process.GetCurrentProcess().MainModule.FileName);
            fileInfo.MoveTo(@"C:\Temp\PAB - Praise And Bible.exe");

            string browserUrl = $"{ serverDomain }/update/PAB - Praise And Bible.exe";

            WebClient webClient = new WebClient();
            //webClient.DownloadFile(browserUrl, Process.GetCurrentProcess().MainModule.FileName);
            webClient.DownloadFile(browserUrl, "PAB - Praise And Bible.exe");
        }

        public static void SaveLyriceOnServer(string macAdress, string churchName)
        {
            foreach (ShowingObject showingObject in ListManager.GetMakeList())
            {
                if (!(showingObject is Music music))
                {
                    continue;
                }
                string musicTitle = music.musicTitle;
                string musicLyrice = music.content;

                string url = $"{ serverDomain }/SaveLyrice.php";
                HttpWebRequest http = (HttpWebRequest)WebRequest.Create(url);
                http.UserAgent = header_UA;
                http.ContentType = header_ConType;
                http.ContentType = header_Accept;
                http.Method = "POST";
                http.Timeout = 5000;

                using (Stream str = http.GetRequestStream())
                {
                    using (StreamWriter streamWriter = new StreamWriter(str))
                    {
                        streamWriter.Write($"musicTitle={ musicTitle }&musicLyrice={ musicLyrice }&macAdress={ macAdress }&churchName={ churchName }");
                    }
                    using (var webReauest = new StreamReader(http.GetResponse().GetResponseStream()))
                    {
                        string temp = webReauest.ReadToEnd();
                    }
                }
            }
        }

        public static JObject GetLyriceOnIubnsServer(string searchWord)
        {
            string url = $"{ serverDomain }/getLyrice.php?musicTitle={ searchWord }&churchName={ searchWord }";
            return JObject.Parse(GetResultToHttpWebRequest(url));
        }

        private static string GetResultToHttpWebRequest(string url)
        {
            HttpWebRequest http = (HttpWebRequest)WebRequest.Create(url);
            http.UserAgent = header_UA;
            http.ContentType = header_ConType;
            http.Method = "POST";
            http.Timeout = 5000;
            try
            {
                using (var webReauest = new StreamReader(http.GetResponse().GetResponseStream()))
                {
                    return webReauest.ReadToEnd();
                }
            }
            catch
            {
                MessageBox.Show("인터넷 연결 오류");
                return string.Empty;
            }
        }

        private static JObject GetJObjectToHttpWebRequest(string url)
        {
            HttpWebRequest http = (HttpWebRequest)WebRequest.Create(url);
            http.UserAgent = header_UA;
            http.Accept = header_Accept;
            http.Method = "GET";
            http.Timeout = 5000;
            try
            {
                using (var webReauest = new StreamReader(http.GetResponse().GetResponseStream()))
                {
                    string result = webReauest.ReadToEnd();
                    return JObject.Parse(result);
                }
            }
            catch
            {
                MessageBox.Show("인터넷 연결 오류");
                return null;
            }
        }
    }
}
