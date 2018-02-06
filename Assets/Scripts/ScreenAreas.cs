using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenAreas : MonoBehaviour {

    public BoxCollider2D[] allScreens;
    public BoxCollider2D[] topScreens;
    public BoxCollider2D mainScreen;

    [HideInInspector]
    public Vector2 minMaxX;

    void Awake()
    {
        minMaxX = new Vector2(float.MaxValue, float.MinValue);
        foreach (BoxCollider2D c in allScreens)
        {
            if (c.bounds.min.x < minMaxX.x)
            {
                minMaxX.x = c.bounds.min.x;
            }
            if (c.bounds.max.x > minMaxX.y)
			{
                minMaxX.y = c.bounds.max.x;
			}
        }
    }
}
