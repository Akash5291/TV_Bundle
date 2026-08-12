using System.Collections.Generic;
using UnityEngine;

public class MyWebServer : MonoBehaviour
{
    [SerializeField] UnityTCPServer _tcpServer;
    [SerializeField] UnityWSServer _wsServer;
    public List<ConnectedPlayer> ConnectedIPs = new List<ConnectedPlayer>();
    [SerializeField] string ip = "";

    // Use this for initialization
    void Start()
    {
        ip = _wsServer.GetIP(false);
        // UI objects:
        _wsServer._serverURL = "ws://" + ip + ":" + _wsServer._port + "/ws/";//ws://192.168.0.103:60003/ws/
        //Setup();// _tcpServer.GetIP();
        //_tcpServer.transform.gameObject.SetActive(false);
    }

    public string GetMyIP()
    {
        return ip;
    }

    bool isAlreadyConnected(string ip)
    {
        bool value = false;
        for (int i = 0; i < ConnectedIPs.Count; i++)
        {
            if (ConnectedIPs[i].ip.Equals(ip))
            {
                value = true; break;
            }
        }
        return value;
    }

    void addNewConnection(string ip)
    {
        if (!isAlreadyConnected(ip))
        {
            Debug.Log("addNewConnection: " + ip);
            ConnectedPlayer p = new ConnectedPlayer();
            p.id = ConnectedIPs.Count + 1;
            p.ip = ip;
            ConnectedIPs.Add(p);
            MyController.Instance.sendMessage("PlayerID", p.id.ToString() + p.ip);
        }
    }

    void removeConnection(string ip)
    {
        Debug.Log("removeConnection: " + ip);
        WifiManager.Instance.isReady--;
        for (int i = 0; i < ConnectedIPs.Count; i++)
        {
            if (ConnectedIPs[i].ip.Equals(ip))
            {
                ConnectedIPs.RemoveAt(i); break;
            }
        }
    }

    // Set server local bound settings:
    public void Setup()
    {
        _wsServer.Setup();
        // Setup forces the disconnection:
        WifiManager.Instance.isClientConnected = false;
    }
    // Connect and start the server:
    public void Connect()
    {
        Debug.Log("Connect");
        Setup();
        _wsServer.Connect();
    }
    // Disconnect the server:
    public void Disconnect()
    {
        Debug.Log("Disconnect");
        _wsServer.Disconnect();
        if (WifiManager.Instance.isClientConnected)
            WifiManager.Instance.onClientDisconnected();
    }

    // Send:
    public void Send(string msg)
    {
        Debug.Log("send: " + msg);
        _wsServer.Distribute(msg);
    }

    // Events assigned in editor to UnityWSServer (Server events):
    public void OnWSSOpen(UnityWSServer server)
    {
        Debug.Log("OnWSSOpen: "+ server.GetURL());
        //if (APIManager.Instance.GameServerType == 3)
        //    GameManager.Instance.setStates(StaticData.PairingScreen);
    }
    public void OnWSSNewConnection(WSConnection connection, UnityWSServer server)
    {
        string connectIP = connection.GetDefaultIPAddress("ipv4");
        if(string.IsNullOrEmpty(connectIP))
            connectIP = connection.GetDefaultIPAddress("ipv6");

        Debug.Log("New Connection: " + _wsServer.GetConnectionsCount().ToString() + ", ip: " + connectIP);
        addNewConnection(connectIP);

        if (!WifiManager.Instance.isClientConnected)
            WifiManager.Instance.onClientConnected();
    }
    public void OnWSSError(int code, string message, UnityWSServer server)
    {
        Debug.Log("OnWSSError: " + message);
    }
    public void OnWSSClose(UnityWSServer server)
    {
        Debug.Log("OnWSSClose");
    }

    // Events assigned in editor to UnityWSServer (Connection events):
    public void OnWSMessage(byte[] message, WSConnection connection)
    {
        // Get the content up to char 35 (#):
        int msgLen = 0;
        for (int i = 0; i < message.Length; i++)
        {
            if (message[i] == '#')
            {
                msgLen = i;         // '#' is excluded.
                break;
            }
        }
        if (msgLen > 0)
        {
            // Stress test protocol:
            byte[] msg = new byte[msgLen];
            System.Buffer.BlockCopy(message, 0, msg, 0, msgLen);
            string[] fields = connection.ByteArrayToString(msg).Split(';');
            switch (fields[0])
            {
                case "PING":
                    // Send the PONG message back to remoteIP:
                    string pong = "PONG;" + fields[1] + ";" + fields[2] + ";" + NTP_RealTime.GetUTCTime().TimeOfDay.TotalMilliseconds + "#";
                    connection.SendData(pong);
                    break;
            }
        }
        else
        {
            // Shows received messages on top of the screen and disappears automatically after 10 seconds:
            MyController.Instance.myMessageReceived(connection.ByteArrayToString(message));
        }
    }
    public void OnWSError(int code, string message, WSConnection connection)
    {
        Debug.Log("OnWSError: " + code + ", " + message);
    }
    public void OnWSClose(WSConnection connection)
    {
        string connectIP = connection.GetDefaultIPAddress("ipv4");
        if (string.IsNullOrEmpty(connectIP))
            connectIP = connection.GetDefaultIPAddress("ipv6");

        Debug.Log("OnTCPClose: " + _wsServer.GetConnectionsCount().ToString() + ", ip: " + connectIP);

        if (_wsServer.GetConnectionsCount() != 0)
        {
            if (string.IsNullOrEmpty(connectIP))
            {
                for (int j = 0; j < ConnectedIPs.Count; j++)
                {
                    bool isGone = true;
                    for (int i = 0; i < _wsServer.GetConnectionsCount(); i++)
                    {
                        //if (ConnectedIPs[j].ip.Equals(_wsServer.GetConnection(i).GetRemoteIP()))
                        //{ isGone = false; break; }
                    }
                    if (isGone)
                    { removeConnection(ConnectedIPs[j].ip); break; }
                }
            }
            else
                removeConnection(connectIP);
        }
        else
        {
            ConnectedIPs.Clear();// remove all connection ip list
        }

        if (WifiManager.Instance.isClientConnected)
            WifiManager.Instance.onClientDisconnected();
    }
}
