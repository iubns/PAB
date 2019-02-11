using PAB.Model;
using PAB.Objects;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

#pragma warning disable IDE1006 // 명명 스타일
namespace PAB.ViewModel
{
    public class ViewModelObject : INotifyPropertyChanged
    {
        public Setting setting { get; set; }
        public ViewModelObject()
        {
            setting = new Setting();
            OnProperChanged("setting");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnProperChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ICommand _search;
        public ICommand search => _search ?? (_search = new CommandHandler(() => {
            searchList = ListManager.SearchAndGetResultList(searchWord);
            OnProperChanged("searchList");
        }));

        private ICommand _pptMake;
        public ICommand pptMake => _pptMake ?? (_pptMake = new CommandHandler(() => {
            if(setting.churchName == string.Empty)
            {
                MessageBox.Show("오른쪽 상단에 교회 이름을 적어 주세요.");
                return;
            }
            Web.SaveLyriceOnServer(setting.productionKey, setting.churchName);
            new PresnetationObject().CreatePowerPointSlides(setting);
            Properties.Settings.Default.Save();
        }));

        private ICommand _musicAdd;
        public ICommand musicAdd  => _musicAdd ?? (_musicAdd = new CommandHandler(() => {
            ListManager.MakeMusicListAdd(selectMusic);
            makeList = ListManager.GetMakeList();
            OnProperChanged("makeList");
        }));

        private ICommand _musicDelete;
        public ICommand musicDelete => _musicDelete ?? (_musicDelete = new CommandHandler(() => {
            ListManager.MakeMusicListDelete(makeSelectMusic);
            makeList = ListManager.GetMakeList();
            OnProperChanged("makeList");
        }));

        private ICommand _selectBackgound;
        public ICommand selectBackgound => _selectBackgound ?? (_selectBackgound = new CommandHandler(() => {
            setting.backgoundFilePath = new FileDialog().GetBackgoundUrl();
            OnProperChanged("setting");
        }));

        private ICommand _CheckChurchName;
        public ICommand CheckChurchName => _CheckChurchName ?? (_CheckChurchName = new CommandHandler(() => {
            if(Web.GetCanUseChurchName(setting.productionKey, setting.churchName))
            {
                MessageBox.Show("사용 가능 합니다.");
                Properties.Settings.Default.Save();
            }
            else
            {
                MessageBox.Show("다른 이름을 사용 하세요.");
                setting.churchName = string.Empty;
            }
        }));
        
        private int _selectMusic = 0;
        public int selectMusic
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
                    content = ListManager.GetSearchMusic(_selectMusic).content;
                }
            }
        }
        
        private int _makeSelectMusic = 0;
        public int makeSelectMusic
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
                    content = ListManager.GetMakeMusic(_makeSelectMusic).content;
                }
            }
        }

        bool searchLyrics;
        private string _content;
        public string content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = Regex.Replace(value, "\r", "");
                Music music = searchLyrics ? ListManager.GetSearchMusic(selectMusic) as Music : ListManager.GetMakeMusic(makeSelectMusic) as Music;
                if (music != null)
                {
                    music.content = _content;
                }
                else
                {
                    Bible bible = searchLyrics ? ListManager.GetSearchMusic(selectMusic) as Bible: ListManager.GetMakeMusic(makeSelectMusic) as Bible;
                    bible.content = _content;
                }
                OnProperChanged("content");
            }
        }

        private string _searchWord;
        public string searchWord
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

        public ObservableCollection<ShowingObject> searchList { get; set; } = new ObservableCollection<ShowingObject>();

        public ObservableCollection<ShowingObject> makeList { get; set; } = new ObservableCollection<ShowingObject>();
    }
}
#pragma warning restore IDE1006 // 명명 스타일