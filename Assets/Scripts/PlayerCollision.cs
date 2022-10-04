using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class PlayerCollision : MonoBehaviour
{
    PlayerController ps;
    GameManager gm;
    InvertColor ic;

    GameObject go;
    Vector3 defaultRotation;
    Rigidbody2D rb;
    ParticleSystem particles;
    TimeManager tm;
    SpriteRenderer[] sprites;

    bool dead = false;

    [SerializeField] Transform respawnPoint;
    [SerializeField] LayerMask explosionLayer;


    void Start(){
        ps = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        sprites = GetComponentsInChildren<SpriteRenderer>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        tm = GameObject.Find("TimeManager").GetComponent<TimeManager>();
        ic = GameObject.Find("LevelManager").GetComponent<InvertColor>();
        particles = GameObject.Find("Explosion").GetComponent<ParticleSystem>();
        defaultRotation = transform.eulerAngles;
    }

    
    void OnCollisionEnter2D(Collision2D collision){
        ProcessCollision(collision.gameObject);
    }

    void OnCollisionExit2D(Collision2D collision){

        if(collision.gameObject.CompareTag("MovingPlatform")){
  
            // ps.activeMovespeed = ps.movementSpeed;
        }
    
    }

    
    void OnTriggerEnter2D(Collider2D collider){

       ProcessCollision(collider.gameObject);
    }

    void OnTriggerExit2D(Collider2D collider){
        
    }

    void ProcessCollision(GameObject collider){

        switch(collider.tag){
            case "Reverter":
                ic.Invert();
                collider.SetActive(false);
                
                gm.darkMode = !gm.darkMode;
                go = collider;

                StartCoroutine(restoreObject(go,2f));
                break;
            case "HyperDash":
                HyperDash hyperDash = collider.gameObject.GetComponent<HyperDash>();
                if((Mathf.Sign(hyperDash.dir.x) == ps.facing) || (hyperDash.dir.y > 0f)){
                    ps.Dash(hyperDash.dir, hyperDash.speed, hyperDash.duration, true, hyperDash.gravity);
                }
                

                if(hyperDash.changeFacing){
                    ps.flip();
                }
                break;

            case "CrystalDash":

                SpriteRenderer sprite = collider.gameObject.GetComponent<SpriteRenderer>();
                if(!sprite.enabled) return;
                ParticleSystem partSystem = collider.gameObject.GetComponent<ParticleSystem>();
   
                var main = partSystem.main;
                main.startSpeed = tm.isTryingToSlow ? 5f : 20f; // set speed to slow-mo manually 
               
                partSystem.Stop();
                partSystem.Play();

                sprite.enabled = false;
                ps.canMove = false;
                ps.canDash = true;
                rb.gravityScale = 0.5f;
                Vector2 direction = collider.transform.position - transform.position;
                rb.AddForce(direction * 30f, ForceMode2D.Impulse);

                StartCoroutine(restoreMovement(0.2f));
                StartCoroutine(restoreComponent(sprite, 2f));
                break;
            
            case "Spike":
            case "Death":

                if(dead) break;
                

                foreach(SpriteRenderer sr in sprites){
                    if(sr.gameObject.name != "Background"){
                        sr.enabled = false;
                    } 
                }

                rb.constraints = RigidbodyConstraints2D.FreezePositionX | 
                                RigidbodyConstraints2D.FreezePositionY | 
                                RigidbodyConstraints2D.FreezeRotation;
            
                ps.canMove = false;
                ps.activeMovespeed = 0f;
                rb.velocity = Vector2.zero;
                particles.Play();
                dead = true;
                
                // tm.SlowMotion(0.1f,1.5f);
                StartCoroutine(newGame());   
                break;

            default: break;

        }
     

    }



    IEnumerator restoreObject(GameObject gameObject, float duration){
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(true);
        
        
    }
    IEnumerator restoreMovement(float duration){
        yield return new WaitForSeconds(duration);
        ps.canMove = true;
        rb.gravityScale = ps.initialGravity;
        
    }

    
    IEnumerator restoreComponent(SpriteRenderer component, float duration){
        yield return new WaitForSeconds(duration);
        component.enabled = true;  
    }


    IEnumerator newGame(){
        yield return new WaitForSeconds(1.5f);
        transform.position = respawnPoint.position;
        ps.canMove = true;
        foreach(SpriteRenderer sr in sprites){
            if(sr.gameObject.name != "Background"){
                sr.enabled = true;
            } 
        }
        dead = false;
        rb.constraints = RigidbodyConstraints2D.None;
        particles.Stop();

    }    

}
