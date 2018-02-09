using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Login : Task {

    public InputField loginField;
    public Text attemptsRemainingUI;

    bool loginNeedsFocus;
    int attemptsRemaining = 3;

    string actualPassword = "test";
	
	void Start () {
        attemptsRemainingUI.text = "Attempts remaining: " + attemptsRemaining;
        loginField.onEndEdit.AddListener(OnEndEdit);
        ShowLoginPage();
	}
	
	void LateUpdate () {
        if (!taskOver)
        {
            if (loginNeedsFocus)
            {
                loginNeedsFocus = false;
                loginField.Select();
                loginField.ActivateInputField();
                loginField.ForceLabelUpdate();
            }
        }
	}

    void ShowLoginPage()
    {
		loginField.Select();

    }

    void OnEndEdit(string s)
    {
        if (!taskOver)
        {
            SubmitInput(s);
            loginField.text = "";
            loginNeedsFocus = true;
        }
    }

    void SubmitInput(string enteredPassword)
    {
        if (actualPassword.ToLower() == enteredPassword.ToLower())
        {
            OnCorrectPassword();
        }
        else
        {
            OnIncorrectPassword();
        }
    }

    void OnCorrectPassword()
    {
		print("Win");
        TaskCompleted();
    }

    void OnIncorrectPassword()
    {
        if (attemptsRemaining > 0)
        {
            attemptsRemaining--;
            attemptsRemainingUI.text = "Attempts remaining: " + attemptsRemaining;

            if (attemptsRemaining == 0)
            {
                print("Game over");
                TaskFailed();
            }
        }
    }
}
