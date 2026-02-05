using UnityEngine;
using UnityEngine.UI;

public class DashCooldownUI : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private Image cooldownImage;
    [SerializeField] private Text cooldownText; // 可选：显示数字
    
    [Header("玩家引用")]
    [SerializeField] private PlayerController playerController;
    
    [Header("颜色配置")]
    [SerializeField] private Color readyColor = Color.green;
    [SerializeField] private Color cooldownColor = Color.red;

    private void Start()
    {
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }
        
        if (cooldownImage == null)
        {
            cooldownImage = GetComponent<Image>();
        }
    }

    private void Update()
    {
        if (playerController == null || cooldownImage == null)
            return;
        
        // 获取冷却百分比
        float cooldownPercent = playerController.GetDashCooldownPercent();
        
        // 更新填充量
        cooldownImage.fillAmount = cooldownPercent;
        
        // 更新颜色
        cooldownImage.color = cooldownPercent >= 1f ? readyColor : cooldownColor;
        
        // 可选：更新文本
        if (cooldownText != null)
        {
            if (cooldownPercent >= 1f)
            {
                cooldownText.text = "就绪";
            }
            else
            {
                cooldownText.text = $"{(int)((1f - cooldownPercent) * 100)}%";
            }
        }
    }
}
