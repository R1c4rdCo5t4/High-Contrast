using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyperDash : MonoBehaviour
{
    public Dash dash;
    [SerializeField] Vector2 dir;
    [SerializeField] float speed;
    [SerializeField] float duration;
    // public bool changeFacing;
    // public bool gravity;


    void Update()
    {
        dir = new Vector2(Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(transform.eulerAngles.z * Mathf.Deg2Rad)) * speed;
        dash = new Dash(dir, speed, duration, true);
    }
}



