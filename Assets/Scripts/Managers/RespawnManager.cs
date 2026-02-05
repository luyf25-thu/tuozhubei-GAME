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
    
    [Header("音效")]
    [SerializeField] private AudioClip deathSound;
    private AudioSource audioSource;
    
    private Vector3 lastCheckpointPosition;
    private bool hasCheckpoint = false;
    private bool isRespawning;

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
    }

    private void Update()
    {
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
                if (!outOfBoundsArea.bounds.Contains(player.position))
                {
                    TriggerDeath();
                }
            }
            else if (player.position.y < fallOutMinY)
            {
                TriggerDeath();
            }
        }
    }

    public void SetCheckpoint(Vector3 position)
    {
        lastCheckpointPosition = position;
        hasCheckpoint = true;
        Debug.Log($"检查点已设置: {position}");
    }

    public void TriggerDeath()
    {
        if (isRespawning)
            return;

        isRespawning = true;

        // 播放死亡音效
        if (audioSource && deathSound)
        {
            audioSource.PlayOneShot(deathSound);
        }
        
        Debug.Log("玩家死亡");
        
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
        
        Debug.Log($"玩家在 {respawnPosition} 重生");

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
    }
}
