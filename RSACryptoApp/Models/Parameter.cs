using CommunityToolkit.Mvvm.ComponentModel;

namespace RSACryptoApp.Models
{
    public class Parameter : ObservableObject
    {
        private string domain = "domain";
        public string Domain
        {
            get { return domain; }
            set { SetProperty(ref domain, value); }
        }

        private string country = "KR";
        public string Country
        {
            get { return country; }
            set { SetProperty(ref country, value); }
        }
        private string state = "Gyeonggi-do";
        public string State
        {
            get { return state; }
            set { SetProperty(ref state, value); }
        }
        private string locality= "Uiwang";
        public string Locality
        {
            get { return locality; }
            set { SetProperty(ref locality, value); }
        }
        private string organization = "DM";
        public string Organization
        {
            get { return organization; }
            set { SetProperty(ref organization, value); }
        }
        private DateTime startDate = DateTime.Now;
        public DateTime StartDate
        {
            get { return startDate; }
            set { SetProperty(ref startDate, value); }
        }
        private DateTime endDate = DateTime.Now.AddDays(7);
        public DateTime EndDate
        {
            get { return endDate; }
            set { SetProperty(ref endDate, value); }
        }

    }
}
