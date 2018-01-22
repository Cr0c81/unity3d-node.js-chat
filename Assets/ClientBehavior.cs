using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;
using System.Net.Sockets;
using UnityEngine.Events;

public class ClientBehavior : MonoBehaviour {

    public static ClientBehavior Instance;

	TcpClient tcpClient;
	NetworkStream serverStream;

	public string host;
	public int port;
    public int clientID = -1;

    public delegate void OnMessageDelegate(string value);

    public System.Action OnConnectEvent = delegate { };
    public System.Action OnDisconnectEvent = delegate { };
    public System.Action<string> OnMessageEvent = delegate { };

    public void Awake()
    {
        Instance = this;
    }

    public void Connect () {
		tcpClient = new TcpClient();
		try {
			tcpClient.Connect(host, port);
		} catch (SocketException e) {
			Debug.LogError(e.ErrorCode + ": " + e.Message);
			tcpClient = null; // FIXME should I dispose first?
			return;
		}
		serverStream = tcpClient.GetStream();
        OnConnectEvent.Invoke();
	}

	public bool Send(string msg) {
		if (tcpClient == null) // can't send if we're disconnected
			return false;

        byte[] outstream2 = Encoding.UTF8.GetBytes(msg);
        try
        {
			serverStream.Write(outstream2, 0, outstream2.Length);
		} catch (IOException e) {
			Debug.LogError(e.Message);
			Disconnect();
			return false;
		}
		return true;
	}
	
	public void Disconnect () {

        OnDisconnectEvent.Invoke();

        if (serverStream != null)
		    serverStream.Close();
		serverStream = null;
		tcpClient.Close ();
		tcpClient = null;
	}

	void Update () {
		if (tcpClient == null || serverStream == null || !serverStream.DataAvailable)
			return;

		// Assumption: max message size is 1024 characters
		byte [] msg = new byte[1024];
		int bytesRead = serverStream.Read(msg, 0, 1024);

        UTF8Encoding encoder2 = new UTF8Encoding();
        string message2 = encoder2.GetString(msg, 0, bytesRead);

        OnMessageEvent(message2);
	}

	void OnApplicationQuit () {
        Disconnect();
		if (tcpClient != null)
			Disconnect (); // just in case client exits before clicking disconnect button
	}
}