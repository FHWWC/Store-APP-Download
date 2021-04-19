using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace 获取商店应用直链
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SearchPage : Page
    {
        public SearchPage()
        {
            this.InitializeComponent();
        }

        private async void 搜索_Click(object sender, RoutedEventArgs e)
        {
            string reg = "";
            string link = "";

            if (市场.SelectedIndex<0)
            {
                return;
            }
            else if(string.IsNullOrWhiteSpace(内容.Text))
            {
                return;
            }

            APPList.Items.Clear();
            搜索.Visibility = Visibility.Collapsed;

            switch(市场.SelectedIndex)
            {
                case 0:
                    reg = "zh-cn";
                    break;
                case 1:
                    reg = "en-us";
                    break;
                case 2:
                    reg = "ru-ru";
                    break;
            }

            try
            { 
                link = string.Format("https://www.microsoft.com/{0}/search/shop/{1}?q={2}", reg,FindType.SelectionBoxItem.ToString()+"s", 内容.Text);

                HttpRequestMessage httpRequest = new HttpRequestMessage();
                httpRequest.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                httpRequest.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4412.3 Safari/537.36");
                httpRequest.Method = HttpMethod.Get;
                httpRequest.RequestUri = new Uri(link);

                HttpResponseMessage responseMessage =await new HttpClient().SendAsync(httpRequest);
                WebView1.NavigateToString(await responseMessage.Content.ReadAsStringAsync());
            }
            catch(Exception)
            {
                await new MessageDialog("链接微软服务器失败！").ShowAsync();
                搜索.Visibility = 0;
                return;
            }

            await Task.Delay(1000);
            try
            { 
                for (int j = 0; j < 10000; j += 500)
                {
                    var SendScrLoc = string.Format(@"window.scrollTo(document.body.scrollWidth,{0})", j);
                    await WebView1.InvokeScriptAsync("eval", new string[] { SendScrLoc });
                    await Task.Delay(200);
                }

                var checkCount = string.Format(@"document.getElementsByClassName('c-group f-wrap-items context-list-page')[0].querySelectorAll('.m-channel-placement-item').length.toString();");
                string get2 = await WebView1.InvokeScriptAsync("eval", new string[] { checkCount });
                int itemCount = Convert.ToInt32(get2);

                for (int i = 0; i < itemCount; i++)
                {
                    BitmapImage bitmapImage = null;                    
                    var send3 = string.Format(@"document.getElementsByClassName('c-group f-wrap-items context-list-page')[0].querySelectorAll('.m-channel-placement-item h3')[{0}].innerText;", i);
                    string get3 = await WebView1.InvokeScriptAsync("eval", new string[] { send3 });
               
                    if (get3.Length > 内容.Text.Length)
                    {
                        if (!get3.ToLower().Contains(内容.Text.ToLower()))
                        {
                            continue;
                        }
                    }
                    else if (内容.Text.Length > get3.Length)
                    {
                        if (!内容.Text.ToLower().Contains(get3.ToLower()))
                        {
                            continue;
                        }
                    }
                    var send7 = string.Format(@"document.getElementsByClassName('c-group f-wrap-items context-list-page')[0].querySelectorAll('.m-channel-placement-item a')[{0}].href;", i);
                    string get7 = await WebView1.InvokeScriptAsync("eval", new string[] { send7 });
                    string get7a = get7.Substring(get7.Length - 12, 12);

                    try
                    {
                        var send4 = string.Format(@"document.getElementsByClassName('c-group f-wrap-items context-list-page')[0].querySelectorAll('.m-channel-placement-item source')[{0}].srcset;", i);
                        Uri get4 = new Uri(await WebView1.InvokeScriptAsync("eval", new string[] { send4 }));

                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            bitmapImage = new BitmapImage(get4);
                        });
                    }
                    catch(Exception)
                    {
                        continue;
                    }
                        

                    APPList.Items.Add(new 搜索信息 { 应用图标 = bitmapImage, 应用名 = get3, 应用ID = get7a });
                }
            }
            catch(Exception)
            {

            }

            搜索.Visibility = 0;
        }

        private void 跳转_Click(object sender, RoutedEventArgs e)
        {
            var appID = (APPList.SelectedItem as 搜索信息).应用ID;

            this.Frame.Navigate(typeof(DownloadPackage));

            MainPage.PageTrans.LeftList.SelectedIndex = 0;
            DownloadPackage.PageTras.类型.SelectedIndex = 1;
            DownloadPackage.PageTras.信息.Text = appID;
        }
        private void APPList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(APPList.SelectedIndex>=0)
            {
                跳转.Visibility = Visibility.Visible;
            }
            else
            {
                跳转.Visibility = Visibility.Collapsed;
            }
        }
    }
}
