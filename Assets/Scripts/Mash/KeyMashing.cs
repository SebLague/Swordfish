﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;

public class KeyMashing : Task {

    public TextMesh progressText;
    public Transform progressBar;
    int totalNum = 512;
    int num;
    bool done;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!done)
        {
            if (Input.anyKeyDown)
            {
                num += Input.inputString.Length;
                num = Mathf.Clamp(num, 0, totalNum);
            }

            progressText.text = "Brute-forcing " + num + "/" + totalNum;
            float percent = Mathf.Clamp01(num / (float)totalNum);
            progressBar.localScale = new Vector3(percent, 1, 1);

            if (num == totalNum)
            {
                done = true;
                TaskCompleted();
            }
        }
	}
}
