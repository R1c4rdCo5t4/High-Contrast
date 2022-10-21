using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    Rigidbody2D rb;
    SpriteRenderer sr;
    AnimationCurve trailWidth;
    TrailRenderer trail;
    ParticleSystem ps;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] LayerMask airBorneLayer;
    [SerializeField] Transform groundCheckPoint;
    [SerializeField] Transform wallCheckPoint;
    [SerializeField] Transform airBorneCheckPoint;
    [SerializeField] Transform topCheckPoint;

    GameObject rotatable;
    State currState;


    [Header("Vectors")]
    public Vector2 wallJumpDir;
    public Vector2 wallHopDir;
    public Vector2 wallOutDir;
    public Vector2 hyperDashForce = Vector2.zero;
    Quaternion initialRotation;

    [Header("Floats")]
    public float movementSpeed;
    public float activeMovespeed;
    public float minJumpForce = 45f;
    public float maxJumpForce = 60f;
    public float wallJumpForce;
    public float wallSlideSpeed;
    public float defaultSlideSpeed;
    public float wallGrabSpeed;
    public float dashSpeed = 30f;
    public float dashCoolDown = 2f;
    public float dashDuration = 0.2f;
    private float dashCoolTimer, dashTimer;
    public float hyperDashSpeed = 30f;
    public float crystalDashSpeed = 35f;
    public float boosterSpeed = 30f;
    public float gravityMultiplier;
    public float initialGravity;
    public float coyoteTime = 0.2f;
    public float coyoteTimeCounter;



    [SerializeField] float groundCheckRadius, topCheckRadius;
    [SerializeField] float wallCheckDistance = 0.4f;
    [SerializeField] float airBorneCheckRadius;
    [SerializeField] float stopGroundSpeed = 1f;
    [SerializeField] float stopAirSpeed = 0.2f;


    [Header("Ints")]
    public int facing;
    [SerializeField] int numOfDashes;
    [SerializeField] int dashesLeft;

    [Header("Booleans")]
    public bool isGrounded;
    public bool isTouchingWall;
    public bool isJumping;
    public bool isWallSliding, isWallGrabbing, isWallJumping;
    public bool isAirBorne;
    public bool isDashing;
    public bool canMove = true;
    public bool canWallHop = true;
    public bool canDash = true;
    public bool inMovingPlatform;
    public bool inHyperDashZone;
    public bool isDead;
    public bool isInSlope;
    public bool isBoosting;
    public bool isTouchingTop;
    [SerializeField] bool hasJump = true;
    [SerializeField] bool hasWallJump = true;
    [SerializeField] bool hasAirDash = true;





    enum State { Idle, Moving, Jumping, Dashing, WallJumping, WallSliding, InSlope, inHyperDashZone, Dead, Boosting, inMovingPlatform, Undefined }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        ps = GameObject.Find("Explosion").GetComponent<ParticleSystem>();
        trail = GameObject.Find("Trail").GetComponent<TrailRenderer>();
        rotatable = GameObject.Find("Rotate");
        activeMovespeed = 0f;
        initialGravity = rb.gravityScale;
        trailWidth = trail.widthCurve;
        wallSlideSpeed = defaultSlideSpeed;
        initialRotation = transform.rotation;
    }


    void FixedUpdate()
    {
        movePlayer();
        wallSlide();
        handleRotation();
    }


    void Update()
    {
        checkSurroundings();
        trailRenderer();
        dashController();
        checkStates();
    }


    void movePlayer()
    {
        if (!inHyperDashZone)
        {
            if (canMove && !isDashing && !isWallJumping && !isBoosting)
            {
                if (activeMovespeed > 0)
                {
                    rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, facing * activeMovespeed, 0.5f), rb.velocity.y);
                }
                else
                {
                    rb.velocity = new Vector2(facing * Mathf.Clamp(Mathf.Abs(rb.velocity.x) - (isGrounded ? stopGroundSpeed : stopAirSpeed), 0f, movementSpeed), rb.velocity.y);
                }
            }
        }
    }


    void wallSlide(){
        if (isWallSliding && rb.velocity.y < -wallSlideSpeed && !isBoosting){
            float wallSlideLerp =  Mathf.Lerp(rb.velocity.y, -wallSlideSpeed, 0.5f); // wall slide
            rb.velocity = new Vector2(rb.velocity.x, wallSlideLerp);
        }
    }


    public void jump(float force, Vector2 dir){
        if (coyoteTimeCounter > 0 && !isWallSliding && hasJump){
            isJumping = true;
            coyoteTimeCounter = 0f;
            rb.AddForce(dir * force, ForceMode2D.Impulse);
        }
    }

    void checkStates(){

        currState =
            isDead ? State.Dead :
            inHyperDashZone ? State.inHyperDashZone :
            inMovingPlatform ? State.inMovingPlatform :
            isBoosting ? State.Boosting :
            isGrounded && activeMovespeed == 0 ? State.Idle :
            isGrounded && activeMovespeed != 0 ? State.Moving :
            isGrounded && isInSlope ? State.InSlope :
            isDashing ? State.Dashing :
            !isGrounded && isJumping ? State.Jumping :
            isWallJumping ? State.WallJumping :
            isWallSliding ? State.WallSliding :
            State.Undefined;
    }


    void checkSurroundings(){

        isGrounded = Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckRadius, groundLayer);
        isTouchingWall = Physics2D.Raycast(wallCheckPoint.position, facing == 1 ? Vector2.right : Vector2.left, wallCheckDistance, wallLayer);
        isWallSliding = isTouchingWall && !isGrounded && (rb.velocity.y < 0 || isDashing);
        isWallGrabbing = wallSlideSpeed == wallGrabSpeed;
        isAirBorne = !Physics2D.OverlapCircle(airBorneCheckPoint.position, airBorneCheckRadius, airBorneLayer);
        isTouchingTop = Physics2D.Raycast(topCheckPoint.position, Vector2.right, topCheckRadius, groundLayer);


        
        if (!inMovingPlatform){
            transform.localScale = Vector3.one;
        }
        if (!inHyperDashZone){
            if (isGrounded){
                canWallHop = true;
                canDash = true;
                coyoteTimeCounter = coyoteTime;
                rb.gravityScale = initialGravity;
                dashesLeft = numOfDashes;
            }
            
            else{
                if (rb.velocity.y <= 0 && rb.gravityScale == initialGravity){
                    isJumping = false;
                    rb.gravityScale *= gravityMultiplier;
                }
                coyoteTimeCounter -= Time.deltaTime;
                isInSlope = false;
            }
            if(Mathf.Abs(rb.velocity.x) < movementSpeed / 1.75f) isBoosting = false;
        }


    }


    void trailRenderer(){
        trail.emitting = !isDead && (activeMovespeed > 0f || isDashing || (isJumping && rb.velocity.y > 0) || inHyperDashZone);
    }

    public void Dash(Dash dash){
        if (coyoteTimeCounter < 0 && dashesLeft > 0 && !isTouchingWall && canDash && hasAirDash && 
           (Mathf.Sign(dash.dir.x) == facing || Mathf.Abs(dash.dir.x) < 0.3f)){

            isDashing = true;
            dashesLeft--;
            canDash = dashesLeft > 0;
            // rb.AddForce(new Vector2(dash.dir.x, dash.dir.y) * dash.speed, ForceMode2D.Impulse); // 55
            rb.velocity = new Vector2(dash.dir.x, dash.dir.y) * dash.speed; // 30
            dashTimer = dash.duration;
            rb.gravityScale = 0f;
            // trail.startWidth = 0.65f;
            // trail.endWidth = 0.65f;

            // Vector3 newDirection = Vector3.RotateTowards(transform.forward, dash.dir * 100, 5f * Time.deltaTime, 0.0f);

            // // Draw a ray pointing at our target in
            // Debug.DrawRay(transform.position, newDirection, Color.red);

            // Calculate a rotation a step closer to the target and applies rotation to this object

            // Vector3 dir = new Vector3(0f, Mathf.Sin(Mathf.Deg2Rad * angle), Mathf.Cos(Mathf.Deg2Rad * angle));

            // transform.rotation = Quaternion.LookRotation(dir);

            // transform.rotation = initialRotation;

            transform.rotation = Quaternion.Slerp(transform.rotation, dash.rotation * transform.rotation, 0.5f);


        }
    }



    void dashController()
    {

        // if(isDashing){
        //     transform.rotation = Quaternion.Slerp(transform.rotation, dashRotation * transform.rotation, 0.5f);
        // }


        if (dashTimer > 0){
            dashTimer -= Time.deltaTime;


            if (dashTimer <= 0 || isTouchingWall || !isDashing){  //(Mathf.Abs(rb.velocity.x) > 0.5 && Mathf.Sign(rb.velocity.x) != facing))
                isDashing = false;
                activeMovespeed = movementSpeed;
                dashCoolTimer = dashCoolDown;
                trail.widthCurve = trailWidth;
                rb.gravityScale = initialGravity;
            }
        }
        if (dashCoolTimer > 0){
            dashCoolTimer -= Time.deltaTime;
        }
        
    }

    public void hyperDash(Vector2 force){

        if (isAirBorne && !(Mathf.Abs(rb.velocity.x) < 15f && Mathf.Abs(rb.velocity.y) < 15f)) return;

        rb.AddForce(force, ForceMode2D.Impulse);
        if (Mathf.Sign(force.x) != facing){
            flip();
        }
        rb.gravityScale = 0f;
        Quaternion dashRotation = Quaternion.FromToRotation(new Vector3(facing,0f,0f), force);
        transform.rotation = Quaternion.Slerp(transform.rotation, dashRotation * transform.rotation, 0.5f);
    }

    public void wallJump(float dirX, float dirY){
        if (!isGrounded && isTouchingWall && (hasWallJump || Mathf.Sign(dirX) == facing)){
            isWallJumping = Mathf.Sign(dirX) != facing;
            // rb.gravityScale = 0f;
            Vector2 force = new Vector2(dirX, dirY) * wallJumpForce;
            rb.AddForce(force, ForceMode2D.Impulse);
            // rb.gravityScale = initialGravity;
            TimeManager.executeAfterSeconds(.275f, () => isWallJumping = false);
        }
    }

    public void gravityController(float gravityModifier, float lerpSpeed){
        
        rb.gravityScale = Mathf.Lerp(rb.gravityScale, initialGravity * gravityModifier, lerpSpeed);
    }

    public void handleRotation(){
    
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector3.down), 5f, groundLayer);
        Quaternion slopeRotation = Quaternion.FromToRotation(transform.up, hit.normal);
        float angle = (slopeRotation.eulerAngles.z > 180 ? slopeRotation.eulerAngles.z - 360 : slopeRotation.eulerAngles.z);
   
        if (hit && (Mathf.Sign(angle) == 1 ? angle < 50f : angle > -50f)) transform.rotation = Quaternion.Slerp(transform.rotation, slopeRotation * transform.rotation, 0.5f);

        else
        {
            resetRotation(transform);
        }
    }

    void resetRotation(Transform transf){
        if (!isDashing && !inHyperDashZone){
            // transf.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Mathf.Lerp(transform.eulerAngles.z, 0f, 0.2f));
            // Quaternion resetRotation = Quaternion.FromToRotation(transform.rotation.eulerAngles, Vector3.zero);
            //  Quaternion.Slerp(transform.rotation, resetRotation  * transform.rotation, 1f);

            // transf.eulerAngles = new Vector3(0f, 0f, Mathf.Lerp(transform.eulerAngles.z - (facing == 1 ? 0 : 180f), 0f, 0.5f));
            transf.eulerAngles = Vector3.zero;
        }

        // if(!isDashing) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0f,facing,0f)) * transform.rotation, 0.2f);
    }




    public void flip(){
        facing = -facing;
        sr.flipX = !sr.flipX;
        rotatable.transform.Rotate(0f, 180f, 0f);
        
        // transform.Rotate(0f, 180f, 0f);
        // nonRotatable.transform.Rotate(0f, 180f, 0f); // keep rotation
        isBoosting = false;

    }

    public void enterhyperDashZone(){
 
    }

    public void exithyperDashZone(){
        hyperDashForce = Vector2.zero;
        rb.gravityScale = initialGravity;
    }

    private void OnDrawGizmos()
    {
        // Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        Debug.DrawRay(topCheckPoint.position, new Vector3(topCheckRadius, 0f, 0f), Color.red);
        Gizmos.DrawWireSphere(airBorneCheckPoint.position, airBorneCheckRadius);
        Debug.DrawRay(groundCheckPoint.position, new Vector3(0f, -groundCheckRadius, 0f), Color.blue);
        Debug.DrawRay(wallCheckPoint.position, new Vector3(facing == 1 ? 0.1f : 0.1f, 0f, 0f), Color.blue);
        // Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down), Color.red);
    }


}


public class Dash {
    public Vector2 dir;
    public float speed;
    public float duration;
    public Quaternion rotation;

    public Dash(Vector2 dir, float speed, float duration, Quaternion rotation)
    {
        this.dir = dir;
        this.speed = speed;
        this.duration = duration;
        this.rotation = rotation;

    }

}

