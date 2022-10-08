using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera vcam;
    [SerializeField] float initialZoom, maxZoom;

    PlayerController ps;
    
    void Start()
    {
        ps = GameObject.Find("Player").GetComponent<PlayerController>();
        initialZoom = vcam.m_Lens.OrthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        var zoom = vcam.m_Lens.OrthographicSize;

        if(ps.inInfiniteDashZone){
            if(zoom != maxZoom ) vcam.m_Lens.OrthographicSize = Mathf.Lerp(zoom, maxZoom, 0.1f);
        }
        else{
            if(zoom != initialZoom) vcam.m_Lens.OrthographicSize = Mathf.Lerp(zoom, initialZoom, 0.1f);
        }
    }
}
