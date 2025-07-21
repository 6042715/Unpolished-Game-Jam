using UnityEngine;

public class Background_follow : MonoBehaviour
{
    private CameraMovement camMo;
    private Movement_player playMo;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camMo = FindFirstObjectByType<CameraMovement>();
        playMo = FindFirstObjectByType<Movement_player>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 crystalPos = new Vector3(transform.position.x + (playMo.velocity / 12), camMo.cameraPos.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, crystalPos, (camMo.LerpSpeed - 0.6f) * Time.deltaTime);
    }
}
