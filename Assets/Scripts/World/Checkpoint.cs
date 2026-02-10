using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("检查点状态")]
    [SerializeField] private bool isActivated = false;
    
    [Header("视觉反馈")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color inactiveColor = Color.gray;
    [SerializeField] private Color activeColor = Color.green;
    [SerializeField] private ParticleSystem activationEffect;
    
    [Header("音效")]
    [SerializeField] private AudioClip activationSound;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        UpdateVisual();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            ActivateCheckpoint();
        }
    }

    private void ActivateCheckpoint()
    {
        isActivated = true;
        
        // 通知重生管理器 - 设置偏移后的重生点（避免重生在 Checkpoint 正中心或附近 Hazard）
        if (RespawnManager.Instance != null)
        {
            // 在 Checkpoint 上方 0.5 单位设置重生点，避免碰撞问题
            Vector3 safeRespawnPos = transform.position + Vector3.up * 0.5f;
            RespawnManager.Instance.SetCheckpoint(safeRespawnPos);
            Debug.Log($"Checkpoint 激活: {transform.position}, 重生点: {safeRespawnPos}");
        }
        
        // 更新视觉
        UpdateVisual();
        
        // 播放激活特效
        if (activationEffect != null)
        {
            activationEffect.Play();
        }
        
        // 播放音效
        if (audioSource && activationSound)
        {
            audioSource.PlayOneShot(activationSound);
        }
        
        Debug.Log($"检查点已激活: {gameObject.name}");
    }

    private void UpdateVisual()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = isActivated ? activeColor : inactiveColor;
        }
    }

    // 可视化检查点
    private void OnDrawGizmos()
    {
        Gizmos.color = isActivated ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }

    // 允许在编辑器中重置
    public void ResetCheckpoint()
    {
        isActivated = false;
        UpdateVisual();
    }
}
