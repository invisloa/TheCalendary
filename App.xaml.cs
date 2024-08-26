using AsyncAwaitBestPractices;
using Kalendarzyk.Services;
using Kalendarzyk.Views;
using System.Diagnostics;

namespace Kalendarzyk
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
            InitializeAppAsync().SafeFireAndForget();
            Debug.WriteLine("App initialized");


        }
        private async Task InitializeAppAsync()
        {
            // Display a splash screen or loading page
            MainPage = new LoadingPage();

            // Initialize the repository
            await Factory.InitializeEventRepository();

            // Once initialized, set the actual main page
            MainPage = new AppShell();
        }

        protected override async void OnStart()
        {
            // Call base method 
            base.OnStart();

            // Check or request StorageRead permission
            var statusStorageRead = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
            if (statusStorageRead != PermissionStatus.Granted)
            {
                statusStorageRead = await Permissions.RequestAsync<Permissions.StorageRead>();
            }
            var statusStorageWrite = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            if (statusStorageWrite != PermissionStatus.Granted)
            {
                statusStorageWrite = await Permissions.RequestAsync<Permissions.StorageRead>();
            }
        }


        public static class Styles
        {
            public static Style GoogleFontStyle = new Style(typeof(Label))
            {
                Setters =
                {
                    new Setter { Property = Label.FontFamilyProperty, Value = "GoogleMaterialFont" },
                    new Setter { Property = Label.FontSizeProperty, Value = 32 }
                }
            };
        }


    }
}
