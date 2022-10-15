using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering.Universal;

public class TimeManager : MonoBehaviour
{

    float slowDownLength = 2f;
    float slowDownFactor;
    float slowDownTimer = 0f;
    float defaultTimeScale;

    [SerializeField] float particleSpeed;
    public bool isTryingToSlow = false;
    public bool isSlowing = false;
    bool canRestoreSlow;
    SlowMoBar slowBar;

    void Start()
    {
        defaultTimeScale = Time.timeScale;
        slowBar = GameObject.Find("SlowMoBar").GetComponent<SlowMoBar>();

    }

    void FixedUpdate()
    {

        if (isSlowing) slowBar.takeSlow();
        
        else{
            if (canRestoreSlow && !isTryingToSlow) slowBar.restoreSlow(); else Invoke("restoreSlow", 2f);
        }
    }


    void Update()
    {
        isSlowing = slowDownTimer > 0 && isTryingToSlow && slowBar.currentSlow > 0;
        var slowDownScale = isSlowing ? slowDownLength : 0.5f;
        Time.timeScale += (1f / slowDownScale) * Time.unscaledDeltaTime;
        Time.timeScale = Mathf.Clamp(Time.timeScale, 0, 1f);

        if (isSlowing)
        {
            slowDownTimer -= Time.deltaTime;
            canRestoreSlow = false;
        }

    }


    public void SlowMotion(float slowDownStrength, float slowDownLen)
    {
        isTryingToSlow = true;
        if (slowBar.currentSlow <= 0) return;
        Time.timeScale = slowDownStrength;
        slowDownLength = slowDownLen;
        slowDownTimer = slowDownLen;

    }


    void restoreSlow()
    {
        canRestoreSlow = true;
    }


}






