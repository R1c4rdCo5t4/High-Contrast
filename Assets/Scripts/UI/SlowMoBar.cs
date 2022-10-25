using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlowMoBar : MonoBehaviour
{
    public Image slowMoBar;

    public static float currentSlow, maxSlow = 100f;
    public static float slowRate = 1f;
    float lerpSpeed;

    void Start(){
        currentSlow = maxSlow;
    }


    void Update()
    {
        Mathf.Clamp(currentSlow, 0f, maxSlow);
        slowMoBarFiller();
        lerpSpeed = 10f * Time.deltaTime;
    }

    void slowMoBarFiller() =>  slowMoBar.fillAmount = Mathf.Lerp(slowMoBar.fillAmount, currentSlow / maxSlow, lerpSpeed);
    

    public static void takeSlow()
    {
        if (currentSlow > 0) currentSlow -= slowRate;   
    }

    public static void restoreSlow()
    {
        if (currentSlow < maxSlow) currentSlow += slowRate / 2;
    }

    public static void restoreAllSlow() => currentSlow = maxSlow;
}