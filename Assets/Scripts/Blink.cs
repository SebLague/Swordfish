using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : MonoBehaviour {

    public GameObject[] objs;
    public float timeOn = .5f;
    public float timeOff = .2f;
    public int reps = 3;
    public float delay;

	// Use this for initialization
	void Start () {
        StartCoroutine(BlinkRoutine());
	}


    IEnumerator BlinkRoutine()
    {
        yield return new WaitForSeconds(delay);
        for (int i = 0; i < reps; i++)
		{
            foreach (GameObject g in objs)
            {
                g.SetActive(true);
            }
			yield return new WaitForSeconds(timeOn);
            foreach (GameObject g in objs)
            {
                g.SetActive(false);
            }
            yield return new WaitForSeconds(timeOff);
			

        }
      
       
    }
}
