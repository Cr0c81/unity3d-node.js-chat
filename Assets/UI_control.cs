using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;

public class UI_control : MonoBehaviour {
    public UI_control Instance;
    private void Awake()
    {
        Instance = this;
    }
    private ClientBehavior client;
    public Text text_Nickname;
    public string nickname;
    [SerializeField]
    private ChatLogBehavior clb;

    private void Start()
    {
        client = ClientBehavior.Instance;
        client.OnConnectEvent += ConnectEvent;
        client.OnDisconnectEvent += DisconnectEvent;
        client.OnMessageEvent += OnMessage;
    }

    public GameObject[] goOn;
    public GameObject[] goOff;
    public void ConnectEvent()
    {
        foreach (GameObject go in goOn)
            go.SetActive(true);
        foreach (GameObject go in goOff)
            go.SetActive(false);

        nickname = text_Nickname.text;
        JSONObject j = JSONObject.obj;
        j.Add("SYSTEM");
        j.Add("NICKNAME");
        j.Add(nickname);
        //client.Send(j.ToString());
        //clb.OnReceiveMessage(j.ToString());
    }

    public void DisconnectEvent()
    {
        foreach (GameObject go in goOn)
            go.SetActive(false);
        foreach (GameObject go in goOff)
            go.SetActive(true);
    }
    public void ButtonConnect()
    {
        client.Connect();
    }
    public void OnMessage(string value)
    {
        JSONObject j = new JSONObject(value);
        /*
        string s1 = "";
        for (int i = 0; i < j.Count; i++)
            s1 += j[i].str + " :: ";
        s1 = s1.Substring(0, s1.Length - 4);
        clb.OnReceiveMessage(value);
        clb.OnReceiveMessage(s1);
        */
        if (j[0].str == "SYSTEM")
        {
            if (j[1].str == "ID")
            {
                client.clientID = Convert.ToInt32(j[2].str);
                j.Clear();
                j.Add("SYSTEM");
                j.Add("NICKNAME");
                j.Add(client.clientID.ToString());
                j.Add(nickname);
                client.Send(j.ToString());
            }
            if (j[1].str == "USERS")
                if (Convert.ToInt32(j[2].str) == client.clientID)
                {
                    clb.OnReceiveMessage(j[3].str);
                }
        }
        if (j[0].str == "MESSAGE")
        {
            string s = j[1].str + " : " + j[3].str;
            clb.OnReceiveMessage(s);
            print(s);
            j.Clear();
        }
    }

    public void ButtonList()
    {
        JSONObject j = new JSONObject();
        j.Add("SYSTEM");
        j.Add("LIST");
        j.Add(client.clientID.ToString());
        client.Send(j.ToString());
    }
}
