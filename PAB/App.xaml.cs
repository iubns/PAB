using PAB.dll;
using PAB.Model;
using PAB.Objects;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace PAB
{
    public partial class App : Application
    {
        internal App()
        {
            try
            {
                FileInfo fileInfo = new FileInfo(@"C:\Temp\PPTMaker.exe");
                fileInfo.Delete();
            }
            catch { }

            try
            {
                FileInfo fileInfo = new FileInfo(@"C:\Temp\PAB - Praise And Bible.exe");
                fileInfo.Delete();
            }
            catch { }

            string newVersion = Web.GetNewVersion();
            if (newVersion != string.Empty &&
                newVersion != "1.1.3.1" &&
                newVersion == Web.GetNewVersionInIubnsNet())
            {
                MessageBox.Show($"새로운 버전(v{ newVersion }) 있습니다.\n프로그램을 업데이트 합니다.", "업데이트 안내");
                Web.GetUpdate();
                Process.GetCurrentProcess().Kill();
                return;
            }

            if(DllManager.LoadDll("dll.Newtonsoft.Json.dll") == false)
            {
                MessageBox.Show($"dll 파일 불러오기 실패", "프로그램 오류");
                Process.GetCurrentProcess().Kill();
                return;
            }

            Setting setting = new Setting();

            if (!Web.GetStatuProductionKey(IPAdress.GetMacAdress(), newVersion))
            {
                MessageBox.Show($"서버에 오류가 있습니다.", "서버 오류");
                Process.GetCurrentProcess().Kill();
                return;
            }

            new MainWindow().Show();
        }
    }
}
