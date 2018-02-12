using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Login : Task
{
    public GameObject fish;
    public InputField loginField;
    public Text attemptsRemainingUI;
    public Text hintsUI;
    public GameObject mainScreen;
    public GameObject denied;
    public GameObject granted;
    public GameObject menu;
    public GameObject successObject;

    string fullHintsTxt;
    bool showingMessage;
    bool loginNeedsFocus;
    int attemptsRemaining = 3;

    public AudioClip acceptedAudio;
    public AudioClip deniedAudio;

    string actualPassword = "swordfish";

    void Start()
    {
        fullHintsTxt = hintsUI.text;
        hintsUI.text = fullHintsTxt.Split('\n')[0];
        attemptsRemainingUI.text = "Attempts remaining: " + attemptsRemaining;
        loginField.onEndEdit.AddListener(OnEndEdit);
        ShowLoginPage();
    }

    protected override void Update()
    {
        base.Update();
    }

    void LateUpdate()
    {
        if (!taskOver)
        {
            if (loginNeedsFocus)
            {
                //loginNeedsFocus = false;
                loginField.Select();
                loginField.ActivateInputField();
                //loginField.ForceLabelUpdate();
            }
        }
    }

    void ShowLoginPage()
    {
        loginField.Select();

    }

    void OnEndEdit(string s)
    {
        if (!taskOver && !showingMessage)
        {
            SubmitInput(s);
            loginField.text = "";
            loginNeedsFocus = true;
        }
    }

    void SubmitInput(string enteredPassword)
    {
        if (enteredPassword.Length > 0)
        {
            if (actualPassword.ToLower() == enteredPassword.ToLower().Replace(" ",""))
            {
                OnCorrectPassword();
            }
            else
            {
                OnIncorrectPassword();
            }
        }
    }

    void OnCorrectPassword()
    {
        Sfx.Play(acceptedAudio);
        StartCoroutine(Message(true, true));
        TaskCompleted();
        FindObjectOfType<Sequencer>().GameWin();
        StartCoroutine(ShowWin());
    }

    void OnIncorrectPassword()
    {
        if (attemptsRemaining > 0)
        {
           // Sfx.Play(deniedAudio);
            attemptsRemaining--;
            attemptsRemainingUI.text = "Attempts remaining: " + attemptsRemaining;

            if (attemptsRemaining == 0)
            {
                menu.SetActive(false);
                //StartCoroutine(Message(false, true));
                TaskFailed();
                FindObjectOfType<Sequencer>().GameLose();
            }
            else
            {
                if (attemptsRemaining == 1)
                {
                    StartCoroutine(ShowFish());
                }
                StartCoroutine(Message(false, false));
            }
        }
    }

    IEnumerator ShowFish() {
        yield return new WaitForSeconds(5);
        fish.SetActive(true);

    }

    IEnumerator ShowWin()
    {
        yield return new WaitForSeconds(.5f);
        successObject.SetActive(true);
        fish.SetActive(true);
    }
    IEnumerator Message(bool accessGranted, bool loopForever)
    {
        showingMessage = true;
        mainScreen.SetActive(false);
        GameObject m = (accessGranted) ? granted : denied;
        int i = 3;
        while (i > 0 || loopForever)
        {
            i--;
			yield return new WaitForSeconds(.2f);
			m.SetActive(true);
            if (!accessGranted)
            {
                Sfx.Play(deniedAudio, Mathf.Lerp(.8f, .3f, 1 - i / 3f));
            }
            yield return new WaitForSeconds(.5f);
            m.SetActive(false);
			
        }

        if (!accessGranted)
        {
            hintsUI.text += "\n"+fullHintsTxt.Split('\n')[3 - attemptsRemaining];
        }

        m.SetActive(false);
        mainScreen.SetActive(true);
        showingMessage = false;
    }
}
