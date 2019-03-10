using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoPlayer.Modle
{
    class PlayingTime
    {
        private static string list_hh;
        private static string list_mm;
        private static string list_ss;
        private static string current_hh;
        private static string current_mm;
        private static string current_ss;
        private static string showTime_str;
        public static string GetPlayTime(int allTime,int currentTime)
        {
            int HH = allTime / 3600;
            int MM = (allTime - HH * 3600) / 60;
            int SS = allTime % 60;
            int current_HH = currentTime / 3600;
            int current_MM = (currentTime - current_HH * 3600) / 60;
            int current_SS = currentTime % 60;
            #region 总时间
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
            #endregion
            #region 实时时间
            if (current_HH < 10)
            {
                current_hh = "0" + current_HH.ToString();
            }
            else
            {
                current_hh = current_HH.ToString();
            }
            if (current_MM < 10)
            {
                current_mm = "0" + current_MM.ToString();
            }
            else
            {
                current_mm = current_MM.ToString();
            }
            if (current_SS < 10)
            {
                current_ss = "0" + current_SS.ToString();
            }
            else
            {
                current_ss = current_SS.ToString();
            }
            #endregion
            showTime_str = $"{current_hh}:{current_mm}:{current_ss} / {list_hh}:{list_mm}:{list_ss}";
            return showTime_str;
        }
    }
}
