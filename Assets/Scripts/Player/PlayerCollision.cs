using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class PlayerCollision : MonoBehaviour {

    PlayerController ps;
    GameObject po;
    GameManager gm;
    GameObject go;
    Vector3 defaultRotation;
    Rigidbody2D rb;
    ParticleSystem particles;
    SpriteRenderer[] playerSprites;
    GameObject lastTouchedWall;
    [SerializeField] Transform respawnPoint;

    void Start(){
        ps = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        playerSprites = GetComponentsInChildren<SpriteRenderer>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        particles = GameObject.Find("Explosion").GetComponent<ParticleSystem>();
        defaultRotation = transform.eulerAngles;
    }

    void OnCollisionEnter2D(Collision2D collision){
        if (ps.inHyperDashZone){
            ps.hyperDashForce = Vector2.zero;
            rb.velocity = Vector2.zero;
        }        
        ProcessCollision(collision.gameObject);
    }

    void OnCollisionExit2D(Collision2D collision){}

    void OnTriggerEnter2D(Collider2D collider){
        ProcessCollision(collider.gameObject);
    }

    void OnTriggerExit2D(Collider2D collider){
        switch (collider.tag){
            case "Booster":
                TimeManager.executeAfterSeconds(0.5f, () => { 
                    changeBoosterOpacity(collider.gameObject, 0.4f);
                    rb.gravityScale = ps.initialGravity;
                    });
                break;

            case "BoosterZone": TimeManager.executeAfterSeconds(2.5f, () => ps.isBoosting = false); break;
            default: break;
        }
    }

    void ProcessCollision(GameObject collider){
        FloatScript fs = collider.GetComponent<FloatScript>();
        if (ps.isGrounded && ps.isTouchingTop && (fs != null && fs.speed.y < 0f)){
            processDeath();
            return;
        } 
                
        switch (collider.tag){
            case "Ground": 
                ps.isInSlope = collider.transform.eulerAngles.z != 0 && !ps.isTouchingWall;
                if(ps.isTouchingWall){
                    if (lastTouchedWall == collider.gameObject) ps.canWallJump = false;
                    else {
                        ps.canWallJump = true;
                        lastTouchedWall = collider.gameObject;
                    }
                }
                else if(ps.isGrounded){
                    ps.canWallJump = true;
                    lastTouchedWall = null;
                }
                break;
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

            case "Crusher":
                break;

            case "Transition":
                TransitionManager tm = collider.gameObject.GetComponent<TransitionManager>();
                if (tm != null) tm.moveCamera();
                break;

            default: break;
        }
    }

    void processReverter(GameObject inverter){
        gm.invert();
        inverter.SetActive(false);
        TimeManager.executeAfterSeconds(2f, () => inverter.gameObject.SetActive(true));
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
        ps.dashesLeft = ps.numOfDashes;
        rb.gravityScale = 0f;
        Vector2 direction = crystalDash.transform.position - transform.position;
        rb.AddForce(direction * ps.crystalDashSpeed, ForceMode2D.Impulse);

        TimeManager.executeAfterSeconds(0.25f, () => {
            ps.canMove = true;
            rb.gravityScale = ps.initialGravity;
            ps.wallSlideSpeed = ps.defaultSlideSpeed;
        });
        TimeManager.executeAfterSeconds(2f, () => sprite.enabled = true);
    }

    void processDeath(){
        if (ps.isDead) return;
        ps.isDead = true;
        ps.inHyperDashZone = false;
        ps.canMove = false;
        ps.activeMovespeed = 0f;
        rb.velocity = Vector2.zero;
        rb.gravityScale = ps.initialGravity;
        var p = Instantiate(particles, transform.position, Quaternion.identity);
        p.Play();
        TimeManager.executeAfterSeconds(1.5f, () => newGame(p.gameObject));
        gameObject.SetActive(false);
    }

    void newGame(GameObject p) {
        gameObject.SetActive(true);
        transform.position = respawnPoint.position;
        ps.canMove = true;
        ps.isDead = false;
        Destroy(p);
        SlowMoBar.restoreAllSlow();
    }

    void processBooster(GameObject booster){
        changeBoosterOpacity(booster, 1f);
        float rotation = booster.transform.eulerAngles.z;
        Vector2 dir = new Vector2(Mathf.Cos(rotation), Mathf.Sin(rotation));
        if(Mathf.Sign(dir.x) != ps.facing) return;
        if (!gm.playing()) return;
        rb.AddForce(dir * ps.boosterSpeed * (dir.y != 0 ? 2 : 1) , ForceMode2D.Impulse);
    }

    void changeBoosterOpacity(GameObject booster, float opacity){
        SpriteRenderer boostSprite = booster.GetComponent<SpriteRenderer>();
        if(boostSprite.color.a != opacity) boostSprite.color = ColorManager.changeOpacity(boostSprite.color, opacity);
    }
}
