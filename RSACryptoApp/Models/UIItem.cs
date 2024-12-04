using CommunityToolkit.Mvvm.ComponentModel;

namespace RSACryptoApp.Models
{
    public class UIItem : ObservableObject
    {
        private string path = "D:\\temp\\텍스트문서.txt";
        public string Path
        {
            get { return path; }
            set { SetProperty(ref path, value); }
        }
    }
}
