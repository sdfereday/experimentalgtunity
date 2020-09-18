using UnityEngine;
using System.Collections.Generic;

// http://engineeringdotnet.blogspot.com/2010/04/simple-2d-car-physics-in-games.html
public class PlayerCar : MonoBehaviour
{
    Rigidbody2D rb;
    Vector2 carLocation;

    float carHeading = 0f;
    float carSpeed = 0f;
    float acceleration = 10f;
    float maxSpeed = 10f;
    float steerAngle = 0f;
    float maxSteerAngle = 40f;
    float wheelBase = 4;

    List<GameObject> Skidmarks = new List<GameObject>();
    public GameObject SkidFab;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        carLocation = transform.position;
    }

    private void Update()
    {
        // Step Alpha - Get inputs
        steerAngle = -Input.GetAxis("Horizontal") * maxSteerAngle * Mathf.Deg2Rad;
        carSpeed = Input.GetAxis("Vertical") * acceleration * (maxSpeed / 10);
        
        // Step Beta - Some experimental skid stuff
        if (Mathf.Abs(steerAngle / (maxSteerAngle * Mathf.Deg2Rad) * 100) > 80 && carSpeed / maxSpeed * 100 > 80 && SkidFab != null)
        {
            //instantiate the prefab
            GameObject skid = Instantiate(SkidFab);

            //Set some properties
            skid.transform.position = rb.position;
            skid.transform.rotation = Quaternion.Euler(0f, 0f, rb.rotation);

            //keep reference of all skidmarks
            Skidmarks.Add(skid);

            if (Skidmarks.Count > 50)
            {
                Skidmarks.RemoveAt(0);
            }
        }
    }

    private void FixedUpdate()
    {
        // Step 1 - Wheel positions
        // -> Position on pythagoram unit circle (across and up)
        var unitCirclePos = new Vector2( Mathf.Cos(carHeading), Mathf.Sin(carHeading));
        var unitCirclePosWithSteer = new Vector2(Mathf.Cos(carHeading + steerAngle), Mathf.Sin(carHeading + steerAngle));
        var wheelBaseCenter = wheelBase / 2;

        // -> Position of wheels from center of vehicle
        Vector2 frontWheel = carLocation + wheelBaseCenter * unitCirclePos;
        Vector2 rearWheel = carLocation - wheelBaseCenter * unitCirclePos;

        // Step 2 - Wheel velocities
        // -> Apply velocity to the wheels in sync with frames, then combine velocity scale with wheel vector positions
        frontWheel += carSpeed * Time.deltaTime * unitCirclePosWithSteer;
        rearWheel += carSpeed * Time.deltaTime * unitCirclePos;

        // Step 3 - Application
        // -> Find the center of the car by inversing the wheels position vectors (I think this is actually the velocity tbh)
        carLocation = (frontWheel + rearWheel) / 2;// * Input.GetAxis("Vertical");
        // -> Update the car heading angle by measuring the new angle between the front and back wheel's vectors
        carHeading = Mathf.Atan2(frontWheel.y - rearWheel.y, frontWheel.x - rearWheel.x);

        // Step 4 - Return it to the rigidBody component - Only works if kinematic
        rb.position = carLocation;
        rb.rotation = carHeading * Mathf.Rad2Deg;
    }
}
