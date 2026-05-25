using System.Windows;
using UP1.Services;

namespace UP1
{
    public partial class App : Application
    {
        public static DataService DataService { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            DataService = new DataService();
            base.OnStartup(e);
        }
    }
}