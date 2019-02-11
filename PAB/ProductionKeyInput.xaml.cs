using PAB.Model;
using PAB.Objects;
using System;
using System.Windows;

namespace PAB
{
    public partial class ProductionKeyInput : Window
    {
        public ProductionKeyInput()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string FirstKey { get; set; }
        public string SecondKey { get; set; }
        public string ThirdKey { get; set; }
        public string ForthKey { get; set; }
        public void CheckProductionKey(object o, EventArgs e)
        {
            string productionKey = $"{ FirstKey }-{SecondKey}-{ThirdKey}-{ForthKey}";
            if (Web.GetStatuProductionKey(IPAdress.GetMacAdress(), productionKey))
            {
                Setting setting = new Setting()
                {
                    productionKey = productionKey,
                };
                Properties.Settings.Default.Save();
                new MainWindow().Show();
                Close();
            }
            else
            {
                MessageBox.Show("정품키 인증에 실패 하였습니다. \n키번호 또는 인터넷을 확인 하세요.");
            }
        }
    }
}
