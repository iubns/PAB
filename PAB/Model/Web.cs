using Newtonsoft.Json.Linq;
using PAB.Objects;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;

namespace PAB.Model
{
    public static class Web
    {
        const string header_UA = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)";
        const string header_ConType = "application/x-www-form-urlencoded";
        const string serverDomain = "http://iubns.com/PPTMaker";
            
        public static ObservableCollection<ShowingObject> GetMusicList(string searchWord)
        {
            ObservableCollection<ShowingObject> musicList = new ObservableCollection<ShowingObject>();
            string url = $"https://music.naver.com/search/search.nhn?query={ searchWord }&target=track";
            string reslut = GetResultToHttpWebRequest(url);
            reslut = Regex.Replace(reslut, "<b>", "");
            reslut = Regex.Replace(reslut, "</b>", "");
            string[] trSplitResult = reslut.Split(new string[] { "<tr class=\"_tracklist_move data" }, StringSplitOptions.None);
            for (int i = 1; i<trSplitResult.Length; i++)
            {
                Music music = new Music();

                try
                {
                    music.musicTitle = trSplitResult[i].Split(new string[] { "<span class=\"ellipsis\">" }, StringSplitOptions.None)[1].Split(new string[] { "</span>" }, StringSplitOptions.None)[0];
                }
                catch
                {
                    music.musicTitle = "지원하지 않는 노래";
                }

                try
                {
                    music.artist = trSplitResult[i].Split(new string[] { "<span class=\"ellipsis\" >\r\n\t\t\t\r\n\t\t\t" }, StringSplitOptions.None)[1].Split(new string[] { "\r\n\t\t\t\r\n\t\t</span>" }, StringSplitOptions.None)[0];
                }
                catch
                {
                    music.artist = "알 수 없음";
                }

                music.code = int.Parse(trSplitResult[i].Split(new string[] { "<input type=\"checkbox\" class=\"_chkbox_item _disc_0 input_chk NPI" }, StringSplitOptions.None)[1].Split(new string[] { "\" title=\"선택\" test=\"false\" ></td>" }, StringSplitOptions.None)[0].Split(new string[] { "i:" },StringSplitOptions.None)[1]);
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
            string url = $"https://music.naver.com/lyric/index.nhn?trackId={ music.code }";
            string reslut = GetResultToHttpWebRequest(url);
            reslut = Regex.Replace(reslut, "<br />", "\n");
            try
            {
                music.lyrics = reslut.Split(new string[] { "<div id=\"lyricText\" class=\"show_lyrics\">" }, StringSplitOptions.None)[1].Split(new string[] { "</div>" }, StringSplitOptions.None)[0];
            }
            catch
            {
                music.lyrics = "가사를 찾을 수 없음";
            }
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

            for (int bookIndex = 0; bookIndex<66; bookIndex++)
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
    }
}
