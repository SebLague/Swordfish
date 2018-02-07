using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {

	public event System.Action OnDeath;
	public event System.Action OnPlayerDeath;

    public float speed = 6;
    public float smoothTime = .2f;
    public float detractionForce = 3;
    public float detractionRadius = 3;
    Vector2 currV;
    Vector2 smoothV;
    public LayerMask missileMask;

    Dodger target;
    Collider2D myCol;

	// Use this for initialization
	void Start () {
        target = FindObjectOfType<Dodger>();
        myCol = GetComponent<Collider2D>();
	}
	
	// Update is called once per frame
	void Update () {

        Vector2 detractDir = Vector2.zero;
        Collider2D[] c = Physics2D.OverlapCircleAll(transform.position, detractionRadius, missileMask);
        float detractForceFactor = 0;
        foreach (Collider2D otherCol in c)
        {
            if (otherCol != myCol)
            {
                Vector2 offset = -(otherCol.transform.position - transform.position);
                detractDir += offset.normalized / offset.sqrMagnitude;
                detractForceFactor += detractionRadius / offset.magnitude;
            }
        }
        detractDir.Normalize();
        detractForceFactor = Mathf.Clamp01(detractForceFactor);

        float dstToTarget = (target.transform.position - transform.position).magnitude;
		Vector2 targetPoint = EstimatePointOfImpact();
        Vector2 dirToTarget = (targetPoint - (Vector2)transform.position).normalized;
        float currSmoothTime = Mathf.Lerp(0,smoothTime, (dstToTarget - .3f));


        Vector2 targetDir = (dirToTarget * speed + detractDir * detractionForce * detractForceFactor).normalized;
        Vector2 targetV = targetDir * speed;
 
        currV = Vector2.SmoothDamp(currV, targetV, ref smoothV, currSmoothTime, float.MaxValue, Time.deltaTime);

        transform.Translate(currV * Time.deltaTime,Space.World);
       
	}


    Vector2 EstimatePointOfImpact()
    {
        int iterations = 4;
        Vector2 initialTargetPos = target.transform.position;
        Vector2 targetPos = initialTargetPos;
        Vector2 targetDir = target.currV.normalized;

		for (int i = 0; i < iterations; i++)
        {
            float dstToTarget = ((Vector2)transform.position - targetPos).magnitude;
			float timeToImpact = dstToTarget / speed;
            targetPos = initialTargetPos + targetDir * timeToImpact;
        }

        return targetPos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Missile")
        {
            float dstToPlayer = (target.transform.position - transform.position).magnitude;
            bool killPlayer = dstToPlayer < .5f;
            Explode(killPlayer);
        }
        else if (collision.tag == "Player")
        {
            Explode(true);
        }
    }

    void Explode(bool killPlayer)
    {
        if (killPlayer)
        {
            if (OnPlayerDeath != null)
            {
                OnPlayerDeath();
            }
        }

        if (OnDeath != null)
        {
            OnDeath();
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
           // Gizmos.DrawSphere(EstimatePointOfImpact(), .1f);

        }
    }
}
