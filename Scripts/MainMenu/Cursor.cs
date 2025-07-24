using UnityEngine;
using UnityEngine.InputSystem;

public class Cursor : MonoBehaviour
{
    private Rigidbody rb;
    public float speed;
    private Vector3 targetPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
        targetPosition = Camera.main.ScreenToWorldPoint(mousePos);
    }

    void FixedUpdate()
    {
        Vector3 newPos = Vector3.Lerp(transform.position, targetPosition, Time.fixedDeltaTime * 10f);
        rb.MovePosition(newPos);

        speed = rb.linearVelocity.magnitude;
    }
}