using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class FlickeringLight : MonoBehaviour
{
    [SerializeField] Light2D light;
    [Tooltip("How often should the light flicker?")]
    [SerializeField] [Range(0f, 100f)] float flickerPosibility = 3f;
    [Tooltip("How much should the light flicker vary?")]
    [SerializeField] [Range(0f, 100f)] float flickerAmount = 3f;
    [Tooltip("What kind of light is it?")]
    [SerializeField] LightType lightType = LightType.flame;
    [Tooltip("Should the radius be affected as well?")]
    [SerializeField] bool affectRadius = false;

    [SerializeField] float defaultIntensity;
    [SerializeField] float defaultInnerRadius;
    [SerializeField] float defaultOuterRadius;

    public enum LightType
    {
        electrical,
        flame
    }

    private void Awake()
    {
        if (light == null)
        {
            light = GetComponent<Light2D>();
        }
        defaultIntensity = light.intensity;
        defaultInnerRadius = light.pointLightInnerRadius;
        defaultOuterRadius = light.pointLightOuterRadius;
    }

    private void FixedUpdate()
    {
        float randomNum = Random.Range(0f, 100f);

        if (flickerPosibility > randomNum)
        {
            Flicker();
        }
    }

    void Flicker()
    {
        if (lightType == LightType.flame)
        {
            light.intensity = Mathf.Lerp(light.intensity, Random.Range(defaultIntensity - (flickerAmount / 100f) * defaultIntensity, defaultIntensity + (flickerAmount / 100f) * defaultIntensity), Time.deltaTime);
            //light.intensity = Random.Range(defaultIntensity - (flickerAmount / 100f) * defaultIntensity, defaultIntensity + (flickerAmount / 100f) * defaultIntensity);
            if (affectRadius)
            {
                light.pointLightInnerRadius = Mathf.Lerp(light.pointLightInnerRadius, Random.Range(defaultInnerRadius - (flickerAmount / 100f) * defaultInnerRadius, defaultInnerRadius + (flickerAmount / 100f) * defaultInnerRadius), Time.deltaTime);
                light.pointLightOuterRadius = Mathf.Lerp(light.pointLightOuterRadius, Random.Range(defaultOuterRadius - (flickerAmount / 100f) * defaultOuterRadius, defaultOuterRadius + (flickerAmount / 100f) * defaultOuterRadius), Time.deltaTime);
            }
        }
        else if (lightType == LightType.electrical)
        {
            light.intensity = Random.Range(defaultIntensity - (flickerAmount / 100f) * defaultIntensity, defaultIntensity + (flickerAmount / 100f) * defaultIntensity);
            if (affectRadius)
            {
                light.pointLightInnerRadius = Random.Range(defaultInnerRadius - (flickerAmount / 100f) * defaultInnerRadius, defaultInnerRadius + (flickerAmount / 100f) * defaultInnerRadius);
                light.pointLightOuterRadius = Random.Range(defaultOuterRadius - (flickerAmount / 100f) * defaultOuterRadius, defaultOuterRadius + (flickerAmount / 100f) * defaultOuterRadius);
            }
        }
    }
}
