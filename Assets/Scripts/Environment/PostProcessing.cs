using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PostProcessing : MonoBehaviour {
    PlayerController ps;
    GameManager gm;

    [Header("Chromatic Aberration")]
    [SerializeField] float maxChromaticIntensity = 0.4f; // .32f
    [SerializeField] float minChromaticIntensity = 0.1f;
    [SerializeField] float chromaticMultiplier = 1f;
    ChromaticAberration chromaticAberration;

    [Header("Vignette")]
    [SerializeField] float maxVigValue = 0.25f;
    [SerializeField] float minVigValue = 0.15f;
    [SerializeField] float vignetteMultiplier = 1f;
    Vignette vignette;


    void Start(){
        ps = GameObject.Find("Player").GetComponent<PlayerController>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        UnityEngine.Rendering.VolumeProfile profile = GetComponent<UnityEngine.Rendering.Volume>().profile;
        profile.TryGet<ChromaticAberration>(out chromaticAberration);
        profile.TryGet<Vignette>(out vignette);
    }

    void Update(){
        vignette.intensity.value = valueChanger(vignetteMultiplier * Time.deltaTime, vignette.intensity.value, minVigValue, maxVigValue, gm.tm.isSlowing);
    }

    float valueChanger(float mult, float value, float min, float max, bool cond) => (cond) ? value < max ? value + mult : max : (value > min) ? value - mult : min;
}
