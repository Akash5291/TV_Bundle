using UnityEngine;
using UnityEngine.UI;

public class TCP_test : MonoBehaviour
{
    UnityTCPConnection _tcp;
    // Message PopUp (Set in editor):
    public GameObject popupPrefab;
    // Graphic UI objects:
    Text t_localIP;
    Image i_state;
    InputField if_port;
    InputField if_ip;
    InputField if_data;

    // Use this for initialization
    void Start ()
    {
        _tcp = GetComponent<UnityTCPConnection>();

        _tcp._localIP = "";
        _tcp._remoteIP = WifiManager.Instance.serverIP;
        _tcp._remotePort = 20000;
        /*
        // UI objects:
        t_localIP = transform.Find("PanelSetup").Find("LabelLocalIP").Find("Text").GetComponent<Text>();
        i_state = transform.Find("PanelSetup").Find("ImageState").GetComponent<Image>();
        if_port = transform.Find("PanelData").Find("InputFieldPort").GetComponent<InputField>();
        if_port.text = "60002";
        if_ip = transform.Find("PanelData").Find("InputFieldIp").GetComponent<InputField>();
        if_data = transform.Find("PanelData").Find("InputFieldData").GetComponent<InputField>();
        if_data.text = "TCP Message";
        // Put the local IP for testing:
        if_ip.text = _tcp.GetDefaultIPAddress();*/
    }

    // Set new connection settings:
    public void Setup()
    {
        _tcp._remoteIP = WifiManager.Instance.serverIP;
        _tcp._remotePort = 20000;
        _tcp.Setup();
        // Setup forces the disconnection:
        //i_state.color = Color.red;
    }
    // Connect and start:
    public void Connect()
    {
        Setup();
        _tcp.Connect();
    }
    // Close the connection:
    public void Disconnect()
    {
        _tcp.Disconnect();
        //i_state.color = Color.red;
        if (!WifiManager.Instance.isClientConnected)
            WifiManager.Instance.onClientDisconnected();
    }
    // Send:
    public void Send()
    {
        _tcp.SendData(if_data.text);
    }

    // Events assigned in editor to UnityTCPConnection:
    public void OnTCPOpen(UnityTCPConnection connection)
    {
        //i_state.color = Color.green;
        //t_localIP.text = _tcp.GetIP();
        if (!WifiManager.Instance.isClientConnected)
            WifiManager.Instance.onClientConnected();
    }
    public void OnTCPMessage(byte[] message, UnityTCPConnection connection)
    {
        // Shows received messages on top of the screen and disappears automatically after 10 seconds:
        string msg = connection.ByteArrayToString(message);
        // Filter "Stress test" protocol":
        if (!msg.StartsWith("PING") && !msg.StartsWith("PONG") && !msg.StartsWith("DATA"))
        {
            Debug.Log("Message received: " + msg);
            MyController.Instance.myMessageReceived(msg);
            //GameObject popup = Instantiate(popupPrefab);
            //popup.GetComponent<PopUp>().SetMessage("[TCP received] " + msg, transform, 10f);
        }
    }
    public void OnTCPError(int code, string message, UnityTCPConnection connection)
    {
        //APIManager.Instance.GetServerIP();
        //GameObject popup = Instantiate(popupPrefab);
        //popup.GetComponent<PopUp>().SetMessage("[TCP_test] Error (" + code.ToString() + "): " + message, transform, 10f);
    }
    public void OnTCPClose(UnityTCPConnection connection)
    {
        //i_state.color = Color.red;
        if (WifiManager.Instance.isClientConnected)
            WifiManager.Instance.onClientDisconnected();
    }
}
