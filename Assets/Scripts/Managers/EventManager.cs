using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{

    public delegate void Event();
    public static event Event OnPlayerDeath;

    public static void invokePlayerDeath(){
        if (OnPlayerDeath != null) OnPlayerDeath(); // invoke event 
    }


}