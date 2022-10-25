using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState
{

    public abstract PlayerState currentState { get; set; }
    public virtual PlayerState handleState(){
        return this;
    }

}


// https://www.youtube.com/watch?v=OjreMoAG9Ec&list=PLy78FINcVmjA0zDBhLuLNL1Jo6xNMMq-W&index=22&t=19s
