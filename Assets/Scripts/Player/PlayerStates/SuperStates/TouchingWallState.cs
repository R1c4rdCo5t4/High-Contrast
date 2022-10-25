using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchingWallState : PlayerState {

    public override PlayerState currentState { get ; set; }

    public override PlayerState handleState(){

        currentState = this;
        return this;
    }

}
