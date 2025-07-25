
using UnityEngine;
using UnityEngine.Tilemaps;

public class UnRender : MonoBehaviour
{
    private TilemapRenderer tilemapRenderer;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private int option;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (gameObject.GetComponent<TilemapRenderer>() != null)
        {
            tilemapRenderer = gameObject.GetComponent<TilemapRenderer>();
            option = 0;
        }
        if (gameObject.GetComponent<SpriteRenderer>() != null)
        {
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            option = 1;
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "RenderColliderCheck")
        {
            if (option == 0)
            {
                tilemapRenderer.enabled = true;
            }
            else
            {
                spriteRenderer.enabled = true;
            }
        }
    }
    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.name == "RenderColliderCheck")
        {
            if (option == 0)
            {
                tilemapRenderer.enabled = false;
            }
            else
            {
                spriteRenderer.enabled = false;
            }
        }
    }
}
