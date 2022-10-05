using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering.Universal;

public class InvertColor : MonoBehaviour
{
    Transform[] objects;
    public Volume volume;
    Bloom bloom;
    public Vignette vignette; 
    GameManager gm;
   

    void Start()
    {
        objects = GetComponentsInChildren<Transform>();
        volume.profile.TryGet<Bloom>(out bloom);
        bloom.tint.Override(Color.white);
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        volume = GameObject.Find("Volume").GetComponent<Volume>();
        volume.profile.TryGet<Vignette>(out vignette);
        
    }

    Color getCurrentColor(Color prevColor, float alpha=1f) => new Color(1f-prevColor.r, 1f-prevColor.g, 1f-prevColor.b, prevColor.a);

    public void Invert(){
        foreach(Transform obj in objects){
            invertObject(obj.gameObject);
        }
    }

    
    public void invertObject(GameObject obj){
        
        if(obj.tag == "Non-Invertable"){
                return;
        }
       
        if(obj.tag == "Spike" || obj.tag == "Platform"){

            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            sr.enabled = !sr.enabled;

            BoxCollider2D bc = obj.GetComponent<BoxCollider2D>();
            bc.enabled = !bc.enabled;
            
            return;
        }


     
        if(obj.GetComponent<SpriteRenderer>() != null){
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            sr.color = getCurrentColor(sr.color);
            
        }

        if(obj.GetComponent<TrailRenderer>() != null){
            TrailRenderer tr = obj.GetComponent<TrailRenderer>();
            tr.startColor = getCurrentColor(tr.startColor,tr.startColor.a); 
            tr.endColor = getCurrentColor(tr.endColor, tr.endColor.a);
        }  
        

        if(obj.GetComponent<Image>() != null){
            Image img = obj.GetComponent<Image>();
            img.color = getCurrentColor(img.color);
        }    

        if(obj.GetComponent<Text>() != null){
            Text text = obj.GetComponent<Text>();
            text.color = getCurrentColor(text.color); 

        }    

        if(obj.GetComponent<ParticleSystem>() != null){
            ParticleSystem particles = obj.GetComponent<ParticleSystem>();
            particles.startColor = getCurrentColor(particles.startColor);
               
        }   

        if(obj.GetComponent<Camera>() != null){
            Camera cam = obj.GetComponent<Camera>();
            cam.backgroundColor = getCurrentColor(cam.backgroundColor);
        }
    }
}
