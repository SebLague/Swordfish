using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Muter : MonoBehaviour {
    //AudioListener listener;

    string target = "mute";
    string input;
    bool mute;

    AudioSource[] s;
    float[] vols;
	void Start () {
        s = FindObjectsOfType<AudioSource>();
        vols = s.Select(x => x.volume).ToArray();
       // listener = FindObjectOfType<AudioListener>();
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
        mute = !mute;
        if (mute)
        {
            foreach (AudioSource source in s)
            {
                source.volume = 0;
            }
        }
        else
        {
            for (int i = 0; i < s.Length; i++)
            {
                s[i].volume = vols[i];
            }
        }
    }
}
