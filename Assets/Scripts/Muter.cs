using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Muter : MonoBehaviour {
    AudioListener listener;

    string target = "mute";
    string input;
	
	void Start () {
        listener = FindObjectOfType<AudioListener>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.anyKeyDown)
        {
            input += Input.inputString.ToLower();
            int matchIndex = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == target[matchIndex])
                {
                    matchIndex++;
                    if (matchIndex == target.Length)
                    {
                        ToggleMute();
                        input = "";
                        break;
                    }
                }
                else
                {
                    matchIndex = 0;
                }
            }

            if (matchIndex == 0)
            {
                input = "";
            }
        }
	}

    void ToggleMute()
    {
        listener.enabled = !listener.enabled;
    }
}
