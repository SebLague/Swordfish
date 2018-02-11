using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextFade : MonoBehaviour {

    public Color targetCol;
    public float duration;
    public TextMesh m;
    float t;

	// Update is called once per frame
	void Update () {
        t += Time.deltaTime / duration;
        m.color = Color.Lerp(Color.clear, targetCol, t);
	}
}
