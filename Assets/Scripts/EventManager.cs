using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{

    public delegate void PlayerDeath();
    public static event PlayerDeath OnPlayerDeath;

    public static void invokePlayerDeath(){
        if (OnPlayerDeath != null) OnPlayerDeath(); // invoke event 
    }


}