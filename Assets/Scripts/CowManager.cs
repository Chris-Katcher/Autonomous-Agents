using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
  * class CowManager instantiates all the cows and all the pigs
  * into their respective lists
  */
public class CowManager : MonoBehaviour {

    // Game Objects and Prefabs for the ufo, cow and pig
    public GameObject seekerGO;
    public GameObject cowGO;
    public GameObject cowPrefab;
    public GameObject pigPrefab;
    
    // used for creating new pigs and cows
    public GameObject go;

    //Fence boundaries for the cows and pigs
    private Vector3 SECow;
    private Vector3 NWCow;
    private Vector3 SEPig;
    private Vector3 NWPig;

    private float minX;
    private float maxX;
    private float minZ;
    private float maxZ;

    //Position vector
    private Vector3 randPos;
    
    // Lists of the cows and pigs
    public List<GameObject> cows;
    public List<GameObject> pigs;
    
    // Initialize the lists
    void Start ()
    {
        // Get the boundaries
        SECow = GameObject.Find("CowFenceSE").transform.position;
        NWCow = GameObject.Find("CowFenceNW").transform.position;

        SEPig = GameObject.Find("PigFenceSE").transform.position;
        NWPig = GameObject.Find("PigFenceNW").transform.position;

        minX = SECow.x;
        minZ = SECow.z;
        maxZ = NWCow.z;
        maxX = NWCow.x;

        // Instantiate the cows and pigs lists
        cows = new List<GameObject>();
        pigs = new List<GameObject>();

        // Put a cow and a pig at a random spot in their pen, then add them to the list
        for (int i = 0; i < 10; i++)
        {
            randPos = new Vector3(Random.Range(minX, maxX), 2, Random.Range(minZ, maxZ));
            go = (GameObject)Instantiate(cowPrefab, randPos, Quaternion.identity);
            go.transform.localScale = new Vector3(0.0035f, 0.0035f, 0.0035f);
           
            cows.Add(go);

            randPos = new Vector3(Random.Range(SEPig.x, NWPig.x), 0.25f, Random.Range(SEPig.z, NWPig.z));
            go = (GameObject)Instantiate(pigPrefab, randPos, Quaternion.identity);
            go.transform.localScale = new Vector3(.025f, .025f, .025f);
            pigs.Add(go);
            
        }
        
    }
	
    // Cow manager doesn't need to update anything
	void Update () {
	
	}

}
