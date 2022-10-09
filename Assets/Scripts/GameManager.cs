using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    public bool darkMode = false;
    public GameManager gm;
    public TimeManager tm;
    public InvertColor ic;

    void Start()
    {
        gm = GetComponent<GameManager>();
        tm = GetComponent<TimeManager>();
        ic = GetComponent<InvertColor>();
    }

    void Update()
    {
        
        
    }
}
