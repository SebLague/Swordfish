using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequencer : MonoBehaviour {
    public Transform screenParent;
    public Task[] tasks;
    public int taskIndex;
    Task currentTask;

    public Material monitorMat;
    public Material screenOverlayMat;

    private void Start()
    {
        SpawnTask();
    }

    void SpawnTask()
    {
		currentTask = Instantiate(tasks[taskIndex]);
        currentTask.transform.parent = screenParent;
        currentTask.transform.localPosition = Vector3.zero;
        currentTask.transform.localRotation = Quaternion.identity;
		currentTask.OnLose += Restart;
        currentTask.OnWin += NextTask;
    }
	
    void NextTask()
    {
        print("Next");
    }

    void Restart()
    {
        StartCoroutine(RestartSequence());
    }

    IEnumerator RestartSequence()
    {
        float p = 0;
        bool hasResetGame = false;
        while (p < 1)
        {
            
            p += Time.deltaTime * .75f;
            // float brightnessPercent = Mathf.Clamp01(restartBrightnessCurve.Evaluate(p));
            float brightnessPercent = Mathf.Clamp01(-(p * 2 - 1) * (p * 2 - 1) + 1);
            //print(p+"  " + brightnessPercent);
            monitorMat.SetColor("_EmissionColor", Color.white*brightnessPercent*3);
            //screenOverlayMat.color = new Color(1, 1, 1, p);
            screenOverlayMat.color = new Color(1, 1, 1, brightnessPercent);
            if (p >= .5f && !hasResetGame)
            {
                Destroy(currentTask.gameObject);
                hasResetGame = true;
            }
            yield return null;
        }
        screenOverlayMat.color = Color.clear;

        OnRestartComplete();
    }

    void OnRestartComplete()
    {
        SpawnTask();
    }
}
