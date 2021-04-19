using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;
using Windows.Management.Deployment;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.System.UserProfile;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace 获取商店应用直链
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DownloadPackage : Page
    {
        public static DownloadPackage PageTras;
        public DownloadPackage()
        {
            this.InitializeComponent();
            PageTras = this;           
        }
        private async void 确认_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(信息.Text) | string.IsNullOrEmpty(信息.Text))
            {
                return;
            }
            else if (类型.SelectedIndex < 0)
            {
                return;
            }

            提示.Visibility = (Visibility)1;
            提示2.Visibility = (Visibility)1;
            布局1.Visibility = Visibility.Collapsed;
            MainPage.PageTrans.MainLoading.IsActive = true;
            MainPage.PageTrans.MainLoading.Visibility = Visibility.Visible;
            获取结果.Items.Clear();

            try
            {
                HttpClient httpClient = new HttpClient();

                var comTag1 = (类型.SelectedItem as ComboBoxItem).Tag.ToString();
                var comTag2 = (通道.SelectedItem as ComboBoxItem).Tag.ToString();
                var values = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("type", comTag1),new KeyValuePair<string, string>("url", 信息.Text),
                new KeyValuePair<string, string>("ring", comTag2),new KeyValuePair<string, string>("lang", "en-us")
            };
                var content = new FormUrlEncodedContent(values);

                HttpResponseMessage responseMessage=await httpClient.PostAsync("https://store.rg-adguard.net/api/GetFiles", content);
                var result = await responseMessage.Content.ReadAsStringAsync();
                if(!result.Contains("The links were successfully received from the Microsoft Store server."))
                {
                    提示.Visibility = 0;
                    布局1.Visibility = 0;
                    MainPage.PageTrans.MainLoading.IsActive = false;
                    MainPage.PageTrans.MainLoading.Visibility = Visibility.Collapsed;

                    return;
                }

                MainPage.PageTrans.WebView1.NavigateToString(result);
                await Task.Delay(1000);
                LoadHTMLData();

            }
            catch (Exception)
            {
                提示2.Visibility = 0;
            }

            布局1.Visibility = 0;
            MainPage.PageTrans.MainLoading.IsActive = false;
            MainPage.PageTrans.MainLoading.Visibility = Visibility.Collapsed;
        }

        private void 类型_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (类型.SelectedIndex)
            {
                case 0:
                    信息.PlaceholderText = "https://www.microsoft.com/p/AppName/XXXXXXXXXXXX";
                    break;
                case 1:
                    信息.PlaceholderText = "ABCDEFGHIJKL";
                    break;
                case 2:
                    信息.PlaceholderText = "DeveloperName.AppName_ID";
                    break;
                case 3:
                    信息.PlaceholderText = "EN=XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX";
                    break;
            }
        }
        public async void LoadHTMLData()
        {
            try
            {
                var count = string.Format(@"document.querySelectorAll('tbody tr').length.toString();");
                string get = await MainPage.PageTrans.WebView1.InvokeScriptAsync("eval", new string[] { count });
                int linkCount = Convert.ToInt32(get);
                string colors = "";

                for (int i = 0; i < linkCount; i++)
                {
                    var getfilename = string.Format(@"document.querySelectorAll('tbody tr')[{0}].querySelector('a').innerText;", i);
                    string filename = await MainPage.PageTrans.WebView1.InvokeScriptAsync("eval", new string[] { getfilename });
                    if (filename.ToLower().Contains(".appxbundle"))
                    {
                        colors = "Orange";
                    }
                    else if (filename.ToLower().Contains(".appx"))
                    {
                        colors = "Red";
                    }
                    else if (filename.ToLower().Contains(".eappxbundle"))
                    {
                        colors = "Blue";
                    }
                    else
                    {
                        colors = "Black";
                    }

                    var getlink = string.Format(@"document.querySelectorAll('tbody tr')[{0}].querySelector('a').href;", i);
                    Uri filelink = new Uri(await MainPage.PageTrans.WebView1.InvokeScriptAsync("eval", new string[] { getlink }));
                    var getIfm = string.Format(@"
document.querySelectorAll('tbody tr')[{0}].querySelectorAll('td')[1].innerText+'\n'+document.querySelectorAll('tbody tr')[{0}].querySelectorAll('td')[2].innerText+'\n'+document.querySelectorAll('tbody tr')[{0}].querySelectorAll('td')[3].innerText;", i);
                    string fileIfm = await MainPage.PageTrans.WebView1.InvokeScriptAsync("eval", new string[] { getIfm });

                    获取结果.Items.Add(new 链接信息 { 文件名 = filename, 下载链接 = filelink, 详细信息 = fileIfm, 标题颜色 = colors });

                }
            }
            catch (Exception)
            {

            }
            布局1.Visibility = 0;
        }
        Uri downloadLink;
        string fileName;
        private void 获取结果_ItemClick(object sender, ItemClickEventArgs e)
        {
            downloadLink = (e.ClickedItem as 链接信息).下载链接;
            fileName = (e.ClickedItem as 链接信息).文件名;
            弹出.ShowAt(获取结果 as FrameworkElement);
        }
        private void 复制_Click(object sender, RoutedEventArgs e)
        {
            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(downloadLink.ToString());
            Clipboard.SetContent(dataPackage);
        }

        StorageFile storageFile;
        DownloadOperation downloadOperation = null;
        private async void 下载_Click(object sender, RoutedEventArgs e)
        {
            if (进度条.Value > 0 && 进度条.Value < 100)
            {
                await 下载提示.ShowAsync();
            }
            else
            {
                StartGet();
            }

        }
        public async void StartGet()
        {
            打开.IsEnabled = false;
            string saveLocation = "";
            var 文件选择 = new FileSavePicker();
            文件选择.SuggestedStartLocation = PickerLocationId.Desktop;
            文件选择.FileTypeChoices.Add("所有文件", new List<string>() { "." });
            文件选择.FileTypeChoices.Add("APPX File", new List<string>() { ".appxbundle" });
            文件选择.FileTypeChoices.Add("Encrypted APPX File", new List<string>() { ".eappxbundle" });
            文件选择.FileTypeChoices.Add("XAP File", new List<string>() { ".xap" });
            文件选择.SuggestedFileName = fileName;

            storageFile = await 文件选择.PickSaveFileAsync();

            if (storageFile != null)
            {
                saveLocation = storageFile.Path;
                try
                {
                    进度条.Value = 0;
                    BackgroundDownloader backgroundDownloader = new BackgroundDownloader();
                    downloadOperation = backgroundDownloader.CreateDownload(downloadLink, storageFile);

                    var progress = new Progress<DownloadOperation>(ProgressCallback);
                    await downloadOperation.StartAsync().AsTask(progress);
                    进度条.Foreground = new SolidColorBrush(Colors.Black);
                    if (进度条.Value == 100)
                    {
                        进度条.Foreground = new SolidColorBrush(Colors.LightGreen);
                    }
                }
                catch (Exception)
                {
                    return;
                }
            }
            else
            {

            }
        }
        private void ProgressCallback(DownloadOperation downloadOperation)
        {
            try
            {
                double progress = (downloadOperation.Progress.BytesReceived * 100) / downloadOperation.Progress.TotalBytesToReceive;
                进度条.Value = progress;
                已下载大小.Text = (downloadOperation.Progress.BytesReceived / 100).ToString();
                总大小.Text = (downloadOperation.Progress.TotalBytesToReceive / 100).ToString();

                if (progress == 100)
                {
                    进度条.Foreground = new SolidColorBrush(Colors.LightGreen);
                    打开.IsEnabled = true;
                }
                if (downloadOperation.Progress.Status == BackgroundTransferStatus.PausedNoNetwork | downloadOperation.Progress.Status == BackgroundTransferStatus.PausedByApplication | downloadOperation.Progress.Status == BackgroundTransferStatus.PausedSystemPolicy)
                {
                    try
                    {
                        downloadOperation.Pause();
                        进度条.Foreground = new SolidColorBrush(Colors.Red);
                        继续.Visibility = Visibility.Visible;
                    }
                    catch (Exception)
                    {

                    }

                }
                else if (downloadOperation.Progress.Status == BackgroundTransferStatus.Error)
                {
                    try
                    {
                        downloadOperation.Pause();
                        进度条.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void 继续_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (downloadOperation.Progress.Status == BackgroundTransferStatus.PausedByApplication | downloadOperation.Progress.Status == BackgroundTransferStatus.PausedSystemPolicy | downloadOperation.Progress.Status == BackgroundTransferStatus.PausedNoNetwork)
                {
                    downloadOperation.Resume();
                    进度条.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
            catch (Exception)
            {

            }
            继续.Visibility = Visibility.Collapsed;
        }

        private void 下载提示_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            StartGet();
        }
        private async void 打开_Click(object sender, RoutedEventArgs e)
        {
            OpenError.Visibility = Visibility.Collapsed;
            try
            {
                await Launcher.LaunchFileAsync(storageFile);
            }
            catch (Exception)
            {
                OpenError.Visibility = Visibility.Visible;
            }
        }

    }
}
