using System;
using System.Collections.Generic;

public class SerializableClasses
{
    //https://playwifigames.com/tv_bundle_games/bundle_game_list
    #region Bundle Game Data
    [Serializable]
    public class BundleGameData
    {
        public string title;
        public string description;
        public string image_url;
        public string download_link;
    }

    [Serializable]
    public class BundleGameList
    {
        public BundleGameData feature_game;
        public List<BundleGameData> new_arrival = new List<BundleGameData>();
        public List<BundleGameData> our_games = new List<BundleGameData>();
    }
    #endregion

    [Serializable]
    public class InsertData
    {
        public string user_id;
        public string server_ip_1;
        public string server_ip_2;
        public string server_ip;
        public string gamecode;
    }

    [Serializable]
    public class UpdateData
    {
        public string user_id;
        public string gamecode;
    }

    [Serializable]
    public class GameCode
    {
        public string user_id;
        public string server_ip;
    }

    public class CommonData
    {
        public string device_id;
        public string game_id;
    }

    [Serializable]
    public class GetGameProfile
    {
        public string score;
        public string max_score;
        public string rating;
        public string avg_rating;
        public string active_users;
    }

    [Serializable]
    public class TVURL
    {
        public string base_url;
    }


    [Serializable]
    public class InhouseAds
    {
        public List<AdsItemData> ourAds = new List<AdsItemData>();
    }

    [Serializable]
    public class AdsItemData
    {
        public int index;
        public int priority;
        public string game_icon;
        public string game_name;
        public string game_bundle_name;
        public string download_url;
        public string preview_video_url;
    }
}
