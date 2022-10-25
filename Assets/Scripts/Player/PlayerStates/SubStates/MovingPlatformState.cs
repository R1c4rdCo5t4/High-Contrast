using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformState : GroundedState {


    public override PlayerState handleState(){
        return this;
    }

}
