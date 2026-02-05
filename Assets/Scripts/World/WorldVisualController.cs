using System.Collections;
using UnityEngine;

public class WorldVisualController : MonoBehaviour
{
    [Header("相机引用")]
    [SerializeField] private Camera mainCamera;
    
    [Header("颜色配置")]
    [SerializeField] private Color worldAColor = new Color(1f, 0.7f, 0.85f); // 粉色 #FFB3D9
    [SerializeField] private Color worldBColor = new Color(0.83f, 0.65f, 1f); // 紫色 #D4A5FF
    
    [Header("过渡参数")]
    [SerializeField] private float transitionDuration = 0.2f;
    
    [Header("特效")]
    [SerializeField] private ParticleSystem switchEffect;
    [SerializeField] private Transform player;
    
    private Coroutine transitionCoroutine;

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        // 订阅世界切换事件
        if (WorldManager.Instance != null)
        {
            WorldManager.Instance.OnWorldSwitched.AddListener(OnWorldSwitch);
            
            // 设置初始颜色
            Color initialColor = WorldManager.Instance.currentWorld == WorldType.WorldA ? worldAColor : worldBColor;
            mainCamera.backgroundColor = initialColor;
        }
    }

    private void OnDestroy()
    {
        if (WorldManager.Instance != null)
        {
            WorldManager.Instance.OnWorldSwitched.RemoveListener(OnWorldSwitch);
        }
    }

    private void OnWorldSwitch()
    {
        // 停止之前的过渡
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }
        
        // 开始新的颜色过渡
        transitionCoroutine = StartCoroutine(TransitionColor());
        
        // 播放切换特效
        PlaySwitchEffect();
    }

    private IEnumerator TransitionColor()
    {
        Color startColor = mainCamera.backgroundColor;
        Color targetColor = WorldManager.Instance.currentWorld == WorldType.WorldA ? worldAColor : worldBColor;
        
        float elapsed = 0f;
        
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionDuration;
            
            // 使用平滑插值
            mainCamera.backgroundColor = Color.Lerp(startColor, targetColor, t);
            
            yield return null;
        }
        
        // 确保最终颜色准确
        mainCamera.backgroundColor = targetColor;
    }

    private void PlaySwitchEffect()
    {
        if (switchEffect != null && player != null)
        {
            // 在玩家位置播放粒子效果
            switchEffect.transform.position = player.position;
            switchEffect.Play();
        }
    }
}
