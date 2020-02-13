using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeScript : MonoBehaviour {

    /* base  line force magnitude used for particle system emission quantity */
    [SerializeField]
    private float baseLineForceMag = 20.0f;
    [SerializeField]
    private int baseLineEmissionRate = 100;

    [SerializeField]
    private Color color;
    public Color Color
    {
        get { return color; }
        set
        {
            color = value;
            GetComponent<MeshRenderer>().material.color = color;
        }
    }

    [SerializeField]
    protected Vector3 relativeForce;
    public Vector3 RelativeForce
    {
        get { return relativeForce; }
        set
        {
            relativeForce = value;

            //orient particle system to match the relative force direction/magnitude
            particleSys.transform.localPosition = -0.5f * relativeForce.normalized;
            particleSys.transform.LookAt(transform.position);
            particleSys.transform.rotation *= Quaternion.AngleAxis(180.0f, Vector3.up);
            
            //TODO: change later as this is depracated
            particleSys.emissionRate = (relativeForce.magnitude/baseLineForceMag) * baseLineEmissionRate;
        }
    }

    public float BurstDuration;
    public float BurstDelay;

    [SerializeField]
    protected ParticleSystem particleSys;

    [SerializeField]
    protected float delayPeriod;

    /* prevents this cube from sticking to other cubes */
    public bool PreventSticky;

    private bool active;

    private bool inBurst;
    private bool inForce;

    public void Awake()
    {
        //make material instance unique
        MeshRenderer mr = GetComponent<MeshRenderer>();
        Material mat = mr.material;
        mr.material = new Material(mat);

        StartCoroutine(DelayedActivation());
    }

    IEnumerator DelayedActivation()
    {
        yield return new WaitForSeconds(delayPeriod);
        active = true;
    }

    public void OnValidate()
    {
        RelativeForce = relativeForce;
    }

    public void FixedUpdate()
    {
        if (active)
        {
            if (!inBurst)
            {
                StartCoroutine(Burst());
            }
            else if (inForce)
            {
                //SEE IF this causes issues, used to be GetComponent<Rigidbody>
                GetComponentInParent<Rigidbody>().AddForceAtPosition(transform.TransformVector(relativeForce), 
                    transform.position, ForceMode.Force);
            }
        }
    }

    /*
     * perform burst, by applying a force for "burstDuration" seconds and then waiting "burstDelay" seconds until bursting
     * again */
    IEnumerator Burst()
    {
        inBurst = true;

        //apply force
        inForce = true;
        //display trail
        particleSys.Play();
        yield return new WaitForSeconds(BurstDuration);
        inForce = false;
        //stop displaying trail
        particleSys.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        //wait a bit until next burst
        yield return new WaitForSeconds(BurstDelay);
        inBurst = false;
    }
}
