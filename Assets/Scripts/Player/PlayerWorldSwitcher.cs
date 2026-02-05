using UnityEngine;

public class PlayerWorldSwitcher : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip switchSound;
    
    private void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        
        if (playerController == null)
            playerController = GetComponent<PlayerController>();
        
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        
        // 初始化物理参数
        UpdatePhysics();
    }

    public void SwitchWorld()
    {
        WorldManager.Instance.SwitchWorld();
        UpdatePhysics();
        
        // 播放切换音效
        if (audioSource && switchSound)
        {
            audioSource.PlayOneShot(switchSound);
        }
    }

    private void UpdatePhysics()
    {
        WorldRules rules = WorldManager.Instance.GetCurrentRules();
        
        // 更新重力倍率
        rb.gravityScale = rules.gravityMultiplier;
        
        // 更新碰撞层（根据当前世界）
        UpdateCollisionLayer();
        
        // 通知PlayerController更新速度倍率
        if (playerController != null)
        {
            playerController.UpdateSpeedMultiplier(rules.speedMultiplier);
        }
    }

    private void UpdateCollisionLayer()
    {
        // 这里设置玩家的碰撞层
        // 注意：实际的碰撞配置需要在Unity编辑器的Physics2D设置中配置
        // 这里只是示例代码，具体实现取决于您的层设置
        
        if (WorldManager.Instance.currentWorld == WorldType.WorldA)
        {
            // 在世界A，玩家应该与WorldA层的对象碰撞
            gameObject.layer = LayerMask.NameToLayer("Player");
        }
        else
        {
            // 在世界B，玩家应该与WorldB层的对象碰撞
            gameObject.layer = LayerMask.NameToLayer("Player");
        }
    }
}
