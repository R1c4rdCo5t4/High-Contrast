using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyperDash : MonoBehaviour
{
    public Vector2 dir;
    public float speed = 1f;
    public float duration = 1f;
    public bool changeFacing;
    public bool gravity;

    void Update()
    {
        dir = new Vector2(Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(transform.eulerAngles.z * Mathf.Deg2Rad)) * speed;
    }
}
