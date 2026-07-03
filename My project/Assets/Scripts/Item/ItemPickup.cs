using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float fallSpeed = 6f;

    float offset;

    private bool playerInRange = false;
    private bool landed = false;
    private bool collected = false;
    private bool waitingToReturn = false;
    private string originSceneName;

    public void SetItem(ItemData data) => itemData = data;
    public void SetGroundLayer(LayerMask layer) => groundLayer = layer;

    private void Awake()
    {
        // 콜라이더 높이의 절반 → 오브젝트 중심에서 바닥까지의 거리
        Collider2D col = GetComponent<Collider2D>();
        offset = col != null ? col.bounds.extents.y : 0f;

        SceneTransitionTrigger.BeforeSceneChange += OnBeforeSceneChange;
    }

    private void OnDestroy()
    {
        SceneTransitionTrigger.BeforeSceneChange -= OnBeforeSceneChange;
        SceneManager.sceneLoaded -= OnOriginSceneReloaded;
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
        if (other.GetComponent<PlayerController>() != null)
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() != null)
            playerInRange = false;
    }

    private void Collect()
    {
        if (itemData == null) return;
        collected = true;
        InventoryManager.Instance?.AddItem(itemData);
        ItemPickupUI.Instance?.Show(itemData.itemName);
        Destroy(gameObject);
    }

    // 줍지 않은 채로 씬이 실제로 사라지기 직전(오브젝트가 아직 살아있을 때) 호출됨 → 파괴 대신 숨겨서 보존
    private void OnBeforeSceneChange()
    {
        if (itemData == null || !itemData.stayInPlace || collected || waitingToReturn) return;

        waitingToReturn = true;
        originSceneName = gameObject.scene.name;
        DontDestroyOnLoad(gameObject);
        gameObject.SetActive(false);
        SceneManager.sceneLoaded += OnOriginSceneReloaded;
    }

    // 원래 있던 씬으로 다시 돌아왔을 때 제자리에 복귀
    private void OnOriginSceneReloaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != originSceneName) return;

        waitingToReturn = false;
        SceneManager.sceneLoaded -= OnOriginSceneReloaded;
        SceneManager.MoveGameObjectToScene(gameObject, scene);
        gameObject.SetActive(true);
    }
}
