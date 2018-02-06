using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour {

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
        if (loginNeedsFocus)
        {
            loginNeedsFocus = false;
			loginField.Select();
            loginField.ActivateInputField();
            loginField.ForceLabelUpdate();
		}
	}

    void ShowLoginPage()
    {
		loginField.Select();

    }

    void OnEndEdit(string s)
    {
        SubmitInput(s);
        loginField.text = "";
        loginNeedsFocus = true;
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
    }

    void OnIncorrectPassword()
    {
        attemptsRemaining--;
        attemptsRemainingUI.text = "Attempts remaining: " + attemptsRemaining;

        if (attemptsRemaining <= 0)
        {
            print("Game over");
        }
    }
}
