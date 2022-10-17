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

    void Update() => print(rb.velocity.y);



    void OnCollisionEnter2D(Collision2D collision)
    {

        if (ps.inInfiniteDashZone){
            ps.infiniteDashForce = Vector2.zero;
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
                    changeBoosterOpacity(collider.gameObject, Way.Exit);}));
                break;
            
            case "BoosterZone": StartCoroutine(executeAfterSeconds(0.5f, () => ps.isBoosting = false)); break;

            default: break;
        }
    }

    void ProcessCollision(GameObject collider)
    {
        
        switch (collider.tag){
            case "Ground": ps.isInSlope = collider.transform.eulerAngles.z != 0 && !ps.isTouchingWall; break;
            case "Reverter": processReverter(collider); break;
            case "HyperDash": processHyperDash(collider); break;
            case "CrystalDash": processCrystalDash(collider); break;
            case "Spike":
            case "Death": processDeath(); break;
            case "ZoneIn":
                ps.inInfiniteDashZone = true;
                ps.enterInfiniteDashZone();
                break;
            case "ZoneOut":
                ps.inInfiniteDashZone = false;
                ps.exitInfiniteDashZone();
                break;


            case "Booster": 
                processBooster(collider);
                break;

            case "BoosterZone": ps.isBoosting = true; break;

            default: break;

        }


    }

    void processReverter(GameObject inverter){
        gm.ic.Invert();
        inverter.SetActive(false);
        gm.darkMode = !gm.darkMode;
        StartCoroutine(executeAfterSeconds(2f, () => inverter.gameObject.SetActive(true)));
    }

    void processHyperDash(GameObject hyperDash){
        HyperDash hd = hyperDash.gameObject.GetComponent<HyperDash>();
        if ((Mathf.Sign(hd.dash.dir.x) == ps.facing)) ps.Dash(hd.dash);
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

        StartCoroutine(executeAfterSeconds(2f, () => sprite.gameObject.SetActive(true)));

    }

    void processDeath(){

        if (ps.isDead) return;
        foreach (SpriteRenderer sr in playerSprites) sr.enabled = false;

        rb.constraints = RigidbodyConstraints2D.FreezePositionX |
                         RigidbodyConstraints2D.FreezePositionY |
                         RigidbodyConstraints2D.FreezeRotation;

        ps.canMove = false;
        ps.inInfiniteDashZone = false;
        rb.gravityScale = ps.initialGravity;
        ps.activeMovespeed = 0f;
        rb.velocity = Vector2.zero;
        particles.Play();
        ps.isDead = true;

        // gm.tm.SlowMotion(0.1f,1.5f);
        StartCoroutine(executeAfterSeconds(1.5f, () => newGame()));

    }


    enum Way { Enter, Exit }

    void processBooster(GameObject booster){
        float rotation = booster.transform.eulerAngles.z;
        Vector2 dir = new Vector2(Mathf.Cos(rotation), Mathf.Sin(rotation));
        if(Mathf.Sign(dir.x) != ps.facing) return;

        rb.AddForce(dir * ps.boosterSpeed * 2, ForceMode2D.Impulse);
        changeBoosterOpacity(booster, Way.Enter);
        ps.isDashing = true;
    }

    void changeBoosterOpacity(GameObject booster, Way way){
        SpriteRenderer boostSprite = booster.GetComponent<SpriteRenderer>();
        boostSprite.color = ColorManager.changeOpacity(boostSprite.color, way == Way.Enter ? 1f : 0.5f);
       
    }




    void newGame() {
        transform.position = respawnPoint.position;
        ps.canMove = true;
        foreach (SpriteRenderer sr in playerSprites) sr.enabled = true;
        
        ps.isDead = false;
        rb.constraints = RigidbodyConstraints2D.None;
        particles.Stop();
    }


    public delegate void Function();

    IEnumerator executeAfterSeconds(float duration, Function functionToExecute){
        yield return new WaitForSeconds(duration);
        functionToExecute();
    }


  
}
