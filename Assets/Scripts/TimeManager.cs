using UnityEngine;
using UnityEngine.PostProcessing;

public class TimeManager : MonoBehaviour
{

    public const float DEFAULT_FIXED_DELTA_TIME = 1.0f / 60.0f;

    /* slow down factor at which the temp is set to minimum */
    [SerializeField]
    private float slowDownMinThreshold;
    /* minimum contrast value */
    [SerializeField]
    private float minimumTemperature;

    private float slowDownFactor;
    private float slowDownDuration;
    /* how great is the rate of change from slow down to normal */
    private float exponentialGrowthFactor;

    private float elapsedUnscaled;

    private PostProcessingProfile postProcessing;

    //must do this in start so camera is initialized
    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        slowDownDuration = 0.0f;
        slowDownFactor = 0.0f;
        elapsedUnscaled = 0.0f;
        exponentialGrowthFactor = 0.0f;

        SetTimeScale(1.0f);
    }

    void Update()
    {
        if (slowDownFactor != 0.0f && slowDownDuration != 0.0f)
        {
            elapsedUnscaled += Time.unscaledDeltaTime;
            if (elapsedUnscaled >= slowDownDuration)
            {
                Initialize();
            }
            else
            {
                SetTimeScale(Mathf.Clamp(CalculateTimeScale(), 0.0f, 1.0f));
                Time.fixedDeltaTime = Mathf.Clamp(
                    DEFAULT_FIXED_DELTA_TIME * Time.timeScale, 0.0f, DEFAULT_FIXED_DELTA_TIME);
            }
        }
    }

    public void SetSlowDownFactor(float slowDownFactor, float slowDownDuration, float exponentialGrowthFactor)
    {
        this.slowDownDuration = slowDownDuration;
        this.slowDownFactor = slowDownFactor;
        this.exponentialGrowthFactor = exponentialGrowthFactor;

        SetTimeScale(slowDownFactor);

        elapsedUnscaled = 0.0f;
    }

    private float CalculateTimeScale()
    {
        return slowDownFactor + (1.0f - slowDownFactor) * NormalizedExponentialFraction(exponentialGrowthFactor);
    }

    /* elapsedFraction = elapsedTime / slowDownFactor , returns 0 if elapsedTime is 0, 1 if elapsedTime = slowDownFactor,
     increases from 0 to 1 exponentially. */
    private float NormalizedExponentialFraction(float b)
    {
        return (Mathf.Pow(b, elapsedUnscaled / slowDownDuration) - 1.0f) / (b - 1.0f);
    }

    public void ResetTimeScale()
    {
        SetTimeScale(1.0f);
    }

    private void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = DEFAULT_FIXED_DELTA_TIME * timeScale;

        postProcessing = Camera.main.GetComponent<PostProcessingBehaviour>().profile;
        ColorGradingModel.Settings settings = postProcessing.colorGrading.settings;
        settings.basic.temperature = GetTemperature();

        postProcessing.colorGrading.settings = settings;
    }

    private float GetTemperature()
    {
        //add proper range
        return Mathf.Lerp(minimumTemperature, 0.0f, (Time.timeScale - slowDownMinThreshold) / (1.0f - slowDownMinThreshold));
    }
}
