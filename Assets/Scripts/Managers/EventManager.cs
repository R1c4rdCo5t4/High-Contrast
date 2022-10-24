using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager {

    public static event UnityAction playerDeath;
    public static void OnPlayerDeath() => playerDeath?.Invoke();

    


}

// https://stackoverflow.com/questions/35462266/c-sharp-creating-a-basic-event-manager-using-action-passing-optional-params-to










// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class EventManager : MonoBehaviour
// {

//     public delegate void Event();
//     public static event Event OnPlayerDeath;

//     public static void invokePlayerDeath(){
//         if (OnPlayerDeath != null) OnPlayerDeath(); // invoke event 
//     }


// }