using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityState : PlayerState {

    public override PlayerState currentState { get ; set; }

    public override PlayerState handleState(){
        return this;
    }

}
