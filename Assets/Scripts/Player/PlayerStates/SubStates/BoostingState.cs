using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostingState : AbilityState {


    public override PlayerState handleState(){
        currentState = this;
        return this;
    }

}
