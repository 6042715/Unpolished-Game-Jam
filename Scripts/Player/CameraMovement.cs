using UnityEditor;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Movement_player movement;
    private GameObject player;
    [SerializeField] private float camHeight;
    public float cameraYOffset = 0.5f;
    public float LerpSpeed = 2f;
    public Vector3 cameraPos;
    public GameObject startPlatform;
    private float cameraZoomHeight;
    private Camera TScamera;
    public bool shouldZoom = false;
    public bool specialFollow = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraZoomHeight = startPlatform.transform.position.y;

        TScamera = GetComponent<Camera>();
        player = GameObject.FindGameObjectWithTag("Player");

        movement = FindFirstObjectByType<Movement_player>();

        Debug.Log(cameraZoomHeight);
    }

    // Update is called once per frame
    // void Update()
    // {

    //     if (movement.timeFromGround > 1.75f)
    //     {
    //         shouldZoom = true;
    //         if (shouldZoom)
    //         {
    //             float targetFOV = 20f + (movement.timeFromGround * 1.5f) ;
    //             TScamera.orthographicSize = Mathf.Lerp(TScamera.orthographicSize, targetFOV, Time.deltaTime * 2f);

    //             if (TScamera.orthographicSize == targetFOV)
    //             {
    //                 shouldZoom = false;
    //             }
    //         }

    //     }


    //     // if (specialFollow)
    //     // {
    //     //     cameraPos = new Vector3(transform.position.x, camHeight, transform.position.z);
    //     //     transform.position = Vector3.Lerp(transform.position, cameraPos, LerpSpeed * Time.deltaTime);
    //     // }
    //     if (player != null)
    //     {
    //         float targetY = specialFollow ? camHeight : player.transform.position.y + cameraYOffset;
    //         cameraPos = new Vector3(player.transform.position.x, targetY, transform.position.z);
    //         transform.position = Vector3.Lerp(transform.position, cameraPos, LerpSpeed * Time.deltaTime);
    //     }


    //     if (shouldZoom)
    //     {
    //         float targetFOV = 10f;
    //         TScamera.orthographicSize = Mathf.Lerp(TScamera.orthographicSize, targetFOV, Time.deltaTime * 2f);

    //         if (TScamera.orthographicSize == targetFOV)
    //         {
    //             shouldZoom = false;
    //         }
    //     }

    // }
    void Update()
    {
        if (player != null)
        {
            Vector3 targetPos = new Vector3(player.transform.position.x, player.transform.position.y + cameraYOffset, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPos, LerpSpeed * Time.deltaTime);
        }

        if (movement.timeFromGround > 1.75f)
        {
            float targetFOV = 20f + (movement.timeFromGround * 1.5f);
            TScamera.orthographicSize = Mathf.Lerp(TScamera.orthographicSize, targetFOV, Time.deltaTime * 2f);
        }
        else
        {
            float targetFOV = 10f;
            TScamera.orthographicSize = Mathf.Lerp(TScamera.orthographicSize, targetFOV, Time.deltaTime * 2f);
        }
    }

    public void ZoomOut()
    {
        shouldZoom = true;
    }
    public void SetLowestPoint(float posY)
    {
        camHeight = posY + cameraYOffset;
        // transform.position = new Vector3(transform.position.x, camHeight, transform.position.z);
    }
}
