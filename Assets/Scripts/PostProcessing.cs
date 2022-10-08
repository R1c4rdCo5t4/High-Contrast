using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering.Universal;
 
public class PostProcessing : MonoBehaviour
{
   
    float maxChromaticIntensity = 0.4f; // .32f
    float minChromaticIntensity = 0.1f;
    float chromaticMultiplier = 10f;
    UnityEngine.Rendering.Universal.ChromaticAberration chromaticAberration;
    

    PlayerController ps;
 
    SpriteRenderer[] zones;

 
    void Start()
    {
        ps = GameObject.Find("Player").GetComponent<PlayerController>();
        UnityEngine.Rendering.VolumeProfile profile = GetComponent<UnityEngine.Rendering.Volume>().profile;
        profile.TryGet(out chromaticAberration);

        zones = GameObject.Find("Infinite Dash Zones").GetComponentsInChildren<SpriteRenderer>();
        
 
    }
 
    void Update()
    {   
        var acc = chromaticMultiplier * Time.deltaTime;
        if(ps.inInfiniteDashZone){
            
            if(chromaticAberration.intensity.value + acc < maxChromaticIntensity){
                chromaticAberration.intensity.value += acc; 
            }
            
        }
        else{ 
            if(chromaticAberration.intensity.value - acc > minChromaticIntensity){
                chromaticAberration.intensity.value -= acc; 
            }
            
            
        }   
       
    }
 
   
}