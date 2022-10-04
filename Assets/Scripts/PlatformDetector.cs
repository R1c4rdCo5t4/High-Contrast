using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformDetector : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Transform wallCheckPoint;

    PlayerController ps; // player script
    Transform po; // player object

    enum PlatformType { Ground, Wall}

    void Start(){
        ps = GameObject.Find("Player").GetComponent<PlayerController>();
        po = GameObject.Find("PlayerObject").GetComponent<Transform>();

    }

    // Update is called once per frame
    void Update()
    {
        if(ps.inMovingPlatform){
            if(player.transform.eulerAngles.z != 0){ 
                player.transform.eulerAngles = new Vector3(player.transform.eulerAngles.x, player.transform.eulerAngles.y, 0f); 
            }
           
        }
        if(!ps.isGrounded && !ps.isTouchingWall){ // exit out of moving platform || (ps.inMovingPlatform && (ps.activeMovespeed > 0f || ps.isWallGrabbing))
            player.SetParent(po, true);
            ps.inMovingPlatform = false;
            return;
            
        }

        if(ps.isGrounded || ps.isTouchingWall){
        
            RaycastHit2D groundHit = Physics2D.Raycast(transform.position, Vector2.down, 0.5f);
            RaycastHit2D wallHit = Physics2D.Raycast(wallCheckPoint.position, ps.facing == 1 ? Vector2.right : Vector2.left, 0.15f);


            if(groundHit.collider != null && ps.activeMovespeed == 0f){    
                checkParent(groundHit, PlatformType.Ground);
            }
            else if(wallHit.collider != null) {
                checkParent(wallHit, PlatformType.Wall);
            }
        }

        
        
    }

    void checkParent(RaycastHit2D hit, PlatformType platform){
        
        if(hit.collider.CompareTag("MovingPlatform")){
            if(platform == PlatformType.Wall){
                if(!ps.isWallGrabbing){
                    return; 
                }
                ps.activeMovespeed = 0f;
            }
            player.SetParent(hit.transform, true);
            ps.inMovingPlatform = true;
        }
       
    }
    

  
   

}

             


             