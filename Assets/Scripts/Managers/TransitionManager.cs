using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    public float offset;
    public float transitionTime = 1f;

    private Vector3 initialPosition;
    private float elapsedTime = 1f;

    public void moveCamera()
    {
        Vector3 camPos = Camera.main.transform.position;
        if (offset != camPos.x && elapsedTime >= transitionTime){
            initialPosition = camPos;
            elapsedTime = 0.0f;
        }
        
    }

    void Update()
{
    if (elapsedTime < transitionTime)
    {
        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / transitionTime);
        Vector3 targetPosition = new Vector3(offset, initialPosition.y, initialPosition.z);
        Camera.main.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
    }
}

}
