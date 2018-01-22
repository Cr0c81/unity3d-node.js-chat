using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Text;

public class ChatInputBehavior : MonoBehaviour {

	public ClientBehavior client;
	public string alias { get; set; }
	InputField inField;
    public Text text_chat;

	void Start () {
		inField = GetComponent<InputField>();
        client = ClientBehavior.Instance;
	}
	
	// Note: End Edit event is called when losing focus (mouse clicks outside input field)
	public void OnEndEdit(string s) {
		if (s.Length < 1 || client.clientID < 0) // ignore empty strings
			return;

        JSONObject j = JSONObject.obj;
        j.Add("MESSAGE");
        j.Add(alias);
        j.Add(client.clientID.ToString());
        j.Add(s);
        string ss = j.ToString();
		if (client.Send(j.ToString()))
			inField.text = "";
			
		// by default, Unity releases focus on the inputfield after pressing enter
		inField.ActivateInputField(); // keep the focus
	}
}
