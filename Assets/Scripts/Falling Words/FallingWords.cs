using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FallingWords : Task {

    public GameObject validAreas;
    public TextAsset t;
    string[] words;
    public Vector2 spawnRange;
    public float spawnHeight;
    public float destroyHeight;
   
    public TextMesh textPrefab;
    int wordIndex;
    float nextMinSpawnTime;
    BoxCollider2D[] topScreens;
    Bounds[] validBounds;
    public Vector2 speedMinMax;

    public Vector2 delayMinMax;
    bool done;
    List<Word> activeWords;
    List<Word> potentialWordMatches;
    string inputString;
    bool allWordsSpawned;
    bool spawnedFirst;
    bool spawnedSecond;
    int numWordsDone;
    bool requestedVoice;
    Stan stan;

    bool recordingWordTime;
    int numLettersTyped;
    float wordStartTime;
    float totalTypeTime;
    float startTypeSpeed = 3;
    public float speedFac = .1f;
    public float minDelay = 1;
    float speed;
    public AudioClip[] wordCompleteAudio;
    List<Word> wordsToDelete;
    public float maxTimeBetweenSpawns = 5;
    float nextForcedSpawnTime;
    int lastScreenIndex;
    int widestScreenIndex;

	// Use this for initialization
	void Start () {
        stan = FindObjectOfType<Stan>();
        if (stan != null)
        {
            stan.ResetTaskOneAudio();
        }

        wordsToDelete = new List<Word>();
        inputString = "";
        activeWords = new List<Word>();
        potentialWordMatches = new List<Word>();

        words = t.text.Split(',');
        //Utility.Shuffle(words);
        for (int i = 0; i < words.Length; i++)
        {
            words[i] = words[i].Trim().ToLower();
        }
        topScreens = FindObjectOfType<ScreenAreas>().topScreens;
        validBounds = validAreas.GetComponents<BoxCollider2D>().Select(v=>v.bounds).ToArray();
        float widest = 0;
        for (int i = 0; i < validBounds.Length; i++)
        {
            if (validBounds[i].size.x > widest)
            {
                widestScreenIndex = i;
                widest = validBounds[i].size.x;
            }
        }
        lastScreenIndex = widestScreenIndex;
    }



    // Update is called once per frame
    protected override void Update () {
        base.Update();
		float completionPercent = wordIndex / ((float)words.Length - 1);
        float baseSpeed = Mathf.Lerp(speedMinMax.x, speedMinMax.y, completionPercent);
        float restartDifficultyPercent = Mathf.Clamp01((numRestarts - 1) / 4f);
        float skillBasedSpeedFac = Mathf.Lerp(speedFac, 0, restartDifficultyPercent);
        float targetSpeed = baseSpeed + TypeSpeed * skillBasedSpeedFac;
        speed = Mathf.Lerp(speed, targetSpeed, Time.deltaTime);

        float percentDoneWithFirstWord = 0;
        if (activeWords.Count > 0)
        {
            percentDoneWithFirstWord = inputString.Length / (float)activeWords[0].word.Length;
        }

        if (percentDoneWithFirstWord > .3f && !requestedVoice)
        {
            if (stan != null)
            {
                stan.PlayNextTaskOneAudio();
            }
            requestedVoice = true;
        }

        if (wordIndex < words.Length)
        {
            if (wordIndex < words.Length - 1)
            {
                if (activeWords.Count < 3 && Time.time > nextMinSpawnTime || Time.time > nextForcedSpawnTime)
                {
                    if (activeWords.Count == 0 || Time.time > nextForcedSpawnTime || ((percentDoneWithFirstWord > .3f && !spawnedFirst) || (percentDoneWithFirstWord > .75f && !spawnedSecond)))
                    {
                        nextForcedSpawnTime = Time.time + maxTimeBetweenSpawns;
                        if (percentDoneWithFirstWord > .3f)
                        {
                            spawnedFirst = true;
                        }
                        if (percentDoneWithFirstWord > .75f)
                        {
                            spawnedSecond = true;
                        }
                        TextMesh mesh = Instantiate<TextMesh>(textPrefab);
                        mesh.text = words[wordIndex];
                        PositionWord(mesh);
                        activeWords.Add(new Word(words[wordIndex], mesh));
                        if (activeWords.Count == 1)
                        {
                            activeWords[0].UpdateColour(inputString);
                        }
                        wordIndex++;
                        nextMinSpawnTime = Time.time + minDelay;
                       // nextSpawnTime = Time.time + Mathf.Lerp(delayMinMax.x, delayMinMax.y, completionPercent);
                        potentialWordMatches = new List<Word>(activeWords);
                    }
                }
            }
        }
        else
        {
            allWordsSpawned = true;
        }

        for (int i = activeWords.Count-1; i >= 0; i--)
        {
   
            Word word = activeWords[i];
            word.mesh.transform.position += Vector3.down * Time.deltaTime * speed;

			if (word.mesh.transform.position.y < destroyHeight)
			{
				TaskFailed();
                activeWords.Remove(word);
				if (potentialWordMatches.Contains(word))
				{
					potentialWordMatches.Remove(word);
					if (potentialWordMatches.Count == 0)
					{
						inputString = "";
					}
				}
                Destroy(word.mesh.gameObject);
			}
        }

        for (int i = wordsToDelete.Count - 1; i >= 0; i--)
        {

            Word word = wordsToDelete[i];
            word.mesh.transform.position += Vector3.down * Time.deltaTime * speed;
            if (word.DeletionEffect())
            {
                wordsToDelete.RemoveAt(i);
                Destroy(word.mesh.gameObject);
            }
        }

			if (activeWords.Count > 0)
        {
            activeWords[0].mesh.color = Color.white;
            HandleInput();
        }

        bool easyModeVictory = percentDoneWithFirstWord > 0 && inEasyMode_debug;
        if ((activeWords.Count == 0 && allWordsSpawned || easyModeVictory) && !done)
        {
            TaskCompleted ();
        }
	}

    void PositionWord(TextMesh mesh)
    {
        float width = mesh.GetComponent<MeshRenderer>().bounds.size.x;

        int randIndex = Random.Range(0, validBounds.Length);
        Bounds screen = new Bounds();
        bool fits = false;
        for (int i = 0; i < validBounds.Length; i++)
        {

            int index = (randIndex + i) % validBounds.Length;
            Bounds potScreen = validBounds[index];
            if (index != lastScreenIndex && potScreen.size.x > width)
            {
                screen = potScreen;
                lastScreenIndex = index;
                fits = true;
				break;
			}
          
        }
        if (!fits)
        {
            screen = validBounds[widestScreenIndex];
            lastScreenIndex = widestScreenIndex;
        }

		float spawnX = Random.Range(screen.min.x, screen.max.x - width);
		mesh.transform.parent = transform;
		mesh.transform.position = new Vector3(spawnX, spawnHeight);
		mesh.transform.localEulerAngles = Vector3.up * 180;

	}

    protected override void TaskCompleted()
    {

        base.TaskCompleted();
        done = true;
    }

    protected override void TaskFailed()
    {
        base.TaskFailed();
    }

    void OnWordSucceeded(string w)
    {
        numLettersTyped += w.Length;
        totalTypeTime += (Time.time - wordStartTime);
        recordingWordTime = false;
       // print(TypeSpeed);

        spawnedFirst = false;
        spawnedSecond = false;
        numWordsDone++;
        requestedVoice = false;
        Sfx.Play(wordCompleteAudio, .2f);

        inputString = "";
        if (activeWords.Count > 0)
        {
            activeWords[0].UpdateColour("");
        }
    }

    float TypeSpeed
    {
        get
        {
            if (totalTypeTime > 0)
            {
                return numLettersTyped / totalTypeTime;
            }
            return startTypeSpeed;
        }
    }

    void HandleInput()
    {
        bool hasChanged = false;
		Word word = activeWords[0];
        foreach (char c in Input.inputString.ToLower())
        {
            bool newInputStringValid = false;
            string newInputString = inputString + c;

           
            if (word.word.StartsWith(newInputString,System.StringComparison.CurrentCulture))
            {
                newInputStringValid = true;
                hasChanged = true;
            }
        

            if (newInputStringValid)
            {
                if (!recordingWordTime)
                {
                    recordingWordTime = true;
                    wordStartTime = Time.time;
                }
                inputString = newInputString;
            }
            else
            {
                break;
            }
        }

        if (hasChanged)
        {
			word.UpdateColour(inputString);
            if (word.word == inputString)
            {
                inputString = "";
                activeWords.RemoveAt(0);
                word.Completed();
                wordsToDelete.Add(word);
                OnWordSucceeded(inputString);

            }
        
        }
    }


    class Word
    {
        public string word;
        public TextMesh mesh;

        const string highlightCol = "#b71b1b";
        const string currLetterCol = "#e5811f";
        int deletionIndex;
        float nextDeleteTime;
        string delString;

        public Word(string word, TextMesh mesh)
        {
            this.word = word;
            this.mesh = mesh;
        }

        public void Completed()
        {
            mesh.transform.position = mesh.GetComponent<MeshRenderer>().bounds.center;
            mesh.anchor = TextAnchor.MiddleCenter;
            deletionIndex = word.Length;
            mesh.text = word;
            mesh.color = new Color(1, 1, 1, .2f);
            delString = word;

        }

        public bool DeletionEffect()
        {
            if (Time.time > nextDeleteTime)
            {

                nextDeleteTime = Time.time + .012f;
                delString = delString.Substring(1, delString.Length - 2);
                /*
                string bin = "";
                foreach (char c in delString)
                {
                    if (c == ' ')
                    {
                        bin += " ";
                    }
                    else
                    {
                        bin += Random.Range(0, 2);
                    }
                }
                */
                mesh.text = delString;
            }
            return delString.Length <= 2;
        }

        public void UpdateColour(string inputString)
        {
            int colFormatLength = 0;
            if (word.StartsWith(inputString, System.StringComparison.CurrentCulture) && inputString != "") {
                string colFormat = "<color=" + highlightCol + ">" + "</color>";
                colFormatLength = colFormat.Length;
                mesh.text = "<color=" + highlightCol + ">" + inputString + "</color>";

                if (word != inputString)
                {
                    mesh.text += word.Substring(inputString.Length, word.Length - inputString.Length);
                }
            }
            else
            {
                mesh.text = word;
            }

            if (inputString.Length < word.Length)
            {
                if (word[inputString.Length] == ' ')
                {
                    mesh.text=mesh.text.Remove(inputString.Length+colFormatLength,1);
                    mesh.text=mesh.text.Insert(inputString.Length + colFormatLength, "_");
                }
                string currCharColStr = "<color=" + currLetterCol + ">";
                mesh.text = mesh.text.Insert(inputString.Length+colFormatLength, currCharColStr);
                mesh.text = mesh.text.Insert(inputString.Length + colFormatLength + currCharColStr.Length +1, "</color>");
            }
        }
    }
}
