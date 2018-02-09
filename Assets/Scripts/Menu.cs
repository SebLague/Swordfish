using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Cinemachine;

public class Menu : MonoBehaviour {

    public bool autoPlay;
    public Text title;
    public Text subtitle;
    public Text instruction;
 

    bool keyPressed;
    public float fadeTime = 1;
    public TypingAnim[] titleAnim;
    int titleAnimIndex;
    float timeSinceLastTitleAnim;
    bool playing;
    public Image fadePlane;
    bool readyToSkip;
    public MeshRenderer screenOverlay;

    Coroutine startRoutine;
    Coroutine textRoutine;

	// Use this for initialization
	void Start () {
        title.text = "";
		subtitle.gameObject.SetActive(false);
        instruction.gameObject.SetActive(false);
        screenOverlay.material.color = Color.black;

        if (autoPlay)
        {
            readyToSkip = true;
            playing = true;
            startRoutine = StartCoroutine(StartUpRoutine());
        }
	}

    IEnumerator StartUpRoutine()
    {
        while (Time.time < fadeTime)
        {
            float fadePercent = Time.time / fadeTime;
            fadePlane.color = Color.Lerp(Color.black, Color.clear, fadePercent);
            yield return null;
        }

        readyToSkip = true;

		yield return new WaitForSeconds(.5f);

        while (titleAnimIndex < titleAnim.Length)
        {
            title.text = titleAnim[titleAnimIndex].text;
            titleAnimIndex++;

            if (titleAnimIndex < titleAnim.Length)
            {
                yield return new WaitForSeconds(titleAnim[titleAnimIndex].delay);
            }
        }
        textRoutine = StartCoroutine(ShowExtraText());
	}
	
	// Update is called once per frame
	void Update () {
        if (playing)
        {
            if (Input.anyKeyDown && !keyPressed && readyToSkip)
            {
                keyPressed = true;
                if (startRoutine != null)
                {
                    StopCoroutine(startRoutine);
                }
                if (textRoutine != null)
                {
                    StopCoroutine(textRoutine);
                }

                FindObjectOfType<Sequencer>().Begin();
                title.gameObject.SetActive(false);
                subtitle.gameObject.SetActive(false);
                instruction.gameObject.SetActive(false);
            }
        }
	}

    IEnumerator ShowExtraText()
    {
        yield return new WaitForSeconds(1);

        string targetString = subtitle.text;
        subtitle.text = "";
        subtitle.gameObject.SetActive(true);

        for (int i = 0; i < targetString.Length; i++)
        {
            subtitle.text += targetString[i];
            yield return new WaitForSeconds(.03f);
        }

        yield return new WaitForSeconds(1);
		//targetString= instruction.text;
		//instruction.text = "";
		instruction.gameObject.SetActive(true);
        /*
		for (int i = 0; i < targetString.Length; i++)
		{
			instruction.text += targetString[i];
			yield return new WaitForSeconds(.03f);
		}
*/
        while (!keyPressed)
        {
            for (int i = 0; i < 3; i++)
            {
				yield return new WaitForSeconds(.2f);
                instruction.text += ".";
            }
            yield return new WaitForSeconds(.4f);
            instruction.text = instruction.text.Substring(0, instruction.text.Length - 3);
			yield return new WaitForSeconds(.4f);

        }
    }

    [System.Serializable]
    public class TypingAnim
    {
        public string text;
        public float delay;
    }
}
