using Microsoft.Win32;

namespace PAB.Model
{
    class FileDialog
    {
        public string GetBackgoundUrl()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "모든 파일(*,*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }
            else
            {
                return "";
            }
        }
    }
}
