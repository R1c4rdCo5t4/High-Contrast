using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager {

    #region Player Events
    public static event UnityAction playerDeath;
    public static void OnPlayerDeath() => playerDeath?.Invoke();


    #endregion


    #region TouchManager Events
    public static event UnityAction startTouch;
    public static event UnityAction moveTouch;
    public static event UnityAction stationaryTouch;
    public static event UnityAction endTouch;
    public static void OnStartTouch() => startTouch?.Invoke();
    public static void OnMoveTouch(Vector2 swipeDir) => moveTouch?.Invoke();
    public static void OnStationaryTouch() => stationaryTouch?.Invoke();
    public static void OnEndTouch() => endTouch?.Invoke();

    #endregion


    #region TimeManager Events

    public static event UnityAction slowMotion;
    public static void OnSlowMotion() => slowMotion?.Invoke();

    #endregion

    #region ColorManager Events

    public static event UnityAction invert;
    public static void OnInvert() => invert?.Invoke();

    #endregion

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