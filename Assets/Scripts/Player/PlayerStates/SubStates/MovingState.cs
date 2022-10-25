using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingState : GroundedState {

    public override PlayerState handleState(){
        return this;
    }

}
