using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class Sequencer : MonoBehaviour {

    public bool autoStart;
    public bool[] easyMode;

    public Transform screenParent;
    public Task[] tasks;
    public int taskIndex;
    Task currentTask;
    public Transition[] taskTransitions;
    int numRestarts;

    public MeshRenderer monitor;
    public MeshRenderer screenOverlay;
    public CinemachineVirtualCamera menuCam;
    public AudioClip restartAudio;
    public AudioClip gameOverSirens;
    public Image fadePlane;
    bool readyForGameReload;
    public AudioClip endBeepAudio;

    private void Start()
    {

        if (autoStart)
        {
            Begin();
        }
    }

    private void Update()
    {
        if (readyForGameReload)
        {
            if (Input.anyKeyDown)
            {
				UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void Begin()
    {
        if (!FindObjectOfType<SceneState>().inMenuState)
        {
            FindObjectOfType<SceneState>().SetMenuState();
        }
        else
        {
            menuCam.Priority = 0;
        }

		StartTransition();

	}

    void StartTransition()
    {
		if (currentTask != null)
		{
			Destroy(currentTask.gameObject);
			currentTask = null;
		}

        if (taskIndex > 0)
        {
            taskTransitions[taskIndex - 1].gameObject.SetActive(false);
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
        menuCam.Priority = 5;
   
        if (taskIndex < tasks.Length)
        {
            currentTask = Instantiate(tasks[taskIndex]);
            currentTask.transform.SetParent(screenParent);
            currentTask.transform.localPosition = Vector3.zero;
            currentTask.transform.localRotation = Quaternion.identity;
            currentTask.gameObject.SetActive(true);
            currentTask.SetNumRestarts(numRestarts);
            currentTask.OnLose += Restart;
            currentTask.OnWin += TransitionToNextTask;

            if (easyMode[taskIndex])
            {
                currentTask.EnterEasyMode_Debug();
            }
        }
    }
	
    void TransitionToNextTask()
    {
        numRestarts = 0;
        taskIndex++;
        StartTransition();
    }

    void Restart()
    {
        StartCoroutine(RestartSequence(currentTask.restartDelay));
    }

    IEnumerator RestartSequence(float delay)
    {
        Sfx.Play(restartAudio);
        numRestarts++;
        FindObjectOfType<Stan>().OnTaskFail();
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


    public void GameWin()
    {
		//StartCoroutine(GameLoseSequence(true));
        FindObjectOfType<Stan>().OnWinGame();
    }

    public void GameLose()
    {
        StartCoroutine(GameLoseSequence());
        FindObjectOfType<Stan>().OnLoseGame();
    }

    IEnumerator GameLoseSequence()
	{
        Sfx.Play(gameOverSirens);
        float endDuration = gameOverSirens.length;
        float speed = (1 / .7f);
        Color themeCol = Color.red;
		yield return new WaitForSeconds(.2f);
        float t = 0;
        bool hasPlayedBeep = false;
		while (t < endDuration)
		{
            if (t > 1 && !readyForGameReload)
            {
                readyForGameReload = true;
                //FindObjectOfType<Stan>().SayWithText("press spacebar to play again",null);
            }

            float p = Mathf.Repeat(t * speed,1);
            t += Time.deltaTime;
			float brightnessPercent = Mathf.Clamp01(-(p * 2 - 1) * (p * 2 - 1) + 1);
			//print(p+"  " + brightnessPercent);
			monitor.material.SetColor("_EmissionColor", themeCol * brightnessPercent * 3);
			//screenOverlayMat.color = new Color(1, 1, 1, p);
			screenOverlay.material.color = new Color(1, 1, 1, brightnessPercent);

            if (p > .3f && !hasPlayedBeep)
            {
                Sfx.Play(endBeepAudio);
                hasPlayedBeep = true;
            }
            if (p < .3f)
            {
                hasPlayedBeep = false;
            }

            if (t + 3 > endDuration)
            {
                float fadePercent = Mathf.InverseLerp(endDuration-3, endDuration, t);
                fadePlane.color = Color.Lerp(Color.clear, Color.black, fadePercent);
            }

            yield return null;
		}
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
	}
  
}
