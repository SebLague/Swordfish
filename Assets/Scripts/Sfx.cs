using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sfx : MonoBehaviour {

    AudioSource s;
    static Sfx instance;
	// Use this for initialization
	void Awake () {
        s = GetComponent<AudioSource>();
        instance = this;
	}

    public static void Play(AudioClip clip, float vol = 1)
    {
        if (clip != null)
        {
            instance.s.PlayOneShot(clip, vol);
        }
        else
        {
            Debug.Log("Null audio provided");
        }
    }

	public static void Play(AudioClip[] clip, float vol = 1)
	{
		if (clip != null)
		{
            instance.s.PlayOneShot(clip[Random.Range(0,clip.Length)], vol);
		}
		else
		{
			Debug.Log("Null audio provided");
		}
	}
}
