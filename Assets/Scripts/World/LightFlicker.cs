using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{
    [Header("Intensity")]
    public float minIntensity = 0.5f;
    public float maxIntensity = 1.2f;

    [Header("Timing")]
    public float minFlickerTime = 0.03f;
    public float maxFlickerTime = 0.12f;

    [Header("Off Flickers")]
    [Range(0f, 1f)]
    public float chanceToTurnOff = 0.1f;
    public float maxOffTime = 0.08f;

    private Light lightSource;
    private float timer;

    private void Awake()
    {
        lightSource = GetComponent<Light>();
        ScheduleNextFlicker();
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            Flicker();
            ScheduleNextFlicker();
        }
    }

    private void Flicker()
    {
        if (Random.value < chanceToTurnOff)
        {
            StartCoroutine(TurnOffBriefly());
        }
        else
        {
            lightSource.intensity = Random.Range(minIntensity, maxIntensity);
        }
    }

    private System.Collections.IEnumerator TurnOffBriefly()
    {
        float previousIntensity = lightSource.intensity;

        lightSource.enabled = false;
        yield return new WaitForSeconds(Random.Range(0.01f, maxOffTime));
        lightSource.enabled = true;

        lightSource.intensity = Random.Range(minIntensity, maxIntensity);
    }

    private void ScheduleNextFlicker()
    {
        timer = Random.Range(minFlickerTime, maxFlickerTime);
    }
}