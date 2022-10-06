using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody2D rb;
    TimeManager tm;
    AnimationCurve trailWidth;
    GameManager gm;
    GameObject otherComponents;
    TrailRenderer trail;
    ParticleSystem ps;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] LayerMask airBorneLayer;
    [SerializeField] Transform groundCheckPoint;
    [SerializeField] Transform wallCheckPoint;
    [SerializeField] Transform airBorneCheckPoint;


    [Header("Vectors")]
    [SerializeField] Vector2 movementInput;
    private Vector3 respawnPoint;
    public Vector2 wallJumpDir;
    public Vector2 wallHopDir;
    public Vector2 wallOutDir;
    public Vector2 currVelocity;
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

    [Header("Booleans")]
    public bool isGrounded, isTouchingWall;
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
    [SerializeField] bool hasJump = true;
    [SerializeField] bool hasWallJump = true;
    [SerializeField] bool hasAirDash = true;


   
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        tm = GameObject.Find("TimeManager").GetComponent<TimeManager>();
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

    void movePlayer(){

        if(!inInfiniteDashZone){
            if(activeMovespeed > 0){ 
                if(canMove && !isDashing && !isWallJumping){
                    rb.velocity = new Vector2( Mathf.Lerp(rb.velocity.x, facing*activeMovespeed, 0.5f),rb.velocity.y);
                }
            }
            else{
                rb.velocity = new Vector2(facing*Mathf.Clamp(Mathf.Abs(rb.velocity.x)- (isGrounded ? stopGroundSpeed : stopAirSpeed),0f,movementSpeed), rb.velocity.y);
            }
        }
        else{
             
            if(infiniteDashForce != Vector2.zero){
                rb.velocity = infiniteDashForce;
                if(Mathf.Sign(infiniteDashForce.x) != facing){
                    flip();
                }
            }

        }
        
    }


    void wallSlide(){

        if(isWallSliding && rb.velocity.y < -wallSlideSpeed){ 
            if(!inMovingPlatform){
                rb.isKinematic = false;
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed); // wall slide
               
            }
            else{
                rb.isKinematic = true;
                rb.velocity = Vector2.zero;
            }
                
        }
        else{
            rb.isKinematic = false;
        }

        
    }


    public void jump(float force)
    {
        if(coyoteTimeCounter > 0 && !isWallSliding && hasJump){
            isJumping = true;
            coyoteTimeCounter = 0f;
            rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        }
    }


    void checkSurroundings(){
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position,groundCheckRadius,groundLayer);
        isTouchingWall = Physics2D.Raycast(wallCheckPoint.position,facing == 1 ? Vector2.right : Vector2.left,wallCheckDistance,wallLayer);
        isWallSliding =  isTouchingWall && !isGrounded && (rb.velocity.y < 0 || isDashing);
        isWallGrabbing = (wallSlideSpeed == wallGrabSpeed);
        isAirBorne = !Physics2D.OverlapCircle(airBorneCheckPoint.position,airBorneCheckRadius,airBorneLayer);
        currVelocity = rb.velocity;

        if(!inMovingPlatform){
            transform.localScale = new Vector3(1, 1, 1);
        }

        if(isGrounded){
            canWallHop = true;
            canDash = true;
            coyoteTimeCounter = coyoteTime;
            // rb.gravityScale = initialGravity;
            if(rb.velocity.y <= 0){
                isJumping = false;
            }
            
        }   
        else{
            if(rb.velocity.y <= 0 && rb.gravityScale == initialGravity){
                rb.gravityScale *= gravityMultiplier;
            }
            coyoteTimeCounter -= Time.deltaTime;
        }
    }


    void trailRenderer()
    {
        trail.emitting = !isDead && (activeMovespeed > 0f || isDashing || (isJumping && rb.velocity.y > 0) || infiniteDashForce != Vector2.zero);  
    }

    public void Dash(Vector2 dashDir, float speed, float duration, bool hyperDash=false, bool gravity=false){

        if(hyperDash || (coyoteTimeCounter < 0 && !isTouchingWall && canDash && hasAirDash && (Mathf.Sign(dashDir.x) == facing || Mathf.Abs(dashDir.x) < 0.3f) )){
            isDashing = true;
            canDash = hyperDash;
            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2(dashDir.x, dashDir.y) * speed * 100);
            dashTimer = duration;
            rb.gravityScale *= gravity ? 1 : 0;   
            trail.startWidth = 0.65f;
            trail.endWidth = 0.65f;
        }
    }


    void dashController(){
        if(dashTimer > 0){
            dashTimer -= Time.deltaTime;

            if(dashTimer <= 0 || isTouchingWall){  //(Mathf.Abs(rb.velocity.x) > 0.5 && Mathf.Sign(rb.velocity.x) != facing))
                isDashing = false;
                activeMovespeed = movementSpeed;
                dashCoolTimer = dashCoolDown;
                trail.widthCurve = trailWidth;
                rb.gravityScale = initialGravity;
            }
        }
        if(dashCoolTimer > 0){
            dashCoolTimer -= Time.deltaTime;
        }
    }

    public void wallJump(float dirX,float dirY){
        if(!isGrounded && isTouchingWall && (hasWallJump || Mathf.Sign(dirX) == facing)){
            isWallJumping = Mathf.Sign(dirX) != facing;
            rb.gravityScale = 0f;
            Vector2 force = new Vector2(wallJumpForce * dirX, wallJumpForce * dirY);
            rb.AddForce(force, ForceMode2D.Impulse);
            rb.gravityScale = initialGravity;
            StartCoroutine(setWallJumpingFalse(.275f));
        }
    }

    public void handleRotation(){
 
        if(!isGrounded){
            resetRotation(transform);
        } 

        
        if(!isAirBorne){
            rb.freezeRotation = false; // unfreezes rotation when on ground
        }
        else {
            if(transform.eulerAngles.z != 0){ 
                resetRotation(transform);
            }
            rb.freezeRotation = true; // freezes rotation when in the air
        }        
    }

    void resetRotation(Transform transf){
        transf.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Mathf.Lerp(transform.eulerAngles.z, 0f, 1f)); 
    }
   


    public void flip(){
        facing = -facing;
        Vector3 prevParticlePos = ps.gameObject.transform.position;
        transform.Rotate(0f, 180f, 0f);
        ps.gameObject.transform.position = prevParticlePos; // lock particle Z pos

    }

    private void OnDrawGizmos(){
        Gizmos.DrawWireSphere(groundCheckPoint.position,groundCheckRadius);
        Gizmos.DrawWireSphere(airBorneCheckPoint.position,airBorneCheckRadius);
        // Gizmos.DrawLine(wallCheckPoint.position, new Vector3(wallCheckPoint.position.x + wallCheckDistance, wallCheckPoint.position.y, wallCheckPoint.position.z));
        Vector3 vec = new Vector3(facing == 1 ? 0.1f : 0.1f,0f,0f);


        Debug.DrawRay(wallCheckPoint.position,vec,Color.blue);
    }

    IEnumerator setWallJumpingFalse(float duration){
        yield return new WaitForSeconds(duration);
        isWallJumping = false;
    }

}
