using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {

	public event System.Action OnDeath;
	public event System.Action OnPlayerDeath;

    public GameObject explosion;
    public float speed = 6;
    public float slerpSpeed = 4;
    //public float maxSlerpSpeed = 10;
    public float detractionForce = 3;
    public float detractionRadius = 3;
    public float targetNoise = .2f;
    public float retargetTime = 1;
    public float retargetThresholdDst = 2;
    public float timeBetweenRetargets = 8;
    public float circleTimeBeforeRetarget = 1;
    float lastRetargetTime;
    float timeCircling;
    public float timeBetweenTargetUpdates = .1f;
    Vector2 retargetDir;
    float endRetargetTime;
    public float noiseFac = .3f;
    Vector2 screenMinMaxX;
    Vector2 screenMinMaxY;

    Vector2 currV;
    Vector2 smoothV;
    public LayerMask missileMask;

    Dodger target;
    Collider2D myCol;
    Vector2 targetPoint;
    float nextUpdateTime;

	// Use this for initialization
	void Start () {
        target = FindObjectOfType<Dodger>();
        myCol = GetComponent<Collider2D>();
		screenMinMaxX = FindObjectOfType<ScreenAreas>().minMaxX;
		screenMinMaxY = FindObjectOfType<ScreenAreas>().minMaxY;
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

        Vector2 targetDir = Vector2.zero;
        if (Time.time > endRetargetTime)
        {
            if (Time.time > nextUpdateTime)
            {
                nextUpdateTime = Time.time + timeBetweenTargetUpdates;
				targetPoint = EstimatePointOfImpact();
            }
            float dstToTarget = (target.transform.position - transform.position).magnitude;
           
            Vector2 dirToTarget = (targetPoint - (Vector2)transform.position).normalized;

            float angleToTarget = Vector2.Angle(transform.up, dirToTarget);
            if (angleToTarget > 90-30 && angleToTarget < 90+30)
            {
                timeCircling += Time.deltaTime;
            }
            else
            {
                timeCircling = 0;
            }
            if (dstToTarget < retargetThresholdDst && timeCircling > circleTimeBeforeRetarget && Time.time-lastRetargetTime>timeBetweenRetargets)
            {
               // print("Send: " + timeCircling);
                retargetDir = Random.insideUnitCircle.normalized;
                endRetargetTime = Time.time + retargetTime;
                lastRetargetTime = endRetargetTime;
                timeCircling = 0;

            }
           
            //float currSmoothTime = Mathf.Lerp(0,slerpSpeed, (dstToTarget - .3f));
            // float currSlerpSpeed = Mathf.Lerp(maxSlerpSpeed, slerpSpeed, (dstToTarget - .3f));

            targetDir = (dirToTarget * speed + detractDir * detractionForce * detractForceFactor).normalized;
        }
        else
        {
            
            targetDir = retargetDir;
        }
        Vector2 targetV = targetDir * speed;

        //currV = Vector2.SmoothDamp(currV, targetV, ref smoothV, currSmoothTime, float.MaxValue, Time.deltaTime);
        currV = Vector3.Slerp(currV, targetV, Time.deltaTime * slerpSpeed);
       // currV = targetV;
        //print(Time.deltaTime * smoothTime);
        transform.up = currV.normalized;
        transform.Translate(currV * Time.deltaTime,Space.World);
       
	}


    Vector2 EstimatePointOfImpact()
    {
        //return target.transform.position;
        Vector2 initialTargetPos = target.transform.position;
        Vector2 estimatedImpactPos = initialTargetPos;
        Vector2 targetDir = target.targetV.normalized;

        float dstToTarget = ((Vector2)transform.position - estimatedImpactPos).magnitude;
		float timeToImpact = dstToTarget / speed;
        //print(dstToTarget + " t: " + timeToImpact);
        estimatedImpactPos = initialTargetPos + targetDir * timeToImpact * target.speed;
		estimatedImpactPos = new Vector2(Mathf.Clamp(estimatedImpactPos.x, screenMinMaxX.x, screenMinMaxX.y), Mathf.Clamp(estimatedImpactPos.y, screenMinMaxY.x, screenMinMaxY.y));

		Vector2 myPos = transform.position;
        float angle = Vector2.Angle((estimatedImpactPos - myPos), (initialTargetPos - myPos));
       
        Vector2 noise = Random.insideUnitCircle * noiseFac;
        if (angle > 90)
        {
            return initialTargetPos + noise;
        }


        return estimatedImpactPos + noise;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*
        if (collision.tag == "Missile")
        {
            float dstToPlayer = (target.transform.position - transform.position).magnitude;
            bool killPlayer = dstToPlayer < .5f;
            Explode(killPlayer);
        }
        */
        if (collision.tag == "Player")
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

        Destroy(Instantiate(explosion, transform.position, Quaternion.identity), 2);

        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            //Gizmos.DrawSphere(EstimatePointOfImpact(), .1f);
            Gizmos.DrawSphere(targetPoint, .1f);

        }
    }
}
