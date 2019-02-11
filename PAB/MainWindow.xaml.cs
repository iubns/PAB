using PAB.Model;
using PAB.ViewModel;
using System.Windows;
using System.Windows.Input;

namespace PAB
{
    public partial class MainWindow : Window
    {
        ViewModelObject viewModelObject = new ViewModelObject();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = viewModelObject;
            Closing += (o, e) => {
                Properties.Settings.Default.Save();
            };
        }

        private void SearchTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.ToString() == "Return")//EnterKey
            {
                searchingButton.Focus();
                searchingButton.Command.Execute(null);
            }
        }

        private void CheckChurchName(object sender, RoutedEventArgs e)
        {
            if (Web.GetCanUseChurchName(IPAdress.GetMacAdress(), churchNameTextBox.Text))
            {
                Properties.Settings.Default.Save();
            }
            else
            {
                MessageBox.Show("다른 이름을 사용 하세요.");
                churchNameTextBox.Text = string.Empty;
            }
        }
    }
}
