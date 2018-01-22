using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Text;

public class ChatLogBehavior : MonoBehaviour {

    public ClientBehavior client;
	Text textUI;
	public int maxLines = 10;
	int numLines = 0;

	void Awake() {
		textUI = GetComponent<Text>();
	}
    private void Start()
    {
        client = ClientBehavior.Instance;
    }

    void OnEnable () {
		textUI.text = "";
		numLines = 0;
	}
	
	public void OnReceiveMessage(string s) {
		textUI.text += s + "\n";
		
		numLines++;
		if (numLines > maxLines) {
			// remove older lines lines
			string t = textUI.text;
			t = t.Substring(t.IndexOf('\n') + 1);
			textUI.text = t;
			numLines--;
		}
	}
}
