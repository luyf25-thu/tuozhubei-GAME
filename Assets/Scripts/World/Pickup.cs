using UnityEngine;

/// <summary>
/// 可拾取物品基础类
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Pickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public PickupItemType itemType = PickupItemType.EchoShard;
    public int pointValue = 1;
    public bool destroyOnPickup = true;

    [Header("Audio")]
    public AudioClip pickupSound;

    [Header("Effects")]
    public GameObject pickupEffect;

    private bool hasBeenCollected = false;

    public enum PickupItemType
    {
        EchoShard,          // 普通收集品（加分）
        EchoCrystalDashReset, // 重置dash冷却并恢复空中dash次数
        MirrorRuneKey       // 钥匙（用于开门）
    }

    void Start()
    {
        // 确保collider是trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasBeenCollected) return;

        if (other.CompareTag("Player"))
        {
            CollectItem(other.gameObject);
        }
    }

    void CollectItem(GameObject player)
    {
        hasBeenCollected = true;

        // 播放音效
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }

        // 生成特效
        if (pickupEffect != null)
        {
            Instantiate(pickupEffect, transform.position, Quaternion.identity);
        }

        // 根据类型执行不同的效果
        PlayerController playerController = player.GetComponent<PlayerController>();
        
        switch (itemType)
        {
            case PickupItemType.EchoShard:
                // 增加分数/收集品计数
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddScore(pointValue);
                }
                Debug.Log($"[Pickup] 收集了 Echo Shard！+{pointValue}分");
                break;

            case PickupItemType.EchoCrystalDashReset:
                // 重置dash冷却
                if (playerController != null)
                {
                    playerController.ResetDashCooldown();
                    Debug.Log("[Pickup] Dash已重置！");
                }
                break;

            case PickupItemType.MirrorRuneKey:
                // 获得钥匙
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.CollectKey("MirrorRune");
                    Debug.Log("[Pickup] 获得了Mirror Rune钥匙！");
                }
                break;
        }

        // 销毁或隐藏物品
        if (destroyOnPickup)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
