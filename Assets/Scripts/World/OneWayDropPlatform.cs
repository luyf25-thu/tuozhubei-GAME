using UnityEngine;

/// <summary>
/// 单向掉落平台
/// 在某个世界中是固体，在另一个世界中玩家可以从下方穿过
/// 用于实现B世界固体、A世界穿透的效果
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class OneWayDropPlatform : MonoBehaviour
{
    [Header("One-Way Settings")]
    [Tooltip("在哪个世界中是固体")]
    public WorldType solidInWorld = WorldType.WorldB;
    
    [Tooltip("是否允许玩家按下键从上方穿过")]
    public bool allowDropThrough = true;

    [Header("Visual")]
    public SpriteRenderer spriteRenderer;
    public Color solidColor = new Color(1f, 0.9f, 0.5f, 0.6f);
    public Color passthroughColor = new Color(1f, 0.9f, 0.5f, 0.2f);

    private Collider2D platformCollider;
    private WorldManager worldManager;

    void Start()
    {
        platformCollider = GetComponent<Collider2D>();
        worldManager = FindObjectOfType<WorldManager>();

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        UpdatePlatformState();

        // 订阅世界切换事件
        if (worldManager != null)
        {
            worldManager.OnWorldSwitched.AddListener(UpdatePlatformState);
        }
    }

    void OnDestroy()
    {
        if (worldManager != null)
        {
            worldManager.OnWorldSwitched.RemoveListener(UpdatePlatformState);
        }
    }

    void UpdatePlatformState()
    {
        if (worldManager == null || platformCollider == null)
        {
            return;
        }

        bool shouldBeSolid = (worldManager.currentWorld == solidInWorld);

        // 更新碰撞器状态
        platformCollider.isTrigger = !shouldBeSolid;

        // 更新视觉效果
        if (spriteRenderer != null)
        {
            spriteRenderer.color = shouldBeSolid ? solidColor : passthroughColor;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.9f, 0.5f, 0.4f);
        
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
            
            // 绘制箭头指示单向性
            Vector3 center = col.bounds.center;
            Vector3 arrowStart = center + Vector3.up * col.bounds.extents.y;
            Vector3 arrowEnd = center - Vector3.up * col.bounds.extents.y * 0.3f;
            
            Gizmos.DrawLine(arrowStart, arrowEnd);
            Gizmos.DrawLine(arrowEnd, arrowEnd + Vector3.left * 0.2f + Vector3.up * 0.2f);
            Gizmos.DrawLine(arrowEnd, arrowEnd + Vector3.right * 0.2f + Vector3.up * 0.2f);
        }
    }
}
