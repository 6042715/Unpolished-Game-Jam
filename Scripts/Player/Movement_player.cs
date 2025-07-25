using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
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
    private GameManager game;
    public GameObject finalLayer;
    private float finalLayerHeight;
    private bool hasEnded = false;
    public Gradient deathCurve;
    private Vector2 deathSequencePos;
    public bool shouldLock;
    public GameObject deathLasers;
    private Transform[] deathLaserTran;
    private LineRenderer deathLines;
    public GameObject deathLaserOrigin;
    private float lastTargetX;
    public GameObject explodePlayer;
    public AudioClip explosionClip;
    [SerializeField] private float laserMarginX;
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

    //stats
    public int blocksMined = 0;
    public float timeSpent = 0f;
    public int itemsCrafted = 0;
    public float totalAirtime = 0f;

    // private bool shouldJump = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camMov = FindFirstObjectByType<CameraMovement>();
        notifs = FindFirstObjectByType<Notifications>();
        inventory = FindAnyObjectByType<Inventory>();
        game = FindFirstObjectByType<GameManager>();

        blockMinable = mapTile.GetComponent<BlockMinable>();
        returnToSurfaceButton = returnToSurfaceButtonGOB.GetComponent<Button>();
        returnsLeftCounter = returnsLeftCounterGOB.GetComponent<TextMeshProUGUI>();
        returnPCS = returnPCSGOB.GetComponent<ParticleSystem>();

        rb = GetComponent<Rigidbody2D>();
        col2 = GetComponent<Collider2D>();
        deathLines = deathLasers.GetComponent<LineRenderer>();
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
        StartCoroutine(GeneralTimer());

        LaserLines = new Transform[2];
        SetupLine(LaserLines);

        originalEyePos = eyeTarget.transform.localPosition;
        flippedEyePos = new Vector2(-originalEyePos.x, originalEyePos.y);

        startHeight = transform.position.y;
        startPos = transform.position;
        finalLayerHeight = finalLayer.transform.position.y;

        startGrav = rb.gravityScale;
        startText = returnsLeftCounter.text;

        returnPCS.Stop();

        returnToSurfaceButton.onClick.AddListener(ReturnToSurface);

        deathLines.enabled = false;

        //----------------------------------
        deathLaserTran = new Transform[2];

        deathLaserTran[0] = transform;
        deathLaserTran[1] = deathLaserOrigin.transform;

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

        if (shouldLock)
        {
            transform.position = Vector2.Lerp(transform.position, deathSequencePos, 3f * Time.deltaTime);

            deathLaserTran[0] = transform;

            for (int i = 0; i < deathLaserTran.Length; i++)
            {
                deathLines.SetPosition(i, deathLaserTran[i].position);
                if (i == 0)
                {
                    float targetX = Mathf.Lerp(lastTargetX, deathLaserTran[i].position.x + laserMarginX, Time.deltaTime * 6f);
                    deathLines.SetPosition(i, new Vector2(targetX, deathLaserTran[i].position.y));
                    lastTargetX = targetX;
                }
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

        if (transform.position.y < finalLayerHeight + 1)
        {
            if (game.hasFoundGoal == true && !hasEnded)
            {
                SPRenderer.enabled = false;
                game.ShowEndScreen();

                hasEnded = true;
            }
            else if (!game.hasFoundGoal && !hasEnded)
            {
                StartCoroutine(WaitForTeleport(0.5f));
                var ColOLifetime = returnPCS.colorOverLifetime;
                ColOLifetime.color = deathCurve;

                StartCoroutine(WaitForLockPos(7.5f));
                hasEnded = true;
            }
        }

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

        blocksMined += 1;

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
                totalAirtime += 0.1f;

                game.toggleDebugLight(2);

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

    private IEnumerator GeneralTimer()
    {
        while (true)
        {
            timeSpent += 0.1f;
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }

    private IEnumerator resetRecentlyMined()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        RecentlyMined = false;
        game.toggleDebugLight(3);
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

    private IEnumerator WaitForLockPos(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        deathSequencePos = transform.position;
        shouldLock = true;

        deathLines.enabled = true;
        StartCoroutine(RandomLaserMove());

        camMov.isTheEnd = true;
    }

    private IEnumerator RandomLaserMove()
    {
        while (shouldLock)
        {
            float newWait = UnityEngine.Random.Range(0.2f, 0.8f);
            yield return new WaitForSecondsRealtime(newWait);

            laserMarginX = UnityEngine.Random.Range(-0.5f, 0.5f);
        }
    }

    public void WaitForExplode(float delay)
    {
        StartCoroutine(Explode(delay));
    }

    private IEnumerator Explode(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        GameObject explosion = Instantiate(explodePlayer, transform.position, Quaternion.Euler(0f, 0f, 0f));
        explosion.GetComponent<SpriteRenderer>().sortingOrder = 200;

        StartCoroutine(AfterExplosion());
        StartCoroutine(ExplosionSoundLoop(explosion));
    }

    private IEnumerator AfterExplosion()
    {
        yield return new WaitForSecondsRealtime(1f);
        SPRenderer.enabled = false;

        yield return new WaitForSecondsRealtime(1f);
        game.ShowEndScreen(true);
    }

    private IEnumerator ExplosionSoundLoop(GameObject explosion)
    {
        AudioSource audioSource2 = explosion.GetComponent<AudioSource>();

        int i = 0;
        while (true)
        {
            audioSource2.PlayOneShot(explosionClip);
            yield return new WaitForSecondsRealtime(1f);

            i++;

            if (i == 3)
            {
                audioSource2.volume = 0.4f;
            }
        }
    }
}
