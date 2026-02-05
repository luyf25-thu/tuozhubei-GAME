using UnityEngine;

public enum WorldBelonging
{
    WorldA,
    WorldB,
    Both
}

public class WorldSpecificObject : MonoBehaviour
{
    [Header("世界归属")]
    [SerializeField] private WorldBelonging worldBelonging = WorldBelonging.Both;
    
    [Header("组件（自动获取）")]
    private Collider2D col;
    private SpriteRenderer spriteRenderer;
    private bool hasCollider;
    private bool hasRenderer;

    private void Start()
    {
        // 获取组件
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        hasCollider = col != null;
        hasRenderer = spriteRenderer != null;
        
        // 订阅世界切换事件
        if (WorldManager.Instance != null)
        {
            WorldManager.Instance.OnWorldSwitched.AddListener(UpdateVisibility);
            UpdateVisibility();
        }
        else
        {
            Debug.LogWarning("WorldManager未找到，WorldSpecificObject无法正常工作！");
        }
    }

    private void OnDestroy()
    {
        // 取消订阅
        if (WorldManager.Instance != null)
        {
            WorldManager.Instance.OnWorldSwitched.RemoveListener(UpdateVisibility);
        }
    }

    private void UpdateVisibility()
    {
        bool shouldBeActive = ShouldBeActive();
        
        // 更新碰撞器
        if (hasCollider)
        {
            col.enabled = shouldBeActive;
        }
        
        // 更新渲染器
        if (hasRenderer)
        {
            spriteRenderer.enabled = shouldBeActive;
        }
    }

    public WorldBelonging WorldBelonging => worldBelonging;

    private bool ShouldBeActive()
    {
        if (worldBelonging == WorldBelonging.Both)
            return true;
        
        if (WorldManager.Instance == null)
            return true;
        
        if (worldBelonging == WorldBelonging.WorldA)
            return WorldManager.Instance.currentWorld == WorldType.WorldA;
        
        if (worldBelonging == WorldBelonging.WorldB)
            return WorldManager.Instance.currentWorld == WorldType.WorldB;
        
        return true;
    }

    // 在编辑器中可视化
    private void OnDrawGizmos()
    {
        Color gizmoColor = Color.white;
        
        switch (worldBelonging)
        {
            case WorldBelonging.WorldA:
                gizmoColor = new Color(1f, 0.7f, 0.85f, 0.3f); // 粉色
                break;
            case WorldBelonging.WorldB:
                gizmoColor = new Color(0.83f, 0.65f, 1f, 0.3f); // 紫色
                break;
            case WorldBelonging.Both:
                gizmoColor = new Color(1f, 1f, 1f, 0.3f); // 白色
                break;
        }
        
        Gizmos.color = gizmoColor;
        
        if (GetComponent<Collider2D>() != null)
        {
            Bounds bounds = GetComponent<Collider2D>().bounds;
            Gizmos.DrawCube(bounds.center, bounds.size);
        }
    }
}
