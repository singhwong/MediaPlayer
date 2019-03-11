using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.MediaProperties;
using Windows.Media.Transcoding;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace VideoPlayer.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ConversionPage : Page
    {
        //MainPage rootPage = MainPage.Current;
        private ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();
        CoreDispatcher _dispatcher = Window.Current.Dispatcher;
        CancellationTokenSource _cts;
        //string _OutputFileName = "TranscodeSampleOutput";
        MediaEncodingProfile _Profile;
        StorageFile _InputFile = null;
        StorageFile _OutputFile = null;
        MediaTranscoder _Transcoder = new MediaTranscoder();
        string _OutputType = "MP4";
        string _OutputFileExtension = ".mp4";

        public ConversionPage()
        {
            this.InitializeComponent();

            //if (MainPage.page.RequestedTheme == ElementTheme.Light)
            //{
            //    this.RequestedTheme = ElementTheme.Light;
            //}
            //else
            //{
            //    this.RequestedTheme = ElementTheme.Dark;
            //}

            _cts = new CancellationTokenSource();

            // Hook up UI
            PickFileButton.Click += new RoutedEventHandler(PickFile);
            SetOutputButton.Click += new RoutedEventHandler(PickOutput);
            TargetFormat.SelectionChanged += new SelectionChangedEventHandler(OnTargetFormatChanged);
            Transcode.Click += new RoutedEventHandler(TranscodePreset);
            Cancel.Click += new RoutedEventHandler(TranscodeCancel);
            //input_button.Click += new RoutedEventHandler(InputPathClick);
            //OutputPath.Click += new RoutedEventHandler(OutputPathClick);

            // Media Controls
            //InputPlayButton.Click += new RoutedEventHandler(InputPlayButton_Click);
            //InputPauseButton.Click += new RoutedEventHandler(InputPauseButton_Click);
            //InputStopButton.Click += new RoutedEventHandler(InputStopButton_Click);
            //OutputPlayButton.Click += new RoutedEventHandler(OutputPlayButton_Click);
            //OutputPauseButton.Click += new RoutedEventHandler(OutputPauseButton_Click);
            //OutputStopButton.Click += new RoutedEventHandler(OutputStopButton_Click);

            // File is not selected, disable all buttons but PickFileButton
            DisableButtons();
            SetPickFileButton(true);
            //SetOutputFileButton(true);
            SetCancelButton(false);
        }
        public void Dispose()
        {
            _cts.Dispose();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        async void TranscodePreset(Object sender, RoutedEventArgs e)
        {
            StopPlayers();
            DisableButtons();
            GetPresetProfile(ProfileSelect);

            // Clear messages
            StatusMessage.Text = "";
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                OutputPath.Text = resourceLoader.GetString("output_str");
            });

            try
            {
                if ((_InputFile != null) && (_OutputFile != null))
                {                 
                    if (EnableMrfCrf444.IsChecked.HasValue && (bool)EnableMrfCrf444.IsChecked)
                    {
                        _Transcoder.VideoProcessingAlgorithm = MediaVideoProcessingAlgorithm.MrfCrf444;
                    }
                    else
                    {
                        _Transcoder.VideoProcessingAlgorithm = MediaVideoProcessingAlgorithm.Default;
                    }
                    var preparedTranscodeResult = await _Transcoder.PrepareFileTranscodeAsync(_InputFile, _OutputFile, _Profile);
                    if (preparedTranscodeResult.CanTranscode)
                    {
                        SetCancelButton(true);
                        var progress = new Progress<double>(TranscodeProgress);
                        await preparedTranscodeResult.TranscodeAsync().AsTask(_cts.Token, progress);
                        TranscodeComplete();
                    }
                    else
                    {
                        TranscodeFailure(preparedTranscodeResult.FailureReason);
                    }
                }
            }
            catch
            {
                OutputText("");
                TranscodeError(resourceLoader.GetString("cancel_str"));
            }
            //catch (Exception exception)
            //{
            //    TranscodeError(exception.Message);
            //}
        }

        void GetPresetProfile(ComboBox combobox)
        {
            _Profile = null;
            VideoEncodingQuality videoEncodingProfile = VideoEncodingQuality.Wvga;
            switch (combobox.SelectedIndex)
            {
                case 0:
                    videoEncodingProfile = VideoEncodingQuality.HD1080p;
                    break;
                case 1:
                    videoEncodingProfile = VideoEncodingQuality.HD720p;
                    break;
                case 2:
                    videoEncodingProfile = VideoEncodingQuality.Wvga;
                    break;
                case 3:
                    videoEncodingProfile = VideoEncodingQuality.Ntsc;
                    break;
                case 4:
                    videoEncodingProfile = VideoEncodingQuality.Pal;
                    break;
                case 5:
                    videoEncodingProfile = VideoEncodingQuality.Vga;
                    break;
                case 6:
                    videoEncodingProfile = VideoEncodingQuality.Qvga;
                    break;
            }

            switch (_OutputType)
            {
                case "AVI":
                    _Profile = MediaEncodingProfile.CreateAvi(videoEncodingProfile);
                    break;
                case "WMV":
                    _Profile = MediaEncodingProfile.CreateWmv(videoEncodingProfile);
                    break;
                default:
                    _Profile = MediaEncodingProfile.CreateMp4(videoEncodingProfile);
                    break;
            }

            /*
            For transcoding to audio profiles, create the encoding profile using one of these APIs:
                MediaEncodingProfile.CreateMp3(audioEncodingProfile)
                MediaEncodingProfile.CreateM4a(audioEncodingProfile)
                MediaEncodingProfile.CreateWma(audioEncodingProfile)
                MediaEncodingProfile.CreateWav(audioEncodingProfile)

            where audioEncodingProfile is one of these presets:
                AudioEncodingQuality.High
                AudioEncodingQuality.Medium
                AudioEncodingQuality.Low
            */
        }

        void TranscodeProgress(double percent)
        {
            OutputText(resourceLoader.GetString("progress_str") + string.Format("{0:F}",percent) + "%");
            progress_progressBar.Visibility = Visibility.Visible;
            progress_progressBar.Value = percent;
        }

        async void TranscodeComplete()
        {
            OutputText(resourceLoader.GetString("complete_str"));
            progress_progressBar.Value = 0;
            progress_progressBar.Visibility = Visibility.Collapsed;
            OutputPathText(_OutputFile.Path);
            IRandomAccessStream stream = await _OutputFile.OpenAsync(FileAccessMode.Read);
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                //OutputVideo.SetSource(stream, _OutputFile.ContentType);
            });

            EnableButtons();
            SetCancelButton(false);
        }

        async void TranscodeCancel(object sender, RoutedEventArgs e)
        {
            try
            {
                progress_progressBar.Visibility = Visibility.Collapsed;
                _cts.Cancel();
                _cts.Dispose();
                _cts = new CancellationTokenSource();

                if (_OutputFile != null)
                {
                    await _OutputFile.DeleteAsync();
                }
            }
            catch
            {
                //TranscodeError("取消转换！");
            }
        }

        async void TranscodeFailure(TranscodeFailureReason reason)
        {
            try
            {
                if (_OutputFile != null)
                {
                    await _OutputFile.DeleteAsync();
                }
            }
            catch
            {
                //TranscodeError(exception.Message);
            }

            switch (reason)
            {
                case TranscodeFailureReason.CodecNotFound:
                    TranscodeError(resourceLoader.GetString("CodecNotFound_str"));
                    break;
                case TranscodeFailureReason.InvalidProfile:
                    TranscodeError(resourceLoader.GetString("InvalidProfile_str"));
                    break;
                default:
                    TranscodeError(resourceLoader.GetString("unknowError_str"));
                    break;
            }
        }

        private StorageFile file;
        async void PickFile(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.VideosLibrary;
            picker.FileTypeFilter.Add(".wmv");
            picker.FileTypeFilter.Add(".mp4");
            picker.FileTypeFilter.Add(".mkv");
            picker.FileTypeFilter.Add(".avi");
            picker.FileTypeFilter.Add(".3gp");
            //picker.FileTypeFilter.Add(".rmvb");
            picker.FileTypeFilter.Add(".mov");
            picker.FileTypeFilter.Add(".ogg");
            picker.FileTypeFilter.Add(".flv");
            picker.FileTypeFilter.Add(".webm");
            picker.FileTypeFilter.Add(".mpg");
            picker.FileTypeFilter.Add(".vob");

            file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);

                _InputFile = file;
                //InputVideo.SetSource(stream, file.ContentType);
                //InputVideo.Play();

                // Enable buttons
                EnableButtons();

                InputPathText(_InputFile.Path);
            }
        }

        async void PickOutput(object sender, RoutedEventArgs e)
        {
            FileSavePicker picker = new FileSavePicker();
            picker.SuggestedStartLocation = PickerLocationId.VideosLibrary;
            picker.SuggestedFileName = file.DisplayName;
            picker.FileTypeChoices.Add(_OutputType, new System.Collections.Generic.List<string>() { _OutputFileExtension });

            _OutputFile = await picker.PickSaveFileAsync();

            if (_OutputFile != null)
            {
                SetTranscodeButton(true);
            }
        }

        void OnTargetFormatChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (TargetFormat.SelectedIndex)
            {
                case 1:
                    _OutputType = "WMV";
                    _OutputFileExtension = ".wmv";
                    EnableNonSquarePARProfiles();
                    break;
                case 2:
                    _OutputType = "AVI";
                    _OutputFileExtension = ".avi";

                    // Disable NTSC and PAL profiles as non-square pixel aspect ratios are not supported by AVI
                    DisableNonSquarePARProfiles();
                    break;
                default:
                    _OutputType = "MP4";
                    _OutputFileExtension = ".mp4";
                    EnableNonSquarePARProfiles();
                    break;
            }
        }

        //void InputPlayButton_Click(Object sender, RoutedEventArgs e)
        //{
        //    if (InputVideo.DefaultPlaybackRate == 0)
        //    {
        //        InputVideo.DefaultPlaybackRate = 1.0;
        //        InputVideo.PlaybackRate = 1.0;
        //    }

        //    InputVideo.Play();
        //}

        void InputStopButton_Click(Object sender, RoutedEventArgs e)
        {
            //InputVideo.Stop();
        }

        //void InputPauseButton_Click(Object sender, RoutedEventArgs e)
        //{
        //    InputVideo.Pause();
        //}

        //void OutputPlayButton_Click(Object sender, RoutedEventArgs e)
        //{
        //    if (OutputVideo.DefaultPlaybackRate == 0)
        //    {
        //        OutputVideo.DefaultPlaybackRate = 1.0;
        //        OutputVideo.PlaybackRate = 1.0;
        //    }

        //    OutputVideo.Play();
        //}

        //void OutputStopButton_Click(Object sender, RoutedEventArgs e)
        //{
        //    OutputVideo.Stop();
        //}

        //void OutputPauseButton_Click(Object sender, RoutedEventArgs e)
        //{
        //    OutputVideo.Pause();
        //}

        async void SetPickFileButton(bool isEnabled)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                PickFileButton.IsEnabled = isEnabled;
            });
        }

        async void SetOutputFileButton(bool isEnabled)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                SetOutputButton.IsEnabled = isEnabled;
            });
        }

        async void SetTranscodeButton(bool isEnabled)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Transcode.IsEnabled = isEnabled;
            });
        }

        async void SetCancelButton(bool isEnabled)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Cancel.IsEnabled = isEnabled;
            });
        }

        async void EnableButtons()
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                PickFileButton.IsEnabled = true;
                SetOutputButton.IsEnabled = true;
                TargetFormat.IsEnabled = true;
                ProfileSelect.IsEnabled = true;
                EnableMrfCrf444.IsEnabled = true;

                // The transcode button's initial state should be disabled until an output
                // file has been set.
                Transcode.IsEnabled = false;
            });
        }

        async void DisableButtons()
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ProfileSelect.IsEnabled = false;
                Transcode.IsEnabled = false;
                PickFileButton.IsEnabled = false;
                SetOutputButton.IsEnabled = false;
                TargetFormat.IsEnabled = false;
                EnableMrfCrf444.IsEnabled = false;
            });
        }

        void EnableNonSquarePARProfiles()
        {
            ComboBoxItem_NTSC.IsEnabled = true;
            ComboBoxItem_PAL.IsEnabled = true;
        }

        void DisableNonSquarePARProfiles()
        {
            ComboBoxItem_NTSC.IsEnabled = false;
            ComboBoxItem_PAL.IsEnabled = false;

            // Ensure a valid profile is set
            if ((ProfileSelect.SelectedIndex == 3) || (ProfileSelect.SelectedIndex == 4))
            {
                ProfileSelect.SelectedIndex = 2;
            }
        }

        async void StopPlayers()
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                //if (InputVideo.CurrentState != MediaElementState.Paused)
                //{
                //    InputVideo.Pause();
                //}
                //if (OutputVideo.CurrentState != MediaElementState.Paused)
                //{
                //    OutputVideo.Pause();
                //}
            });
        }

        async void PlayFile(StorageFile MediaFile)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                try
                {
                    IRandomAccessStream stream = await MediaFile.OpenAsync(FileAccessMode.Read);
                    //OutputVideo.SetSource(stream, MediaFile.ContentType);
                    //OutputVideo.Play();
                }
                catch
                {
                    //TranscodeError(exception.Message);
                }
            });
        }

        async void TranscodeError(string error)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                StatusMessage.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                StatusMessage.Text = error;
            });

            EnableButtons();
            SetCancelButton(false);
        }

        async void OutputText(string text)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                OutputMsg.Foreground = new SolidColorBrush(Windows.UI.Colors.SkyBlue);
                OutputMsg.Text = text;
            });
        }

        async void OutputPathText(string text)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                OutputPath.Text += text;
            });
        }

        async void InputPathText(string text)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                input_textBlock.Text += text;
            });
        }
        // void InputPathClick(object sender, RoutedEventArgs e)
        //{
        //    //if (File.Exists(_InputFile.Path))
        //    //{
        //        Process.Start("explorer.exe",_InputFile.Path);
        //    //}
        //}
        //private string path;
        //void OutputPathClick(object sender,RoutedEventArgs e)
        //{
        //    path = _OutputFile.Path.Replace($"{_OutputFile.Name}","");
        //    //OutputPathText(path);
        //    //Process.Start("explorer.exe", path);
        //    //if (File.Exists(_OutputFile.Path))
        //    //{
        //        Process.Start("explorer",path);
        //    //}
        //}
    }
}
