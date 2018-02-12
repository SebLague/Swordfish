using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyCount : MonoBehaviour {

    public Text transfer;
    public Text money;

    int speedMin = 100;
    int speedMax = 500;
    int speed;
    float n;

    private void Start()
    {
        speed = Random.Range(speedMin, speedMax);
        StartCoroutine(Animate());
    }

    void Update () {
        n += Time.deltaTime * speed;
        money.text = "$" + (int)n;
	}

    IEnumerator Animate()
    {
        string t = "Transferring...";
        while (true)
        {
            for (int i = 0; i <= 3; i++)
            {
                transfer.text = t.Substring(0, t.Length - 3 + i);
                yield return new WaitForSeconds(.2f);
            }
        }
    }
}
