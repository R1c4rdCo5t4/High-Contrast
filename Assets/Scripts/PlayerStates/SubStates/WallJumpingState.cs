using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJumpingState : TouchingWallState {


    public override PlayerState handleState(){
        return this;
    }

}
