using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : GroundedState {

    public override PlayerState handleState(){

        currentState = this;
        return this;
    }

}
