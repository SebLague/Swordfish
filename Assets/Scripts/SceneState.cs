using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SceneState : MonoBehaviour {

    [HideInInspector]
    public bool inMenuState;

    public GameObject[] activeInMenu;
    public GameObject[] inactiveInMenu;
    public MeshRenderer screenOverlay;
    public CinemachineVirtualCamera menuCam;

    public void SetMenuState()
    {
        menuCam.Priority = 15;
        foreach (GameObject g in inactiveInMenu)
        {
            g.SetActive(false);
        }
        foreach (GameObject g in activeInMenu)
		{
			g.SetActive(true);
		}
        screenOverlay.sharedMaterial.color = Color.black;
    }

	public void SetPlayableState()
	{
        menuCam.Priority = 5;
		foreach (GameObject g in inactiveInMenu)
		{
			g.SetActive(true);
		}
		foreach (GameObject g in activeInMenu)
		{
			g.SetActive(false);
		}
        screenOverlay.sharedMaterial.color = Color.clear;
	}
}
