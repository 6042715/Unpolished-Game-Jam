using System;
using System.Collections;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Movement_player : MonoBehaviour
{
    private Inventory inventory;
    public AudioClip windClip;
    private AudioSource audioSource;
    private SpriteRenderer SPRenderer;
    private Rigidbody2D rb;
    public float speed = 5;
    public float jumpStrength = 100;
    public float jumpTime = 0.2f;
    public int maxPoundBreaks = 3;
    private int currentPoundBreaks = 0;
    private float direction;
    public float velocity;
    public GameObject startPlatform;
    private float distToGround;
    private Collider2D col2;
    private bool isPounding = false;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;
    private Vector2 generalVelocity;
    private ParticleSystem TparticleSystem;
    private ParticleSystemRenderer TpcsR;
    private bool wasGrounded = true;
    private bool isSPressed = false;
    private float groundBasePosY;
    private CameraMovement camMov;
    private Notifications notifs;
    private LineRenderer lineRenderer;
    public GameObject lineRenderOBJ;
    private Transform[] LaserLines;
    public GameObject laserTarget;
    public GameObject eyeTarget;
    private Vector2 originalEyePos;
    private Vector2 flippedEyePos;
    private BlockMinable blockMinable;
    public GameObject mapTile;
    private Animator animator;
    public float mineSpeed = 1;
    public GameObject depthMeterHolder;
    private TextMeshProUGUI depthMeter;
    private float startHeight;
    private float curHeight;
    public GameObject returnToSurfaceButtonGOB;
    public Button returnToSurfaceButton;
    private Vector2 startPos;
    private float startGrav;
    public GameObject returnsLeftCounterGOB;
    private TextMeshProUGUI returnsLeftCounter;
    public GameObject returnPCSGOB;
    private ParticleSystem returnPCS;
    [SerializeField] private string startText;
    [SerializeField] private bool shouldReturn = false;
    [SerializeField] private int returnsLeft = 3;
    [SerializeField] private bool RecentlyMined = false;
    [SerializeField] private bool hasTouchedOtherGround = false;
    [SerializeField] public float timeFromGround;
    [SerializeField] private float lastJumpTime;
    [SerializeField] private float highestJumpPoint;
    public List<string> inventoryNames = new List<string>();
    public List<float> inventoryWeights = new List<float>();
    public List<int> inventoryIDs = new List<int>();
    public List<Sprite> inventorySprites = new List<Sprite>();

    // private bool shouldJump = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camMov = FindFirstObjectByType<CameraMovement>();
        notifs = FindFirstObjectByType<Notifications>();
        inventory = FindAnyObjectByType<Inventory>();
        blockMinable = mapTile.GetComponent<BlockMinable>();
        returnToSurfaceButton = returnToSurfaceButtonGOB.GetComponent<Button>();
        returnsLeftCounter = returnsLeftCounterGOB.GetComponent<TextMeshProUGUI>();
        returnPCS = returnPCSGOB.GetComponent<ParticleSystem>();

        rb = GetComponent<Rigidbody2D>();
        col2 = GetComponent<Collider2D>();
        SPRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        depthMeter = depthMeterHolder.GetComponent<TextMeshProUGUI>();

        lineRenderer = lineRenderOBJ.GetComponent<LineRenderer>();

        TparticleSystem = GetComponentInChildren<ParticleSystem>();
        TpcsR = GetComponentInChildren<ParticleSystemRenderer>();

        distToGround = col2.bounds.extents.y;

        audioSource.resource = windClip;

        StartCoroutine(AirTimer());

        LaserLines = new Transform[2];
        SetupLine(LaserLines);

        originalEyePos = eyeTarget.transform.localPosition;
        flippedEyePos = new Vector2(-originalEyePos.x, originalEyePos.y);

        startHeight = transform.position.y;
        startPos = transform.position;

        startGrav = rb.gravityScale;
        startText = returnsLeftCounter.text;

        returnPCS.Stop();

        returnToSurfaceButton.onClick.AddListener(ReturnToSurface);

    }
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }



    // Update is called once per frame
    void Update()
    {
        direction = Input.GetAxisRaw("Horizontal");

        float poundCheck = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded())
        {
            // Debug.Log("lololol");
            isSPressed = false;
            jump();
        }
        if (poundCheck < 0f)
        {
            if (!isPounding)
            {
                isSPressed = true;
                StartCoroutine(WaitForPound(0.1f));
            }
        }

        //test for animation (will fix later)
        if (direction > 0)
        {
            // SPRenderer.color = Color.blue;
            SPRenderer.flipX = false;
            eyeTarget.transform.localPosition = originalEyePos;
        }
        else if (direction < 0)
        {
            // SPRenderer.color = Color.red;
            SPRenderer.flipX = true;
            eyeTarget.transform.localPosition = flippedEyePos;
        }
        else if (poundCheck < 0)
        {
            // SPRenderer.color = Color.green;
        }
        else if (generalVelocity.y > 0)
        {
            // SPRenderer.color = Color.yellow;
        }

        // if (timeFromGround > 0.6)
        // {
        //     camMov.specialFollow = false;
        // }
        // else
        // {
        //     camMov.specialFollow = true;
        // }

        if (timeFromGround > 1.0f)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }

        if (direction != 0f)
        {
            animator.SetBool("shouldMove", true);
        }
        else
        {
            animator.SetBool("shouldMove", false);
        }

        if (shouldReturn)
        {
            transform.position = Vector2.Lerp(transform.position, startPos, 1.5f * Time.deltaTime);
            if (transform.position.y >= startHeight - 0.1f)
            {
                shouldReturn = false;
                col2.enabled = true;
                rb.gravityScale = startGrav;

                SPRenderer.color = new Color(SPRenderer.color.r, SPRenderer.color.g, SPRenderer.color.b, 1f);
                returnPCS.Stop();
            }
        }

    }
    void FixedUpdate()
    {
        velocity = rb.linearVelocityX;
        rb.AddRelativeForceX(direction * speed);

        generalVelocity = rb.linearVelocity;

        // Debug.Log(generalVelocity);


        bool grounded = isGrounded();

        if (!wasGrounded && grounded)
        {
            if (isSPressed)
            {
                isSPressed = false;
                currentPoundBreaks = 0;

                var PCSmain = TparticleSystem.main;

                //actually very proud of this lol :3
                PCSmain.startSpeed = (lastJumpTime + 1) * highestJumpPoint * 5;
                TparticleSystem.Play();
            }

            camMov.SetLowestPoint(transform.position.y);
            groundBasePosY = transform.position.y;
        }

        if (transform.position.y < startPlatform.transform.position.y)
        {
            camMov.shouldZoom = true;
        }

        wasGrounded = grounded;

        Collider2D hitCursorCols = Physics2D.OverlapPoint(laserTarget.transform.position, groundLayer);
        LaserLines[0] = eyeTarget.transform;
        laserTarget.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        LaserLines[1] = laserTarget.transform;
        if (hitCursorCols != null && hitCursorCols.TryGetComponent(out BlockMinable block) && block.mining)
        {
            if (!lineRenderer.enabled)
            {
                lineRenderer.enabled = true;
            }
            for (int i = 0; i < LaserLines.Length; i++)
            {
                lineRenderer.SetPosition(i, LaserLines[i].position);
            }
        }
        else
        {
            if (lineRenderer.enabled)
            {
                lineRenderer.enabled = false;
            }
        }

        curHeight = startHeight - (startHeight - transform.position.y);
        depthMeter.text = curHeight + "M";

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject)
        {
            var PCSmain = TparticleSystem.main;
            PCSmain.startColor = collision.gameObject.GetComponent<SpriteRenderer>().color;
        }
        if (collision.gameObject.tag == "Ground" && transform.position.y < startPlatform.transform.position.y)
        {
            hasTouchedOtherGround = true;
            camMov.specialFollow = true;
        }
        if (collision.gameObject.tag == "Ground" && isSPressed)
        {
            BlockMinable minable = collision.gameObject.GetComponent<BlockMinable>();

            if (currentPoundBreaks < maxPoundBreaks)
            {
                minable.OtherAddInventory();
                currentPoundBreaks += 1;
            }
        }
    }
    public void SetupLine(Transform[] points)
    {
        lineRenderer.positionCount = points.Length;
        Transform[] TSpoints = points;
    }

    bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void jump()
    {
        rb.AddForce(new Vector2(rb.linearVelocity.x, jumpStrength));
    }

    public void AddToInventory(string name, float weight, int ID, Sprite sprite)
    {
        RecentlyMined = true;
        StartCoroutine(resetRecentlyMined());

        inventoryNames.Add(name);
        inventoryWeights.Add(weight);
        inventoryIDs.Add(ID);
        inventorySprites.Add(sprite);

        notifs.ShowNotification(name, (float)Math.Round(weight, 2), ID, sprite);

        inventory.RefreshInventory();

        Debug.Log("total weight: " + GetTotalWeight(inventoryWeights).ToString());
    }

    private IEnumerator WaitForPound(float time)
    {
        isPounding = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        yield return new WaitForSecondsRealtime(time);

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.linearVelocityY = -500f;

        isPounding = false;

        // if (!TparticleSystem.isPlaying)
        // {
        //     StartCoroutine(PoundParticles());
        // }
    }
    // private IEnumerator PoundParticles()
    // {
    //     yield return new WaitForSecondsRealtime(0.2f);
    //     while (true)
    //     {
    //         if (isGrounded())
    //         {
    //             TparticleSystem.Play();

    //             yield return new WaitForSecondsRealtime(TparticleSystem.main.duration);
    //             TparticleSystem.Stop();
    //         }
    //         yield return new WaitForFixedUpdate();
    //     }
    // }
    // private IEnumerator StartJumpTime()
    // {
    //     shouldJump = true;
    //     yield return new WaitForSecondsRealtime(jumpTime);

    //     shouldJump = false;
    // }
    private IEnumerator AirTimer()
    {
        bool isRecording = true;
        while (true)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            if (!isGrounded())
            {
                isRecording = true;
                timeFromGround += 0.1f;

                Debug.Log(rb.linearVelocityY);
                if (Mathf.Abs(rb.linearVelocityY) < 1.5f)
                {
                    highestJumpPoint = transform.position.y - groundBasePosY;
                }
            }
            else
            {
                if (isRecording)
                {
                    lastJumpTime = timeFromGround;
                }
                isRecording = false;
                timeFromGround = 0f;
            }
        }
    }

    private IEnumerator resetRecentlyMined()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        RecentlyMined = false;
    }

    private float GetTotalWeight(List<float> list)
    {
        float total = 0f;
        for (int i = 0; i < list.Count; i++)
        {
            total += (float)Math.Round(list[i], 2);
        }
        return total;
    }
    GameObject getGameObjectAtPosition()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
            Debug.Log("found " + hit.collider.gameObject.name + " at distance: " + hit.distance);
        return hit.collider.gameObject;
    }

    public void ReturnToSurface()
    {
        if (transform.position.y < startHeight && returnsLeft > 0)
        {
            if (!shouldReturn)
            {
                StartCoroutine(WaitForTeleport(0.1f));
            }
        }
    }

    private IEnumerator WaitForTeleport(float delay)
    {
        returnsLeft--;
        returnsLeftCounter.text = startText + "(" + returnsLeft.ToString() + "X)";

        yield return new WaitForSecondsRealtime(delay);
        SPRenderer.color = new Color(SPRenderer.color.r, SPRenderer.color.g, SPRenderer.color.b, 0.5f);

        shouldReturn = true;
        col2.enabled = false;
        rb.gravityScale = 0f;
        returnPCS.Play();
    }
    
}
