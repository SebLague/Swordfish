using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stan : MonoBehaviour {

    Animator anim;
    float noInputTime;
    float targetTypeBlend;

	void Start () {
        anim = GetComponent<Animator>();
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
           // anim.SetTrigger("Spin");
        }

        if (Input.inputString.Length > 0)
        {
            targetTypeBlend = 1;
            noInputTime = 0;
        }
        else
        {
            noInputTime += Time.deltaTime;
            if (noInputTime > .3f)
            {
                targetTypeBlend = 0;
            }
        }
        anim.SetFloat("Typing", targetTypeBlend, .5f, Time.deltaTime);
    }
}
