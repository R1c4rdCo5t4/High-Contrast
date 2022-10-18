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

    enum Direction { Up, Down, Left, Right }


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

    }

    void moveTouch(Touch touch)
    {

        // https://answers.unity.com/questions/663784/is-it-the-way-to-find-swipe-speed.html

        currentPosition = touch.position;
        Vector2 Swipe = currentPosition - startTouchPosition;
        Vector2 swipeDir = Swipe.normalized;

        // print(swipeDir);

        if (currentPosition.x < Screen.width / 3 || Swipe.magnitude < 100) return;


        if (!stopTouch){

            if (Swipe.x < -swipeRange){ // left
                if (!ps.inhyperDashZone) leftSwipe(swipeDir);
                swipeController(swipeDir, Direction.Left);
            }

            else if (Swipe.x > swipeRange){  // right
                if (!ps.inhyperDashZone) rightSwipe(swipeDir);
                swipeController(swipeDir, Direction.Right);
            }

            else if (Swipe.y > swipeRange /* && Input.touchCount == 1*/){ // up
                // if (!ps.inhyperDashZone) //upSwipe(swipeDir);
                swipeController(swipeDir, Direction.Up);
            }

            else if (Swipe.y < -swipeRange){ // down
                if (!ps.inhyperDashZone) downSwipe();
                swipeController(swipeDir, Direction.Down);
            }

        }


    }

    void swipeController(Vector2 swipeDir, Direction dir){
        // print(swipeDir);

        if (!ps.inhyperDashZone)
        {
            if (!ps.isTouchingWall && !ps.isGrounded){ // dash
                if ((dir == Direction.Left && ps.facing == 1) || (dir == Direction.Right && ps.facing == -1)){
                    ps.flip();
                    if(Mathf.Abs(swipeDir.y) < 0.5f) return;
                } 
            
                ps.Dash(new Dash(swipeDir, ps.dashSpeed, ps.dashDuration));
            }
            else { // jump
                if(swipeDir.y < 0.5f) return;
                ps.jump(ps.maxJumpForce, new Vector2(0f, swipeDir.y));
            }
        }

        else ps.hyperDash(swipeDir * ps.hyperDashSpeed);

        stopTouch = true;
    }

    void leftSwipe(Vector2 swipeDir)
    {
        ps.activeMovespeed = ps.movementSpeed;

        if (ps.facing < 0){
            if (ps.canWallHop){
                ps.wallJump(-ps.wallHopDir.x, ps.wallHopDir.y); // wall hop
                ps.canWallHop = false;
            }
        }

        else{
            ps.wallJump(-ps.wallJumpDir.x, ps.wallJumpDir.y); // wall jump 
            if (ps.isGrounded || ps.isTouchingWall) ps.flip();
        }

        stopTouch = true;
    }


    void rightSwipe(Vector2 swipeDir)
    {
        ps.activeMovespeed = ps.movementSpeed;

        if (ps.facing > 0){
            if (ps.canWallHop){
                ps.wallJump(ps.wallHopDir.x, ps.wallHopDir.y); // wall hop 
                ps.canWallHop = false;
            }
        }

        else{
            ps.wallJump(ps.wallJumpDir.x, ps.wallJumpDir.y); // wall jump 
            if (ps.isGrounded || ps.isTouchingWall) ps.flip();
        }

        stopTouch = true;
    }



    void upSwipe(Vector2 swipeDir){

        // print(swipeForce);
        // float jumpForce = /*swipeForce < Screen.height / 2 ? ps.minJumpForce :*/ ps.maxJumpForce;
        
        // // print(jumpForce);
        // ps.jump(jumpForce, swipeDir);
        // stopTouch = true;

    }



    void downSwipe()
    {
        if (ps.isTouchingWall && !ps.isGrounded){
            ps.wallJump(-ps.facing * ps.wallOutDir.x, -ps.wallOutDir.y);
            ps.flip();
        }

        stopTouch = true;
    }


    void stationaryTouch(Touch touch){

        if ((touch.position.x > Screen.width / 3)){

            if(ps.isWallSliding) ps.wallSlideSpeed = ps.wallGrabSpeed;
            if(ps.inhyperDashZone && ps.isAirBorne){
                var gravitySign = touch.position.y > Screen.height / 2 ? -1 : 1;
                ps.gravityController(gravitySign, 1f);
            } 
        }

        Vector2 movedDist = touch.position - startTouchPosition;


        if (touch.position.x < Screen.width / 3)
        { // Mathf.Abs(movedDist.x) < tapRange && Mathf.Abs(movedDist.y) < tapRange && tapTimer <= 0
            gm.tm.SlowMotion(0.25f, 4f);

        }

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
            if(ps.inhyperDashZone){
                ps.gravityController(0f, 2f);
                ps.hyperDashForce = Vector2.zero;
            }
        }
    }


    void tap(){
        if (!ps.inhyperDashZone) {
            ps.activeMovespeed = 0f;
            ps.isJumping = false;
            ps.isDashing = false;
        }
    }
}

