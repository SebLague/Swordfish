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
    public AnimationCurve skillCurve;
    public bool overideTypeSpeed_debug;
    public float forcedTypeSpeed_debug;
    public float actualTypeSpeed_readonly;
    public float scrollSpeed_readonly;
    public bool autoType_debug;
    float nextAutoTypeTime;
    public float skillExtra = .01f;

    List<int> possibleActiveWordIndices = new List<int>();
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
        float skillBasedCurveAdd = skillCurve.Evaluate(Mathf.Clamp(TypeSpeed,0,15)) * (1-restartDifficultyPercent);
        //float skillBasedSpeedFac = Mathf.Lerp(speedFac*skillBasedCurveFac, 0, restartDifficultyPercent);
        //float targetSpeed = baseSpeed + TypeSpeed * skillBasedSpeedFac;
        float targetSpeed = baseSpeed + skillBasedCurveAdd / 100f;
        speed = Mathf.Lerp(speed, targetSpeed, Time.deltaTime);
        scrollSpeed_readonly = speed;

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
            if (wordIndex < words.Length - 1 || activeWords.Count == 0)
            {

                bool isForcedSpawn = Time.time > nextForcedSpawnTime || activeWords.Count == 0;
                bool minTimeHasPassed = Time.time > nextMinSpawnTime;
                bool shouldSpawnFirst = percentDoneWithFirstWord > .1f && !spawnedFirst && activeWords.Count <= 3 && minTimeHasPassed;
                bool shouldSpawnSecond = percentDoneWithFirstWord > .5f && !spawnedSecond && activeWords.Count <= 3 && minTimeHasPassed;


                if (isForcedSpawn || shouldSpawnFirst || shouldSpawnSecond)
                {
                    nextForcedSpawnTime = Time.time + maxTimeBetweenSpawns;
                    if (shouldSpawnFirst)
                    {
                        spawnedFirst = true;
                    }
                    if (shouldSpawnSecond)
                    {
                        spawnedSecond = true;
                    }
                    TextMesh mesh = Instantiate<TextMesh>(textPrefab);
                    mesh.text = words[wordIndex];
                    PositionWord(mesh);
                    activeWords.Add(new Word(words[wordIndex], mesh));
                  
                    wordIndex++;
                    nextMinSpawnTime = Time.time + minDelay;
                    // nextSpawnTime = Time.time + Mathf.Lerp(delayMinMax.x, delayMinMax.y, completionPercent);
                    potentialWordMatches = new List<Word>(activeWords);

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
                //print("delete mesh " + i + " " + word.word);
                Destroy(word.mesh.gameObject);
            }
        }

		if (activeWords.Count > 0)
        {
            //activeWords[0].mesh.color = Color.white;
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
        if (wordIndex == 0)
        {
            randIndex = 0;
        }
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
        mesh.transform.position = new Vector3(spawnX, screen.center.y);
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
      
    }

    float TypeSpeed
    {
        get
        {
            float typeSpeed = startTypeSpeed;

            if (totalTypeTime > 0)
            {
                typeSpeed = numLettersTyped / totalTypeTime;
            }
            actualTypeSpeed_readonly = typeSpeed;
            //print(typeSpeed + " " +numLettersTyped + " " + totalTypeTime);
			if (Application.isEditor && overideTypeSpeed_debug)
			{
				typeSpeed = forcedTypeSpeed_debug;
			}

            return typeSpeed;

		}
    }

    void HandleInput()
    {
        string frameInput = Input.inputString;
        if (autoType_debug && Application.isEditor && Time.time > nextAutoTypeTime)
        {
            if (activeWords.Count > 0)
            {
                if (activeWords[0].mesh.transform.position.y < 1.55f)
                {
                    nextAutoTypeTime = Time.time + 1f / TypeSpeed;
                    frameInput = activeWords[0].word[inputString.Length] + "";
                }
            }
        }
        string initialInput = inputString;
        bool hasChanged = false;
        List<int> newPossibleIndices = new List<int>();
        for (int i = 0; i < activeWords.Count; i ++) {
		    Word word = activeWords[i];
            //print("testing: " + word.word + " : " + inputString);
            foreach (char c in frameInput.ToLower())
            {
                bool newInputStringValid = false;
                string newInputString = initialInput + c;


                if (word.word.StartsWith(newInputString, System.StringComparison.CurrentCulture))
                {
                    newInputStringValid = true;
                    hasChanged = true;
                    possibleActiveWordIndices.Add(i);
                    newPossibleIndices.Add(i);
                   // print(i + ": " + word.word + " starts with " + newInputString);
                }
                else
                {
					//print(i + ": " + word.word + " does not sstart with " + newInputString);
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
            }
        }

        if (newPossibleIndices.Count > 0)
        {
            possibleActiveWordIndices = newPossibleIndices;
        }

        if (hasChanged)
        {
			for (int i = 0; i < activeWords.Count; i++)
			{
                if (activeWords[i].word == inputString)
				{
					//print("remv " + i);

					activeWords[i].Completed();
					wordsToDelete.Add(activeWords[i]);
                    activeWords.RemoveAt(i);

					OnWordSucceeded(inputString);
                    break;
				}
			}

            string poss = "possible indices: ";
            foreach (int a in possibleActiveWordIndices)
            {
                poss += a + ", ";
            }
            string acc = "";
            for (int i = 0; i < activeWords.Count; i++)
            {
                if (possibleActiveWordIndices.Contains(i))
                {
                    
                    activeWords[i].UpdateColour(inputString);
                    acc += "update " + i + ", ";
                }
                else
                {
                    activeWords[i].ClearColour();
                    acc += "clear " + i + ", ";
                }
            }
            //print(poss + " : " + acc);
          
        
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
        Color defaultCol;

        public Word(string word, TextMesh mesh)
        {
            this.word = word;
            this.mesh = mesh;
            defaultCol = mesh.color;
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
                mesh.color = Color.white;
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

            if (inputString.Length > 0)
            {
                if (inputString.Length < word.Length)
                {
                    if (word[inputString.Length] == ' ')
                    {
                        mesh.text = mesh.text.Remove(inputString.Length + colFormatLength, 1);
                        mesh.text = mesh.text.Insert(inputString.Length + colFormatLength, "_");
                    }
                    string currCharColStr = "<color=" + currLetterCol + ">";
                    mesh.text = mesh.text.Insert(inputString.Length + colFormatLength, currCharColStr);
                    mesh.text = mesh.text.Insert(inputString.Length + colFormatLength + currCharColStr.Length + 1, "</color>");
                }
            }
        }

        public void ClearColour()
        {
            mesh.text = word;
            mesh.color = defaultCol;
        }
    }
}
