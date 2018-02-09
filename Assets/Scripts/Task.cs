using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task : MonoBehaviour {

	public event System.Action OnLose;
	public event System.Action OnWin;
    public float restartDelay = .1f;
    protected bool taskOver;
    protected bool inEasyMode_debug;

    protected virtual void TaskCompleted()
    {
        taskOver = true;
        if (OnWin != null)
        {
            OnWin();
        }
    }

	protected virtual void TaskFailed()
	{
        taskOver = true;
		if (OnLose != null)
		{
			OnLose();
		}
	}

    public virtual void EnterEasyMode_Debug()
    {
        inEasyMode_debug = true;
    }
}
