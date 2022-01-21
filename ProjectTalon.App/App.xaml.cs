using Application = Microsoft.Maui.Controls.Application;

namespace ProjectTalon.App
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }
    }
}
