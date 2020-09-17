using UnityEngine;

// http://engineeringdotnet.blogspot.com/2010/04/simple-2d-car-physics-in-games.html
public class PlayerCar : MonoBehaviour
{
    Rigidbody2D rb;
    Vector2 carLocation;

    float carHeading = 0f;
    float carSpeed = 0f;
    float acceleration = 10f;
    float maxSpeed = 30f;
    float steerAngle = 0f;
    float maxSteerAngle = 35f;
    float wheelBase = 4;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        carLocation = transform.position;
    }

    private void Update()
    {
        // Step Alpha - Get inputs
        steerAngle = Input.GetAxis("Horizontal") * maxSteerAngle * Mathf.Deg2Rad;
        carSpeed = Input.GetAxis("Vertical") * acceleration * (maxSpeed / 10);
    }

    private void FixedUpdate()
    {
        // Step 1 - Wheel positions
        // -> Position on pythagoram unit circle (across and up)
        var unitCirclePos = new Vector2( Mathf.Cos(carHeading), Mathf.Sin(carHeading));
        var unitCirclePosWithSteer = new Vector2(Mathf.Cos(carHeading + steerAngle), Mathf.Sin(carHeading + steerAngle));

        // -> Position of wheels from center of vehicle
        Vector2 frontWheel = carLocation + wheelBase / 2 * unitCirclePos;
        Vector2 rearWheel = carLocation - wheelBase / 2 * unitCirclePos;

        // Step 2 - Wheel velocities
        // -> Apply velocity to the wheels in sync with frames, then combine velocity scale with wheel vector positions
        frontWheel += carSpeed * Time.deltaTime * unitCirclePosWithSteer;
        rearWheel += carSpeed * Time.deltaTime * unitCirclePos;

        // Step 3 - Application
        // -> Find the center of the car by inversing the wheels position vectors
        carLocation = (frontWheel + rearWheel) / 2;
        // -> Update the car heading angle by measuring the new angle between the front and back wheel's vectors
        carHeading = Mathf.Atan2(frontWheel.y - rearWheel.y, frontWheel.x - rearWheel.x);

        // Step 4 - Return it to the rigidBody component (testing with transform first)
        rb.position = carLocation;
        rb.rotation = carHeading * Mathf.Rad2Deg;
    }

    /*
    private void FixedUpdate()
    {
        // Get input
        float h = -Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Calculate speed from input and acceleration (transform.up is forward)
        var a = v > 0 ? acceleration : reverseAcceleration;
        Vector2 speed = transform.right * (v * a);
        rb.AddForce(speed);

        // Create car rotation
        float direction = Vector2.Dot(rb.velocity, rb.GetRelativeVector(Vector2.right));
        if (direction >= 0.0f)
        {
            rb.rotation += h * (steering) * (rb.velocity.magnitude / maxSpeed);
        }
        else
        {
            rb.rotation -= h * (steering) * (rb.velocity.magnitude / maxSpeed);
        }

        // Change velocity based on rotation
        float driftForce = Vector2.Dot(rb.velocity, rb.GetRelativeVector(Vector2.left)) * 2.0f;
        Vector2 relativeForce = Vector2.right * driftForce;
        rb.AddForce(rb.GetRelativeVector(relativeForce));

        // Debug.DrawLine(rb.position, rb.GetRelativePoint(relativeForce), Color.green);
        // Debug.Log(relativeForce.magnitude);
        
        // Force max speed limit
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        currentSpeed = rb.velocity.magnitude;

        // Some experimental skid stuff
        if (rb.GetRelativeVector(relativeForce).magnitude > 15 && Mathf.Abs(h * steering * (rb.velocity.magnitude / maxSpeed)) > 2)
        {
            //instantiate the prefab
            GameObject skid = Instantiate(SkidFab);

            //Set some properties
            skid.transform.position = transform.position;
            skid.transform.rotation = transform.rotation;

            //keep reference of all skidmarks
            Skidmarks.Add(skid);

            if (Skidmarks.Count > 50)
            {
                Skidmarks.RemoveAt(0);
            }
        }
    }*/
}
