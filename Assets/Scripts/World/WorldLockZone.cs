using UnityEngine;

/// <summary>
/// 世界锁定区域
/// 玩家进入此区域时无法切换世界
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class WorldLockZone : MonoBehaviour
{
    [Header("Lock Settings")]
    [Tooltip("是否在玩家进入时锁定世界切换")]
    public bool lockOnEnter = true;
    
    [Tooltip("锁定在哪个世界（如果指定）")]
    public WorldType? forcedWorld = null;

    [Header("Visual Feedback")]
    public Color activeColor = new Color(0.8f, 0.5f, 1f, 0.3f);

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (lockOnEnter && WorldManager.Instance != null)
            {
                WorldManager.Instance.LockWorldSwitching(true);
                
                // 如果指定了强制世界，切换到该世界
                if (forcedWorld.HasValue)
                {
                    WorldManager.Instance.SwitchWorld(forcedWorld.Value);
                }

                Debug.Log("[WorldLockZone] 世界切换已锁定");
            }

            // 视觉反馈
            if (spriteRenderer != null)
            {
                spriteRenderer.color = activeColor;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (lockOnEnter && WorldManager.Instance != null)
            {
                WorldManager.Instance.LockWorldSwitching(false);
                Debug.Log("[WorldLockZone] 世界切换已解锁");
            }

            // 恢复透明度
            if (spriteRenderer != null)
            {
                Color c = spriteRenderer.color;
                c.a = 0.2f;
                spriteRenderer.color = c;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.8f, 0.5f, 1f, 0.3f);
        
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
        }
    }
}
