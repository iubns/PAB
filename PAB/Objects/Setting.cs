namespace PAB.Objects
{
    public class Setting
    {
        #pragma warning disable IDE1006 // 명명 스타일
        public Setting()
        {
            fontName = Properties.Settings.Default["fontName"].ToString();
            fontSize = Properties.Settings.Default["fontSize"].ToString();
            backgoundFilePath = Properties.Settings.Default["backgoundFilePath"].ToString();
            productionKey = Properties.Settings.Default["productionKey"].ToString();
            churchName = Properties.Settings.Default["churchName"].ToString();
        }

        private string _fontName { get; set; } = "HY견명조";
        public string fontName
        {
            get
            {
                return _fontName;
            }
            set
            {
                _fontName = value;
                Properties.Settings.Default["fontName"] = _fontName;
            }
        }

        private int _fontSize = 64;
        public string fontSize
        {
            get
            {
                return _fontSize.ToString();
            }
            set
            {
                string result = "";
                foreach (char eachWord in value)
                {
                    if (48 <= eachWord && eachWord <= 57) //numberic or letter check
                    {
                        result += eachWord;
                    }
                }
                _fontSize = int.Parse(result);
                Properties.Settings.Default["fontSize"] = _fontSize;
            }
        }

        private string _backgoundFilePath = "파일 선택";
        public string backgoundFilePath {
            get
            {
                return _backgoundFilePath;
            }
            set
            {
                if(value == string.Empty)
                {
                    _backgoundFilePath = "파일 선택";
                }
                else
                {
                    _backgoundFilePath = value;
                }
                Properties.Settings.Default["backgoundFilePath"] = _backgoundFilePath;
            }
        }

        private string _productionKey { get; set; }
        public string productionKey
        {
            get
            {
                return _productionKey;
            }
            set
            {
                _productionKey = value;
                Properties.Settings.Default["productionKey"] = _productionKey;
            }
        }

        private string _churchName { get; set; }
        public string churchName
        {
            get
            {
                return _churchName;
            }
            set
            {
                _churchName = value;
                Properties.Settings.Default["churchName"] = _churchName;
            }
        }
    }
    #pragma warning restore IDE1006 // 명명 스타일
}
