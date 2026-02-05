using UnityEngine;
using UnityEngine.UI;

public class DebugInfoUI : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private Text debugText;
    
    [Header("玩家引用")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Rigidbody2D playerRb;
    
    [Header("显示选项")]
    [SerializeField] private bool showWorld = true;
    [SerializeField] private bool showSpeed = true;
    [SerializeField] private bool showGravity = true;
    [SerializeField] private bool showSpeedMultiplier = true;
    [SerializeField] private bool showState = true;
    [SerializeField] private bool showPosition = true;

    private void Start()
    {
        if (debugText == null)
        {
            debugText = GetComponent<Text>();
        }
        
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }
        
        if (playerRb == null && playerController != null)
        {
            playerRb = playerController.GetComponent<Rigidbody2D>();
        }
    }

    private void Update()
    {
        if (debugText == null || WorldManager.Instance == null)
            return;
        
        string info = "=== 调试信息 ===\n";
        
        // 当前世界
        if (showWorld)
        {
            info += $"当前世界: {WorldManager.Instance.currentWorld}\n";
        }
        
        // 速度
        if (showSpeed && playerRb != null)
        {
            float speed = playerRb.velocity.magnitude;
            info += $"速度: {speed:F2} m/s\n";
            info += $"速度向量: ({playerRb.velocity.x:F2}, {playerRb.velocity.y:F2})\n";
        }
        
        // 重力倍率
        if (showGravity)
        {
            WorldRules rules = WorldManager.Instance.GetCurrentRules();
            info += $"重力倍率: {rules.gravityMultiplier}x\n";
        }
        
        // 速度倍率
        if (showSpeedMultiplier)
        {
            WorldRules rules = WorldManager.Instance.GetCurrentRules();
            info += $"移速倍率: {rules.speedMultiplier}x\n";
        }
        
        // 玩家状态
        if (showState && playerController != null)
        {
            info += $"状态: {playerController.CurrentState}\n";
            info += $"接地: {(playerController.IsGrounded ? "是" : "否")}\n";
            info += $"冲刺中: {(playerController.IsDashing ? "是" : "否")}\n";
        }
        
        // 位置
        if (showPosition && playerController != null)
        {
            Vector3 pos = playerController.transform.position;
            info += $"位置: ({pos.x:F2}, {pos.y:F2})\n";
        }
        
        debugText.text = info;
    }

    // 切换显示/隐藏
    public void ToggleDebugInfo()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
