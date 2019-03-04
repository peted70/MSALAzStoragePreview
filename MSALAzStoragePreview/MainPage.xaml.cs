using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MSALAzStoragePreview
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private PublicClientApplication pca;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async Task<string> AuthAsync()
        {
            // App registered in preview app registration blad on the portal
            pca = new PublicClientApplication("ff165698-5680-441a-9935-e7e3b6f0ca4b");


            // Need to have a scope of some kind here - what should it be? 
            // Using user_impersonation for app registered as not v2 endpoint...
            var res = await pca.AcquireTokenAsync(new string[] { "https://storage.azure.com/user_impersonation" });
            return res.AccessToken;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var accessToken = await AuthAsync();
            await CallContainerAsync(accessToken);
        }

        async Task CallContainerAsync(string token)
        {
            var http = new HttpClient();

            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await http.GetAsync("https://msalazstorage.blob.core.windows.net/models?restype=container&comp=list");

            if (!response.IsSuccessStatusCode)
            {
                return;
            }
        }
    }
}
