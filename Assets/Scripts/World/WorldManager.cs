using UnityEngine;
using UnityEngine.Events;

public enum WorldType
{
    WorldA,
    WorldB
}

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance { get; private set; }
    
    [Header("世界规则")]
    [SerializeField] private WorldRules worldARules;
    [SerializeField] private WorldRules worldBRules;
    
    [Header("初始世界")]
    [SerializeField] private WorldType startingWorld = WorldType.WorldA;
    
    // 当前世界
    public WorldType currentWorld { get; private set; }
    
    // 世界切换事件
    public UnityEvent OnWorldSwitched;
    
    // 世界切换锁定
    private bool worldSwitchingLocked = false;

    private void Awake()
    {
        // 单例模式
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // 初始化
        currentWorld = startingWorld;
        
        if (OnWorldSwitched == null)
            OnWorldSwitched = new UnityEvent();
    }

    public WorldRules GetCurrentRules()
    {
        return currentWorld == WorldType.WorldA ? worldARules : worldBRules;
    }

    public WorldRules GetWorldARules()
    {
        return worldARules;
    }

    public WorldRules GetWorldBRules()
    {
        return worldBRules;
    }

    public void SwitchWorld()
    {
        // 如果锁定了世界切换，则不允许切换
        if (worldSwitchingLocked)
        {
            Debug.LogWarning("[WorldManager] 世界切换已锁定");
            return;
        }
        
        // 切换世界
        currentWorld = currentWorld == WorldType.WorldA ? WorldType.WorldB : WorldType.WorldA;
        
        // 触发事件
        OnWorldSwitched?.Invoke();
        
        Debug.Log($"切换到世界: {currentWorld}");
    }

    public void SwitchWorld(WorldType targetWorld)
    {
        // 强制切换到指定世界（忽略锁定）
        if (currentWorld != targetWorld)
        {
            currentWorld = targetWorld;
            OnWorldSwitched?.Invoke();
            Debug.Log($"[WorldManager] 强制切换到世界: {currentWorld}");
        }
    }

    public void SetWorld(WorldType world)
    {
        if (currentWorld != world)
        {
            currentWorld = world;
            OnWorldSwitched?.Invoke();
        }
    }

    public Color GetCurrentWorldColor()
    {
        return GetCurrentRules().worldColor;
    }
    
    /// <summary>
    /// 锁定或解锁世界切换
    /// </summary>
    public void LockWorldSwitching(bool locked)
    {
        worldSwitchingLocked = locked;
        Debug.Log($"[WorldManager] 世界切换 {(locked ? "已锁定" : "已解锁")}");
    }
    
    /// <summary>
    /// 检查世界切换是否被锁定
    /// </summary>
    public bool IsWorldSwitchingLocked()
    {
        return worldSwitchingLocked;
    }
}
