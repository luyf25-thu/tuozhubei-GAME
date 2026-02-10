using UnityEngine;

/// <summary>
/// 需要钥匙才能打开的门
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class KeyDoor : MonoBehaviour
{
    [Header("Door Settings")]
    public string requiredKeyId = "MirrorRune";
    public bool autoOpen = true; // 玩家持有钥匙时自动打开
    public bool permanentOpen = true; // 打开后永久保持打开状态

    [Header("Visual")]
    public SpriteRenderer spriteRenderer;
    public Color lockedColor = new Color(0.7f, 0.7f, 0.9f);
    public Color unlockedColor = new Color(0.5f, 1f, 0.5f);

    [Header("Audio")]
    public AudioClip unlockSound;

    private Collider2D doorCollider;
    private bool isOpen = false;

    void Start()
    {
        doorCollider = GetComponent<Collider2D>();
        
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        UpdateDoorState();
    }

    void Update()
    {
        if (autoOpen && !isOpen)
        {
            CheckAndOpenDoor();
        }
    }

    void CheckAndOpenDoor()
    {
        if (GameManager.Instance != null && GameManager.Instance.HasKey(requiredKeyId))
        {
            OpenDoor();
        }
    }

    public void OpenDoor()
    {
        if (isOpen) return;

        isOpen = true;

        // 禁用碰撞器（玩家可通过）
        if (doorCollider != null)
        {
            doorCollider.enabled = false;
        }

        // 更新视觉效果
        if (spriteRenderer != null)
        {
            spriteRenderer.color = unlockedColor;
            // 可选：添加半透明效果
            Color c = spriteRenderer.color;
            c.a = 0.5f;
            spriteRenderer.color = c;
        }

        // 播放音效
        if (unlockSound != null)
        {
            AudioSource.PlayClipAtPoint(unlockSound, transform.position);
        }

        Debug.Log($"[KeyDoor] 门已打开！所需钥匙: {requiredKeyId}");
    }

    public void CloseDoor()
    {
        if (!isOpen || permanentOpen) return;

        isOpen = false;

        // 启用碰撞器
        if (doorCollider != null)
        {
            doorCollider.enabled = true;
        }

        // 恢复视觉效果
        UpdateDoorState();

        Debug.Log($"[KeyDoor] 门已关闭！");
    }

    void UpdateDoorState()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = isOpen ? unlockedColor : lockedColor;
            
            Color c = spriteRenderer.color;
            c.a = isOpen ? 0.5f : 1f;
            spriteRenderer.color = c;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = isOpen ? Color.green : Color.red;
        
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
        }
    }
}
