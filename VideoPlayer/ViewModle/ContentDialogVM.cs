using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;

namespace VideoPlayer.ViewModle
{
    public class ContentDialogVM
    {
        private static ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();
        private static string closeButton_str = resourceLoader.GetString("CloseMessageDialog_str");
        public async void OpenFeedBackFailedDialog()
        {
            string content_str = resourceLoader.GetString("openFeedBackFailed_str");
            ContentDialog content = new ContentDialog
            {
                Content = content_str,
                CloseButtonText = closeButton_str,
            };
            await content.ShowAsync();
        }

        public static async void SetAboutContent(string str)
        {

            ContentDialog content = new ContentDialog
            {
                Content = str,
                CloseButtonText = closeButton_str,
            };
            //content.Background = new SolidColorBrush(Colors.Transparent);
            await content.ShowAsync();
        }
    }
}
