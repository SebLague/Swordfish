using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stan : MonoBehaviour {

    Animator anim;
    float noInputTime;
    float targetTypeBlend;
    public MeshRenderer screenOverlay;
    public Text dialogueUI;

    public bool debugAnims;
    public int debugAnimIndex;
    public int numFailAnims = 5;
    Queue<int> failAnimIndexQueue;

    void Start()
    {
        anim = GetComponent<Animator>();

        int[] failIndices = new int[numFailAnims];
        for (int i = 0; i < numFailAnims; i++)
        {
            failIndices[i] = i;
        }
        Utility.Shuffle(failIndices);
        failAnimIndexQueue = new Queue<int>(failIndices);
    }

    // Update is called once per frame
    void Update()
    {
        if (debugAnims && Application.isEditor)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
				anim.SetFloat("Index", debugAnimIndex);
                anim.SetTrigger("Fail");
            }
			if (Input.GetKeyDown(KeyCode.V))
			{
				anim.SetFloat("Index", debugAnimIndex);
				anim.SetTrigger("Victory");
			}
        }

        if (Input.inputString.Length > 0)
        {
            targetTypeBlend = 1;
            noInputTime = 0;
        }
        else
        {
            noInputTime += Time.deltaTime;
            if (noInputTime > .3f)
            {
                targetTypeBlend = 0;
            }
        }
        anim.SetFloat("Typing", targetTypeBlend, .5f, Time.deltaTime);
    }

    public void OnTaskFail()
    {
        int failIndex = failAnimIndexQueue.Dequeue();
        failAnimIndexQueue.Enqueue(failIndex);
        anim.SetFloat("Index", failIndex);
		anim.SetTrigger("Fail");
    }

    public void OnTaskSuccess(int i)
    {
		anim.SetFloat("Index", i);
		anim.SetTrigger("Victory");
    }

    public void OnWinGame()
    {
        anim.SetTrigger("Win Game");
    }

	public void OnLoseGame()
	{
        anim.SetTrigger("Lose Game");
	}

    public void SayWithText(string text, AudioClip clip)
    {
        dialogueUI.text = text;
    }

    public void ClearDialogueText()
    {
        dialogueUI.text = "";
    }

	public void MusicPlay()
	{
        screenOverlay.material.color = Color.clear;
		print("Music");
	}
}
