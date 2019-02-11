using PAB.Objects;
using System;

namespace PAB.Model
{
    public class Music : ShowingObject
    {
        #pragma warning disable IDE1006 // 명명 스타일
        public int code { get; set; }
        private string _artist { get; set; }
        public string artist
        {
            get
            {
                return _artist;
            }
            set
            {
                _artist = value;
                title = $"{artist} - {musicTitle}";
            }
        }
        private string _musicTitle { get; set; }
        public string musicTitle
        {
            get
            {
                return _musicTitle;
            }
            set
            {
                _musicTitle = value;
                title = $"{artist} - {musicTitle}";
            }
        }

        private string _lyrics { get; set; }
        public string lyrics
        {
            get
            {
                return _lyrics;
            }
            set
            {
                string lyricsContent = value;
                while (1 < lyricsContent.Split(new string[] { "\n " }, StringSplitOptions.None).Length)
                {
                    lyricsContent = lyricsContent.Replace("\n ", "\n");
                }
                _lyrics = lyricsContent;
                content = lyricsContent;
            }
        }

        public static implicit operator string(Music v)
        {
            throw new NotImplementedException();
        }
        #pragma warning restore IDE1006 // 명명 스타일
    }
}
