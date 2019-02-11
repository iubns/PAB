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
    }
}
