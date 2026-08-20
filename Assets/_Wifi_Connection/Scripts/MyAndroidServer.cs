using System.Collections.Generic;
using UnityEngine;

public class MyAndroidServer : MonoBehaviour
{
    [SerializeField] UnityTCPServer _tcpServer;
    public List<ConnectedPlayer> ConnectedIPs = new List<ConnectedPlayer>();

    // Use this for initialization
    void Start()
    {
        // UI objects:
        _tcpServer._localIP = _tcpServer.GetIP(false);// false = ipv4 and true = ipv6
        //Debug.Log("My IP: " + _tcpServer._localIP);
    }

    public void maxConnectionAllow(int n)
    {
        _tcpServer._maxConnections = n;
    }

    public string GetMyIP()
    {
        return _tcpServer._localIP;
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

    public void updateNewConnectionDeviceID(string ip, string deviceID)
    {
        for (int i = 0; i < ConnectedIPs.Count; i++)
        {
            if (ConnectedIPs[i].ip.Equals(ip))
            {
                ConnectedIPs[i].uniqueID = deviceID;
                Debug.Log("Device id: " + deviceID + ", updated to: " + ip);
                MyController.Instance.sendMessage("PlayerID", ConnectedIPs[i].id.ToString() + ConnectedIPs[i].uniqueID);
                break;
            }
        }
    }

    void addNewConnection(string ip)
    {
        if (!isAlreadyConnected(ip))
        {
            for (int i = 0; i < ConnectedIPs.Count; i++)
            {
                if (string.IsNullOrEmpty(ConnectedIPs[i].ip))
                {
                    ConnectedIPs.RemoveAt(i);
                }
            }
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
        _tcpServer.Setup();
        // Setup forces the disconnection:
        WifiManager.Instance.isClientConnected = false;
    }
    // Connect and start the server:
    public void Connect()
    {
        Setup();
        Debug.Log("tcp connect");
        _tcpServer.Connect();
    }
    // Diconnect the server:
    public void Disconnect()
    {
        Debug.Log("OnDisconnect");
        _tcpServer.Disconnect();

        if (WifiManager.Instance.isClientConnected)
        {
            ConnectedIPs.Clear();// remove all connection ip list
            WifiManager.Instance.isReady = 0;
        }
        serverClose();
    }

    // Send:
    public void Send(string msg)
    {
        Debug.Log("send: " + msg);
        _tcpServer.Distribute(msg);
    }

    // Events assigned in editor to UnityTCPServer (Server events):
    public void OnTCPSOpen(UnityTCPServer server)
    {
        Debug.Log("OnTCPSOpen");
        //if (!WifiManager.Instance.isClientConnected)
        //    WifiManager.Instance.onClientConnected();
    }
    public void OnTCPSNewConnection(TCPConnection connection, UnityTCPServer server)
    {
        Debug.Log("New Connection: " + _tcpServer.GetConnectionsCount().ToString()+", ip: "+connection.GetRemoteIP());
        addNewConnection(connection.GetRemoteIP());

        if (!WifiManager.Instance.isClientConnected)
            WifiManager.Instance.onClientConnected();
    }
    public void OnTCPSError(int code, string message, UnityTCPServer server)
    {
        Debug.Log("OnTCPSError: " + message);
    }
    public void OnTCPSClose(UnityTCPServer server)
    {
        Debug.Log("OnTCP Server Close");
    }

    // Events assigned in editor to UnityTCPServer (Connection events):
    public void OnTCPMessage(byte[] message, TCPConnection connection)
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
    public void OnTCPError(int code, string message, TCPConnection connection)
    {
        Debug.Log("OnTCPError: " + code + ", " + message);
    }
    public void OnTCPClose(TCPConnection connection)
    {
        Debug.Log("OnTCPClose: " + _tcpServer.GetConnectionsCount().ToString() + ", ip: " + connection.GetRemoteIP());

        if (_tcpServer.GetConnectionsCount() != 0 && _tcpServer._maxConnections > 1)
        {
            if (string.IsNullOrEmpty(connection.GetRemoteIP()))
            {
                for (int j = 0; j < ConnectedIPs.Count; j++)
                {
                    bool isGone = true;
                    for (int i = 0; i < _tcpServer.GetConnectionsCount(); i++)
                    {
                        if (ConnectedIPs[j].ip.Equals(_tcpServer.GetConnection(i).GetRemoteIP()))
                        { isGone = false; break; }
                    }
                    if (isGone)
                    { removeConnection(ConnectedIPs[j].ip); break; }
                }
            }
            else
                removeConnection(connection.GetRemoteIP());
        }
        else
        {
            ConnectedIPs.Clear();// remove all connection ip list
            WifiManager.Instance.isReady = 0;
        }

        /*if (MyController.Instance.isCloseBtnPress)
        {
            MyController.Instance.sendMessage(StaticData.DisconnectAll, StaticData.DisconnectAll);
            //Application.Quit();Akash
            //System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
        else if (WifiManager.Instance.isClientConnected)
            WifiManager.Instance.onClientDisconnected();*/
        serverClose();
    }

    void serverClose()
    {
        if (MyController.Instance.isCloseBtnPress)
        {
            MyController.Instance.sendMessage(StaticData.DisconnectAll, StaticData.DisconnectAll);
        }
        WifiManager.Instance.onClientDisconnected();
    }
}

