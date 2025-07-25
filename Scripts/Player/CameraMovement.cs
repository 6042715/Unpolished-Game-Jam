using System;
using System.Collections;
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
    public bool isTheEnd = false;
    private float goalEndZoom = 5f;
    private float goalEndZoom2 = 2f;
    private bool isTheEnd2 = false;
    private bool startedCoroutine = false;
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
        if (player != null && !isTheEnd && !isTheEnd2)
        {
            Vector3 targetPos = new Vector3(player.transform.position.x, player.transform.position.y + cameraYOffset, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPos, LerpSpeed * Time.deltaTime);
        }

        if (movement.timeFromGround > 1.75f && !isTheEnd && !isTheEnd2)
        {
            float targetFOV = 20f + (movement.timeFromGround * 1.5f);
            TScamera.orthographicSize = Mathf.Lerp(TScamera.orthographicSize, targetFOV, Time.deltaTime * 2f);
        }
        else
        {
            if (!isTheEnd && !isTheEnd2)
            {
                float targetFOV = 10f;
                TScamera.orthographicSize = Mathf.Lerp(TScamera.orthographicSize, targetFOV, Time.deltaTime * 2f);
            }
        }

        if (isTheEnd)
        {
            float curTarZoom = Mathf.Lerp(TScamera.orthographicSize, goalEndZoom, Time.deltaTime * 2f);
            TScamera.orthographicSize = curTarZoom;

            if (!startedCoroutine)
            {
                StartCoroutine(SecondEndEffect(2f));
            }
        }
        if (isTheEnd2)
        {
            float curTarZoom = Mathf.Lerp(TScamera.orthographicSize, goalEndZoom2, Time.deltaTime * 1.5f);
            TScamera.orthographicSize = curTarZoom;
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

    private IEnumerator SecondEndEffect(float delay)
    {
        startedCoroutine = true;
        yield return new WaitForSecondsRealtime(delay);
        isTheEnd2 = true;
        isTheEnd = false;

        StartCoroutine(WaitForPlayerEnd());
    }

    private IEnumerator WaitForPlayerEnd()
    {
        yield return new WaitForSecondsRealtime(2f);
        movement.WaitForExplode(1f);
    }
}
