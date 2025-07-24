using UnityEngine;

public class PlanetRotate : MonoBehaviour
{
    private Rigidbody rb;
    public bool shouldMove;
    private Cursor cursor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        cursor = FindFirstObjectByType<Cursor>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (shouldMove)
        {
            rb.AddRelativeTorque(new Vector3(-cursor.speed, -cursor.speed, -cursor.speed));
        }
    }

    void OnTriggerEnter(Collider other)
    {
        shouldMove = true;
    }
    void OnTriggerExit(Collider other)
    {
        shouldMove = false;
    }
}
