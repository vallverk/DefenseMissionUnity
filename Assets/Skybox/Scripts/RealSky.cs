////////////////////////////////////////////
///                                      ///
///         RealSky Version 1.4          ///
///  Created by: Black Rain Interactive  ///
///                                      ///
//////////////////////////////////////////// 

// Super stripped down version 			  //
// Edited to be just a sky which spins 	  //

using UnityEngine;
using System.Collections;

public class RealSky : MonoBehaviour {

	public Texture dayTime;
	public float skySpeed = 0.0f;
	
    public Camera mainCamera;
    public int skyBoxLayer = 8;

    float counter = 0.0f;
    bool isPaused = false;
    GameObject skyCamera;
	
	void Awake(){

        StartCoroutine("Counter");
		StartCoroutine("SkyRotation");

        if (mainCamera == null)
            return;

        gameObject.layer = skyBoxLayer;

        skyCamera = new GameObject("SkyboxCamera");
        skyCamera.AddComponent<Camera>();
        skyCamera.camera.depth = -10;
        skyCamera.camera.clearFlags = CameraClearFlags.Color;
        skyCamera.camera.cullingMask = 1 << skyBoxLayer;
        skyCamera.transform.position = gameObject.transform.position;

        mainCamera.cullingMask = 1;
        mainCamera.clearFlags = CameraClearFlags.Depth;

	}
	
	void Start(){

		renderer.material.SetTexture("_Texture01", dayTime);

	}

    void Update(){

        if (mainCamera != null){
            skyCamera.transform.rotation = mainCamera.transform.rotation;
        }

    }

    IEnumerator Counter(){

        while (true){

            if (isPaused == false){
                counter += Time.deltaTime;
            }

            yield return null;

        }

    }
	
	IEnumerator SkyRotation(){
		
		while (true){

            transform.Rotate(Vector3.up * Time.deltaTime * skySpeed, Space.World);
			yield return null;
			
		}
	}

}