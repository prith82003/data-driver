using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DriveController : MonoBehaviour
{
    private const int FLWheel = 0;
    private const int FRWheel = 1;
    private const int BLWheel = 2;
    private const int BRWheel = 3;

    private Rigidbody rb;
    
    public Transform[] wheelPositions = new Transform[4];
    public Vector3[] wheelForces = new Vector3[4];
    public Vector3[] hitPos = new Vector3[4];

    public LayerMask wheelHitMask;

    [Header("Suspension Variables")]
    public float maxWheelDist;
    public float wheelRestDist;
    public float suspensionDamping;
    public float suspensionStrength;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        float getOffset(Transform wheelPosition, int i = -1)
        {
            RaycastHit hitInfo;
            bool hit = Physics.Raycast(wheelPosition.position, Vector3.down, out hitInfo, maxWheelDist, wheelHitMask, QueryTriggerInteraction.Ignore);

            if (!hit)
                return -1;

            if (i > -1)
                hitPos[i] = hitInfo.point;

            float offset = wheelRestDist - hitInfo.distance;
            Debug.Log("Wheel " + i + " Offset: " + offset);
            return offset;
        }

        wheelForces = new Vector3[4];

        for (int i = 0; i < 4; i++)
        {
            Transform wheel = wheelPositions[i];

            float wheelOffset = getOffset(wheel, i);

            if (wheelOffset < 0)
                continue;
            float velAtWheel = Vector3.Dot(transform.up, rb.GetPointVelocity(wheel.position));


            float forceMag = (wheelOffset * suspensionStrength) - (velAtWheel * suspensionDamping);
            wheelForces[i] = forceMag * Vector3.up;

            rb.AddForceAtPosition(wheelForces[i], wheel.position);
        }
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < 4; i++)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(wheelPositions[i].position, 0.2f);
            Gizmos.DrawLine(wheelPositions[i].position, wheelPositions[i].position + wheelForces[i]);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(wheelPositions[i].position + wheelForces[i], 0.1f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(wheelPositions[i].position, wheelPositions[i].position - wheelPositions[i].forward * wheelRestDist);
        }

        Gizmos.color = Color.green;
        for (int i = 0; i < 4; i++)
        {
            Gizmos.DrawSphere(hitPos[i], .1f);
        }
    }

    private void OnGUI()
    {
        for (int i = 0; i < 4; i++)
        {
            GUI.Label(new Rect(0, 0 + (i * 20), 100, 100 + (i * 100)), wheelForces[i].magnitude.ToString());
        }
    }
}
