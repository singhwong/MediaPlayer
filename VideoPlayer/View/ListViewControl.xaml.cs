using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VideoPlayer.Modles;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace VideoPlayer
{
    public sealed partial class ListViewControl : UserControl
    {
        private DispatcherTimer tmr = new DispatcherTimer();
        public Video this_video { get { return this.DataContext as Video; } }
        public ListViewControl()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) => Bindings.Update();
            //main_storyBoard.Begin();

            DispatcherTimer tmr = new DispatcherTimer();
            tmr.Interval = TimeSpan.FromSeconds(0.1);
            tmr.Tick += OnTimerTick;
            tmr.Start();


        }

        private void OnTimerTick(object sender, object args)
        {
            videoList_grid.BorderBrush = duration_textblock.Foreground;
            try
            {
                video_progressBar.Value = this_video.History_progress;
                gradient_stop1.Color = this_video.Video_Color;
                tmr.Stop();
            }
            catch
            {
            }
            //if (this_video != null)
            //{
                //video_textblock.Foreground = this_video.ForeGround;
                //videoList_grid.BorderBrush = this_video.ForeGround;
               
            //}
        }

    }
}
