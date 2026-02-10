using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("游戏状态")]
    [SerializeField] private bool isPaused = false;
    
    [Header("UI引用")]
    [SerializeField] private GameObject pauseMenuUI;
    
    [Header("游戏数据")]
    [SerializeField] private int score = 0;
    [SerializeField] private System.Collections.Generic.HashSet<string> collectedKeys = new System.Collections.Generic.HashSet<string>();
    
    // 代理WorldManager的currentWorld访问
    public int currentWorld => WorldManager.Instance != null ? (int)WorldManager.Instance.currentWorld : 0;
    
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
    }

    private void Update()
    {
        // 检测暂停输入
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        
        if (isPaused)
        {
            Pause();
        }
        else
        {
            Resume();
        }
    }

    private void Pause()
    {
        Time.timeScale = 0f;
        
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
        }
        
        Debug.Log("游戏暂停");
    }

    private void Resume()
    {
        Time.timeScale = 1f;
        
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
        
        Debug.Log("游戏继续");
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadLevel(string levelName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(levelName);
    }

    public void QuitGame()
    {
        Debug.Log("退出游戏");
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public bool IsPaused => isPaused;
    
    // === 收集品和分数系统 ===
    
    public void AddScore(int amount)
    {
        score += amount;
        Debug.Log($"[GameManager] 得分: +{amount}，总分: {score}");
    }
    
    public int GetScore()
    {
        return score;
    }
    
    public void CollectKey(string keyId)
    {
        if (collectedKeys.Add(keyId))
        {
            Debug.Log($"[GameManager] 收集钥匙: {keyId}");
        }
    }
    
    public bool HasKey(string keyId)
    {
        return collectedKeys.Contains(keyId);
    }
    
    public void ResetGameData()
    {
        score = 0;
        collectedKeys.Clear();
        Debug.Log("[GameManager] 游戏数据已重置");
    }
}
