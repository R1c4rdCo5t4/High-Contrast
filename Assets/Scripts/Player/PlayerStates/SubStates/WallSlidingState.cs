using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSliding : TouchingWallState {


    public override PlayerState handleState(){
        return this;
    }

}
