using UnityEngine;
using System.Collections;

/**
  * class Cow is a vehicle that will simply wander around.
  * When the ufo "controls" it, it will freeze in place.
  */
public class Cow : Vehicle {

    // booleans to check if it should wander, freeze, or move toward the ufo
    private bool controlled;
    private bool isAbducted;
    private Vector3 ufoPos;
    
    /**
      * Initialize this cow
      */ 
    override public void Start ()
    {
        base.Start();
        controlled = false;
        isAbducted = false;
        
       
    }

    /**
      * Calculates the steering force for this cow.
      */
    protected override void CalcSteeringForce()
    {
        // Reset "ultimate force" that will affect this seeker's movement
        steeringForce = Vector3.zero;

        //** THIS IS WHERE INDIVIDUAL STEERING FORCES ARE CALLED **
        
        if(!controlled)
        {
            steeringForce += Wander();
        }
        if(isAbducted)
        {
            this.maxSpeed = 10;
            steeringForce += Seek(ufoPos);
        }

        // Don't allow the steering force to be too big
        steeringForce = Vector3.ClampMagnitude(steeringForce, maxForce);

        // Have the "ultimate force" affect the seeker's movement
        ApplyForce(steeringForce);
    }

    /**
      * Sets this cow to be controled
      */
    public void setControled()
    {
        controlled = true;
        this.velocity = Vector3.zero;
        
    }

    /**
      * abducts this cow
      */
    public void abduct()
    {
        isAbducted = true;
        ufoPos = this.transform.position + new Vector3(0, 36.4f, 0);
    }
}
