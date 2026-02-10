using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance { get; private set; }
    
    [Header("玩家引用")]
    [SerializeField] private Transform player;
    [SerializeField] private Rigidbody2D playerRb;
    
    [Header("重生配置")]
    [SerializeField] private Vector3 defaultSpawnPoint;
    [SerializeField] private float respawnDelay = 0.5f;

    [Header("掉落检测")]
    [SerializeField] private float fallOutMinY = -20f;
    [SerializeField] private Collider2D outOfBoundsArea;
    [SerializeField] private bool autoCreateOutOfBounds = true;
    [SerializeField] private Vector2 outOfBoundsCenter = new Vector2(0f, 0f);
    [SerializeField] private Vector2 outOfBoundsSize = new Vector2(200f, 200f);

    [Header("边界可视化")]
    [SerializeField] private bool showBoundaryGizmo = true;
    [SerializeField] private bool showBoundaryRuntime = true;
    [SerializeField] private Color boundaryGizmoColor = new Color(0.2f, 0.9f, 1f, 0.9f);
    [SerializeField] private Color boundaryRuntimeColor = new Color(0.2f, 0.9f, 1f, 0.7f);
    [SerializeField] private float boundaryLineWidth = 0.03f;
    [SerializeField] private int boundarySortingOrder = 200;
    
    [Header("音效")]
    [SerializeField] private AudioClip deathSound;
    private AudioSource audioSource;
    
    private Vector3 lastCheckpointPosition;
    private bool hasCheckpoint = false;
    private bool isRespawning;
    private LineRenderer boundaryRenderer;

    private void Awake()
    {
        // 单例模式
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        audioSource = GetComponent<AudioSource>();
        
        // 如果没有设置默认出生点，使用玩家当前位置
        if (defaultSpawnPoint == Vector3.zero && player != null)
        {
            defaultSpawnPoint = player.position;
        }
        
        lastCheckpointPosition = defaultSpawnPoint;

        if (outOfBoundsArea == null && autoCreateOutOfBounds)
        {
            CreateOutOfBoundsArea();
        }
    }

    private void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
        
        if (playerRb == null && player != null)
        {
            playerRb = player.GetComponent<Rigidbody2D>();
        }

        if (outOfBoundsArea == null && autoCreateOutOfBounds)
        {
            CreateOutOfBoundsArea();
        }

        if (showBoundaryRuntime)
        {
            EnsureBoundaryRenderer();
            UpdateBoundaryRenderer();
        }
    }

    private void Update()
    {
        if (showBoundaryRuntime && boundaryRenderer == null)
        {
            EnsureBoundaryRenderer();
            UpdateBoundaryRenderer();
        }

        // R键快速重生
        if (Input.GetKeyDown(KeyCode.R))
        {
            Respawn();
        }

        // 掉出地图范围自动重生
        if (player != null && !isRespawning)
        {
            if (outOfBoundsArea != null)
            {
                Bounds bounds = outOfBoundsArea.bounds;
                if (!IsInsideBounds2D(bounds, player.position))
                {
                    TriggerDeath("OutOfBounds");
                }
            }
            else if (player.position.y < fallOutMinY)
            {
                TriggerDeath("FallOutMinY");
            }
        }
    }

    private bool IsInsideBounds2D(Bounds bounds, Vector3 position)
    {
        return position.x >= bounds.min.x && position.x <= bounds.max.x &&
               position.y >= bounds.min.y && position.y <= bounds.max.y;
    }

    public void SetCheckpoint(Vector3 position)
    {
        lastCheckpointPosition = position;
        hasCheckpoint = true;
        Debug.Log($"[Respawn] Checkpoint set at {position}");
    }

    public void TriggerDeath(string reason = "Unknown")
    {
        if (isRespawning)
            return;

        isRespawning = true;

        // 播放死亡音效
        if (audioSource && deathSound)
        {
            audioSource.PlayOneShot(deathSound);
        }
        
        Vector3 playerPos = player != null ? player.position : Vector3.zero;
        Debug.Log($"[Respawn] Player death triggered. Reason={reason}, PlayerPos={playerPos}, Checkpoint={lastCheckpointPosition}, HasCheckpoint={hasCheckpoint}");
        
        // 延迟重生
        Invoke(nameof(Respawn), respawnDelay);
    }

    private void Respawn()
    {
        if (player == null)
        {
            Debug.LogError("玩家引用丢失，无法重生！");
            return;
        }
        
        // 重置玩家位置
        Vector3 respawnPosition = hasCheckpoint ? lastCheckpointPosition : defaultSpawnPoint;
        player.position = respawnPosition;
        
        // 清空速度
        if (playerRb != null)
        {
            playerRb.velocity = Vector2.zero;
            playerRb.angularVelocity = 0f;
        }
        
        // 注意：保持当前世界状态，不切换世界
        
        Debug.Log($"[Respawn] Player respawned at {respawnPosition}");

        isRespawning = false;
    }

    public void ResetToDefaultSpawn()
    {
        lastCheckpointPosition = defaultSpawnPoint;
        hasCheckpoint = false;
        isRespawning = false;
        Respawn();
    }

    private void CreateOutOfBoundsArea()
    {
        GameObject boundsObject = new GameObject("OutOfBounds_Auto");
        boundsObject.transform.SetParent(transform);
        boundsObject.transform.position = new Vector3(outOfBoundsCenter.x, outOfBoundsCenter.y, 0f);

        BoxCollider2D box = boundsObject.AddComponent<BoxCollider2D>();
        box.isTrigger = true;
        box.size = outOfBoundsSize;

        outOfBoundsArea = box;
    }

    private Rect GetBoundaryRect()
    {
        if (outOfBoundsArea != null)
        {
            Bounds b = outOfBoundsArea.bounds;
            return new Rect(b.min.x, b.min.y, b.size.x, b.size.y);
        }

        return new Rect(
            outOfBoundsCenter.x - outOfBoundsSize.x * 0.5f,
            outOfBoundsCenter.y - outOfBoundsSize.y * 0.5f,
            outOfBoundsSize.x,
            outOfBoundsSize.y
        );
    }

    private void EnsureBoundaryRenderer()
    {
        if (boundaryRenderer != null)
            return;

        GameObject rendererObject = new GameObject("OutOfBounds_Renderer");
        rendererObject.transform.SetParent(transform);
        rendererObject.transform.localPosition = Vector3.zero;

        boundaryRenderer = rendererObject.AddComponent<LineRenderer>();
        boundaryRenderer.useWorldSpace = true;
        boundaryRenderer.loop = true;
        boundaryRenderer.positionCount = 4;
        boundaryRenderer.startWidth = boundaryLineWidth;
        boundaryRenderer.endWidth = boundaryLineWidth;
        boundaryRenderer.material = new Material(Shader.Find("Sprites/Default"));
        boundaryRenderer.startColor = boundaryRuntimeColor;
        boundaryRenderer.endColor = boundaryRuntimeColor;
        boundaryRenderer.sortingOrder = boundarySortingOrder;
    }

    private void UpdateBoundaryRenderer()
    {
        if (boundaryRenderer == null)
            return;

        Rect rect = GetBoundaryRect();
        Vector3 p0 = new Vector3(rect.xMin, rect.yMin, 0f);
        Vector3 p1 = new Vector3(rect.xMax, rect.yMin, 0f);
        Vector3 p2 = new Vector3(rect.xMax, rect.yMax, 0f);
        Vector3 p3 = new Vector3(rect.xMin, rect.yMax, 0f);

        boundaryRenderer.SetPosition(0, p0);
        boundaryRenderer.SetPosition(1, p1);
        boundaryRenderer.SetPosition(2, p2);
        boundaryRenderer.SetPosition(3, p3);
        boundaryRenderer.enabled = showBoundaryRuntime;
    }

    // 用于在编辑器中可视化出生点
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(defaultSpawnPoint, 0.5f);
        
        if (hasCheckpoint)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(lastCheckpointPosition, 0.5f);
        }

        if (showBoundaryGizmo)
        {
            Rect rect = GetBoundaryRect();
            Gizmos.color = boundaryGizmoColor;
            Vector3 center = new Vector3(rect.center.x, rect.center.y, 0f);
            Vector3 size = new Vector3(rect.size.x, rect.size.y, 0f);
            Gizmos.DrawWireCube(center, size);
        }
    }
}
