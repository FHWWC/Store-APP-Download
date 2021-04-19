using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace 获取商店应用直链
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage PageTrans;
        public MainPage()
        {
            this.InitializeComponent();
            PageTrans = this;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Color color = new UISettings().GetColorValue(UIColorType.Background);
            changeTheme(color);
            new UISettings().ColorValuesChanged += MainPage_ColorValuesChanged;

            APPFrame.Navigate(typeof(DownloadPackage));
        }

        private async void MainPage_ColorValuesChanged(UISettings sender, object args)
        {
            Color color = new UISettings().GetColorValue(UIColorType.Background);

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                changeTheme(color);
            });

        }
        public void changeTheme(Color color)
        {
            if (color.ToString() == "#FF000000")
            {
                布局1.Background = new SolidColorBrush(Colors.DarkCyan);
                布局3.Background = new SolidColorBrush(Colors.CadetBlue);
                LPControl.Background = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                LinearGradientBrush gradientBrush = new LinearGradientBrush();
                gradientBrush.StartPoint = new Point(0, 0);
                gradientBrush.EndPoint = new Point(0, 1);

                gradientBrush.GradientStops.Add(new GradientStop() { Color = Colors.LightBlue, Offset = 0 });
                gradientBrush.GradientStops.Add(new GradientStop() { Color = Colors.GhostWhite, Offset = 0.5 });
                gradientBrush.GradientStops.Add(new GradientStop() { Color = Colors.LightBlue, Offset = 1 });

                布局1.Background = gradientBrush;
                布局3.Background = new SolidColorBrush(Colors.LightBlue);
                LPControl.Background = new SolidColorBrush(Colors.SkyBlue);
            }
        }
        private void LPControl_Click(object sender, RoutedEventArgs e)
        {
            LeftPanel.IsPaneOpen = !LeftPanel.IsPaneOpen; ;
        }

        private void LeftList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch(LeftList.SelectedIndex)
            {
                case 0:
                    APPFrame.Navigate(typeof(DownloadPackage));
                    break;
                case 1:
                    APPFrame.Navigate(typeof(SearchPage));
                    break;
                case 2:
                    APPFrame.Navigate(typeof(InstallPackage));
                    break;
            }
        }
    }
}
