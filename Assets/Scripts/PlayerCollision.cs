using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class PlayerCollision : MonoBehaviour
{
    PlayerController ps;
    GameManager gm;
    GameObject go;
    Vector3 defaultRotation;
    Rigidbody2D rb;
    ParticleSystem particles;
    SpriteRenderer[] playerSprites;
    [SerializeField] Transform respawnPoint;



    void Start()
    {
        ps = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        playerSprites = GetComponentsInChildren<SpriteRenderer>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        particles = GameObject.Find("Explosion").GetComponent<ParticleSystem>();
        defaultRotation = transform.eulerAngles;
    }



    void OnCollisionEnter2D(Collision2D collision)
    {

        if (ps.inHyperDashZone){
            ps.hyperDashForce = Vector2.zero;
            rb.velocity = Vector2.zero;
        }

        ProcessCollision(collision.gameObject);
    }

    void OnCollisionExit2D(Collision2D collision)
    {

    }


    void OnTriggerEnter2D(Collider2D collider)
    {

        ProcessCollision(collider.gameObject);
        
    }

    void OnTriggerExit2D(Collider2D collider)
    {

        switch (collider.tag){
            case "Booster":
                
                StartCoroutine(executeAfterSeconds(0.5f, () => { 
                    changeBoosterOpacity(collider.gameObject, 0.4f);
                    rb.gravityScale = ps.initialGravity;
                    }));
                break;
            
            case "BoosterZone": StartCoroutine(executeAfterSeconds(2.5f, () => ps.isBoosting = false)); break;

            default: break;
        }
    }

    void ProcessCollision(GameObject collider)
    {
        
        switch (collider.tag){
            case "Ground": ps.isInSlope = collider.transform.eulerAngles.z != 0 && !ps.isTouchingWall; break;
            case "Reverter": processReverter(collider); break;
            case "CrystalDash": processCrystalDash(collider); break;
            case "Spike":
            case "Death": processDeath(); break;
            case "ZoneIn":
                ps.inHyperDashZone = true;
                ps.enterhyperDashZone();
                break;
            case "ZoneOut":
                ps.inHyperDashZone = false;
                ps.exithyperDashZone();
                break;


            case "Booster": 
                processBooster(collider);
                break;

            case "BoosterZone":
                ps.isBoosting = true; 
                rb.gravityScale *= 0.5f;
                break;

            default: break;

        }


    }

    void processReverter(GameObject inverter){
        gm.ic.Invert();
        inverter.SetActive(false);
        gm.darkMode = !gm.darkMode;
        StartCoroutine(executeAfterSeconds(2f, () => inverter.gameObject.SetActive(true)));
    }



    void processCrystalDash(GameObject crystalDash){
        SpriteRenderer sprite = crystalDash.gameObject.GetComponent<SpriteRenderer>();
        if (!sprite.enabled) return;

        ParticleSystem partSystem = crystalDash.gameObject.GetComponent<ParticleSystem>();
        var main = partSystem.main;
        main.startSpeed = gm.tm.isTryingToSlow ? gm.tm.minParticleSpeed :  gm.tm.maxParticleSpeed; // set particle speed to slow-mo manually 
        partSystem.Play();

        sprite.enabled = false;
        ps.canMove = false;
        ps.canDash = true;
        rb.gravityScale = 0f;
        Vector2 direction = crystalDash.transform.position - transform.position;
        rb.AddForce(direction * ps.crystalDashSpeed, ForceMode2D.Impulse);

    
        StartCoroutine(executeAfterSeconds(0.25f, () => {
            ps.canMove = true;
            rb.gravityScale = ps.initialGravity;
            ps.wallSlideSpeed = ps.defaultSlideSpeed;
        }));

        StartCoroutine(executeAfterSeconds(2f, () => sprite.enabled = true));

    }

    void processDeath(){

        if (ps.isDead) return;
        foreach (SpriteRenderer sr in playerSprites) sr.enabled = false;

        rb.constraints = RigidbodyConstraints2D.FreezePositionX |
                         RigidbodyConstraints2D.FreezePositionY |
                         RigidbodyConstraints2D.FreezeRotation;

        ps.canMove = false;
        ps.inHyperDashZone = false;
        rb.gravityScale = ps.initialGravity;
        ps.activeMovespeed = 0f;
        rb.velocity = Vector2.zero;
        particles.Play();
        ps.isDead = true;

        // gm.tm.SlowMotion(0.1f,1.5f);
        StartCoroutine(executeAfterSeconds(1.5f, () => newGame()));

    }


  

    void processBooster(GameObject booster){

        changeBoosterOpacity(booster, 1f);
        float rotation = booster.transform.eulerAngles.z;
        Vector2 dir = new Vector2(Mathf.Cos(rotation), Mathf.Sin(rotation));
        
        if(Mathf.Sign(dir.x) != ps.facing) return;


        rb.AddForce(dir * ps.boosterSpeed * (dir.y != 0 ? 2 : 1) , ForceMode2D.Impulse);
        
        
    }

    void changeBoosterOpacity(GameObject booster, float opacity){
        SpriteRenderer boostSprite = booster.GetComponent<SpriteRenderer>();
        if(boostSprite.color.a != opacity) boostSprite.color = ColorManager.changeOpacity(boostSprite.color, opacity);
    }




    void newGame() {
        transform.position = respawnPoint.position;
        ps.canMove = true;
        foreach (SpriteRenderer sr in playerSprites) sr.enabled = true;
        
        ps.isDead = false;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        particles.Stop();
    }


    public delegate void Function();

    IEnumerator executeAfterSeconds(float duration, Function functionToExecute){
        yield return new WaitForSeconds(duration);
        functionToExecute();
    }


  
}
