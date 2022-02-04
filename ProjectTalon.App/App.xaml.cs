using Microsoft.Maui.Controls;
using Application = Microsoft.Maui.Controls.Application;

namespace ProjectTalon.App
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //MainPage = new MainPage();
            MainPage = new NavigationPage(new ContentPage
            {
                Content = new Label
                {
                    Text = "Hello Xamarin.Forms!",
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                }
            });
        }
    }
}
