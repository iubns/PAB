using PAB.Model;
using PAB.Objects;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace PAB.ViewModel
{
    public class ViewModelObject : INotifyPropertyChanged
    {
        public Setting Setting { get; set; }
        public ViewModelObject()
        {
            Setting = new Setting();
            OnProperChanged("setting");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnProperChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ICommand _search;
        public ICommand Search => _search ?? (_search = new CommandHandler(() => {
            SearchList = ListManager.SearchAndGetResultList(SearchWord);
            OnProperChanged("searchList");
        }));

        private ICommand _pptMake;
        public ICommand PptMake => _pptMake ?? (_pptMake = new CommandHandler(() => {
            if(Setting.churchName == string.Empty)
            {
                MessageBox.Show("오른쪽 상단에 단체 이름을 적어 주세요.");
                return;
            }
            Web.SaveLyriceOnServer(IPAdress.GetMacAdress(), Setting.churchName);
            new PresnetationObject().CreatePowerPointSlides(Setting);
            Properties.Settings.Default.Save();
        }));

        private ICommand _musicAdd;
        public ICommand MusicAdd  => _musicAdd ?? (_musicAdd = new CommandHandler(() => {
            ListManager.MakeMusicListAdd(SelectMusic);
            MakeList = ListManager.GetMakeList();
            OnProperChanged("makeList");
        }));

        private ICommand _musicDelete;
        public ICommand MusicDelete => _musicDelete ?? (_musicDelete = new CommandHandler(() => {
            ListManager.MakeMusicListDelete(MakeSelectMusic);
            MakeList = ListManager.GetMakeList();
            OnProperChanged("makeList");
        }));

        private ICommand _selectBackgound;
        public ICommand SelectBackgound => _selectBackgound ?? (_selectBackgound = new CommandHandler(() => {
            Setting.backgoundFilePath = new FileDialog().GetBackgoundUrl();
            OnProperChanged("setting");
        }));
        
        private int _selectMusic = 0;
        public int SelectMusic
        {
            get
            {
                return _selectMusic;
            }
            set
            {
                if (value != -1)
                {
                    _selectMusic = value;
                    searchLyrics = true;
                    Web.GetLyrics(_selectMusic);
                    Content = ListManager.GetSearchMusic(_selectMusic).content;
                }
            }
        }
        
        private int _makeSelectMusic = 0;
        public int MakeSelectMusic
        {
            get
            {
                return _makeSelectMusic;
            }

            set
            {
                if (value != -1)
                {
                    _makeSelectMusic = value;
                    searchLyrics = false;
                    Content = ListManager.GetMakeMusic(_makeSelectMusic).content;
                }
            }
        }

        bool searchLyrics;
        private string _content;
        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = Regex.Replace(value, "\r", "");
                Music music = searchLyrics ? ListManager.GetSearchMusic(SelectMusic) as Music : ListManager.GetMakeMusic(MakeSelectMusic) as Music;
                if (music != null)
                {
                    music.content = _content;
                }
                else
                {
                    Bible bible = searchLyrics ? ListManager.GetSearchMusic(SelectMusic) as Bible: ListManager.GetMakeMusic(MakeSelectMusic) as Bible;
                    bible.content = _content;
                }
                OnProperChanged("content");
            }
        }

        private string _searchWord;
        public string SearchWord
        {
            get
            {
                return _searchWord;
            }
            set
            {
                _searchWord = value;
                OnProperChanged("searchWord");
            }
        }

        public ObservableCollection<ShowingObject> SearchList { get; set; } = new ObservableCollection<ShowingObject>();

        public ObservableCollection<ShowingObject> MakeList { get; set; } = new ObservableCollection<ShowingObject>();
    }
}