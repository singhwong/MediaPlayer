using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VideoPlayer.Modle;
using VideoPlayer.Modles;
using VideoPlayer.ViewModle;
using Windows.ApplicationModel.Contacts;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace VideoPlayer.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        private Contact recipient = new Contact();
        private ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();
        private ContentDialogVM contentVM = new ContentDialogVM();
        private ApplicationDataContainer local_systemSound = ApplicationData.Current.LocalSettings;
        private ApplicationDataContainer local_systemSoundVolume = ApplicationData.Current.LocalSettings;
        public SettingPage()
        {
            this.InitializeComponent();

            //if (!Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.IsSupported())
            //{
            //    feedBack_button.Visibility = Visibility.Collapsed;
            //    //var value = main_data.Where(p => p.index == 4).ToList();
            //    //var feedBack_menu = value[0];
            //    //main_data.Remove(feedBack_menu);
            //    //this.item_feedBack.Visibility = Visibility.Visible;
            //}//确定设备是否显示反馈按钮      
            string OnContent_str = resourceLoader.GetString("OnContent_str ");
            string OffContent_str = resourceLoader.GetString("OffContent_str");
            SystemSound_toggleSwitch.OnContent = OnContent_str;
            SystemSound_toggleSwitch.OffContent = OffContent_str;
            email_button.Click += new RoutedEventHandler(Email_buttonClick);
            about_button.Click += new RoutedEventHandler(About_buttonClick);
            comment_button.Click += new RoutedEventHandler(Comment_buttonClick);
            email_button.PointerEntered += new PointerEventHandler(Email_buttonPointerEntered);
            email_button.PointerExited += new PointerEventHandler(Email_buttonPointerExited);
            comment_button.PointerEntered += new PointerEventHandler(Comment_buttonPointerEntered);
            comment_button.PointerExited += new PointerEventHandler(Comment_buttonPointerExited);
            about_button.PointerEntered += new PointerEventHandler(About_buttonPointerEntered);
            about_button.PointerExited += new PointerEventHandler(About_buttonPointerExited);
            SystemSound_toggleSwitch.Toggled += new RoutedEventHandler(SystemSound_toggleSwitchToggled);
            soundVolume_slider.ValueChanged += new RangeBaseValueChangedEventHandler(SoundVolume_sliderValueChanged);
            this.Loaded += new RoutedEventHandler(PageLoaded);
        }

        private async void Email_buttonClick(object sender, RoutedEventArgs e)
        {
            await SetFeedBackClass.ComposeEmail(recipient);
        }

        private async void About_buttonClick(object sender, RoutedEventArgs e)
        {
            //string about_str = resourceLoader.GetString("about_content");
            //ContentDialogVM.SetAboutContent(about_str);
            await about_contentDialog.ShowAsync();
        }

        private async void Comment_buttonClick(object sender, RoutedEventArgs e)
        {
            //await SetFeedBackClass.ShowRatingReviewDialog();
            bool result = await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?ProductId=9N6JVKJH4FFL"));
        }

        private void Email_buttonPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            email_button.FontWeight = Windows.UI.Text.FontWeights.Bold;
        }

        private void Email_buttonPointerExited(object sender, PointerRoutedEventArgs e)
        {
            email_button.FontWeight = Windows.UI.Text.FontWeights.Normal;
        }

        private void Comment_buttonPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            comment_button.FontWeight = Windows.UI.Text.FontWeights.Bold;
        }

        private void Comment_buttonPointerExited(object sender, PointerRoutedEventArgs e)
        {
            comment_button.FontWeight = Windows.UI.Text.FontWeights.Normal;
        }

        private void About_buttonPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            about_button.FontWeight = Windows.UI.Text.FontWeights.Bold;
        }

        private void About_buttonPointerExited(object sender, PointerRoutedEventArgs e)
        {
            about_button.FontWeight = Windows.UI.Text.FontWeights.Normal;
        }

        private void SystemSound_toggleSwitchToggled(object sender, RoutedEventArgs e)
        {
            if (SystemSound_toggleSwitch.IsOn)
            {
                ElementSoundPlayer.State = ElementSoundPlayerState.On;
                local_systemSound.Values["IsSoundOn"] = "On";
                //ElementSoundPlayer.Volume = 0.5;
            }
            else
            {
                ElementSoundPlayer.State = ElementSoundPlayerState.Off;
                local_systemSound.Values["IsSoundOn"] = "Off";
            }
        }

        private void SetIsToggleSwitchIsOn()
        {
            try
            {
                string isOn_str = local_systemSound.Values["IsSoundOn"].ToString();
                switch (isOn_str)
                {
                    case "On":
                        SystemSound_toggleSwitch.IsOn = true;
                        break;
                    case "Off":
                        SystemSound_toggleSwitch.IsOn = false;
                        break;
                }
            }
            catch
            {
                SystemSound_toggleSwitch.IsOn = true;
            }
        }

        private void SoundVolume_sliderValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            var sound_volume = soundVolume_slider.Value;
            ElementSoundPlayer.Volume = sound_volume / 100;
            local_systemSoundVolume.Values["Volume_setting"] = sound_volume / 100;
        }

        private void SetSoundPlayerVolume()
        {
            try
            {
                var soundVolume_value = (double)local_systemSoundVolume.Values["Volume_setting"];
                ElementSoundPlayer.Volume = soundVolume_value;
                soundVolume_slider.Value = soundVolume_value * 100;
            }
            catch
            {
                soundVolume_slider.Value = 30;
            }
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            //if (MainPage.page.RequestedTheme == ElementTheme.Light)
            //{
            //    this.RequestedTheme = ElementTheme.Light;
            //}
            //else
            //{
            //    this.RequestedTheme = ElementTheme.Dark;
            //}
            SetIsToggleSwitchIsOn();
            SetSoundPlayerVolume();
            GetLocalDataMethod();
            //setting_list = SaveProgressVM.ReadData(filePath);
            //progressSetting_tmr.Interval = TimeSpan.FromSeconds(0.1);
            //progressSetting_tmr.Tick += OnTimerTick_progress;
        }
        //private MediaPlaybackItem media_item;
        //private Video media;
        //private DispatcherTimer progressSetting_tmr = new DispatcherTimer();
        //List<Progress> setting_list;
        //private static StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
        //private static string filePath = storageFolder.Path + @"\histroyProgress.xml";
        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    media = e.Parameter as Video;
        //    media_item = new MediaPlaybackItem(MediaSource.CreateFromStorageFile(media.VideoFile));
        //    main_Element.SetPlaybackSource(media_item);



        //}

        //private void Main_Element_MediaOpened(object sender, RoutedEventArgs e)
        //{
        //    if (main_Element.CanSeek)
        //    {
        //        main_Element.Position = media.History_progress / 100 * media.Duration;
        //        //main_Element.Play();

        //    }

        //}

        //private void OnTimerTick_progress(object sender, object args)
        //{
        //    History_Progress.GetHistroyProgress(media, main_Element);
        //    foreach (var progress in setting_list)
        //    {
        //        if (progress.Path == media.Video_Path)
        //        {
        //            progress.Value = media.History_progress;
        //        }
        //    }
        //    SaveProgressVM.SaveData(setting_list, filePath);
        //}

        //private void Main_Element_CurrentStateChanged(object sender, RoutedEventArgs e)
        //{
        //    if (main_Element.CurrentState == MediaElementState.Paused)
        //    {
        //        progressSetting_tmr.Stop();
        //    }
        //    else if (main_Element.CurrentState == MediaElementState.Stopped)
        //    {
        //        progressSetting_tmr.Stop();
        //    }
        //    else if (main_Element.CurrentState == MediaElementState.Playing)
        //    {
        //        progressSetting_tmr.Start();
        //    }
        //}
        private StorageFile data_file;
        private BasicProperties value_size;
        private static StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
        private string filePath = storageFolder.Path + @"\histroyProgress.xml";
        private async void GetLocalDataMethod()
        {                       
            if (File.Exists(filePath))
            {
                data_file = await storageFolder.GetFileAsync("histroyProgress.xml");
                value_size = await data_file.GetBasicPropertiesAsync();
                data_textBlock.Text = string.Format("{0:F}", (double)(value_size.Size) / 1024) + " KB";
            }
        }

        private async void ClearData_button_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(filePath))
            {
                //List<Progress> setting_progressList = SaveProgressVM.ReadData(filePath);
                await data_file.DeleteAsync();
                data_textBlock.Text = "0 KB";               
                //setting_progressList.Clear();
            }           
        }

        private void CloseDialog_button_Click(object sender, RoutedEventArgs e)
        {
            about_contentDialog.Hide();
        }
    }
}
