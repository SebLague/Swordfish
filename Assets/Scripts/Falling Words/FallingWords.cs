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
    float nextSpawnTime;
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

	// Use this for initialization
	void Start () {
        stan = FindObjectOfType<Stan>();
        stan.ResetTaskOneAudio();

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
	}
	
	// Update is called once per frame
	void Update () {
		float completionPercent = wordIndex / ((float)words.Length - 1);
        float speed = Mathf.Lerp(speedMinMax.x, speedMinMax.y, completionPercent);

        float percentDoneWithFirstWord = 0;
        if (activeWords.Count > 0)
        {
            percentDoneWithFirstWord = inputString.Length / (float)activeWords[0].word.Length;
        }

        if (percentDoneWithFirstWord > .3f && !requestedVoice)
        {
			stan.PlayNextTaskOneAudio();
            requestedVoice = true;
        }

        if (wordIndex < words.Length)
        {
            if (wordIndex < words.Length - 2 || activeWords.Count == 0)
            {
                if (activeWords.Count < 3)
                {
                    if (activeWords.Count == 0 || ((percentDoneWithFirstWord > .3f && !spawnedFirst) || (percentDoneWithFirstWord > .75f && !spawnedSecond)))
                    {
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

                        wordIndex++;
                        nextSpawnTime = Time.time + Mathf.Lerp(delayMinMax.x, delayMinMax.y, completionPercent);
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
        for (int i = 0; i < validBounds.Length; i++)
        {
            screen = validBounds[(randIndex+i)%validBounds.Length];
            if (screen.size.x > width)
            {
                break;
            }
        }


		Vector2 minMaxX = new Vector2(screen.min.x, screen.max.x);

		float spawnX = Random.Range(minMaxX.x, minMaxX.y - width);
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

    void OnWordSucceeded()
    {
        spawnedFirst = false;
        spawnedSecond = false;
        numWordsDone++;
        requestedVoice = false;
    }

    void HandleInput()
    {
        bool hasChanged = false;

        foreach (char c in Input.inputString.ToLower())
        {
            bool newInputStringValid = false;
            string newInputString = inputString + c;

            Word word = activeWords[0];
            if (word.word.StartsWith(newInputString,System.StringComparison.CurrentCulture))
            {
                newInputStringValid = true;
                hasChanged = true;
            }
        

            if (newInputStringValid)
            {
                inputString = newInputString;
            }
            else
            {
                break;
            }
        }

        if (hasChanged)
        {
			for (int i = activeWords.Count - 1; i >= 0; i--)
			{
				Word word = activeWords[i];
                if (word.word == inputString)
                {
                    OnWordSucceeded();
                    inputString = "";
					activeWords.RemoveAt(i);
					Destroy(word.mesh.gameObject);
                }
                word.UpdateColour(inputString);
             
            }
        }
    }


    class Word
    {
        public string word;
        public TextMesh mesh;

        const string highlightCol = "red";

        public Word(string word, TextMesh mesh)
        {
            this.word = word;
            this.mesh = mesh;
        }

        public void UpdateColour(string inputString)
        {
           
            if (word.StartsWith(inputString, System.StringComparison.CurrentCulture)) {
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

        }
    }
}
