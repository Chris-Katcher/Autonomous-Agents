using UnityEngine;
using System.Collections;

//add using System.Collections.Generic; to use the generic list format
using System.Collections.Generic;

/**
  * class SimManager manages the character controlled objects (the cameras). 
  * It also creates a new UFO (seeker)
  */
public class SimManager : MonoBehaviour {

    // A Seeker and the object of his desire
    public GameObject seekerGO;
    

    // Prefabs (the models to use when instantiating the above GO)
    public GameObject ufoPrefab;
    
    // Cameras used by the player
    public Camera camera1;
    public Camera camera2;
    public Camera camera3;
    public Camera camera4;
    public Camera camera5;

    // Initialize the ufo object and the cameras
    void Start () 
    {

        // Instantiate the target
        //  define a position
        // then instantiate the GO
        Vector3 position = new Vector3(0f, 36.3f, 0f);
        seekerGO = (GameObject)Instantiate(ufoPrefab, position, Quaternion.identity);
        seekerGO.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        // Set the cowManager for the seeker
        seekerGO.GetComponent<Seeker>().CowManager = GameObject.Find("CowManagerGO");
        seekerGO.GetComponent<Seeker>().seekerTarget = GameObject.Find("CowManagerGO").GetComponent<CowManager>().cows;

        // Get all the cameras in the scene then set the fps controller to be running first
        camera1 = GameObject.Find("FirstPersonCharacter").GetComponent<Camera>();
        camera2 = GameObject.Find("Camera2").GetComponent<Camera>();
        camera3 = GameObject.Find("Camera3").GetComponent<Camera>();
        camera4 = GameObject.Find("Camera4").GetComponent<Camera>();
        camera5 = GameObject.Find("Camera5").GetComponent<Camera>();
        camera1.enabled = true;
        camera2.enabled = false;
        camera3.enabled = false;
        camera4.enabled = false;
        camera5.enabled = false;
    }
	
    /**
      * Update is used to check if the player changed the camera.
      * This is done by pressing 1-5 on the keyboard.
      */
    void Update () 
    {

        // Go to the fps controller
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            camera1.enabled = true;
            camera2.enabled = false;
            camera3.enabled = false;
            camera4.enabled = false;
            camera5.enabled = false;
        }

        // Go to the 2nd camera
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            camera1.enabled = false;
            camera2.enabled = true;
            camera3.enabled = false;
            camera4.enabled = false;
            camera5.enabled = false;
        }

        // Go to the third camera
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            camera1.enabled = false;
            camera2.enabled = false;
            camera3.enabled = true;
            camera4.enabled = false;
            camera5.enabled = false;
        }

        // Go to the forth camera
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            camera1.enabled = false;
            camera2.enabled = false;
            camera3.enabled = false;
            camera4.enabled = true;
            camera5.enabled = false;
        }

        // Go to the fifth camera
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            camera1.enabled = false;
            camera2.enabled = false;
            camera3.enabled = false;
            camera4.enabled = false;
            camera5.enabled = true;
        }
    }
}
