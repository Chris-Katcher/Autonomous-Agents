using UnityEngine;
using System.Collections;

//add using System.Collections.Generic; to use the generic list format
using System.Collections.Generic;


/**
  * class Seeker impliments several properties of the ufo.
  * This will seek out cows and pigs randomly and abduct them.
  * It will make a crop circle when the user presses 'C'
  * It will fly away once there are no more pigs or cows.
  */
public class Seeker : Vehicle {

    // Reference to this seeker's target
    public List<GameObject> seekerTarget;

    // Reference to the cow manager
    public GameObject CowManager;
    
    
    // Reference to the particle system for the ufo
    ParticleSystem ps;
    ParticleSystem.EmissionModule em;

    // Refences to the cows and pigs it will be abducting
    private GameObject co;
    private GameObject pig;

    // The cow manager script
    CowManager cowz;
    
    // The current target of the ufo (can be a pig, cow, or point that its approaching)
    Vector3 target;

    // Helper booleans so it knows what it's doing
    private bool abducting;
    private bool isNotPig;
    private bool cropCircle;
    private bool destroyingCorn;
    bool started;
    bool finished;
    bool allFinished;

    // Next point to approach in the corn field
    private int nextPos;
    
    // Reference to a row of corn
    GameObject cornRow;

    // Next corn to be removed
    int nextCorn;

    // List of all the corn that will be removed by the UFO
    List<GameObject> row1;

    // Count frames and only destroy a corn on the 5th one
    int counter;
    
    // Initialize the UFO's properties, also chose the first target of the UFO
    override public void Start () 
    {
        base.Start();

        // Set the max speed
        this.maxSpeed = 57;

        // Get the particle system
        ps = GameObject.Find("UFO Particles").GetComponent<ParticleSystem>();
        em = ps.emission;

        // Disable emission
        em.enabled = false;

        // Get the CowManager Script
        cowz = CowManager.GetComponent<CowManager>(); 
        nextCorn = 0;

        // Randomly chose to get a cow or a pig
        if(Random.Range(-10, 10) > 0)
        {
            co = cowz.cows[0];
            target = cowz.cows[0].transform.position;
            co.GetComponent<Cow>().setControled();
            isNotPig = true;
        } else
        {
            co = cowz.pigs[0];
            target = cowz.pigs[0].transform.position;
            co.GetComponent<Pig>().setControled();
            isNotPig = false;
        }
        
        // Set the booleans to their correct values
        abducting = false;
        destroyingCorn = false;
        started = false;
        finished = false;
        allFinished = false;
       

    }


