using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchManager : MonoBehaviour
{
    [SerializeField] PlayerController ps;
    PlayerCollision pc;
    GameManager gm;
   
    Vector2 startTouchPosition;
    Vector2 currentPosition;
    Vector2 endTouchPosition;
    bool stopTouch = false;

    [SerializeField] float swipeRangeMultiplier;
    [SerializeField] float tapRange;
    float swipeRange;
    float tapTimer = 0f;
    float tapMaxDuration = .5f;

    // float holdTimer = 0f;
    // float holdMinDuration = .0005f;
    // int taps = 0;

    void Start()
    {
        swipeRange = Screen.height * swipeRangeMultiplier / 100;
        pc = GameObject.Find("Player").GetComponent<PlayerCollision>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
       
    }

    void Update()
    {

        if (ps.isDead) return;

        if (tapTimer > 0) tapTimer -= Time.deltaTime;
        
        if (Input.touchCount > 0){
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

    

    void startTouch(Touch touch)
    {
        startTouchPosition = touch.position;
        tapTimer = tapMaxDuration;
        // holdTimer = holdMinDuration;

        EventManager.OnStartTouch();

    }

    void moveTouch(Touch touch)
    {
        // https://answers.unity.com/questions/663784/is-it-the-way-to-find-swipe-speed.html

        currentPosition = touch.position;
        Vector2 Swipe = currentPosition - startTouchPosition;
        Vector2 swipeDir = Swipe.normalized;

        if (currentPosition.x < Screen.width / 3 || Swipe.magnitude < 100) return;


        if (!stopTouch){
            EventManager.OnMoveTouch(swipeDir);
        }


    }






    void stationaryTouch(Touch touch){

        if ((touch.position.x > Screen.width / 3)){

            if(ps.isWallSliding) ps.wallSlideSpeed = ps.wallGrabSpeed;
            if(ps.inHyperDashZone && ps.isAirBorne){
                var gravitySign = touch.position.y > Screen.height / 2 ? -1 : 1;
                ps.gravityController(gravitySign, 1f);
            } 
        }

        Vector2 movedDist = touch.position - startTouchPosition;


        if (touch.position.x < Screen.width / 3)
        { // Mathf.Abs(movedDist.x) < tapRange && Mathf.Abs(movedDist.y) < tapRange && tapTimer <= 0
            gm.tm.SlowMotion(0.25f, 4f);

        }

        EventManager.OnStationaryTouch();

    }

    void endTouch(Touch touch) {

        ps.wallSlideSpeed = ps.defaultSlideSpeed;
        endTouchPosition = touch.position;
        Vector2 movedDist = endTouchPosition - startTouchPosition;

        if ((Input.touchCount == 1 || (Input.touchCount > 1 && touch.position.x < Screen.width / 3)) && gm.tm.isTryingToSlow){
            gm.tm.isTryingToSlow = false;
        }

        else{
            stopTouch = false;
            if (Mathf.Abs(movedDist.x) < tapRange && Mathf.Abs(movedDist.y) < tapRange && tapTimer > 0) tap();
        }

        if(touch.position.x > Screen.width / 3){
            if(ps.inHyperDashZone){
                ps.gravityController(0f, 2f);
                ps.hyperDashForce = Vector2.zero;
            }
        }
        EventManager.OnEndTouch();
    }


    void tap(){
        if (!ps.inHyperDashZone) {
            ps.activeMovespeed = 0f;
            ps.isJumping = false;
            ps.isDashing = false;
        }
    }
}

