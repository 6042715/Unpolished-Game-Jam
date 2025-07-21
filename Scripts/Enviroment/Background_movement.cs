using UnityEngine;

public class Background_movement : MonoBehaviour
{
    private CameraMovement camMo;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camMo = FindFirstObjectByType<CameraMovement>();  
    }

    // Update is called once per frame
    void Update()
    {
        // transform.position = new Vector3(transform.position.x, transform.position.y + camMo.cameraYOffset, transform.position.z);
    }
}
