using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering.Universal;

public class TimeManager : MonoBehaviour
{
    // public float slowDownFactor = 0.05f;
    float slowDownLength = 2f;
    float defaultTimeScale;

    Vignette vig;
    [SerializeField] float vigValue;
    public float vignetteMultiplier = 4f;

    public bool slowing = false;
    bool applyVignette = false;

    float slowDownTimer = 0f;

    void Start()
    {
        defaultTimeScale = Time.timeScale;
        vig = GameObject.Find("LevelManager").GetComponent<InvertColor>().vignette;
        vigValue = vig.intensity.value;
        print(vigValue);
    }

    
    void Update()
    {
        
        var slowDownScale = slowing ? slowDownLength : 0.5f;
        Time.timeScale += (1f/slowDownScale) * Time.unscaledDeltaTime; 
        Time.timeScale = Mathf.Clamp(Time.timeScale,0,1f);

        if(slowDownTimer > 0 && slowing){
            slowDownTimer -= Time.deltaTime;
            if(applyVignette && vig.intensity.value < vigValue){
                vig.intensity.value += vignetteMultiplier * (Time.deltaTime*10); 
            }
        }
        else{
            if(vig.intensity.value > 0){
                vig.intensity.value -= vignetteMultiplier * (Time.deltaTime*5); 
            }
        }   
    }
    

    public void SlowMotion(float slowDownFactor, float slowDownLen, bool vignette = false){
        Time.timeScale = slowDownFactor;
        slowing = true;
        slowDownLength = slowDownLen;
        slowDownTimer = slowDownLen;
        applyVignette = vignette;        
    }


}






