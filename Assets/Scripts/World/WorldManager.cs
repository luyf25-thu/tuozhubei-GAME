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
        // 切换世界
        currentWorld = currentWorld == WorldType.WorldA ? WorldType.WorldB : WorldType.WorldA;
        
        // 触发事件
        OnWorldSwitched?.Invoke();
        
        Debug.Log($"切换到世界: {currentWorld}");
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
}
