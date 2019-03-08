using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MSALAzStoragePreview
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private PublicClientApplication pca;

        private const string StorageAccountName = "<your storage account name>";
        private const string StorageContainerName = "<your container name>";
        private const string ClientId = "<your client id>";

        public List<string> BlobList
        { get; set; } = new List<string>();

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async Task<string> AuthAsync()
        {
            // App registered in preview app registration blad on the portal
            pca = new PublicClientApplication(ClientId);

            var res = await pca.AcquireTokenAsync(new string[] { "https://storage.azure.com/user_impersonation" });
            return res.AccessToken;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            LoadingText.Visibility = Visibility.Visible;
            ErrorText.Text = string.Empty;
            try
            {
                var accessToken = await AuthAsync();
                await CallContainerAsync(accessToken);
            }
            catch (MsalClientException ex)
            {
                ErrorText.Text = ex.Message;
                if (ex.InnerException != null)
                {
                    ErrorText.Text += " " + ex.InnerException.Message;
                }
            }
            catch (MsalException ex)
            {
                ErrorText.Text = ex.Message;
                if (ex.InnerException != null)
                {
                    ErrorText.Text += " " + ex.InnerException.Message;
                }
            }
            catch (Exception ex)
            {
                ErrorText.Text = ex.Message;
                if (ex.InnerException != null)
                {
                    ErrorText.Text += " " + ex.InnerException.Message;
                }
            }
            finally
            {
                LoadingText.Visibility = Visibility.Collapsed;
            }
        }

        async Task CallContainerAsync(string token)
        {
            var http = new HttpClient();

            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            http.DefaultRequestHeaders.Add("x-ms-version", "2018-03-28");

            var rqstStr = $"https://{StorageAccountName}.blob.core.windows.net/{StorageContainerName}?restype=container&comp=list";
            var response = await http.GetAsync(rqstStr);
            response.EnsureSuccessStatusCode();

            var respString = await response.Content.ReadAsStringAsync();
            XDocument doc = XDocument.Parse(respString);

            var blobNameList = doc.Descendants("Blob").Select(b => b.Descendants("Name").Single().Value);
            BlobListView.Items.Clear();
            foreach (var name in blobNameList)
            {
                BlobListView.Items.Add(name);
            }
        }
    }
}
