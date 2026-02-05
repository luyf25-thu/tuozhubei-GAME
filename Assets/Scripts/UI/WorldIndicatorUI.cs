using UnityEngine;
using UnityEngine.UI;

public class WorldIndicatorUI : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private Image indicatorImage;
    [SerializeField] private Text worldNameText; // 可选：显示世界名称
    
    [Header("颜色配置")]
    [SerializeField] private Color worldAColor = new Color(1f, 0.7f, 0.85f); // 粉色
    [SerializeField] private Color worldBColor = new Color(0.83f, 0.65f, 1f); // 紫色

    private void Start()
    {
        if (indicatorImage == null)
        {
            indicatorImage = GetComponent<Image>();
        }
        
        // 订阅世界切换事件
        if (WorldManager.Instance != null)
        {
            WorldManager.Instance.OnWorldSwitched.AddListener(UpdateIndicator);
            UpdateIndicator();
        }
    }

    private void OnDestroy()
    {
        if (WorldManager.Instance != null)
        {
            WorldManager.Instance.OnWorldSwitched.RemoveListener(UpdateIndicator);
        }
    }

    private void UpdateIndicator()
    {
        if (WorldManager.Instance == null)
            return;
        
        // 更新颜色
        Color targetColor = WorldManager.Instance.currentWorld == WorldType.WorldA ? worldAColor : worldBColor;
        
        if (indicatorImage != null)
        {
            indicatorImage.color = targetColor;
        }
        
        // 可选：更新文本
        if (worldNameText != null)
        {
            string worldName = WorldManager.Instance.currentWorld == WorldType.WorldA ? "世界 A" : "世界 B";
            worldNameText.text = worldName;
        }
    }
}
