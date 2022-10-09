using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchManager : MonoBehaviour
{
    [SerializeField] PlayerController ps;
    // [SerializeField] InvertColor ic;
    PlayerCollision pc;
    GameManager gm;
    Rigidbody2D rb;
    
    Vector2 startTouchPosition;
    Vector2 currentPosition;
    Vector2 endTouchPosition;
    bool stopTouch = false;

    public float swipeRangeMultiplier;
    public float tapRange;
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
        rb = GameObject.Find("Player").GetComponent<Rigidbody2D>();
    }


    void Update()
    {   

        if(ps.isDead) return;

        if(tapTimer > 0){
            tapTimer -= Time.deltaTime;
        }


        if (Input.touchCount > 0){

             foreach (Touch touch in Input.touches){
                switch(touch.phase){
                    case TouchPhase.Began: startTouch(touch); break;
                    case TouchPhase.Moved:  moveTouch(touch); break;
                    case TouchPhase.Stationary: stationaryTouch(touch); break;
                    case TouchPhase.Ended: endTouch(touch); break;
                }
             } 
         }
         


    }

    void startTouch(Touch touch){
        startTouchPosition = touch.position;
        tapTimer = tapMaxDuration;
        // holdTimer = holdMinDuration;
        
    }

    void moveTouch(Touch touch){

        // https://answers.unity.com/questions/663784/is-it-the-way-to-find-swipe-speed.html
    
        currentPosition = touch.position;
        Vector2 Swipe = currentPosition - startTouchPosition;
        Vector2 swipeDir = Swipe.normalized;

        if(currentPosition.x < Screen.width / 3 || Swipe.magnitude < 100) return;

        // swipeDir = new Vector2(currentPosition.x-startTouchPosition.x,currentPosition.y-startTouchPosition.y);
        // print(Swipe.y);
        if (!stopTouch){
        
            if (Swipe.x < -swipeRange){ // left
                if(!ps.inInfiniteDashZone) leftSwipe();
                checkDash(swipeDir, Direction.Left);
                
            }

            else if (Swipe.x > swipeRange){  // right
                if(!ps.inInfiniteDashZone) rightSwipe();
                checkDash(swipeDir, Direction.Right);
            }

            else if (Swipe.y > swipeRange /* && Input.touchCount == 1*/){ // up
                if(!ps.inInfiniteDashZone) upSwipe(Swipe.y);
                checkDash(swipeDir, Direction.Up);
            }
            
            else if (Swipe.y < -swipeRange){ // down
                if(!ps.inInfiniteDashZone) downSwipe();
                checkDash(swipeDir, Direction.Down);
            }

        }

        
    }

    void checkDash(Vector2 swipeDir, Direction dir){
        

        if(!ps.inInfiniteDashZone){
            if(!ps.isTouchingWall && !ps.isGrounded){
                if((dir == Direction.Left && ps.facing == 1) || (dir == Direction.Right && ps.facing == -1)){
                    ps.flip();
                    return;
                }
                Dash dash = new Dash(swipeDir, ps.dashSpeed, ps.dashDuration); 
                ps.Dash(dash);  
            }
        }

        else{
            if(!ps.isAirBorne || ps.infiniteDashForce == Vector2.zero){
                ps.infiniteDashForce = swipeDir * ps.infiniteDashSpeed; 
            }
            
        }


        
     
        
    }

    void leftSwipe(){
        ps.activeMovespeed = ps.movementSpeed;
        
        if(ps.facing < 0){
            if(ps.canWallHop){
                ps.wallJump(-ps.wallHopDir.x,ps.wallHopDir.y); // wall hop to left
                ps.canWallHop = false;
            }
        }

        else {
            ps.wallJump(-ps.wallJumpDir.x,ps.wallJumpDir.y); // wall jump to right
            if(ps.isGrounded || ps.isTouchingWall){
                ps.flip();
            }
        }
        stopTouch = true;
    }

    void rightSwipe(){
        ps.activeMovespeed = ps.movementSpeed;
        
        if(ps.facing > 0){
            if(ps.canWallHop){
                ps.wallJump(ps.wallHopDir.x,ps.wallHopDir.y); // wall hop to right
                ps.canWallHop = false;
            }
        }

        else {
            ps.wallJump(ps.wallJumpDir.x,ps.wallJumpDir.y); // wall jump to left
            if(ps.isGrounded || ps.isTouchingWall){
                ps.flip();
            } 
        }
        stopTouch = true;

    
    }



    void upSwipe(float swipeForce){
        
        // print(swipeForce);
        float jumpForce = swipeForce < Screen.height / 2 ? ps.minJumpForce : ps.maxJumpForce;
        // print(jumpForce);
        ps.jump(jumpForce);
        stopTouch = true;
    }



    void downSwipe(){
        if(ps.isTouchingWall && !ps.isGrounded){
            ps.wallJump(ps.facing*ps.wallOutDir.x, ps.wallOutDir.y);
            ps.flip();
        }
        stopTouch = true;    
    }


    void stationaryTouch(Touch touch){
       
        if((touch.position.x > Screen.width / 3) && ps.isWallSliding){
            ps.wallSlideSpeed = ps.wallGrabSpeed;
        }   

        Vector2 movedDist = touch.position - startTouchPosition;


        if(touch.position.x < Screen.width / 3){ // Mathf.Abs(movedDist.x) < tapRange && Mathf.Abs(movedDist.y) < tapRange && tapTimer <= 0
            gm.tm.SlowMotion(0.25f,4f);
           
        }
        
    }

    void endTouch(Touch touch){
       
        stopTouch = false;
        ps.wallSlideSpeed = ps.defaultSlideSpeed;
        endTouchPosition = touch.position;
        Vector2 movedDist = endTouchPosition - startTouchPosition;

        if((Input.touchCount == 1 || (Input.touchCount > 1 && touch.position.x < Screen.width / 3)) && gm.tm.isTryingToSlow){
            gm.tm.isTryingToSlow = false;
          
        }
        else{
            if(Mathf.Abs(movedDist.x) < tapRange && Mathf.Abs(movedDist.y) < tapRange && tapTimer > 0) tap();
        }
            
    }


    void tap(){
        ps.activeMovespeed = 0f; 
        ps.isJumping = false;
        ps.isDashing = false;
       
    }
}

