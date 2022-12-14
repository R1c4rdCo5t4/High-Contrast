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


    public void Invert(){
        foreach (Transform obj in objects){
            invertColor(obj.gameObject);
        }
    }



    public void invertColor(GameObject obj){

        switch(obj.tag){
            case "Non-Invertable": break;
            case "Spike":
            case "Platform":
                SpriteRenderer sr_ = obj.GetComponent<SpriteRenderer>();
                sr_.enabled = !sr_.enabled;

                BoxCollider2D bc_ = obj.GetComponent<BoxCollider2D>();
                bc_.enabled = !bc_.enabled;
                break;

            default:
                SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                if (sr != null){
                    sr.color = ColorManager.invertColor(sr.color);
                    break;
                }

                TrailRenderer tr = obj.GetComponent<TrailRenderer>();
                if(tr != null){
                    tr.startColor = ColorManager.invertColor(tr.startColor, tr.startColor.a);
                    tr.endColor = ColorManager.invertColor(tr.endColor, tr.endColor.a);
                    break;
                }


                Image img = obj.GetComponent<Image>();
                if(img != null){
                    img.color = ColorManager.invertColor(img.color);
                    break;
                } 
                

                Text text = obj.GetComponent<Text>();
                if(text != null){
                    text.color = ColorManager.invertColor(text.color);
                    break;
                }

                ParticleSystem particles = obj.GetComponent<ParticleSystem>();
                if(particles != null){
                    ParticleSystem.MainModule psMain = particles.main;
                    psMain.startColor = ColorManager.invertColor(psMain.startColor.color);
                    break;
                }
                

                Camera cam = obj.GetComponent<Camera>();
                if(cam != null){
                    cam.backgroundColor = ColorManager.invertColor(cam.backgroundColor);
                    break;
                } 
                
                break;
        }
       
       
       
         
    }

}
