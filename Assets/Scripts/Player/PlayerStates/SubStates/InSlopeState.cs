using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InSlopeState : GroundedState {


    public override PlayerState handleState(){
        return this;
    }

}
