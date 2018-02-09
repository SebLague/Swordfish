using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequencer : MonoBehaviour {
    
    public bool easyMode;

    public Transform screenParent;
    public Task[] tasks;
    public int taskIndex;
    Task currentTask;
    public Transition[] taskTransitions;

    public MeshRenderer monitor;
    public MeshRenderer screenOverlay;

    private void Start()
    {
        StartTransition();
    }

    void StartTransition()
    {
		if (currentTask != null)
		{
			Destroy(currentTask.gameObject);
			currentTask = null;
		}

        if (taskIndex < taskTransitions.Length)
        {
            taskTransitions[taskIndex].gameObject.SetActive(true);
            taskTransitions[taskIndex].OnComplete += SpawnTask;
            taskTransitions[taskIndex].Begin();
        }
        else
        {
            SpawnTask();
        }
    }


    void SpawnTask()
    {
   
        if (taskIndex < tasks.Length)
        {
            currentTask = Instantiate(tasks[taskIndex]);
            currentTask.transform.parent = screenParent;
            currentTask.transform.localPosition = Vector3.zero;
            currentTask.transform.localRotation = Quaternion.identity;
            currentTask.gameObject.SetActive(true);
            currentTask.OnLose += Restart;
            currentTask.OnWin += TransitionToNextTask;

            if (easyMode && Application.isEditor)
            {
                currentTask.EnterEasyMode_Debug();
            }
        }
    }
	
    void TransitionToNextTask()
    {
        taskIndex++;
        StartTransition();
    }

    void Restart()
    {
        StartCoroutine(RestartSequence(currentTask.restartDelay));
    }

    IEnumerator RestartSequence(float delay)
    {
        yield return new WaitForSeconds(delay);
        float p = 0;
        bool hasResetGame = false;
        while (p < 1)
        {
            p += Time.deltaTime * .75f;
            // float brightnessPercent = Mathf.Clamp01(restartBrightnessCurve.Evaluate(p));
            float brightnessPercent = Mathf.Clamp01(-(p * 2 - 1) * (p * 2 - 1) + 1);
            //print(p+"  " + brightnessPercent);
            monitor.material.SetColor("_EmissionColor", Color.white*brightnessPercent*3);
            //screenOverlayMat.color = new Color(1, 1, 1, p);
            screenOverlay.material.color = new Color(1, 1, 1, brightnessPercent);
            if (p >= .5f && !hasResetGame)
            {
                Destroy(currentTask.gameObject);
                hasResetGame = true;
            }
            yield return null;
        }
        screenOverlay.material.color = Color.clear;

        OnRestartComplete();
    }

    void OnRestartComplete()
    {
        SpawnTask();
    }
}
