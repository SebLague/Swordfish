using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Transition : MonoBehaviour {

    public event System.Action OnComplete;

    public bool autoStart;
    public List<FadeEvent> fadeEvents;
	public List<EnableEvent> enableEvents;
    public List<DialogueEvent> dialogueEvents;
    public int winAnimationIndex = -1;

    List<FadeEvent> remainingFadeEvents = new List<FadeEvent>();
    List<EnableEvent> remainingEnableEvents = new List<EnableEvent>();
    List<DialogueEvent> remainingDialogueEvents = new List<DialogueEvent>();
    float endTime;
    bool active;
    float startTime;

    private void Start()
    {
        if (autoStart)
        {
            Begin();
        }
    }

    public void Begin()
    {
        startTime = Time.time;
		remainingFadeEvents = new List<FadeEvent>(fadeEvents);
		remainingEnableEvents = new List<EnableEvent>(enableEvents);
		remainingDialogueEvents = new List<DialogueEvent>(dialogueEvents);

        endTime = Mathf.Max(MaxEndTime(fadeEvents),MaxEndTime(enableEvents),MaxEndTime(dialogueEvents));
        active = true;

		if (winAnimationIndex != -1)
		{
			FindObjectOfType<Stan>().OnTaskSuccess(winAnimationIndex);
		}
    }

    float LocalTime
    {
        get
        {
            return Time.time - startTime;
        }
    }

    void Update () {
        if (active)
        {
            
            for (int i = remainingEnableEvents.Count - 1; i >= 0; i--)
            {
                if (LocalTime > remainingEnableEvents[i].time)
                {
                    foreach (GameObject g in remainingEnableEvents[i].obj)
                    {
                        g.SetActive(remainingEnableEvents[i].active);
                    }
                    remainingEnableEvents.RemoveAt(i);
                }
            }

            for (int i = remainingFadeEvents.Count - 1; i >= 0; i--)
            {
                if (LocalTime > remainingFadeEvents[i].time)
                {
                    StartCoroutine(FadeSequence(remainingFadeEvents[i]));
                    remainingFadeEvents.RemoveAt(i);
                }
            }

            for (int i = remainingDialogueEvents.Count - 1; i >= 0; i--)
            {
                if (LocalTime > remainingDialogueEvents[i].time)
                {
                    StartCoroutine(DialogueSequence(remainingDialogueEvents[i]));
                    remainingDialogueEvents.RemoveAt(i);
                }
            }

            if (LocalTime > endTime)
            {
                active = false;

                if (OnComplete != null)
                {
                    OnComplete();
                }
            }
        }


    }

    IEnumerator FadeSequence(FadeEvent e)
    {
        float p = 0;
        Material mat = (e.renderer == null) ? e.sprite.material : e.renderer.material;
        Color startCol = ((e.useCurrentAsStartCol) ? mat.color : e.startCol);
        while (p < 1)
        {
            p += Time.deltaTime / e.duration;
            mat.color = Color.Lerp(startCol, e.targetCol, p);
            yield return null;
        }
    }

    IEnumerator DialogueSequence(DialogueEvent e)
    {
        if (FindObjectOfType<Stan>())
        {
            FindObjectOfType<Stan>().SayWithText(e.text, e.audio);
            float realDuration = (e.audio == null) ? e.duration : e.audio.length;
            yield return new WaitForSeconds(realDuration);
            FindObjectOfType<Stan>().ClearDialogueText();
        }
        else
        {
            Debug.LogError("WHERE'S STAN?!");
        }
	}

    [System.Serializable]
    public class TransitionEvent
    {
        public float time;
        public float duration = 1;
    }

    [System.Serializable]
    public class FadeEvent : TransitionEvent
    {
        public SpriteRenderer sprite;
        public MeshRenderer renderer;
        public bool useCurrentAsStartCol;
        public Color startCol;
        public Color targetCol;
    }

	[System.Serializable]
	public class EnableEvent : TransitionEvent
	{
        public GameObject[] obj;
        public bool active = true;
	}

	[System.Serializable]
	public class DialogueEvent : TransitionEvent
	{
        public string text;
        public AudioClip audio;
	}

    float MaxEndTime<T>(List<T> e) where T : TransitionEvent
    {
        float maxT = 0;
        if (e != null)
        {
            for (int i = 0; i < e.Count; i++)
            {
                float t = e[i].time + e[i].duration;
                if (t > maxT)
                {
                    maxT = t;
                }
            }
        }
        return maxT;
    }
   
}
