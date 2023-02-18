using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    public bool darkMode = false;
    public GameManager gm;
    public TimeManager tm;
    public GameState gameState;

    public enum GameState { Menu, Playing, Paused, GameOver }
    Transform[] objects;

    void Start()
    {
        gm = GetComponent<GameManager>();
        tm = GetComponent<TimeManager>();
        gameState = GameState.Menu;
        objects = GetComponentsInChildren<Transform>();
    }


    public bool playing(){
        return gameState == GameState.Playing;
    }

    public void invert(){
        foreach (Transform obj in objects){
            Inverter.invertGameObject(obj.gameObject);
        }
    }
}
