using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VideoPlayer.Controls;
using VideoPlayer.Modle;
using VideoPlayer.Modles;
using VideoPlayer.View;
using VideoPlayer.ViewModle;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage.Search;
using Windows.System;
using Windows.System.Display;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace VideoPlayer
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private static StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
        private string filePath = storageFolder.Path + @"\histroyProgress.xml";
        private string playModeStr_default;
        private string playModeStr_recycle;
        private string playModeStr_list;
        private string playModeStr_listRecycle;
        private string hideViewStr_yes;
        private string hideViewStr_no;
        private string hideListStr_yes;
        private string hideListStr_no;
        private string playRate_str;
        private string currentPlayTitle_str;
        private DisplayRequest display_Request = null;
        private bool IsBottomShow = true;
        private bool IsVideoListShow = false;
        private bool scylePlay_bool = false;
        private bool listPlay_bool = false;
        private bool defaultPlay_bool = true;
        private bool listScyle_bool = false;
        private bool IsMediaElementPointEnter_bool = false;
        private ObservableCollection<StorageFile> all_video = new ObservableCollection<StorageFile>();
        private ObservableCollection<Video> use_video = new ObservableCollection<Video>();
        private Video main_video = new Video();
        private Video video_value = new Video();
        #region 初始化color
        private SolidColorBrush lightGray = new SolidColorBrush(Colors.LightGray);
        private SolidColorBrush whiteSmoke = new SolidColorBrush(Colors.WhiteSmoke);
        private SolidColorBrush white = new SolidColorBrush(Colors.White);
        private SolidColorBrush skyblue = new SolidColorBrush(Colors.SkyBlue);
        private SolidColorBrush lightPink = new SolidColorBrush(Colors.LightPink);
        private SolidColorBrush yellow = new SolidColorBrush(Colors.Yellow);
        private SolidColorBrush cornFlowerBlue = new SolidColorBrush(Colors.CornflowerBlue);
        private SolidColorBrush black = new SolidColorBrush(Colors.Black);
        private SolidColorBrush use_foreGround = new SolidColorBrush();
        #endregion
        private ApplicationDataContainer local_theme = ApplicationData.Current.LocalSettings;
        private ApplicationDataContainer local_systemSound = ApplicationData.Current.LocalSettings;
        private ApplicationDataContainer local_systemSoundVolume = ApplicationData.Current.LocalSettings;
        private ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();
        private ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
        private SystemMediaTransportControls systemMedia_TransportControls = SystemMediaTransportControls.GetForCurrentView();
        private ApplicationDataContainer history_volume = ApplicationData.Current.LocalSettings;
        public static MainPage page;
        private DispatcherTimer tmr = new DispatcherTimer();
        private DispatcherTimer progress_tmr = new DispatcherTimer();
        private DispatcherTimer dateTime_tmr = new DispatcherTimer();
        private DispatcherTimer hideBottom_tmr = new DispatcherTimer();
        private Video[] remove_VideoList;
        private FrameworkElement sender_value;
        private MediaPlaybackItem mediaitem;
        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            SetIsElementSoundPlayerIsOn();
            page = this;
            #region  设置计时器，自动隐藏cursor   
            tmr.Interval = TimeSpan.FromSeconds(5);
            tmr.Tick += OnTimerTick;
            #endregion           
            SetDateTimeMethod();
            SetAutoHideBottomMethod();
            main_mediaElement.PointerPressed += new PointerEventHandler(main_mediaElementPointerPressed);
            main_mediaElement.DoubleTapped += new DoubleTappedEventHandler(main_mediaElementDoubleTapped);
            main_mediaElement.MediaEnded += new RoutedEventHandler(Main_mediaElementMediaEnded);
            main_mediaElement.VolumeChanged += new RoutedEventHandler(Main_mediaElementVolumeChanged);
            content_gridView.SelectionChanged += new SelectionChangedEventHandler(Content_gridViewSelectionChanged);
            main_mediaElement.PointerMoved += new PointerEventHandler(Main_mediaElementPointerMoved);
            main_mediaElement.PointerEntered += new PointerEventHandler(Main_mediaElementPointerEntered);
            main_mediaElement.PointerExited += new PointerEventHandler(Main_mediaElementPointerExited);
            main_mediaElement.MediaOpened += new RoutedEventHandler(Main_mediaElementMediaOpened);
            multiple_listBox.SelectionChanged += new SelectionChangedEventHandler(Multiple_listBoxSelectionChanged);
            remove_comboBox.SelectionChanged += new SelectionChangedEventHandler(Remove_comboBoxSelectionChanged);
            main_mediaElement.CurrentStateChanged += new RoutedEventHandler(main_mediaElementCurrentStateChanged);
            play_button.Click += new RoutedEventHandler(Play_buttonClick);
            progress_slider.ValueChanged += new RangeBaseValueChangedEventHandler(Progress_sliderValueChanged);
            volume_slider.ValueChanged += new RangeBaseValueChangedEventHandler(Volume_sliderValueChanged);
            volumeIcon_button.Click += new RoutedEventHandler(VolumeIcon_buttonClick);
            next_button.Click += new RoutedEventHandler(Next_buttonClick);
            previous_button.Click += new RoutedEventHandler(Previous_buttonClick);
            playMode_button.Click += new RoutedEventHandler(PlayMode_buttonClick);
            hideView_button.Click += new RoutedEventHandler(HideView_buttonClick);
            hideList_button.Click += new RoutedEventHandler(HideList_buttonClick);
            fullScreen_button.Click += new RoutedEventHandler(FullScreen_buttonClick);
            bottom_grid.PointerEntered += new PointerEventHandler(Bottom_gridPointerEntered);
            bottom_grid.PointerExited += new PointerEventHandler(Bottom_gridPointerExited);
            item_0_1.Tapped += new TappedEventHandler(Item_0_1Tapped);
            item_0_2.Tapped += new TappedEventHandler(Item_0_2Tapped);
            item_0_3.Tapped += new TappedEventHandler(Item_0_3Tapped);
            item_0_4.Tapped += new TappedEventHandler(Item_0_4Tapped);
            item_0_5.Tapped += new TappedEventHandler(Item_0_5Tapped);
            item_1_0.Tapped += new TappedEventHandler(Item_1_0Tapped);
            item_1_5.Tapped += new TappedEventHandler(Item_1_5Tapped);
            item_2_0.Tapped += new TappedEventHandler(Item_2_0Tapped);
            item_2_5.Tapped += new TappedEventHandler(Item_2_5Tapped);
            item_3_0.Tapped += new TappedEventHandler(Item_3_0Tapped);
            addFolder_button.Click += new RoutedEventHandler(AddFolder_buttonClick);
            content_gridView.ItemClick += new ItemClickEventHandler(Content_gridViewItemClick);
            allSelect_checkBox.Click += new RoutedEventHandler(AllSelect_checkBoxClick);
            settings_button.Click += new RoutedEventHandler(Settings_buttonClick);
            openCutAdress_button.Click += new RoutedEventHandler(OpenCutAdress_buttonClick);
            editPicture_button.Click += new RoutedEventHandler(EditPicture_buttonClick);
            closeCutDialog_button.Click += new RoutedEventHandler(CloseCutDialog_buttonClick);
            Fill_item.Click += new RoutedEventHandler(Fill_itemClick);
            UniformToFill_item.Click += new RoutedEventHandler(UniformToFill_itemClick);
            Uniform_item.Click += new RoutedEventHandler(Uniform_itemClick);
            None_item.Click += new RoutedEventHandler(None_itemClick);
            closeDialog_Button.Click += new RoutedEventHandler(CloseDialog_ButtonClick);
            screenShot_item.Click += new RoutedEventHandler(ScreenShot_itemClick);
            search_autoSuggestBox.QuerySubmitted += new TypedEventHandler<AutoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs>(Search_autoSuggestBoxQuerySubmitted);
            search_autoSuggestBox.TextChanged += new TypedEventHandler<AutoSuggestBox, AutoSuggestBoxTextChangedEventArgs>(Search_autoSuggestBoxTextChanged);
            refreshVideos_button.Click += new RoutedEventHandler(RefreshVideos_buttonClick);
            addFolders_button.Click += new RoutedEventHandler(AddFolders_buttonClick);
        }

        private void SetIsElementSoundPlayerIsOn()
        {
            try
            {
                string isOn_str = local_systemSound.Values["IsSoundOn"].ToString();
                switch (isOn_str)
                {
                    case "On":
                        ElementSoundPlayer.State = ElementSoundPlayerState.On;
                        break;
                    case "Off":
                        ElementSoundPlayer.State = ElementSoundPlayerState.Off;
                        break;
                }
            }
            catch
            {
                ElementSoundPlayer.State = ElementSoundPlayerState.On;
            }
            try
            {
                var soundPlayerVolume = (double)local_systemSoundVolume.Values["Volume_setting"];
                ElementSoundPlayer.Volume = soundPlayerVolume;
            }
            catch
            {
                ElementSoundPlayer.Volume = 0.3;
            }
        }
        private void main_mediaElement_PointerWheelChanged(CoreWindow sender, PointerEventArgs args)
        {
            if (IsMediaElementPointEnter_bool)
            {
                #region 鼠标滚动调节音量
                var value = (double)args.CurrentPoint.Properties.MouseWheelDelta;
                if (value > 0)//value = 120;
                {
                    main_mediaElement.Volume += 0.05;
                }
                else//value = -120;
                {
                    main_mediaElement.Volume -= 0.05;
                }
                #endregion
            }
        }

        #region 将亚克力扩展到标题栏
        private void ExtendAcrylicIntoTitleBar()
        {
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            titleBar.ButtonForegroundColor = Colors.White;
            titleBar.ButtonHoverForegroundColor = Colors.White;
            titleBar.ButtonHoverBackgroundColor = Colors.Black;
        }
        #endregion

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await GetVideoLibrary();
            ReadLocalStr();
            ButtonTipText();
            GetHistoryVlumeValue();
            #region 启用后台播放控件
            systemMedia_TransportControls.IsPlayEnabled = true;
            systemMedia_TransportControls.IsPauseEnabled = true;
            systemMedia_TransportControls.ButtonPressed += SystemControls_ButtonPressed;
            #endregion
            //#region 获取保存的主题(Light\Dark)设置并运行
            //try
            //{
            //    string theme_value = local_theme.Values["theme"].ToString();
            //    switch (theme_value)
            //    {
            //        case "Light":
            //            this.RequestedTheme = ElementTheme.Light; break;
            //        case "Dark":
            //            this.RequestedTheme = ElementTheme.Dark; break;
            //        default:
            //            break;
            //    }
            //}
            //catch
            //{
            //}
            //#endregion
            ExtendAcrylicIntoTitleBar();
        }

        private async void SystemControls_ButtonPressed(SystemMediaTransportControls sender,
   SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            #region 后台控件点击事件
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Play:
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        main_mediaElement.AutoPlay = true;
                        main_mediaElement.Play();
                    });
                    break;
                case SystemMediaTransportControlsButton.Pause:
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        main_mediaElement.Pause();
                    });
                    break;
                case SystemMediaTransportControlsButton.Previous:
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        NextPlaySameMethod();
                    });
                    break;
                case SystemMediaTransportControlsButton.Next:
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        SameCodeMethod();
                    });
                    break;
                default:
                    break;
            }
            #endregion
        }

        private void NextPlaySameMethod()
        {
            GetCurrentTheme();
            index = SetListPlayMusic() - 1;
            if (index == -1)
            {
                main_video = use_video[use_video.Count - 1];
            }
            else
            {
                main_video = use_video[index];
            }
            SameCodeHelp();
        }
        private void main_mediaElementCurrentStateChanged(object sender, RoutedEventArgs e)//MediaPlayerElement无currentStateChanged事件，                                                                                            //改用MediaElement控件
        {
            main_mediaElement.TransportControls.IsCompactOverlayEnabled = true;
            main_mediaElement = sender as MediaElement;
            if (main_mediaElement != null && main_mediaElement.IsAudioOnly == false)
            {
                if (main_mediaElement.CurrentState == Windows.UI.Xaml.Media.MediaElementState.Playing)
                {
                    play_button.Content = "\uE769";
                    SetProgressTimerTick();
                    tmr.Start();
                    if (display_Request == null)
                    {
                        display_Request = new DisplayRequest();
                        display_Request.RequestActive();
                    }
                }
                else
                {
                    play_button.Content = "\uE768";
                    progress_tmr.Stop();
                    if (display_Request != null)
                    {
                        display_Request.RequestRelease();
                        display_Request = null;
                    }
                }
            }
            #region 后台控件状态改变
            switch (main_mediaElement.CurrentState)
            {
                case MediaElementState.Playing:
                    systemMedia_TransportControls.PlaybackStatus = MediaPlaybackStatus.Playing;
                    break;
                case MediaElementState.Paused:
                    systemMedia_TransportControls.PlaybackStatus = MediaPlaybackStatus.Paused;
                    break;
                case MediaElementState.Stopped:
                    systemMedia_TransportControls.PlaybackStatus = MediaPlaybackStatus.Paused;
                    break;
                default:
                    break;
            }
            #endregion
        }

        private void main_mediaElementPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var temp = e.GetCurrentPoint(sender as Button);
            if (temp.Properties.IsRightButtonPressed)
            {
            }
            else if (temp.Properties.IsLeftButtonPressed)
            {
                if (IsBottomShow)
                {
                    show_bottom.Begin();
                    show_bottom_2.Stop();
                    hideBottom_tmr.Stop();
                    IsBottomShow = false;
                }
                if (IsSettingsGridShow_bool)
                {
                    hide_settingsGrid.Begin();
                    show_settingsGrid.Stop();
                    hideView_button.Content = "\uE70E";
                    ToolTipService.SetToolTip(hideView_button, hideViewStr_no);
                    IsSettingsGridShow_bool = false;
                }
                else
                {
                }
            }
        }

        private async Task GetVideoLibrary()
        {
            ToolTipService.SetToolTip(hideList_button, hideListStr_yes);
            await GetLocalVideo();
        }

        private async Task GetAllMedias(ObservableCollection<StorageFile> list, StorageFolder folder)
        {
            string[] fileTypes = new string[] { ".mp4", ".wmv", ".mkv", ".avi", ".3gp", ".flv", ".mpg", ".webm", ".mov", ".Ogg", ".swf", ".rmvb" };
            QueryOptions queryOption = new QueryOptions
       (CommonFileQuery.OrderByTitle, fileTypes);
            queryOption.FolderDepth = FolderDepth.Deep;
            Queue<IStorageFolder> folders = new Queue<IStorageFolder>();
            var files = await folder.CreateFileQueryWithOptions
              (queryOption).GetFilesAsync();
            foreach (var file in files)
            {
                list.Add(file);
            }
        }

        private async Task GridView_Videos(ObservableCollection<StorageFile> files)
        {
            foreach (var video in files)
            {
                Video this_video = new Video();
                VideoProperties video_Properties = await video.Properties.GetVideoPropertiesAsync();
                try//防止专辑图片为空
                {
                    StorageItemThumbnail current_Thumb = await video.GetThumbnailAsync(
                ThumbnailMode.VideosView,
                200,
                ThumbnailOptions.UseCurrentScale
                );
                    BitmapImage video_cover = new BitmapImage();
                    await video_cover.SetSourceAsync(current_Thumb);
                    this_video.Cover = video_cover;
                }
                catch
                {
                }
                this_video.Title = video_Properties.Title;
                SetListTimeMethod((int)video_Properties.Duration.TotalSeconds);
                this_video.Video_Duration = list_hh + ":" + list_mm + ":" + list_ss;
                this_video.Duration = video_Properties.Duration;
                this_video.Video_Path = video.Name;
                this_video.VideoFile = video;
                this_video.IsSelected = false;
                SetDefaultForeGround(this_video);
                SaveProgressVM.ReadProgressData(this_video, progresslist.progress_list, filePath);
                //添加文件夹视频时，跳过重复文件名视频
                var contain_videos = use_video.Where(p => p.Video_Path == this_video.Video_Path).Select(p => p.Video_Path).ToList();
                if (contain_videos.Count == 0)
                {
                    use_video.Add(this_video);
                }
                if (main_video != null)
                {
                    foreach (var item in use_video)
                    {
                        if (item.Video_Path == main_video.Video_Path)
                        {
                            main_video = item;
                            item.Video_Color = Color.FromArgb(50, 70, 0, 70);
                        }
                    }
                }
            }
            SetProgressList();
        }


        private async Task GetLocalVideo()
        {
            all_video.Clear();
            use_video.Clear();
            view_grid.Visibility = Visibility.Visible;
            IsVideoListShow = true;
            StorageFolder folder = KnownFolders.VideosLibrary;
            try
            {
                content_progressRing.Visibility = Visibility.Visible;
                content_progressRing.IsActive = true;
                await GetAllMedias(all_video, folder);
                await GridView_Videos(all_video);
                content_progressRing.Visibility = Visibility.Collapsed;
                content_progressRing.IsActive = false;
            }
            catch
            {
            }
            if (use_video.Count == 0)
            {
                IsVideoListShow = false;
                SetNoVideoContentDialog();
                view_grid.Visibility = Visibility.Collapsed;
                ToolTipService.SetToolTip(hideList_button, hideListStr_no);
                content_progressRing.IsActive = false;
            }
        }

        private void SetNoVideoContentDialog()
        {
            string IsHaveVideo_str = resourceLoader.GetString("isHaveVideo_content");
            try
            {
                main_notify.Hide();
            }
            catch
            {
            }
            main_notify = new NotifyShow(IsHaveVideo_str);
            main_notify.Show();
        }

        private void SetWaitContentDialog()
        {
            string wait_str = resourceLoader.GetString("wait_content");
            //MessageDialog content = new MessageDialog(wait_str);
            //content.Commands.Add(new UICommand(CloseMessageDialog_str, (command) =>
            //{
            //}));
            //content.DefaultCommandIndex = 0;
            //await content.ShowAsync();
            try
            {
                main_notify.Hide();
            }
            catch
            {
            }
            main_notify = new NotifyShow(wait_str);
            main_notify.Show();
        }

        private void GetCurrentTheme()
        {
            main_video.Video_Color = Color.FromArgb(50, 0, 55, 100);
        }

        private SolidColorBrush transParent = new SolidColorBrush(Colors.Transparent);
        private void SetDefaultForeGround(Video video)
        {
            video.Video_Color = Color.FromArgb(50, 0, 55, 100);
        }
        private void play_menuItem_Click(object sender, RoutedEventArgs e)
        {
            if (content_progressRing.IsActive)
            {
                SetWaitContentDialog();
            }
            else
            {
                progress_tmr.Stop();
                #region 右键选项点击播放
                GetCurrentTheme();
                main_mediaElement.AutoPlay = true;
                main_video = (Video)sender_value.DataContext;
                SetSelectedVideoSameCode();
                #endregion
            }
        }

        private void SetVideoRemovedContentDialog()
        {
            string playError_str = resourceLoader.GetString("playError_str");
            try
            {
                main_notify.Hide();
            }
            catch
            {
            }
            main_notify = new NotifyShow(playError_str);
            main_notify.Show();
            use_video.Remove(main_video);
        }

        private void pause_menuItem_Click(object sender, RoutedEventArgs e)
        {
            main_mediaElement.Pause();
        }

        private void delete_menuItem_Click(object sender, RoutedEventArgs e)
        {
            RemoveItemSameCode();
        }

        private void main_mediaElementDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            SetFullScreenMethod();
        }

        private void SetFullScreenMethod()
        {
            var view = ApplicationView.GetForCurrentView();
            if (view.IsFullScreenMode)
            {
                dateTime_textBlock.Visibility = Visibility.Collapsed;
                dateTime_tmr.Stop();
                view.ExitFullScreenMode();
                fullScreen_button.Content = "\uE740";
                second_colume.Width = new GridLength(15);
            }
            else
            {
                dateTime_textBlock.Visibility = Visibility.Visible;
                dateTime_tmr.Start();
                view.TryEnterFullScreenMode();
                second_colume.Width = new GridLength(0);
                fullScreen_button.Content = "\uE73F";
            }
            hideView_button.Content = "\uE76C";
            ToolTipService.SetToolTip(hideView_button, hideViewStr_no);
            ToolTipService.SetToolTip(hideList_button, hideListStr_no);
        }

        private void bin_item_Click(object sender, RoutedEventArgs e)
        {
            RemoveTo(StorageDeleteOption.Default);
        }

        private void RemoveItemSameCode()
        {
            video_value = (Video)sender_value.DataContext;
            use_video.Remove(video_value);
            if (search_videoCollection.Contains(video_value))
            {
                search_videoCollection.Remove(video_value);
            }
            AfterRemoveAllVideoFromLocal();
        }

        private async void RemoveTo(StorageDeleteOption value)
        {
            RemoveItemSameCode();
            StorageFile file = video_value.VideoFile;
            try
            {
                await file.DeleteAsync(value);
            }
            catch (System.IO.FileNotFoundException)
            {
            }
        }
        private void directly_item_Click(object sender, RoutedEventArgs e)
        {
            RemoveTo(StorageDeleteOption.PermanentDelete);
        }

        private void AfterRemoveAllVideoFromLocal()
        {
            if (use_video.Count == 0)//当删除所有视频列表本地文件时(主要是本地视频文件夹)，再次点击视频列表，
                                     //弹出无视频文件提示，当再次放入视频文件后，可以重新读取显示
            {
                IsVideoListShow = false;
            }
        }

        private void SetDefaultForeGround_SameCode()
        {
            foreach (var item in use_video)
            {
                SetDefaultForeGround(item);
            }
            main_video.Video_Color = Color.FromArgb(50, 70, 0, 70);
        }

        #region 启用 空格键、ESC键
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            Window.Current.CoreWindow.PointerWheelChanged += main_mediaElement_PointerWheelChanged;
        }

        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            if (args.VirtualKey == Windows.System.VirtualKey.Space)
            {//启用空格键控制播放和暂停
                if (main_mediaElement.CurrentState == Windows.UI.Xaml.Media.MediaElementState.Playing)
                {
                    main_mediaElement.Pause();
                }
                else if (main_mediaElement.CurrentState == Windows.UI.Xaml.Media.MediaElementState.Paused || main_mediaElement.CurrentState == MediaElementState.Stopped)
                {
                    main_mediaElement.Play();
                }
            }
            if (args.VirtualKey == Windows.System.VirtualKey.Escape)
            {//启用ESC键退出全屏
                var view = ApplicationView.GetForCurrentView();
                if (view.IsFullScreenMode)
                    view.ExitFullScreenMode();
                dateTime_textBlock.Visibility = Visibility.Collapsed;
            }
            #region 方向键控制
            if (args.VirtualKey == VirtualKey.Left)
            {
                main_mediaElement.Position -= TimeSpan.FromSeconds(10);//后退10s
            }
            if (args.VirtualKey == VirtualKey.Right)
            {
                main_mediaElement.Position += TimeSpan.FromSeconds(10);//快进10s
            }
            if (args.VirtualKey == VirtualKey.Up)
            {
                main_mediaElement.Volume += 0.05;//音量升0.05
            }
            if (args.VirtualKey == VirtualKey.Down)
            {
                main_mediaElement.Volume -= 0.05;//音量降0.05
            }
            #endregion
        }

        #endregion
        private string list_hh;
        private string list_mm;
        private string list_ss;
        private void SetListTimeMethod(int allTime)
        {
            int HH = allTime / 3600;
            int MM = (allTime - HH * 3600) / 60;
            int SS = allTime % 60;
            if (HH < 10)
            {
                list_hh = "0" + HH.ToString();
            }
            else
            {
                list_hh = HH.ToString();
            }
            if (MM < 10)
            {
                list_mm = "0" + MM.ToString();
            }
            else
            {
                list_mm = MM.ToString();
            }
            if (SS < 10)
            {
                list_ss = "0" + SS.ToString();
            }
            else
            {
                list_ss = SS.ToString();
            }
        }

        private void Scyle_item_Click(object sender, RoutedEventArgs e)
        {
            ScyclePlayMode();
        }

        private void List_item_Click(object sender, RoutedEventArgs e)
        {
            ListPlayMode();
        }

        private void Default_item_Click(object sender, RoutedEventArgs e)
        {
            DefaultMode();
        }

        private int index = 0;
        private void Main_mediaElementMediaEnded(object sender, RoutedEventArgs e)
        {
            progress_tmr.Stop();
            main_mediaElement.AutoPlay = true;
            if (content_progressRing.IsActive)
            {
                SetWaitContentDialog();
            }
            else
            {
                #region 播放模式设置
                if (scylePlay_bool)
                {
                    index = SetListPlayMusic();
                    GetCurrentTheme();
                    if (index >= 0)
                    {
                        main_video = use_video[index];
                        SameCodeHelp();
                    }
                    else
                    {
                        main_mediaElement.Play();
                    }
                    main_video.History_progress = 0;
                    ClearProgressListValueSameCode(main_video);
                }
                else if (listPlay_bool)
                {
                    if (use_video.Count == 1)
                    {
                        index = 0;
                    }
                    else
                    {
                        index = SetListPlayMusic() + 1;
                    }
                    if (index <= use_video.Count - 1)
                    {
                        GetCurrentTheme();
                        main_video = use_video[index];
                        SameCodeHelp();
                    }
                }
                else if (defaultPlay_bool)
                {
                    main_mediaElement.Pause();
                }
                else if (listScyle_bool)
                {
                    SameCodeMethod();
                }
                #endregion
            }
        }
        private int num;
        private int SetListPlayMusic()
        {
            var list_media = SetVideoByPath.GetVideoByStream(use_video, main_video.Video_Path);
            if (list_media.Video_Path != null)
            {
                for (int i = 0; i <= use_video.Count - 1; i++)
                {
                    if (use_video[i] == list_media)
                    {
                        num = i;
                    }
                }
            }
            else
            {
                num = -1;
            }
            return num;
        }

        private void ListScyle_item_Click(object sender, RoutedEventArgs e)
        {
            ListScycleMode();
        }

        private void SetMediaItemMethod()
        {
            metadata_StackPanel.Children.Clear();
            mediaitem = new MediaPlaybackItem(MediaSource.CreateFromStorageFile(main_video.VideoFile));
            mediaitem.AudioTracksChanged += PlaybackItem_AudioTracksChanged;
            mediaitem.VideoTracksChanged += MediaPlaybackItem_VideoTracksChanged;
            mediaitem.TimedMetadataTracksChanged += MediaPlaybackItem_TimedMetadataTracksChanged;
            main_mediaElement.SetPlaybackSource(mediaitem);
        }
        private void SameCodeHelp()
        {
            #region 播放模式设置方法中，重复代码块            
            progress_tmr.Stop();
            try
            {
                SetMediaItemMethod();
            }
            catch
            {
                SetVideoRemovedContentDialog();//正在播放视频文件被删除，再次点击播放该视频引发的异常
            }
            main_mediaElement.Play();
            main_video.Video_Color = Color.FromArgb(50, 70, 0, 70);
            #endregion
        }

        private void SameCodeMethod()
        {
            #region 后台控件点击事件和播放模式方法中，重复代码块
            progress_tmr.Stop();
            GetCurrentTheme();
            index = SetListPlayMusic() + 1;
            if (index <= use_video.Count - 1)
            {
                main_video = use_video[index];
            }
            else
            {
                main_video = use_video[0];
            }
            SameCodeHelp();
            #endregion
        }

        private void Main_mediaElementVolumeChanged(object sender, RoutedEventArgs e)
        {
            var volume_num = main_mediaElement.Volume;
            volume_slider.Value = volume_num * 100;
            history_volume.Values["volume"] = volume_num;
        }

        private void GetHistoryVlumeValue()
        {
            try
            {
                var volume_value = history_volume.Values["volume"];
                main_mediaElement.Volume = (double)volume_value;
                volume_slider.Value = (double)volume_value * 100;
            }
            catch
            {
                main_mediaElement.Volume = 0.3;
            }
        }

        private void SetSelectedVideoSameCode()
        {
            try
            {
                num = 0;
                SetMediaItemMethod();
            }
            catch
            {
                SetVideoRemovedContentDialog();//正在播放视频文件被删除，再次点击播放该视频引发的异常
            }
            main_video.Video_Color = Color.FromArgb(50, 70, 0, 70);
        }
        private List<String> value = new List<string>();
        private string searchResult_str;
        private ObservableCollection<Video> search_videoCollection = new ObservableCollection<Video>();
        private bool IsMultipleClick_bool = false;
        private void Content_gridViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsMultipleClick_bool)
            {
                foreach (var item in use_video)
                {
                    item.IsSelected = false;
                }
                var selected_video = (GridView)sender;
                var value = selected_video.SelectedItems.ToList();
                foreach (var item in value)
                {
                    (item as Video).IsSelected = true;
                }
            }
            else
            {
                var value_item = (GridView)sender;
                value_item.SelectedItem = null;
            }

        }
        private ObservableCollection<VideoLibrary> videoLibrary_collection = new ObservableCollection<VideoLibrary>();
        private AddFoldersAccessDialog access_dialog;
        private async void GetFolderLibraryListViewItems()
        {
            try
            {
                myVideos = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Videos);
                IObservableVector<StorageFolder> myPictureFolders = myVideos.Folders;
                videoLibrary_collection.Clear();
                foreach (var item in myPictureFolders)
                {

                    VideoLibrary library = new VideoLibrary();
                    library.FolderName = item.DisplayName;
                    library.FolderPath = item.Path;
                    videoLibrary_collection.Add(library);
                }
                ShowAddFolderContentDialog();
            }
            catch (UnauthorizedAccessException)
            {
                //MessageDialog content = new MessageDialog(GetVideoAccessError_str);
                //content.Commands.Add(new UICommand(OpenLocalSetting_str, async (command) =>
                // {
                //     bool result = await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-videos"));
                // }));
                //content.Commands.Add(new UICommand(CloseMessageDialog_str, (command) =>
                //{
                //}));
                //content.DefaultCommandIndex = 0;
                //await content.ShowAsync();
                access_dialog = new AddFoldersAccessDialog();
                access_dialog.ShowDialog();
            }
        }

        private string OpenLocalSetting_str;
        private string CloseMessageDialog_str;
        private async void ShowAddFolderContentDialog()
        {
            await addFolder_contentDialog.ShowAsync();
        }

        private void OnTimerTick(object sender, object args)
        {
            HideCursor();
            tmr.Stop();
        }

        private void HideCursor()
        {
            if (main_mediaElement.CurrentState == MediaElementState.Playing)
            {
                Window.Current.CoreWindow.PointerCursor = null;
            }
        }

        private void ShowCursor()
        {
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);
        }

        private void Main_mediaElementPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            ShowCursor();
            tmr.Start();
        }

        private void Main_mediaElementPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            IsMediaElementPointEnter_bool = true;
        }

        private void Main_mediaElementPointerExited(object sender, PointerRoutedEventArgs e)
        {
            tmr.Stop();
            IsMediaElementPointEnter_bool = false;
        }

        public void SetProgressTimerTick()
        {
            progress_tmr.Interval = TimeSpan.FromSeconds(0.1);
            progress_tmr.Tick += OnTimerTick_progress;
            progress_tmr.Start();
        }

        private void OnTimerTick_progress(object sender, object args)
        {
            History_Progress.GetHistroyProgress(main_video, main_mediaElement);
            foreach (var progress in progresslist.progress_list)
            {
                if (progress.Path == main_video.Video_Path)
                {
                    progress.Value = main_video.History_progress;
                }
            }
            SaveProgressVM.SaveData(progresslist.progress_list, filePath);
        }

        private void Main_mediaElementMediaOpened(object sender, RoutedEventArgs e)
        {
            if (IsBottomShow == false)
            {
                show_bottom.Stop();
                show_bottom_2.Begin();
                IsBottomShow = true;
            }
            hideBottom_tmr.Start();
            SetProgressList();//解决，清除本地缓存后，播放记录的显示未清除异常
            if (main_mediaElement.CanSeek)
            {
                try
                {
                    var value = progresslist.progress_list.Where(p => p.Path == main_video.Video_Path).Select(p => p.Value).ToList();
                    main_mediaElement.Position = value[0] / 100 * main_video.Duration;
                    progress_slider.Maximum = main_mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
                    progress_slider.Value = main_mediaElement.Position.TotalSeconds;
                }
                catch
                {
                }
            }
        }

        private ProgressList progresslist = new ProgressList();
        private void SetProgressList()
        {
            if (File.Exists(filePath))
            {
                progresslist.progress_list = SaveProgressVM.ReadData(filePath);
                foreach (var video in use_video)
                {
                    var value = progresslist.progress_list.Where(p => p.Path == video.Video_Path).Select(p => p.Path).ToList();
                    if (value.Count == 0)
                    {
                        Progress progress = new Progress();
                        progress.Path = video.Video_Path;
                        progresslist.progress_list.Add(progress);
                    }
                }
            }
            else
            {
                progresslist.progress_list.Clear();
                foreach (var video in use_video)
                {
                    Progress progress = new Progress();
                    progress.Path = video.Video_Path;
                    progresslist.progress_list.Add(progress);
                }
            }
        }

        private void ClearAll_item_Click(object sender, RoutedEventArgs e)
        {
            progresslist.progress_list.Clear();
            SaveProgressVM.SaveData(progresslist.progress_list, filePath);
            foreach (var video in use_video)
            {
                video.History_progress = 0;
                Progress progress = new Progress();
                progress.Path = video.Video_Path;
                progresslist.progress_list.Add(progress);
            }

        }

        private void Multiple_item_Click(object sender, RoutedEventArgs e)
        {
            IsMultipleClick_bool = true;
            content_gridView.IsItemClickEnabled = false;
            multiple_dropShadowPanel.Visibility = Visibility.Visible;
            show_multipleGrid.Begin();
            hide_multipleGrid.Stop();
            content_gridView.SelectionMode = ListViewSelectionMode.Multiple;
        }

        private void Multiple_listBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (multiple_listBox.SelectedIndex)
            {
                case 0: break;
                case 1: break;
                case 2: ClearSelectedItemProgress(); break;
                case 3:
                    allSelect_checkBox.IsChecked = false;
                    hide_multipleGrid.Begin();
                    show_multipleGrid.Stop();
                    IsMultipleClick_bool = false;
                    content_gridView.SelectionMode = ListViewSelectionMode.None;
                    multiple_listBox.SelectedIndex = -1;
                    content_gridView.IsItemClickEnabled = true;
                    foreach (var item in use_video)
                    {
                        item.IsSelected = false;
                    }
                    break;
                default:
                    break;
            }
        }

        private void Remove_comboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            allSelect_checkBox.IsChecked = false;
            switch (remove_comboBox.SelectedIndex)
            {
                case 0: RemoveSelectedVideo(); break;
                case 1: MultipleRemoveoTo(StorageDeleteOption.Default); break;
                case 2: MultipleRemoveoTo(StorageDeleteOption.PermanentDelete); break;
                default:
                    break;
            }
        }

        private void RemoveSelectedVideo()
        {
            remove_VideoList = MultipleRemoveSameCode();
            foreach (var item in remove_VideoList)
            {
                use_video.Remove(item);
                if (search_videoCollection.Contains(item))
                {
                    search_videoCollection.Remove(item);
                }
            }
            remove_comboBox.SelectedIndex = -1;
            AfterRemoveAllVideoFromLocal();
        }

        private void ClearSelectedItemProgress()
        {
            allSelect_checkBox.IsChecked = false;
            foreach (var progress_video in use_video)
            {
                if (progress_video.IsSelected)
                {
                    progress_video.History_progress = 0;
                    foreach (var progress in progresslist.progress_list)
                    {
                        if (progress.Path == progress_video.Video_Path)
                        {
                            progress.Value = 0;
                        }
                    }
                }
            }

            SaveProgressVM.SaveData(progresslist.progress_list, filePath);
            content_gridView.SelectedItem = null;
            multiple_listBox.SelectedIndex = -1;
        }

        private async void MultipleRemoveoTo(StorageDeleteOption value)
        {
            remove_VideoList = MultipleRemoveSameCode();
            foreach (var item in remove_VideoList)
            {
                use_video.Remove(item);
                if (search_videoCollection.Contains(item))
                {
                    search_videoCollection.Remove(item);
                }
                if (item != null)
                {
                    StorageFile file = item.VideoFile;
                    try
                    {
                        await file.DeleteAsync(value);
                    }
                    catch (System.IO.FileNotFoundException)
                    {
                    }
                }
            }
            remove_comboBox.SelectedIndex = -1;
            AfterRemoveAllVideoFromLocal();
        }

        private Video[] MultipleRemoveSameCode()
        {
            var remove_VideoList = new Video[use_video.Count];
            for (int i = 0; i < use_video.Count; i++)
            {
                if (use_video[i].IsSelected)
                {
                    remove_VideoList[i] = use_video[i];
                }
            }
            return remove_VideoList;
        }

        private void Clear_item_Click(object sender, RoutedEventArgs e)
        {
            video_value = sender_value.DataContext as Video;
            video_value.History_progress = 0;
            ClearProgressListValueSameCode(video_value);
        }

        private void ClearProgressListValueSameCode(Video progress_video)
        {
            foreach (var progress in progresslist.progress_list)
            {
                if (progress.Path == progress_video.Video_Path)
                {
                    progress.Value = 0;
                }
            }
            SaveProgressVM.SaveData(progresslist.progress_list, filePath);
        }

        private void VideoList_stackPanle_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            sender_value = (FrameworkElement)sender;
        }

        private void Play_buttonClick(object sender, RoutedEventArgs e)
        {
            main_mediaElement.AutoPlay = true;
            if (main_mediaElement.CurrentState == MediaElementState.Playing)
            {
                main_mediaElement.Pause();
            }
            else
            {
                main_mediaElement.Play();
            }

        }

        private void Progress_sliderValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            showTime_textBlock.Text = PlayingTime.GetPlayTime((int)main_mediaElement.NaturalDuration.TimeSpan.TotalSeconds, (int)main_mediaElement.Position.TotalSeconds);
        }

        private void Volume_sliderValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            main_mediaElement.Volume = volume_slider.Value / 100;
        }

        private void VolumeIcon_buttonClick(object sender, RoutedEventArgs e)
        {
            if (main_mediaElement.IsMuted)
            {
                main_mediaElement.IsMuted = false;
                volumeIcon_button.Content = "\uE767";
                volume_button.Content = "\uE767"; ;
            }
            else
            {
                main_mediaElement.IsMuted = true;
                volumeIcon_button.Content = "\uE74F";
                volume_button.Content = "\uE74F";
            }
        }

        private void Next_buttonClick(object sender, RoutedEventArgs e)
        {
            if (content_progressRing.IsActive)
            {
                SetWaitContentDialog();
            }
            else
            {
                SameCodeMethod();
            }
        }

        private void Previous_buttonClick(object sender, RoutedEventArgs e)
        {
            if (content_progressRing.IsActive)
            {
                SetWaitContentDialog();
            }
            else
            {
                NextPlaySameMethod();
            }
        }
        private NotifyShow main_notify;
        private void PlayMode_buttonClick(object sender, RoutedEventArgs e)
        {
            if (defaultPlay_bool)
            {
                ScyclePlayMode();
            }
            else if (scylePlay_bool)
            {
                ListPlayMode();
            }
            else if (listPlay_bool)
            {
                ListScycleMode();
            }
            else
            {
                DefaultMode();
            }
        }

        private void SetNotifyContent(string content_str)
        {
            try
            {
                main_notify.Hide();
            }
            catch
            {
            }
            main_notify = new NotifyShow(content_str);
            main_notify.Show();
        }
        private void ScyclePlayMode()
        {
            SetNotifyContent(playModeStr_recycle);
            playMode_button.Content = "\uE8ED";
            ToolTipService.SetToolTip(playMode_button, playModeStr_recycle);
            scylePlay_bool = true;
            listPlay_bool = false;
            defaultPlay_bool = false;
            listScyle_bool = false;
            systemMedia_TransportControls.IsPreviousEnabled = false;
            systemMedia_TransportControls.IsNextEnabled = false;
            previous_button.IsEnabled = false;
            next_button.IsEnabled = false;
        }

        private void ListPlayMode()
        {
            SetNotifyContent(playModeStr_list);
            playMode_button.Content = "\uE845";
            ToolTipService.SetToolTip(playMode_button, playModeStr_list);
            scylePlay_bool = false;
            listPlay_bool = true;
            defaultPlay_bool = false;
            listScyle_bool = false;
            systemMedia_TransportControls.IsPreviousEnabled = true;
            systemMedia_TransportControls.IsNextEnabled = true;
            previous_button.IsEnabled = true;
            next_button.IsEnabled = true;
        }

        private void ListScycleMode()
        {
            SetNotifyContent(playModeStr_listRecycle);
            playMode_button.Content = "\uE895";
            ToolTipService.SetToolTip(playMode_button, playModeStr_listRecycle);
            scylePlay_bool = false;
            listPlay_bool = false;
            defaultPlay_bool = false;
            listScyle_bool = true;
            systemMedia_TransportControls.IsPreviousEnabled = true;
            systemMedia_TransportControls.IsNextEnabled = true;
            previous_button.IsEnabled = true;
            next_button.IsEnabled = true;
        }

        private void DefaultMode()
        {
            SetNotifyContent(playModeStr_default);
            playMode_button.Content = "\uE8D8";
            ToolTipService.SetToolTip(playMode_button, playModeStr_default);
            scylePlay_bool = false;
            listPlay_bool = false;
            defaultPlay_bool = true;
            listScyle_bool = false;
            systemMedia_TransportControls.IsPreviousEnabled = false;
            systemMedia_TransportControls.IsNextEnabled = false;
            previous_button.IsEnabled = false;
            next_button.IsEnabled = false;
        }
        private bool IsSettingsGridShow_bool = false;
        private void HideView_buttonClick(object sender, RoutedEventArgs e)
        {
            settings_gird.Visibility = Visibility.Visible;
            if (IsSettingsGridShow_bool)
            {
                hide_settingsGrid.Begin();
                show_settingsGrid.Stop();
                hideView_button.Content = "\uE70E";
                ToolTipService.SetToolTip(hideView_button, hideViewStr_no);
                IsSettingsGridShow_bool = false;
            }
            else
            {
                show_settingsGrid.Begin();
                hide_settingsGrid.Stop();
                hideView_button.Content = "\uE70D";
                ToolTipService.SetToolTip(hideView_button, hideViewStr_yes);
                IsSettingsGridShow_bool = true;
            }
        }

        private void HideList_buttonClick(object sender, RoutedEventArgs e)
        {
            if (use_video.Count != 0)
            {
                if (IsVideoListShow)
                {
                    second_colume.Width = new GridLength(0, GridUnitType.Pixel);
                    IsVideoListShow = false;
                    ToolTipService.SetToolTip(hideList_button, hideListStr_no);

                }
                else
                {
                    second_colume.Width = new GridLength(290, GridUnitType.Pixel);
                    IsVideoListShow = true;
                    ToolTipService.SetToolTip(hideList_button, hideListStr_yes);
                }
            }
        }

        private void FullScreen_buttonClick(object sender, RoutedEventArgs e)
        {
            SetFullScreenMethod();
        }

        private void Item_0_5Tapped(object sender, TappedRoutedEventArgs e)
        {
            main_mediaElement.PlaybackRate = 0.5;
        }

        private void Item_1_0Tapped(object sender, TappedRoutedEventArgs e)
        {
            main_mediaElement.PlaybackRate = 1.0;
        }

        private void Item_2_0Tapped(object sender, TappedRoutedEventArgs e)
        {
            main_mediaElement.PlaybackRate = 2.0;
        }

        private void Item_2_5Tapped(object sender, TappedRoutedEventArgs e)
        {
            main_mediaElement.PlaybackRate = 2.5;
        }

        private void Item_3_0Tapped(object sender, TappedRoutedEventArgs e)
        {
            main_mediaElement.PlaybackRate = 3.0;
        }

        private void Item_0_1Tapped(object sender, TappedRoutedEventArgs e)
        {
            main_mediaElement.PlaybackRate = 0.1;
        }

        private void Item_0_2Tapped(object sender, TappedRoutedEventArgs e)
        {
            main_mediaElement.PlaybackRate = 0.2;
        }

        private void Item_0_3Tapped(object sender, TappedRoutedEventArgs e)
        {
            main_mediaElement.PlaybackRate = 0.3;
        }

        private void Item_0_4Tapped(object sender, TappedRoutedEventArgs e)
        {
            main_mediaElement.PlaybackRate = 0.4;
        }

        private void Item_1_5Tapped(object sender, TappedRoutedEventArgs e)
        {
            main_mediaElement.PlaybackRate = 1.5;
        }

        private void SetDateTimeMethod()
        {
            #region  设置计时器，显示系统当前时间   
            dateTime_tmr.Interval = TimeSpan.FromSeconds(1);
            dateTime_tmr.Tick += DateTime_OnTimerTick;
            #endregion
        }

        private void DateTime_OnTimerTick(object sender, object args)
        {
            dateTime_textBlock.Text = DateTime.Now.ToString();
        }

        private void ButtonTipText()
        {
            ToolTipService.SetToolTip(playMode_button, playModeStr_default);
            if (IsVideoListShow)
            {
                ToolTipService.SetToolTip(hideList_button, hideListStr_yes);
            }
            else
            {
                ToolTipService.SetToolTip(hideList_button, hideListStr_no);
            }

            ToolTipService.SetToolTip(hideView_button, hideViewStr_yes);
            ToolTipService.SetToolTip(playRate_button, playRate_str);
            ToolTipService.SetToolTip(audioLanguage_button, audioLanguage_str);
            ToolTipService.SetToolTip(fillMode_button, fillMode_str);
            ToolTipService.SetToolTip(videoTracks_button, mediaTrack_str);
            ToolTipService.SetToolTip(metadata_button, metadata_str);
            ToolTipService.SetToolTip(addFolders_button, addFolder_str);
            ToolTipService.SetToolTip(refreshVideos_button, videoFolder_str);
            ToolTipService.SetToolTip(settings_button, setting_str);
        }
        private string audioLanguage_str;
        private string fillMode_str;
        private string mediaTrack_str;
        private string metadata_str;
        private string addFolder_str;
        private string videoFolder_str;
        private string setting_str;
        private string noPicture_str;
        private string noPicturesFolder_str;
        private string saveSucceed_str;
        private void ReadLocalStr()
        {
            playModeStr_default = resourceLoader.GetString("playModeStr_default");
            playModeStr_recycle = resourceLoader.GetString("playModeStr_recycle");
            playModeStr_list = resourceLoader.GetString("playModeStr_list");
            playModeStr_listRecycle = resourceLoader.GetString("playModeStr_listRecycle");
            hideViewStr_yes = resourceLoader.GetString("hideViewStr_yes");
            hideViewStr_no = resourceLoader.GetString("hideViewStr_no");
            hideListStr_yes = resourceLoader.GetString("hideListStr_yes");
            hideListStr_no = resourceLoader.GetString("hideListStr_no");
            playRate_str = resourceLoader.GetString("playRate_str");
            audioLanguage_str = resourceLoader.GetString("audioLanguage_str");
            fillMode_str = resourceLoader.GetString("fillMode_str");
            OpenLocalSetting_str = resourceLoader.GetString("OpenLocalSetting_str");
            CloseMessageDialog_str = resourceLoader.GetString("CloseMessageDialog_str");
            mediaTrack_str = resourceLoader.GetString("mediaTrack_str");
            metadata_str = resourceLoader.GetString("metadata_str");
            addFolder_str = resourceLoader.GetString("addFolder_str");
            videoFolder_str = resourceLoader.GetString("videoFolder_str");
            setting_str = resourceLoader.GetString("setting_str");
            currentPlayTitle_str = resourceLoader.GetString("currentPlayTitle_str");
            noPicture_str = resourceLoader.GetString("noPicture_str");
            noPicturesFolder_str = resourceLoader.GetString("noPicturesFolder_str");
            saveSucceed_str = resourceLoader.GetString("saveSucceed_str");
        }

        private void Click_grid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            show_bottom_2.Begin();
            IsBottomShow = true;
            show_bottom.Stop();
            hideBottom_tmr.Stop();
        }

        private void SetAutoHideBottomMethod()
        {
            #region  设置计时器，自动隐藏bottom   
            hideBottom_tmr.Interval = TimeSpan.FromSeconds(5);
            hideBottom_tmr.Tick += AutoHideBottom_OnTimerTick;
            #endregion
        }

        private void AutoHideBottom_OnTimerTick(object sender, object args)
        {
            show_bottom.Begin();
            IsBottomShow = false;
            show_bottom_2.Stop();
            hideBottom_tmr.Stop();
        }

        private void Bottom_gridPointerExited(object sender, PointerRoutedEventArgs e)
        {
            hideBottom_tmr.Start();
        }
        private List<String> audioLanguage_list = new List<string>();

        private StorageLibrary myVideos;
        private async void AddFolder_buttonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Windows.Storage.StorageFolder newFolder = await myVideos.RequestAddFolderAsync();
                var value = videoLibrary_collection.Where(p => p.FolderPath == newFolder.Path).Select(p => p).ToList();
                if (value.Count == 0)
                {
                    VideoLibrary library = new VideoLibrary();
                    library.FolderName = newFolder.DisplayName;
                    library.FolderPath = newFolder.Path;
                    videoLibrary_collection.Add(library);
                    await GetVideoLibrary();
                }
            }
            catch
            {
            }
        }


        private async void DeleteFolder_button_Click(object sender, RoutedEventArgs e)
        {
            sender_value = (FrameworkElement)sender;
            var remove_library = (VideoLibrary)sender_value.DataContext;
            foreach (var folder in myVideos.Folders)
            {

                if (folder.Path == remove_library.FolderPath)
                {
                    bool result = await myVideos.RequestRemoveFolderAsync(folder);
                }
            }
            for (int i = 0; i < videoLibrary_collection.Count; i++)
            {
                if (videoLibrary_collection[i] == remove_library)
                {
                    videoLibrary_collection.Remove(videoLibrary_collection[i]);
                }
            }
            all_video.Clear();
            use_video.Clear();
            await GetVideoLibrary();
        }

        private void Bottom_gridPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            hideBottom_tmr.Stop();
            ShowCursor();
        }

        private void Content_gridViewItemClick(object sender, ItemClickEventArgs e)
        {
            if (content_progressRing.IsActive)
            {
                SetWaitContentDialog();
            }
            else
            {
                progress_tmr.Stop();
                main_mediaElement.TransportControls.IsVolumeButtonVisible = true;
                GetCurrentTheme();
                main_mediaElement.AutoPlay = false;
                var value = e.ClickedItem;
                main_video = value as Video;
                SetSelectedVideoSameCode();
            }
        }

        private void AllSelect_checkBoxClick(object sender, RoutedEventArgs e)
        {
            if (allSelect_checkBox.IsChecked == true)
            {
                foreach (var video in use_video)
                {
                    video.IsSelected = true;
                }
                content_gridView.SelectAll();
            }
            else
            {
                foreach (var video in use_video)
                {
                    video.IsSelected = false;
                }
                content_gridView.SelectedItem = null;
            }
            multiple_listBox.SelectedIndex = -1;
        }

        private async void MediaPlaybackItem_VideoTracksChanged(MediaPlaybackItem sender, IVectorChangedEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                videoTracks_flyout.Items.Clear();
                for (int index = 0; index < sender.VideoTracks.Count; index++)
                {
                    var videoTrack = sender.VideoTracks[index];
                    MenuFlyoutItem flyout_item = new MenuFlyoutItem()
                    {
                        Text = String.IsNullOrEmpty(videoTrack.Language) ? playModeStr_default : videoTrack.Name + "(" + videoTrack.Language + ")",
                        Tag = index
                    };
                    flyout_item.Click += new RoutedEventHandler(MenuFlyoutVideoItemClick);
                    videoTracks_flyout.Items.Add(flyout_item);
                }
            });
        }

        private async void PlaybackItem_AudioTracksChanged(MediaPlaybackItem sender, IVectorChangedEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                audioLanguage_flyout.Items.Clear();
                for (int index = 0; index < sender.AudioTracks.Count; index++)
                {
                    var audioTrack = sender.AudioTracks[index];
                    MenuFlyoutItem flyout_item = new MenuFlyoutItem()
                    {
                        Text = String.IsNullOrEmpty(audioTrack.Language) ? playModeStr_default : audioTrack.Name + "(" + audioTrack.Language + ")",
                        Tag = index
                    };
                    flyout_item.Click += new RoutedEventHandler(MenuFlyoutAudioItemClick);
                    audioLanguage_flyout.Items.Add(flyout_item);
                }
            });
        }

        private async void MediaPlaybackItem_TimedMetadataTracksChanged(MediaPlaybackItem sender, IVectorChangedEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                metadata_StackPanel.Children.Clear();
                for (int index = 0; index < sender.TimedMetadataTracks.Count; index++)
                {
                    var timedMetadataTrack = sender.TimedMetadataTracks[index];

                    ToggleButton toggle = new ToggleButton()
                    {
                        Content = String.IsNullOrEmpty(timedMetadataTrack.Language) ? $"Track {index}" : timedMetadataTrack.Name + " " + timedMetadataTrack.Language,
                        Tag = (uint)index
                    };
                    toggle.Checked += Toggle_Checked;
                    toggle.Unchecked += Toggle_Unchecked;
                    metadata_StackPanel.Children.Add(toggle);
                }
            });
        }

        private void MenuFlyoutAudioItemClick(object sender, RoutedEventArgs e)
        {
            int trackIndex = (int)(((MenuFlyoutItem)sender).Tag);
            mediaitem.AudioTracks.SelectedIndex = trackIndex;
        }

        private void MenuFlyoutVideoItemClick(object sender, RoutedEventArgs e)
        {
            int trackIndex = (int)(((MenuFlyoutItem)sender).Tag);
            mediaitem.VideoTracks.SelectedIndex = trackIndex;
        }

        private void VideoTracks_listboxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            videoTracks_flyout.Hide();
            try
            {
                int trackIndex = (int)((ListBoxItem)((ListBox)sender).SelectedItem).Tag;
                mediaitem.VideoTracks.SelectedIndex = trackIndex;
            }
            catch
            {
            }
        }

        private void Toggle_Checked(object sender, RoutedEventArgs e)
        {
            try//偶尔出现 checked 崩溃，使用捕获异常语句来防止闪退
            {
                mediaitem.TimedMetadataTracks.SetPresentationMode((uint)((ToggleButton)sender).Tag,
       TimedMetadataTrackPresentationMode.PlatformPresented);
            }
            catch
            {
            }
        }


        private void Toggle_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                mediaitem.TimedMetadataTracks.SetPresentationMode((uint)((ToggleButton)sender).Tag,
       TimedMetadataTrackPresentationMode.Disabled);
            }
            catch
            {
            }
        }


        private void Metadata_girdPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            tmr.Stop();
        }

        private void AddFolders_buttonClick(object sender, RoutedEventArgs e)
        {
            GetFolderLibraryListViewItems();
        }

        private async void RefreshVideos_buttonClick(object sender, RoutedEventArgs e)
        {
            await GetVideoLibrary();
        }

        private void Search_autoSuggestBoxTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            var list = use_video.Where(p => p.Video_Path == search_autoSuggestBox.Text).Select(p => p.Video_Path).ToList();
            if (list.Count == 0)
            {
                if (!String.IsNullOrEmpty(search_autoSuggestBox.Text.Trim()))
                {
                    value = use_video.Where(p => p.Video_Path.Contains(sender.Text.ToUpper())).Select(p => p.Video_Path).ToList();

                    if (value.Count == 0)
                    {
                        searchResult_str = resourceLoader.GetString("searchResult_str");
                        value.Add(searchResult_str);
                    }
                }
                else
                {
                    content_gridView.ItemsSource = use_video;
                }
            }
            search_autoSuggestBox.ItemsSource = value;
        }

        private void Search_autoSuggestBoxQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!String.IsNullOrEmpty(search_autoSuggestBox.Text.Trim()))
            {
                search_videoCollection.Clear();
                var search_videoList = new List<Video>();
                foreach (var item in use_video)
                {
                    search_videoList.Add(item);
                }
                var value_list = search_videoList.Where(p => p.Video_Path == search_autoSuggestBox.Text).ToList();
                value_list.ForEach(p => search_videoCollection.Add(p));
                content_gridView.ItemsSource = search_videoCollection;
            }
            else
            {
                content_gridView.ItemsSource = use_video;
            }
            IsVideoListShow = true;
            view_grid.Visibility = Visibility.Visible;
        }

        private StorageFile cutScreenFile;
        private async void ScreenShot_itemClick(object sender, RoutedEventArgs e)
        {
            cutScreenFile = await CutScreen.GetScreen(main_video, main_mediaElement);
            curScreen_textBlock.Text = $"{saveSucceed_str}  {cutScreenFile.Path}";
            await cutScreen_contentDialog.ShowAsync();
        }

        private void CloseDialog_ButtonClick(object sender, RoutedEventArgs e)
        {
            addFolder_contentDialog.Hide();
        }

        private void None_itemClick(object sender, RoutedEventArgs e)
        {
            main_mediaElement.Stretch = Stretch.None;
        }

        private void Uniform_itemClick(object sender, RoutedEventArgs e)
        {
            main_mediaElement.Stretch = Stretch.Uniform;
        }

        private void UniformToFill_itemClick(object sender, RoutedEventArgs e)
        {
            main_mediaElement.Stretch = Stretch.UniformToFill;
        }

        private void Fill_itemClick(object sender, RoutedEventArgs e)
        {
            main_mediaElement.Stretch = Stretch.Fill;
        }

        private void CloseCutDialog_buttonClick(object sender, RoutedEventArgs e)
        {
            cutScreen_contentDialog.Hide();
        }

        private async void EditPicture_buttonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                bool result = await Windows.System.Launcher.LaunchFileAsync(cutScreenFile);
            }
            catch
            {
                curScreen_textBlock.Text = noPicture_str;
            }
        }

        private async void OpenCutAdress_buttonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                bool result = await Windows.System.Launcher.LaunchFolderAsync(await KnownFolders.PicturesLibrary.GetFolderAsync("M-Player"));
            }
            catch
            {
                curScreen_textBlock.Text = noPicturesFolder_str;
            }

        }

        private void Settings_buttonClick(object sender, RoutedEventArgs e)
        {
            main_mediaElement.AutoPlay = false;
            progress_tmr.Stop();
            tmr.Stop();//解决偶尔出现设置界面指针消失异常
            ShowCursor();
            Frame.Navigate(typeof(SecondPage));
        }

        private void Replay_menuItem_Click(object sender, RoutedEventArgs e)
        {
            if (content_progressRing.IsActive)
            {
                SetWaitContentDialog();
            }
            else
            {
                progress_tmr.Stop();
                #region 右键选项点击播放
                GetCurrentTheme();
                main_mediaElement.AutoPlay = true;
                main_video = (Video)sender_value.DataContext;
                foreach (var progress in progresslist.progress_list)
                {
                    if (progress.Path == main_video.Video_Path)
                    {
                        progress.Value = 0;
                    }
                }
                SaveProgressVM.SaveData(progresslist.progress_list, filePath);

                SetSelectedVideoSameCode();
                #endregion
            }
        }

        private async void OpenTheFolder_button_Click(object sender, RoutedEventArgs e)
        {
            sender_value = (FrameworkElement)sender;
            var remove_library = (VideoLibrary)sender_value.DataContext;
            foreach (var folder in myVideos.Folders)
            {
                if (folder.Path == remove_library.FolderPath)
                {
                    bool result = await Windows.System.Launcher.LaunchFolderAsync(folder);
                }
            }
        }
    }
}
