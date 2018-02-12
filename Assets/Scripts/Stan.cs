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
    public AudioClip music;
    public AudioSource musicSource;
	public AudioSource voiceSource;

    public AudioClip[] taskOneAudio;
    int taskAudioIndex;
    float previousVoiceEndTime;
    public AudioClip[] taskFailAudio;
    public AudioClip[] taskWinAudio;
    public AudioClip keySlam;
    public AudioClip chairRoll;
    public AudioClip[] taskFailSfx;
    public GameObject glass;
    public GameObject handBone;

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
        Sfx.Play(chairRoll);
    }

    public void GrabGlass()
    {
        //Debug.Break();
        glass.transform.parent = handBone.transform;
        StartCoroutine(LerpGlass());
    }

    IEnumerator LerpGlass()
    {
        Vector3 locPos = glass.transform.localPosition;
        Quaternion locRot = glass.transform.localRotation;
        float p = 0;
        while (p < 1)
        {
            p += Time.deltaTime / .2f;
			glass.transform.localPosition = Vector3.Lerp(locPos, Vector3.zero, p);
            glass.transform.localRotation = Quaternion.Slerp(locRot, Quaternion.identity, p);
            yield return null;
        }
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
        if (taskFailAudio != null && taskFailAudio.Length > 0)
        {
            PlayVoiceImmediate(taskFailAudio[Random.Range(0, taskFailAudio.Length)]);
        }
    }

    public void OnTaskSuccess(int i)
    {
		anim.SetFloat("Index", i);
		anim.SetTrigger("Victory");
        if (taskWinAudio != null && taskWinAudio.Length > 0)
        {
            PlayVoiceImmediate(taskWinAudio[Random.Range(0, taskWinAudio.Length)]);
        }
    }

    public void OnWinGame()
    {
        anim.SetTrigger("Win Game");
        //StartCoroutine(PlayWinAnim());
    }

    IEnumerator PlayWinAnim()
    {
        yield return new WaitForSeconds(.5f);
		anim.SetTrigger("Win Game");
    }

	public void OnLoseGame()
	{
        anim.SetTrigger("Lose Game");
	}

    public void SayWithText(string text, AudioClip clip)
    {
        dialogueUI.text = text;
        if (clip != null)
        {
            PlayVoiceImmediate(clip);
        }
    }

    public void ClearDialogueText()
    {
        dialogueUI.text = "";
    }

    public void ResetTaskOneAudio()
    {
        taskAudioIndex = 0;
    }
    public void PlayNextTaskOneAudio()
    {
        if (taskAudioIndex < taskOneAudio.Length)
        {
            if (Time.time > previousVoiceEndTime+1)
            {
                PlayVoiceImmediate(taskOneAudio[taskAudioIndex]);
            }
            
            taskAudioIndex++;
        }
    }

	public void MusicPlay()
	{
        screenOverlay.material.color = Color.clear;
        musicSource.clip = music;
        musicSource.volume = 1;
        musicSource.Play();
        Sfx.Play(keySlam);
	}

    void PlayVoiceImmediate(AudioClip clip)
    {
		voiceSource.clip = clip;
		voiceSource.Play();
		previousVoiceEndTime = Time.time + clip.length;
    }

    public void PlayTaskFailSfx(int i)
    {
        if (taskFailSfx != null && i < taskFailSfx.Length)
        {
            Sfx.Play(taskFailSfx[i]);
        }
    }
}
