using UnityEngine;
using System.Collections;

//add using System.Collections.Generic; to use the generic list format
using System.Collections.Generic;

// The GameObject that this vehicle script is on must have a Character Controller component
[RequireComponent(typeof(CharacterController))]

/**
  * Vehicle is the subclass for all the autonomous agents in the scene.
  * It has methods to calculate the steering forces, then applies them.
  */
abstract public class Vehicle : MonoBehaviour {

    // Reference to the CharacterController component
    // This is a "shortcut" that saves time later in the script by removing the need 
    //   to find the CharacterController component every time you want to use it
    protected CharacterController charControl;

    // Reference to the SimManager script
    // This is a "shortcut" that saves time later in the script by removing the need 
    //   to find the SimManager script on the SimulationManagerGO object
    //   every time you want to use it
    protected CowManager managerScript;

    // Fields necessary for vector-based movement
    // May need velocity, position, acceleration, direction, speed
    // position --> transform.position
    // direction --> transform.forward
    protected Vector3 velocity;
    protected Vector3 acceleration;
    protected float maxSpeed;
    public float mass;
    public float maxForce;
    public float wanderAngle;
    private Vector3 desiredVel;
    protected Vector3 steeringForce;

    // Check to see if this is a cow
    public bool isCow;

    // Classes that extend Vehicle must override CalcSteeringForce
    abstract protected void CalcSteeringForce();


    /// <summary>
    /// Start method
    /// Initialize necessary values
    /// </summary>
    virtual public void Start()
    {
        // CharacterController component is on the same GameObject as this script, 
        //   so we can simply call GetComponent to find it
        charControl = this.GetComponent<CharacterController>();

        // SimManager script is on another GameObject in the scene - to find it, use the static Find method
        //   of the GameObject class and oass in the name of the GameObject in the hierarchy
        managerScript = GameObject.Find("CowManagerGO").GetComponent<CowManager>();

        velocity = new Vector3(Random.Range(-1, 1), 0f, Random.Range(-1, 1));
        //acceleration = Vector3.zero;

        // testing to get the dude to move
        acceleration = new Vector3(0, 0, 0);
        wanderAngle = Random.Range(0, 2 * Mathf.PI);
        if(!isCow)
        {
            maxSpeed = 1;
        }
        
       // mass = 1;
        //maxForce = 15;
    }

	
	// Update is called once per frame
    /// <summary>
    /// Update method
    /// Applies vector-based movement to this autonomous agent
    /// </summary>
    protected void Update () 
    {
        CalcSteeringForce();

        // movement "formula"
        // looks a little different in Unity and when using a CharacterController component
        // 1. add accel to vel
        velocity = velocity + acceleration;

        // 2. limit to max speed using ClampMagnitude method of the Vector3 class
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        // 3. add vel to pos using Move method of CharacterController component
        charControl.Move(velocity * Time.deltaTime);

        // Turn seeker to face right direction
        this.transform.right = velocity.normalized;

        //this.transform.Rotate(velocity.normalized * Time.deltaTime);
        // Zero out acceleration to limit its magnitude
        // Allows seeker ( 
        acceleration = Vector3.zero;

    }

    /// <summary>
    /// ApplyForce method
    /// Applies an incoming force to an acceleration vector
    /// Incoming force is most likely calculated inside CalcSteeringForces() method
    /// </summary>
    /// <param name="steeringForce">Vector3 steeringForce, the force to be applied </param>
    /// <returns></returns>
    protected void ApplyForce(Vector3 steeringForce)
    {
        acceleration += steeringForce / mass;
    }


    /// <summary>
    /// Seek method
    /// Calculates the steering vector necessary to move an agent toward its target
    /// </summary>
    /// <param name="targetPos">targetPos - position of this Vehicle's target</param>
    /// <returns>Steering vector, will be applied to acceleration</returns>
    protected Vector3 Seek(Vector3 targetPos)
    {
        // ME --> Target 
        // Desired = target - mine

        // 1. Calculate desired velocity (vec from myself to my target)

        desiredVel = targetPos - this.transform.position;

        // Zero out Y component so we don't try to fly
        //desiredVel.y = 0;

        // 2. Normalize my desired vector and * max speed to get a vec with the mag
        //      of the max speed
        desiredVel.Normalize();
        desiredVel = desiredVel * maxSpeed;

        // 3. Calculate the steering force required to move this seeker 
        //      toward the target
        //      Try to "match" the vector as much as possible
        steeringForce = desiredVel - velocity;
        // 4. Return the steering force
        return steeringForce;
    }

    /**
      * Wander is an implimentation of an agent randomly wandering around
      */
    public Vector3 Wander()
    {
        // Similar to pursue, get the future pos of this agent
        desiredVel = velocity;
        desiredVel.Normalize();

        Vector3 displacement = Vector3.zero;
        
        // Add a random angle to the future position
        wanderAngle += Random.Range(-.005f, .010f) ;

        
        displacement.x = Mathf.Cos(wanderAngle);
        displacement.z = Mathf.Sin(wanderAngle);

        // force = this velocity + new angled velocity
        steeringForce = desiredVel + displacement;
        
        return steeringForce;
    }

    /**
      * Approach is an implimentation of an agent approaching a target pos
      */
    public Vector3 Approach(Vector3 targetPos)
    {
        // ME --> Target 
        // Desired = target - mine

        // 1. Calculate desired velocity (vec from myself to my target)
         
        desiredVel = targetPos - this.transform.position;

        // Zero out Y component so we don't try to fly
        desiredVel.y = 0;

        float dist = Vector3.Distance(this.transform.position, targetPos);

        // If this agent is 300 units away from the target, go max speed
        if(dist > 300)
        {
            desiredVel *= maxSpeed;
        }

        // Otherwise, go a reduced speed
        else
        {
            desiredVel *= (maxSpeed * dist / 300);

        }

        steeringForce = desiredVel - velocity;
        return steeringForce;
    }

    /**
      * Get the velocity of this vehicle
      */
    public Vector3 getVelocity()
    {
        return this.velocity;
    }

    /**
      * Set the max speed of this vehicle
      */
    public void setMaxSpeed(int speed)
    {
        this.maxSpeed = speed;
    }
}
