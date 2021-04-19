using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Management.Deployment;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
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
    public sealed partial class InstallPackage : Page
    {
        public InstallPackage()
        {
            this.InitializeComponent();
        }

        private async void 网盘1_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/FHWWC/Store-APP-Download/releases"));
        }

        private async void 网盘2_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://pan.baidu.com/s/1ob7xeMwp5PH3muavUj3T0Q"));
        }

        /*
                 MessageDialog messageDialog;
                private async void 安装_Click(object sender, RoutedEventArgs e)
                {
                    var 文件选择 = new FileOpenPicker();
                    文件选择.SuggestedStartLocation = PickerLocationId.Desktop;
                    文件选择.ViewMode = PickerViewMode.List;

                    文件选择.FileTypeFilter.Add(".appx");
                    文件选择.FileTypeFilter.Add(".appxbundle");
                    文件选择.FileTypeFilter.Add(".xap");
                    文件选择.FileTypeFilter.Add("*");

                    StorageFile storageFile = await 文件选择.PickSingleFileAsync();

                    if (storageFile != null)
                    {
                        文件名.Text = storageFile.Name;
                        try
                        {
                            var progres = new Progress<DeploymentProgress>(installCallback);
                            PackageManager packageManager = new PackageManager();
                            DeploymentResult result = await packageManager.AddPackageAsync(new Uri(storageFile.Path), null, DeploymentOptions.None).AsTask(progres);

                            if (result.IsRegistered == true)
                            {
                                messageDialog = new MessageDialog("提示");
                                messageDialog.Content = "Successful!";
                                await messageDialog.ShowAsync();
                            }
                            else
                            {
                                messageDialog = new MessageDialog("提示");
                                messageDialog.Content += result.ErrorText;
                                messageDialog.Content += result.ExtendedErrorCode;
                                await messageDialog.ShowAsync();
                            }
                        }
                        catch (Exception)
                        {
                            messageDialog = new MessageDialog("Error!", "Error");
                            await messageDialog.ShowAsync();
                        }
                    }
                    else
                    {
                        文件名.Text = "Canceled";
                    }
                }
                private void installCallback(DeploymentProgress deploymentProgress)
                {
                    double progressNum = deploymentProgress.percentage;
                    进度2.Text = progressNum.ToString();
                    安装进度.Value = progressNum;
                }
        */
    }
}
