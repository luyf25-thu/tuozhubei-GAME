using UnityEngine;

public class Hazard : MonoBehaviour
{
    [Header("障碍物配置")]
    [SerializeField] private bool destroyOnContact = false;
    [SerializeField] private AudioClip hitSound;
    
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检测是否碰到玩家
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[Hazard] Hit player. Hazard={name} at {transform.position}, Player={other.name} at {other.transform.position}");
            // 播放音效
            if (audioSource && hitSound)
            {
                audioSource.PlayOneShot(hitSound);
            }
            
            // 触发玩家死亡
            if (RespawnManager.Instance != null)
            {
                RespawnManager.Instance.TriggerDeath($"Hazard:{name}");
            }
            
            // 可选：销毁障碍物
            if (destroyOnContact)
            {
                Destroy(gameObject, 0.1f);
            }
        }
    }

    // 可视化障碍物范围
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
        }
    }
}
