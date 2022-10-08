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
    float slowDownFactor;
    float slowDownTimer = 0f;
    float defaultTimeScale;

    Vignette vig;
    float maxVigValue = 0.23f;
    float minVigValue = 0.13f;
    [SerializeField] float particleSpeed;
    public float vignetteMultiplier = 10f;

    public bool isTryingToSlow = false;
    bool isSlowing = false;
    bool applyVignette = false;
    bool canRestoreSlow;
    SlowMoBar slowBar;

    void Start()
    {
        defaultTimeScale = Time.timeScale;
        vig = GameObject.Find("LevelManager").GetComponent<InvertColor>().vignette;
        slowBar = GameObject.Find("SlowMoBar").GetComponent<SlowMoBar>();
        // vigValue = vig.intensity.value;
   
    }

    void FixedUpdate(){
        
        if(isSlowing){
            slowBar.takeSlow();
        }
        else{
            if(canRestoreSlow && !isTryingToSlow){
                slowBar.restoreSlow();
            }
            else{
                Invoke("restoreSlow", 2f);
            }
            
        }
    }

    
    void Update()
    {
        isSlowing = slowDownTimer > 0 && isTryingToSlow && slowBar.currentSlow > 0;
        var slowDownScale = isSlowing ? slowDownLength : 0.5f;
        Time.timeScale += (1f/slowDownScale) * Time.unscaledDeltaTime; 
        Time.timeScale = Mathf.Clamp(Time.timeScale,0,1f);


        if(isSlowing){ // slowing
            slowDownTimer -= Time.deltaTime;
            if(applyVignette && vig.intensity.value < maxVigValue){
                vig.intensity.value += vignetteMultiplier * (Time.deltaTime*10); 
            }
            canRestoreSlow = false;
        }
        else{  // not slowing
            if(vig.intensity.value > minVigValue){
                vig.intensity.value -= vignetteMultiplier * (Time.deltaTime*5); 
            }
            

            
        }   
    }
    

    public void SlowMotion(float slowDownStrength, float slowDownLen, bool vignette = false){
        isTryingToSlow = true;
        if(slowBar.currentSlow <= 0) return;
        Time.timeScale = slowDownStrength;
        slowDownLength = slowDownLen;
        slowDownTimer = slowDownLen;
        applyVignette = vignette;        
    }


    void restoreSlow(){
        canRestoreSlow = true;
    }


}






