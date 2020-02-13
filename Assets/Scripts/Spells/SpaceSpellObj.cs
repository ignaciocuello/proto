using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceSpellObj : MonoBehaviour {

    private const float EPS = 1.0f;

    public float OriginalY;

    public float EaseInRate;
    public float HoldDuration;
    public float FadeOutRate;

    public float TargetZoom;

    private float trgt;

    private void Awake()
    {
        StartCoroutine(PlayEffect());
    }

    private void Update()
    {
        float oldY = Camera.main.transform.position.y;
        if (trgt == TargetZoom)
        {
            Camera.main.transform.parent.position += new Vector3(0.0f, Mathf.Lerp(oldY, trgt, EaseInRate) - oldY);
        }
        else
        {
            Camera.main.transform.parent.position += new Vector3(0.0f, Mathf.Lerp(oldY, trgt, FadeOutRate) - oldY);
        }
    }

    IEnumerator PlayEffect()
    {
        //ease in
        trgt = TargetZoom;
        yield return new WaitUntil(() => FloatEq(Camera.main.transform.position.y, trgt));

        //hold
        yield return new WaitForSeconds(HoldDuration);

        //fade out
        trgt = OriginalY;
        yield return new WaitUntil(() => FloatEq(Camera.main.transform.position.y, trgt));

        Destroy(gameObject);
    }

    private static bool FloatEq(float a, float b)
    {
        return Mathf.Abs(a - b) <= EPS;
    }
}
