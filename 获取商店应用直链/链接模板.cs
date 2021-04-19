using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;

namespace 获取商店应用直链
{
    public class 链接信息
    {
        public string 文件名 { get; set; }
        public string 详细信息 { get; set; }
        public Uri 下载链接 { get; set; }
        public string 标题颜色 { get; set; }
    }
    public class 下载模板
    {
        public double 进度 { get; set; }
        public double 已下载 { get; set; }
        public double 总大小 { get; set; }
        public double 进度条进度 { get; set; }
    }
    public class 搜索信息
    {
        public BitmapImage 应用图标 { get; set; }
        public string 应用名 { get; set; }
        public string 应用ID { get; set; }
    }
}
