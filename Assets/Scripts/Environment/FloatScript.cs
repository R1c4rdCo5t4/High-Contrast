using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatScript : MonoBehaviour
{
    float xDefaultPos, yDefaultPos;
    public Vector2 amplitude;
    public Vector2 speed;

    void Start()
    {
        xDefaultPos = transform.position.x;
        yDefaultPos = transform.position.y;
    }

    void Update() => transform.position = new Vector2(xDefaultPos + amplitude.x * Mathf.Cos(speed.x * Time.time), yDefaultPos + amplitude.y * Mathf.Sin(speed.y * Time.time));

}
