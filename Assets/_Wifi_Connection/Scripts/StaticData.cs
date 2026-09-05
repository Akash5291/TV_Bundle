using System.CodeDom;
using UnityEngine;

public class StaticData
{
    [Header("Bundle Game Ids")]
    public const string Ludo = "nbtest_ludo_28";
    public const string NinjaKnife = "nbtest_nk_16";
    public const string RobotRun = "nbtest_rr_15";
    public const string BoatRushMadness = "nbtest_bm_5";
    public const string HillRacer = "nbtest_hr_6";

    [Header("API data file")]
    public const string bundleData_URL = "tv_bundle_games/bundle_game_list";
    public const string getQRCode_URL = "matchuser_id.php";
    public const string setServerIPURL = "multipleserver_ip_insert.php";
    public const string getUserProfile_URL = "getdetails.php";

    [Header("General Game screen / state")]
    public const string Home = "Home";
    public const string Category = "Categories";
    public const string SelectionScreen = "Selection";
    public const string Story = "Story";
    public const string ShopScreen = "Shop";
    public const string LevelScreen = "Level";
    public const string GameArea = "GameScene";
    public const string GameQuizWheel = "GameQuizWheel";
    public const string GamePause = "GamePause";
    public const string GameOver = "GameOver";
    public const string LevelFinish = "LevelFinish";
    public const string SpinWheel = "Wheel";
    public const string SpinWheelFinish = "WheelFinish";
    public const string LevelUp = "LevelUp";
    public const string Tutorial = "Tutorial";
    public const string DisconnectAll = "Disconnect";
    public const string LoadingScreen = "Loading";

    [Header("Browsing UI")]
    public const string CloseBtn = "close";
    public const string SelectBtn = "Select";
    public const string UpBtn = "Up";
    public const string DownBtn = "Down";
    public const string NextBtn = "Next";
    public const string PreviousBtn = "Previous";

    [Header("Game UI")]
    public const string TapBtn = "Tap";// when touch on screen
    public const string LongTap = "LongTap";
    public const string PauseBtn = "Pause";
    public const string JumpBtn = "Jump";
    public const string TopBtn = "Top";
    public const string BottomBtn = "Bottom";
    public const string LeftBtn = "Left";
    public const string RightBtn = "Right";
    public const string ShootBtn = "Shoot";
    public const string JoystickBtn = "Joystick";
}
