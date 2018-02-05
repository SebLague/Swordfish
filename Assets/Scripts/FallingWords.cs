using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingWords : MonoBehaviour {
    
    float buffer = .2f;
    public TextAsset t;
    string[] words;
    public Vector2 spawnRange;
    public float spawnHeight;
    public float destroyHeight;
    public float delayMinMax;
    public TextMesh textPrefab;
    int wordIndex;
    float nextSpawnTime;
    BoxCollider2D[] topScreens;

    List<Word> activeWords;
    List<Word> potentialWordMatches;
    string inputString;

	// Use this for initialization
	void Start () {
        activeWords = new List<Word>();
        potentialWordMatches = new List<Word>();

        words = t.text.Split(',');
        Utility.Shuffle(words);
        for (int i = 0; i < words.Length; i++)
        {
            words[i] = words[i].Trim().ToLower();
        }
        topScreens = FindObjectOfType<ScreenAreas>().topScreens;
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.time > nextSpawnTime)
        {
            BoxCollider2D screen = topScreens[Random.Range(0, topScreens.Length)];
            Vector2 minMaxX = new Vector2(screen.bounds.min.x, screen.bounds.max.x);
         
            TextMesh mesh = Instantiate<TextMesh>(textPrefab);
            mesh.text = words[wordIndex];
            float width = mesh.GetComponent<MeshRenderer>().bounds.size.x;
            float spawnX = Random.Range(minMaxX.x + buffer, minMaxX.y - width - buffer);
            mesh.transform.parent = transform;
            mesh.transform.position = new Vector3(spawnX, spawnHeight);
            mesh.transform.localEulerAngles = Vector3.up * 180;
            activeWords.Add(new Word(words[wordIndex],mesh));

            wordIndex = (wordIndex + 1) % words.Length;
            nextSpawnTime = Time.time + 3;
            potentialWordMatches = new List<Word>(activeWords);
        }

        for (int i = activeWords.Count-1; i >= 0; i--)
        {
   
            Word word = activeWords[i];
			word.mesh.transform.position += Vector3.down * Time.deltaTime * 1;

			if (word.mesh.transform.position.y < destroyHeight)
			{
				OnWordFailed();
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


        HandleInput();
	}

    void OnWordFailed()
    {

    }

    void OnWordSucceeded()
    {

    }

    void HandleInput()
    {
        bool hasChanged = false;

        foreach (char c in Input.inputString.ToLower())
        {
            bool newInputStringValid = false;
            string newInputString = inputString + c;

            foreach (Word word in activeWords)
            {
                if (word.word.StartsWith(newInputString,System.StringComparison.CurrentCulture))
                {
                    newInputStringValid = true;
                    hasChanged = true;
                }
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
