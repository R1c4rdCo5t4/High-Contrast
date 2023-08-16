using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchManager : MonoBehaviour {

    [SerializeField] PlayerController ps;
    [SerializeField] GameObject cam;
    PlayerCollision pc;
    GameManager gm;
    Vector2 startTouchPosition;
    Vector2 currentPosition;
    Vector2 endTouchPosition;
    bool stopTouch = false;
    float swipeRangeMultiplier = 15;
    float tapRange = 20;
    float swipeRange;
    float tapTimer = 0f;
    float tapMaxDuration = .5f;
    float doubleTapMaxDuration = .1f;
    bool tapped;
    // float holdTimer = 0f;
    // float holdMinDuration = .0005f;
    // int taps = 0;

    void Start(){
        swipeRange = Screen.height * swipeRangeMultiplier / 100;
        pc = GameObject.Find("Player").GetComponent<PlayerCollision>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update(){
        if (ps.isDead) return;
        if (tapTimer > 0) tapTimer -= Time.deltaTime;
        if (tapped && tapTimer <= 0){
            tap();
            tapped = false;
        }
        if (Input.touchCount > 0){
            Touch t = Input.GetTouch(0);
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(t.position);
            Collider2D hitCollider = Physics2D.OverlapPoint(touchPosition);
            if (hitCollider != null && hitCollider.CompareTag("NoTouchZone")) return;
            foreach (Touch touch in Input.touches){
                switch (touch.phase){
                    case TouchPhase.Began: startTouch(touch); break;
                    case TouchPhase.Moved: moveTouch(touch); break;
                    case TouchPhase.Stationary: stationaryTouch(touch); break;
                    case TouchPhase.Ended: endTouch(touch); break;
                }
            }
        }
    }

    void startTouch(Touch touch){
        startTouchPosition = touch.position;
        tapTimer = tapMaxDuration;
    }

    void moveTouch(Touch touch){
        currentPosition = touch.position;
        Vector2 Swipe = currentPosition - startTouchPosition;
        Vector2 swipeDir = Swipe.normalized;
        if (currentPosition.x < Screen.width / 3 || Swipe.magnitude < 100 || stopTouch) return;
        Quaternion levelOrientation = cam.transform.rotation;
        swipeDir = levelOrientation * swipeDir;
        swipeController(swipeDir); 
    }

    void swipeController(Vector2 swipeDir){
        if (!ps.isTouchingWall && !ps.isGrounded){
           dashSwipe(swipeDir);
        } else if(ps.isTouchingWall && !ps.isGrounded && ps.canWallJump){
            wallJumpSwipe(swipeDir);
        }
        if (swipeDir.x < -0.5f){
            leftSwipe(swipeDir);
        } else if (swipeDir.x > 0.5f){
            rightSwipe(swipeDir);
        } else {
            jumpSwipe(swipeDir);
        }
        stopTouch = true;
    }

    void dashSwipe(Vector2 swipeDir){
        if ((swipeDir.x < -Mathf.Abs(swipeDir.y) && ps.facing > 0 && Mathf.Abs(swipeDir.y) < 0.5f) || (swipeDir.x > Mathf.Abs(swipeDir.y) && ps.facing < 0)) return;
        Quaternion dashRotation = Quaternion.FromToRotation(new Vector3(ps.facing, 0f, 0f), swipeDir);
        ps.dash(swipeDir, ps.dashSpeed, ps.dashDuration, dashRotation);
    }

    void wallJumpSwipe(Vector2 swipeDir){        
        if (Mathf.Abs(swipeDir.x) <= 0.2f){
            if (swipeDir.y < -0.5f) ps.flip();
            return;
        } 
        ps.wallJump(swipeDir);
    }

    void jumpSwipe(Vector2 swipeDir){
        if(swipeDir.y < 0.5f) return;
        ps.jump(ps.maxJumpForce, new Vector2(0f, swipeDir.y));
    }

    void leftSwipe(Vector2 swipeDir){
        ps.activeMovespeed = ps.movementSpeed;
        if (ps.facing > 0) ps.flip();
        stopTouch = true;
    }

    void rightSwipe(Vector2 swipeDir){
        ps.activeMovespeed = ps.movementSpeed;
        if (ps.facing < 0) ps.flip();
        stopTouch = true;
    }

    // void upSwipe(Vector2 swipeDir){
    //     if (!ps.isTouchingWall) return;
    //     ps.wallJump(swipeDir.x * 2, swipeDir.y * 3); // wall jump 
    //     if (ps.isGrounded || ps.isTouchingWall) ps.flip();
    // }

    // void downSwipe(){
    //     if (ps.isTouchingWall && !ps.isGrounded){
    //         ps.wallJump(-ps.facing * ps.wallOutDir.x, -ps.wallOutDir.y);
    //         ps.flip();
    //     }
    //     stopTouch = true;
    // }

    void stationaryTouch(Touch touch){
        if ((touch.position.x > Screen.width / 3)){
            if(ps.isWallSliding) ps.wallSlideSpeed = ps.wallGrabSpeed;
            if(ps.inHyperDashZone && ps.isAirBorne){
                var gravitySign = touch.position.y > Screen.height / 2 ? -1 : 1;
                ps.gravityController(gravitySign, 1f);
            } 
        }
        Vector2 movedDist = touch.position - startTouchPosition;
        if (touch.position.x < Screen.width / 3){ // Mathf.Abs(movedDist.x) < tapRange && Mathf.Abs(movedDist.y) < tapRange && tapTimer <= 0
            gm.tm.SlowMotion(0.25f, 4f);
        }
    }

    void endTouch(Touch touch) {
        ps.wallSlideSpeed = ps.defaultSlideSpeed;
        endTouchPosition = touch.position;
        Vector2 movedDist = endTouchPosition - startTouchPosition;

        if ((Input.touchCount == 1 || (Input.touchCount > 1 && touch.position.x < Screen.width / 3)) && gm.tm.isTryingToSlow){
            gm.tm.isTryingToSlow = false;
        } else {
            stopTouch = false;
            if (Mathf.Abs(movedDist.x) < tapRange && Mathf.Abs(movedDist.y) < tapRange && tapTimer > 0){
                if (tapped) doubleTap();
                else tapTimer = doubleTapMaxDuration;
                tapped = !tapped;
            }
        }
        if(touch.position.x > Screen.width / 3){
            if(ps.inHyperDashZone){
                ps.gravityController(0f, 2f);
                ps.hyperDashForce = Vector2.zero;
            }
        }
    }

    void tap(){
        if (!ps.inHyperDashZone) {
            ps.activeMovespeed = 0f;
            ps.isJumping = false;
            ps.isDashing = false;
        }
    }

    void doubleTap(){
        gm.invert();
    }
}
