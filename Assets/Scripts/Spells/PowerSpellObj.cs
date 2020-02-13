using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PowerSpellObj : MonoBehaviour {

    private const float EPS = 1.0f;

    public float EaseInRate;
    public float HoldDuration;
    public float FadeOutRate;

    public float TargetTemp;

    private float trgt;

    [SerializeField]
    private CameraShakeSettings destroyCamShakeSettings;

    private PostProcessingProfile postProcessing;

    private void Awake()
    {
        postProcessing = Camera.main.GetComponent<PostProcessingBehaviour>().profile;
        StartCoroutine(PlayEffect());
    }

    private void Update()
    {
        ColorGradingModel.Settings settings = postProcessing.colorGrading.settings;
        if (trgt == TargetTemp)
        {
            settings.basic.temperature = Mathf.Lerp(settings.basic.temperature, trgt, EaseInRate);
        }
        else
        {
            settings.basic.temperature = Mathf.Lerp(settings.basic.temperature, trgt, FadeOutRate);
        }

        postProcessing.colorGrading.settings = settings;
    }

    IEnumerator PlayEffect()
    {
        //shake cam
        destroyCamShakeSettings.Shake();

        //ease in
        trgt = TargetTemp;
        yield return new WaitUntil(() => FloatEq(postProcessing.colorGrading.settings.basic.temperature, trgt));

        //hold
        yield return new WaitForSeconds(HoldDuration);

        //fade out
        trgt = 0.0f;
        yield return new WaitUntil(() => FloatEq(postProcessing.colorGrading.settings.basic.temperature, trgt));

        Destroy(gameObject);
    }

    private static bool FloatEq(float a, float b)
    {
        return Mathf.Abs(a - b) <= EPS;
    }
}
