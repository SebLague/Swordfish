using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stan : MonoBehaviour {

    Animator anim;
    float noInputTime;
    float targetTypeBlend;

    public Text dialogueUI;

	void Start () {
        anim = GetComponent<Animator>();
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
           // anim.SetTrigger("Spin");
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

    public void SayWithText(string text, AudioClip clip)
    {
        dialogueUI.text = text;
    }

    public void ClearDialogueText()
    {
        dialogueUI.text = "";
    }
}
