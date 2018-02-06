using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Brick")
        {
            Destroy(collision.gameObject);
        }
    }
}
