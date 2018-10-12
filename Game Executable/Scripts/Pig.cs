using UnityEngine;
using System.Collections;

/**
  * class Pig is a vehicle that will simply wander around.
  * When the ufo "controls" it, it will freeze in place.
  */
public class Pig : Vehicle
{

    // booleans to check if it should wander, freeze, or move toward the ufo
    private bool controlled;
    private bool isAbducted;
    private Vector3 ufoPos;

    // Initialize this pig
    override public void Start()
    {
        base.Start();
        controlled = false;
        isAbducted = false;
    }

    /**
      * Calculates the steering force for this pig.
      */
    protected override void CalcSteeringForce()
    {
        // Reset "ultimate force" that will affect this seeker's movement
        steeringForce = Vector3.zero;

        //** THIS IS WHERE INDIVIDUAL STEERING FORCES ARE CALLED **

        // If this pig isn't controlled, wander around
        if (!controlled)
        {
            steeringForce += Wander();
        }
        // If its being abducted move toward the ufo
        if (isAbducted)
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
