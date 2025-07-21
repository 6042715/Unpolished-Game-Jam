using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampFall : MonoBehaviour
{
    public GameObject particles;
    private Rigidbody2D rb2d;
    private Collider2D col2;
    private bool active = false;
    private Movement_player movementPL;
    public List<Sprite> itemSprites = new List<Sprite>();
    public AudioClip glassBreak;
    private AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movementPL = FindFirstObjectByType<Movement_player>();

        rb2d = GetComponent<Rigidbody2D>();
        col2 = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();

        rb2d.simulated = false;
        col2.enabled = false;
    }

    void OnTransformParentChanged()
    {
        rb2d.simulated = true;
        col2.enabled = true;

        rb2d.AddTorque(Random.Range(-5f, 5f));

        active = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" && active)
        {
            StartCoroutine(WaitForBreak(0.1f));
        }
    }

    private IEnumerator WaitForBreak(float delay)
    {
        GameObject particlesSP = Instantiate(particles, transform.position, Quaternion.Euler(0f, 0f, 0f));

        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.PlayOneShot(glassBreak);

        int itemID = Random.Range(-100000, -100);
        float shardWeight = Random.Range(0.05f, 0.15f);
        movementPL.AddToInventory("Glass Shard", shardWeight, itemID, itemSprites[0]);
        if (Random.Range(0, 10) == 5)
        {
            itemID = Random.Range(-100000, -100);
            float wireWeight = Random.Range(0.08f, 0.15f);
            movementPL.AddToInventory("Lamp Wire", wireWeight, itemID, itemSprites[1]);
        }

        yield return new WaitForSecondsRealtime(delay);
        Destroy(particlesSP, 0.8f);

        rb2d.simulated = false;
        col2.enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        
        yield return new WaitForSecondsRealtime(0.5f);
        Destroy(gameObject);
    }
}
