using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : PlayerState {


    public override PlayerState handleState(){
        return this;
    }

}