    // All child vehicles need to override CalcSteeringForce
    /// <summary>
    /// CalcSteeringForce method
    /// Accumulates steering vectors from multiple forces, like Seek and Flee, into one Ultimate Force
    /// Weights the steering force vectors
    /// Applies the resulting Ultimate Force to the vehicle's acceleration
    /// 
    /// There is a lot of logic here because the UFO will need to decide if its going to a cow/pig or a
    /// point in the corn field.
    /// </summary>
    protected override void CalcSteeringForce()
    {
        // Reset "ultimate force" that will affect this seeker's movement
        steeringForce = Vector3.zero;

        // If there are no more cows or pigs, were done. Go out into space
        if (allFinished)
        {
            steeringForce.y += 20;
            Debug.Log(steeringForce);
            em.enabled = false;

        }

        // We're not done
        else {

            // If we are destroying corn, call the destroy corn method
            if (destroyingCorn)
            {
                destroyCorn();
            }

            // If the user wants to destroy corn, check if we are able to do so.
            // If we are start heading towards the corn field
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (!abducting && !finished && !started && !cropCircle)
                {
                    cropCircle = true;

                    this.nextPos += 1;
                    target = GameObject.Find("Pos" + nextPos).transform.position;
                    getCorn();
                }

            }

            //** THIS IS WHERE INDIVIDUAL STEERING FORCES ARE CALLED **

            // If we are moving, continue moving
            if (!abducting)
            {
                if (!cropCircle)
                {
                    steeringForce += Approach(target);
                }
                else
                {
                    steeringForce += Approach(target);
                }

            }

            // Don't allow the steering force to be too big
            steeringForce = Vector3.ClampMagnitude(steeringForce, maxForce);

            // If we arrived at our target we will now check what we should do next
            if (this.steeringForce.magnitude <= 0f)
            {
                // if we are making the crop circle, set the target to the next position in the corn field
                if (cropCircle)
                {
                    destroyingCorn = false;
                    started = true;
                    nextPos += 1;
                    if (nextPos < 12)
                    {
                        this.target = GameObject.Find("Pos" + nextPos).transform.position;
                    }

                }

                // Otherwise we are above a cow/pig and we should abduct it
                else
                {
                    // Emit the tracktor beam
                    em.enabled = true;
                    
                    // Notify the cow/pig that it is being abducted
                    if (isNotPig && cowz.cows.Count != 0)
                    {
                        co.GetComponent<Cow>().abduct();
                    }
                    else if (cowz.pigs.Count != 0)
                    {
                        co.GetComponent<Pig>().abduct();
                    }
                    else
                    {
                        co.GetComponent<Cow>().abduct();
                    }

                    // We are now abducting the creature, don't move
                    abducting = true;

                    // At this point we are going to check if the cow/pig is done being abducted
                    if (isNotPig)
                    {

                        // Make sure this cow doesn't collide with the ufo
                        if (cowz.cows.Count != 0)
                        {
                            Physics.IgnoreCollision(cowz.cows[0].GetComponent<Collider>(), this.GetComponent<Collider>());
                        }

                        // If the cow made it to the ship
                        if (co.transform.position.y >= 45)
                        {

                            // And the count is not 0
                            if (cowz.cows.Count == 0)
                            {

                            }
                            else
                            {
                                // Destroy this game object
                                Destroy(cowz.cows[0]);

                                // Remove it from the list
                                cowz.cows.RemoveAt(0);

                                // And find a new target
                                if (Random.Range(-10, 10) > 0 && cowz.pigs.Count != 0)
                                {
                                    co = cowz.pigs[0];
                                    target = cowz.pigs[0].transform.position;
                                    co.GetComponent<Pig>().setControled();
                                    isNotPig = false;
                                }
                                else if (cowz.cows.Count != 0)
                                {
                                    co = cowz.cows[0];
                                    target = cowz.cows[0].transform.position;
                                    co.GetComponent<Cow>().setControled();
                                    isNotPig = true;
                                }

                                // If that was the last cow/pig, go out into space
                                else
                                {
                                    target = new Vector3(0, 1000, 0);
                                    steeringForce += Seek(target);
                                    allFinished = true;
                                }
                                // Done abducting now
                                abducting = false;
                            }
                        }
                    }

                    // This is a pig, similar logic to perform, just have to remove the pig
                    else
                    {
                        // Ignore the pigs collisions with the ufo
                        if (cowz.pigs.Count != 0)
                        {
                            Physics.IgnoreCollision(cowz.pigs[0].GetComponent<Collider>(), this.GetComponent<Collider>());
                        }

                        // If the pig is in the ship, destroy it and stop abducting
                        if (co.transform.position.y >= 45)
                        {
                            if (cowz.pigs.Count == 0)
                            {

                            }
                            else
                            {
                                // Destroy the pig
                                Destroy(cowz.pigs[0]);
                                cowz.pigs.RemoveAt(0);

                                // Find a new target
                                if (Random.Range(-10, 10) > 0 && cowz.pigs.Count != 0)
                                {
                                    co = cowz.pigs[0];
                                    target = cowz.pigs[0].transform.position;
                                    co.GetComponent<Pig>().setControled();
                                    isNotPig = false;
                                }
                                else if (cowz.cows.Count != 0)
                                {
                                    co = cowz.cows[0];
                                    target = cowz.cows[0].transform.position;
                                    co.GetComponent<Cow>().setControled();
                                    isNotPig = true;
                                }
                                // Again if we're out of creatures, go into space
                                else
                                {
                                    target = new Vector3(0, 1000, 0);
                                    steeringForce += Seek(target);
                                    allFinished = true;
                                }
                                // Stop abducting
                                abducting = false;
                            }
                        }
                    }

                }

            }
            // We haven't reached our current target yet so no need to destroy or abduct anything
            else
            {
                // If we are doing a crop circle, we want the beam to emit while moving
                if (!cropCircle)
                {
                    em.enabled = false;
                }

            }
            // If were moving above the crop field, destroy corn
            if (this.steeringForce.magnitude >= 5f)
            {
                if (cropCircle && started)
                {
                    destroyingCorn = true;
                }
            }
            // We are not moving so don't destroy corn
            else
            {
                destroyingCorn = false;
            }
        }
            // Have the "ultimate force" affect the seeker's movement
            ApplyForce(steeringForce);
    }

    /**
      * getCorn will create a list of all the corn that is to be destroyed by the ufo
      */
    private void getCorn()
    {
        // Affectively, we get the rows and columns of each corn GameObject that needs to be destroyed.
        cornRow = GameObject.Find("Row2");
        row1 = new List<GameObject>();
        
        for(int i = 2; i < cornRow.transform.childCount; i++)
        {
            row1.Add(cornRow.transform.FindChild("corn (" + i + ")").gameObject);
        }
        for(int i = 3; i < 22; i++)
        {
            row1.Add(GameObject.Find("Row" + i).transform.FindChild("corn (" + 55 + ")").gameObject);
        }
        cornRow = GameObject.Find("Row21");
        for (int i = cornRow.transform.childCount - 1; i > 2; i--)
        {
            row1.Add(cornRow.transform.FindChild("corn (" + i + ")").gameObject);
        }
        for (int i = 21; i > 2; i--)
        {
            row1.Add(GameObject.Find("Row" + i).transform.FindChild("corn (" + 2 + ")").gameObject);
        }
        cornRow = GameObject.Find("Row4");
        for (int i = 4; i < cornRow.transform.childCount - 2; i++)
        {
            row1.Add(cornRow.transform.FindChild("corn (" + i + ")").gameObject);
        }
        for (int i = 5; i < 20; i++)
        {
            row1.Add(GameObject.Find("Row" + i).transform.FindChild("corn (" + 53 + ")").gameObject);
        }
        cornRow = GameObject.Find("Row19");
        for (int i = cornRow.transform.childCount - 3; i > 3; i--)
        {
            row1.Add(cornRow.transform.FindChild("corn (" + i + ")").gameObject);
        }
        for (int i = 18; i > 4; i--)
        {
            row1.Add(GameObject.Find("Row" + i).transform.FindChild("corn (" + 4 + ")").gameObject);
        }

    }

    /**
      * destroyCorn will destroy the next corn in the list if this is the 5th time it was called
      */
    private void destroyCorn()
    {
        // Emit the tracktor beam
        em.enabled = true;

        // If the list is empty, were done making the crop circle, go back to abducting cows/pigs
        if(row1.Count == 0)
        {
            destroyingCorn = false;
            cropCircle = false;
            started = false;
            finished = true;
            if(cowz.cows.Count != 0)
            {
                co = cowz.cows[0];
                target = cowz.cows[0].transform.position;
                co.GetComponent<Cow>().setControled();
                isNotPig = true;
            }
            else if(cowz.pigs.Count != 0)
            {
                co = cowz.pigs[0];
                target = cowz.pigs[0].transform.position;
                co.GetComponent<Pig>().setControled();
                isNotPig = false;
            }
            else
            {
                target = new Vector3(0, 1000f, 0);
                allFinished = true;
            }

        }
        // Otherwise if this is the 5th time this was called, destroy a corn
        else if(counter % 5 == 0 && row1.Count != 0)
        {
            GameObject.Destroy(row1[nextCorn]);
            row1.RemoveAt(nextCorn);
        }
        counter++;
    }

    
}
