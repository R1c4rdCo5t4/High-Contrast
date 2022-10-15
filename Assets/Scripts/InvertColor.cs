using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class InvertColor : MonoBehaviour
{
    Transform[] objects;

    void Start(){
        objects = GetComponentsInChildren<Transform>();
    }


    Color getCurrentColor(Color prevColor, float alpha = 1f) => new Color(1f - prevColor.r, 1f - prevColor.g, 1f - prevColor.b, prevColor.a);


    public void Invert(){
        foreach (Transform obj in objects){
            invertObject(obj.gameObject);
        }
    }


    public void invertObject(GameObject obj){

        if (obj.tag == "Non-Invertable") return;

        if (obj.tag == "Spike" || obj.tag == "Platform"){

            SpriteRenderer sr_ = obj.GetComponent<SpriteRenderer>();
            sr_.enabled = !sr_.enabled;

            BoxCollider2D bc_ = obj.GetComponent<BoxCollider2D>();
            bc_.enabled = !bc_.enabled;
            return;
            
        }
       
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if(sr != null) sr.color = getCurrentColor(sr.color);
        

        TrailRenderer tr = obj.GetComponent<TrailRenderer>();
        if(tr != null){
            tr.startColor = getCurrentColor(tr.startColor, tr.startColor.a);
            tr.endColor = getCurrentColor(tr.endColor, tr.endColor.a);
        }


        Image img = obj.GetComponent<Image>();
        if(img != null) img.color = getCurrentColor(img.color);
        

        Text text = obj.GetComponent<Text>();
        if(text != null) text.color = getCurrentColor(text.color);
        

        ParticleSystem particles = obj.GetComponent<ParticleSystem>();
        if(particles != null){
            ParticleSystem.MainModule psMain = particles.main;
            psMain.startColor = getCurrentColor(psMain.startColor.color);
        }
        

        Camera cam = obj.GetComponent<Camera>();
        if(cam != null) cam.backgroundColor = getCurrentColor(cam.backgroundColor);
         
    }

  

}
