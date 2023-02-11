using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularFloat : MonoBehaviour
{
    [SerializeField] float angularSpeed = 1f;
    [SerializeField] float circleRad = 1f;
    [SerializeField] float rotationSpeed = 1f;

    Vector2 fixedPoint;
    float currentAngle;


    void Start()
    {
        fixedPoint = transform.position;
    }

    void Update()
    {
        currentAngle += angularSpeed * Time.deltaTime;
        Vector2 offset = new Vector2(Mathf.Sin(currentAngle), Mathf.Cos(currentAngle)) * circleRad;
        transform.position = fixedPoint + offset;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + rotationSpeed);
    }
}
