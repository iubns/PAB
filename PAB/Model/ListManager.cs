using Newtonsoft.Json.Linq;
using PAB.Objects;
using System.Collections.ObjectModel;

namespace PAB.Model
{
    public static class ListManager
    {
        private static ObservableCollection<ShowingObject> searchMusicList = new ObservableCollection<ShowingObject>();
        private static ObservableCollection<ShowingObject> makeMusicList = new ObservableCollection<ShowingObject>();

        public static ObservableCollection<ShowingObject> SearchAndGetResultList(string searchWord)
        {
            if (1 < searchWord.Split('@').Length)
            {
                searchMusicList.Clear();
                JObject jObject = Web.GetBible(searchWord);
                if (jObject == null) return searchMusicList;
                string[] bibleNames = new string[] { "개역개정", "개역한글", "공동번역", "새번역", "쉬운성경", "niv" };
                foreach (string bibleName in bibleNames)
                {
                    Bible bible = new Bible()
                    {
                        bibleName = bibleName,
                        content = jObject[bibleName]["content"].ToString(),
                        rang = searchWord.Split('@')[1],
                    };
                    searchMusicList.Add(bible);
                }
            }
            else
            {
                searchMusicList = Web.GetMusicList(searchWord);
                JObject jObject = Web.GetLyriceOnIubnsServer(searchWord);
                foreach (var music in jObject["musics"])
                {
                    searchMusicList.Insert(0, new Music()
                    {
                        artist = music["churchName"].ToString(),
                        musicTitle = music["musicTitle"].ToString(),
                        lyrics = music["musicLyrice"].ToString(),
                        code = 0,
                    });
                }
            }
            return searchMusicList;
        }

        public static ShowingObject GetSearchMusic(int index)
        {
            return searchMusicList[index];
        }

        public static ShowingObject GetMakeMusic(int index)
        {
            return makeMusicList[index];
        }

        public static void MakeMusicListAdd(int index)
        {
            if (index < 0) return;
            if (0 < searchMusicList.Count)
            {
                makeMusicList.Add(searchMusicList[index]);
            }
        }

        public static void MakeMusicListDelete(int index)
        {
            if (0 < makeMusicList.Count)
            {
                makeMusicList.RemoveAt(index);
            }
        }

        public static ObservableCollection<ShowingObject> GetMakeList()
        {
            return makeMusicList;
        }
    }
}
