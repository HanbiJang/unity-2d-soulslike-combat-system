using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float fallSpeed = 6f;
    float offset;

    private bool playerInRange = false;
    private bool landed = false;

    public void SetItem(ItemData data) => itemData = data;
    public void SetGroundLayer(LayerMask layer) => groundLayer = layer;

    private void Awake()
    {
        // 콜라이더 높이의 절반 → 오브젝트 중심에서 바닥까지의 거리
        Collider2D col = GetComponent<Collider2D>();
        offset = col != null ? col.bounds.extents.y : 0f;
    }

    private void Update()
    {
        if (!landed)
            Fall();

        // 착지 후에만 E키 입력 받음
        if (landed && playerInRange && Input.GetKeyDown(KeyCode.E))
            Collect();
    }

    private void Fall()
    {
        float step = fallSpeed * Time.deltaTime;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, step + offset, groundLayer);

        if (hit.collider != null)
        {
            transform.position = new Vector3(transform.position.x, hit.point.y + offset, 0f);
            landed = true;
        }
        else
        {
            transform.position += Vector3.down * step;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    private void Collect()
    {
        if (itemData == null) return;
        InventoryManager.Instance?.AddItem(itemData);
        ItemPickupUI.Instance?.Show(itemData.itemName);
        Destroy(gameObject);
    }
}
