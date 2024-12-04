using CommunityToolkit.Mvvm.Input;
using RSACryptoApp.Models;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace RSACryptoApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public Parameter    Parameter   { get; set; }
        public UIItem       UIItem      { get; set; }

        public ICommand CreateCommand   { get; set; }
        public ICommand FileCommand     { get; set; }

        public RSACryptoService Service;

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;

            Parameter = new Parameter();
            UIItem = new UIItem();

            CreateCommand = new RelayCommand(CreateMethod);
            FileCommand = new RelayCommand(FileMethod);

            Service = new RSACryptoService();

            Service.WriteLog = new Action<string>(WriteText);
        }
        public void CreateMethod()
        {
            this.doc.Blocks.Clear();

            Service.Create(Parameter);
        }
        public void FileMethod()
        {
            Service.Export(UIItem.Path);
        }


        #region log
        public void WriteText(string text)
        {
            this.doc.Blocks.Add(new Paragraph(new Run(text)));
        }
        #endregion
    }
}