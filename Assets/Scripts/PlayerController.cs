using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody2D rb;
    AnimationCurve trailWidth;
    TrailRenderer trail;
    ParticleSystem ps;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] LayerMask airBorneLayer;
    [SerializeField] Transform groundCheckPoint;
    [SerializeField] Transform wallCheckPoint;
    [SerializeField] Transform airBorneCheckPoint;


    [Header("Vectors")]
    public Vector2 wallJumpDir;
    public Vector2 wallHopDir;
    public Vector2 wallOutDir;
    public Vector2 infiniteDashForce = Vector2.zero;

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
    public float infiniteDashSpeed = 30f;
    public float gravityMultiplier;
    public float initialGravity;
    public float coyoteTime = 0.2f;
    public float coyoteTimeCounter;


    [SerializeField] float groundCheckRadius;
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
    public bool inMovingPlatform = false;
    public bool inInfiniteDashZone;
    public bool isDead = false;
    public bool isInSlope = false;
    [SerializeField] bool hasJump = true;
    [SerializeField] bool hasWallJump = true;
    [SerializeField] bool hasAirDash = true;


    // enum State{Idle, Moving, Jumping, Dashing, WallJumping, WallSliding, InSlope, InInfiniteDashZone, Dead}

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ps = GameObject.Find("Explosion").GetComponent<ParticleSystem>();
        trail = GameObject.Find("Trail").GetComponent<TrailRenderer>();
        activeMovespeed = 0f;
        initialGravity = rb.gravityScale;
        trailWidth = trail.widthCurve;
        wallSlideSpeed = defaultSlideSpeed;
    }


    void FixedUpdate()
    {
        movePlayer();
        wallSlide();
    }


    void Update()
    {
        checkSurroundings();
        trailRenderer();
        dashController();
        handleRotation();

      

    }


    void movePlayer()
    {
        if (!inInfiniteDashZone){
            if (canMove && !isDashing && !isWallJumping && !isWallSliding){
                if (activeMovespeed > 0){
                    rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, facing * activeMovespeed, 0.5f), rb.velocity.y);
                }
                else{
                    rb.velocity = new Vector2(facing * Mathf.Clamp(Mathf.Abs(rb.velocity.x) - (isGrounded ? stopGroundSpeed : stopAirSpeed), 0f, movementSpeed), rb.velocity.y);
                }
            }

        }
        
    }


    void wallSlide(){
        if (isWallSliding && rb.velocity.y < -wallSlideSpeed){
            rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed); // wall slide
        }
    }


    public void jump(float force, Vector2 dir){
        if (coyoteTimeCounter > 0 && !isWallSliding && hasJump){
            isJumping = true;
            coyoteTimeCounter = 0f;
            rb.AddForce(dir * force, ForceMode2D.Impulse);
        }
    }


    void checkSurroundings(){

        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
        isTouchingWall = Physics2D.Raycast(wallCheckPoint.position, facing == 1 ? Vector2.right : Vector2.left, wallCheckDistance, wallLayer);
        isWallSliding = isTouchingWall && !isGrounded && (rb.velocity.y < 0 || isDashing);
        isWallGrabbing = (wallSlideSpeed == wallGrabSpeed);
        isAirBorne = !Physics2D.OverlapCircle(airBorneCheckPoint.position, airBorneCheckRadius, airBorneLayer);


        if (!inMovingPlatform){
            transform.localScale = new Vector3(1, 1, 1);
        }
        if (!inInfiniteDashZone){
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
        }


    }


    void trailRenderer(){
        trail.emitting = !isDead && (activeMovespeed > 0f || isDashing || (isJumping && rb.velocity.y > 0) || inInfiniteDashZone);
    }

    public void Dash(Dash dash){
        if (dash.hyperDash || (coyoteTimeCounter < 0 && dashesLeft > 0 && !isTouchingWall && canDash && hasAirDash &&
            (Mathf.Sign(dash.dir.x) == facing || Mathf.Abs(dash.dir.x) < 0.3f))){

            isDashing = true;
            dashesLeft--;
            canDash = dash.hyperDash ? true : dashesLeft > 0;
            // rb.AddForce(new Vector2(dash.dir.x, dash.dir.y) * dash.speed, ForceMode2D.Impulse); // 55
            rb.velocity = new Vector2(dash.dir.x, dash.dir.y) * dash.speed; // 30
            dashTimer = dash.duration;
            rb.gravityScale = 0f;
            trail.startWidth = 0.65f;
            trail.endWidth = 0.65f;
        }
    }


    void dashController()
    {
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

    public void wallJump(float dirX, float dirY){
        if (!isGrounded && isTouchingWall && (hasWallJump || Mathf.Sign(dirX) == facing)){
            isWallJumping = Mathf.Sign(dirX) != facing;
            // rb.gravityScale = 0f;
            Vector2 force = new Vector2(dirX, dirY) * wallJumpForce;
            rb.AddForce(force, ForceMode2D.Impulse);
            // rb.gravityScale = initialGravity;
            StartCoroutine(setWallJumpingFalse(.275f));
        }
    }

    public void handleRotation(){
        if (!isGrounded && !isInSlope && !isTouchingWall){
            resetRotation(transform);
        }

        if (!isAirBorne){
            rb.freezeRotation = false; // unfreezes rotation when on ground
        }
        else{
            resetRotation(transform);
            rb.freezeRotation = true; // freezes rotation when in the air
        }
    }

    void resetRotation(Transform transf){
        if (transform.eulerAngles.z != 0){
            transf.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Mathf.Lerp(transform.eulerAngles.z, 0f, 1f));
        }

    }



    public void flip(){
        facing = -facing;
        Vector3 prevParticlePos = ps.gameObject.transform.position;
        transform.Rotate(0f, 180f, 0f);
        ps.gameObject.transform.position = prevParticlePos; // lock particle Z pos

        if (transform.eulerAngles.z != 0 && !isTouchingWall && isInSlope) rb.velocity = Vector2.down;

        if (isDashing) isDashing = false;

    }

    public void enterInfiniteDashZone(){
 
    }

    public void exitInfiniteDashZone(){
        infiniteDashForce = Vector2.zero;
        rb.gravityScale = initialGravity;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        Gizmos.DrawWireSphere(airBorneCheckPoint.position, airBorneCheckRadius);
        Debug.DrawRay(wallCheckPoint.position, new Vector3(facing == 1 ? 0.1f : 0.1f, 0f, 0f), Color.blue);
    }

    IEnumerator setWallJumpingFalse(float duration)
    {
        yield return new WaitForSeconds(duration);
        isWallJumping = false;
    }

}


public class Dash{
    public Vector2 dir;
    public float speed;
    public float duration;
    public bool hyperDash;

    public Dash(Vector2 dir, float speed, float duration, bool hyperDash = false)
    {
        this.dir = dir;
        this.speed = speed;
        this.duration = duration;
        this.hyperDash = hyperDash;
    }
}
