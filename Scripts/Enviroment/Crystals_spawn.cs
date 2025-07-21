using UnityEngine;

public class Crystals_spawn : MonoBehaviour
{
    public GameObject layer;
    public int SpawnAmount = 10;
    private CameraMovement camMo;
    private SpriteRenderer SPrenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SPrenderer = GetComponent<SpriteRenderer>();

        camMo = FindFirstObjectByType<CameraMovement>();

        GameObject CrystalHolder = new GameObject("crystalHolder");
        if (tag != "cloned")
        {
            for (int i = 1; i < SpawnAmount; i++)
            {
                float x = transform.position.x;
                float xOffset = x += SPrenderer.bounds.size.x * i;
                GameObject clone = Instantiate(layer, new Vector3(xOffset, camMo.cameraPos.y, transform.position.y), transform.rotation);
                clone.tag = "cloned";

                clone.transform.SetParent(CrystalHolder.transform);
            }
            float totalWidth = SPrenderer.bounds.size.x * SpawnAmount;
            CrystalHolder.transform.position = new Vector3(-totalWidth / 2f + SPrenderer.bounds.size.x / 2f, 0f, 0f);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
