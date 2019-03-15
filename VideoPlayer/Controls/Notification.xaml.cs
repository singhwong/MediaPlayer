using System;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace VideoPlayer.Controls
{
    public  sealed partial class NotifyShow : UserControl
    {
        private Popup popup;
        private string showContent_str;
        private TimeSpan showTime_tmr;
        private NotifyShow()
        {
            this.InitializeComponent();
            popup = new Popup();
            MeasurePopupSize();
            popup.Child = this;
            this.Loaded += NotifyPopup_Loaded;
            this.Unloaded += NotifyPopup_Unloaded;
        }

        public NotifyShow(string content, TimeSpan showTime) : this()
        {
            this.showContent_str = content;
            this.showTime_tmr = showTime;
        }

        public NotifyShow(string content) : this(content, TimeSpan.FromSeconds(2))
        {
        }

        public void Show()
        {
            this.popup.IsOpen = true;
        }

        public void Hide()
        {
            this.popup.IsOpen = false;
        }
        private void MeasurePopupSize()
        {
            this.Width = ApplicationView.GetForCurrentView().VisibleBounds.Width;

            double marginTop = 0;
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                marginTop = StatusBar.GetForCurrentView().OccludedRect.Height;
            this.Height = ApplicationView.GetForCurrentView().VisibleBounds.Height;
            this.Margin = new Thickness(0, marginTop, 0, 0);
        }

        private void NotifyPopup_Loaded(object sender, RoutedEventArgs e)
        {
            this.showContent_textBlock.Text = showContent_str;
            this.story_Board.BeginTime = this.showTime_tmr;
            this.story_Board.Begin();
            this.story_Board.Completed += storyBoard_Completed;
            ApplicationView.GetForCurrentView().VisibleBoundsChanged += NotifyPopup_VisibleBoundsChanged;
        }

        private void NotifyPopup_VisibleBoundsChanged(ApplicationView sender, object args)
        {
            MeasurePopupSize();
        }

        private void storyBoard_Completed(object sender, object e)
        {
            this.popup.IsOpen = false;
        }

        private void NotifyPopup_Unloaded(object sender, RoutedEventArgs e)
        {
            ApplicationView.GetForCurrentView().VisibleBoundsChanged -= NotifyPopup_VisibleBoundsChanged;
        }
    }
}
