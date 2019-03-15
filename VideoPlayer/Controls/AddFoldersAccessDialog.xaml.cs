using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace VideoPlayer.Controls
{
    public sealed partial class AddFoldersAccessDialog : UserControl
    {
        private Popup popup;
        public AddFoldersAccessDialog()
        {
            this.InitializeComponent();
            popup = new Popup();
            popup.Child = this;
            
        }

        private async void OpenSettings_button_Click(object sender, RoutedEventArgs e)
        {
            bool result = await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-videos"));
        }

        public void ShowDialog()
        {
            popup.IsOpen = true;
           
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await folderAccess_contentDialog.ShowAsync();
        }

        private void Close_button_Click(object sender, RoutedEventArgs e)
        {
            folderAccess_contentDialog.Hide();
        }
    }
}
