using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlowMoBar : MonoBehaviour
{
    public Image slowMoBar;

    public float currentSlow, maxSlow = 100f;
    [SerializeField] float slowRate = 1f;
    float lerpSpeed;
 
    void Start()
    {
        currentSlow = maxSlow;
    }

    // Update is called once per frame
    void Update()
    {
        Mathf.Clamp(currentSlow, 0f, maxSlow);
        slowMoBarFiller();

        lerpSpeed = 10f * Time.deltaTime;
    }

    void slowMoBarFiller(){
        slowMoBar.fillAmount = Mathf.Lerp(slowMoBar.fillAmount, currentSlow / maxSlow, lerpSpeed);
    }

    public void takeSlow(){
        if(currentSlow > 0){
            currentSlow -= slowRate;
        }
    }

    public void restoreSlow(){
        if(currentSlow < maxSlow){
            currentSlow += slowRate / 2;
        }
    }
}
