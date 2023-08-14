using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour {

    static TimeManager instance;
    float slowDownLength = 2f;
    float slowDownTimer = 0f;
    float defaultTimeScale;
    public float minParticleSpeed = 5f;
    public float maxParticleSpeed = 20f;
    public bool isTryingToSlow = false;
    public bool isSlowing = false;
    bool canRestoreSlow;

    void Awake(){
        instance = this;
    }

    void Start(){
        defaultTimeScale = Time.timeScale;
    }

    void FixedUpdate(){
        if(instance == null) instance = this;
        if (isSlowing) SlowMoBar.takeSlow();
        else{
            if (canRestoreSlow && !isTryingToSlow) SlowMoBar.restoreSlow(); 
            else TimeManager.executeAfterSeconds(2f, () => canRestoreSlow = true);
        }
    }

    void Update(){
        isSlowing = slowDownTimer > 0 && isTryingToSlow && SlowMoBar.currentSlow > 0;
        var slowDownScale = isSlowing ? slowDownLength : 0.5f;
        Time.timeScale += (1f / slowDownScale) * Time.unscaledDeltaTime;
        Time.timeScale = Mathf.Clamp(Time.timeScale, 0, 1f);

        if (isSlowing){
            slowDownTimer -= Time.deltaTime;
            canRestoreSlow = false;
        }
    }

    public void SlowMotion(float slowDownStrength, float slowDownLen){
        isTryingToSlow = true;
        if (SlowMoBar.currentSlow <= 0) return;
        Time.timeScale = slowDownStrength;
        slowDownLength = slowDownLen;
        slowDownTimer = slowDownLen;
    }

    public delegate void Function();
    public static void executeAfterSeconds(float duration, Function func){
        instance.StartCoroutine(execute(duration, func));
    }

    static IEnumerator execute(float duration, Function functionToExecute){
        yield return new WaitForSeconds(duration);
        functionToExecute();
    }
}
