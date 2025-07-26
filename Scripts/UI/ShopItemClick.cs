using UnityEngine;
using UnityEngine.UI;

public class ShopItemClick : MonoBehaviour
{
    [SerializeField] private string thisName;
    [SerializeField] private Sprite sprite;
    [SerializeField] private Button button;
    private ShopStand shopStand;
    private FullInventory inventory;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shopStand = FindFirstObjectByType<ShopStand>();
        inventory = FindFirstObjectByType<FullInventory>();

        button = transform.GetChild(3).GetComponent<Button>();

        thisName = gameObject.name;
        sprite = transform.GetChild(0).gameObject.GetComponent<Image>().sprite;

        button.onClick.AddListener(ShowStats);
    }

    private void ShowStats()
    {
        inventory.PlayUIsound(inventory.ballTap, true);

        Debug.Log("clicked!");
        shopStand.ShowStatsOnScreen(thisName, float.Parse(transform.GetChild(0).name), sprite, gameObject);

    }
}
