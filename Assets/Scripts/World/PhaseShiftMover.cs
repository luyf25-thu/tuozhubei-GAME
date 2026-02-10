using UnityEngine;

/// <summary>
/// 相位移动平台
/// 在World A和World B中以不同的相位沿同一路径移动
/// </summary>
public class PhaseShiftMover : MonoBehaviour
{
    [Header("Path Settings")]
    [Tooltip("路径点（房间内局部坐标）")]
    public Vector2[] pathPoints = new Vector2[0];
    
    [Tooltip("移动速度（单位/秒）")]
    public float speed = 2f;

    [Header("World Phase Settings")]
    [Tooltip("World A的相位偏移（0-1）")]
    [Range(0f, 1f)]
    public float phaseA = 0f;
    
    [Tooltip("World B的相位偏移（0-1）")]
    [Range(0f, 1f)]
    public float phaseB = 0.5f;

    [Header("Optional")]
    [Tooltip("是否循环移动")]
    public bool loop = true;
    
    [Tooltip("是否往返移动（ping-pong）")]
    public bool pingPong = false;

    private Vector2 localStartPosition;
    private float pathLength = 0f;
    private float[] segmentLengths;
    private float startTime;
    private WorldManager worldManager;

    void Start()
    {
        localStartPosition = transform.localPosition;
        startTime = Time.time;

        worldManager = FindObjectOfType<WorldManager>();
        if (worldManager == null)
        {
            Debug.LogWarning("[PhaseShiftMover] WorldManager not found!");
        }

        CalculatePathLength();
    }

    void Update()
    {
        if (pathPoints == null || pathPoints.Length < 2)
        {
            return;
        }

        MovePlatform();
    }

    /// <summary>
    /// 由LevelBuilderFromJson调用，设置路径和参数
    /// </summary>
    public void SetPathLocal(Vector2[] points, float moveSpeed, float worldAPhase, float worldBPhase)
    {
        pathPoints = points;
        speed = moveSpeed;
        phaseA = worldAPhase;
        phaseB = worldBPhase;

        // 如果还没Start，记录第一个点作为起始位置
        if (pathPoints != null && pathPoints.Length > 0)
        {
            transform.localPosition = pathPoints[0];
        }

        CalculatePathLength();
    }

    void CalculatePathLength()
    {
        if (pathPoints == null || pathPoints.Length < 2)
        {
            pathLength = 0f;
            return;
        }

        segmentLengths = new float[pathPoints.Length - 1];
        pathLength = 0f;

        for (int i = 0; i < pathPoints.Length - 1; i++)
        {
            float segmentLength = Vector2.Distance(pathPoints[i], pathPoints[i + 1]);
            segmentLengths[i] = segmentLength;
            pathLength += segmentLength;
        }
    }

    void MovePlatform()
    {
        if (pathLength <= 0f) return;

        // 获取当前世界的相位
        float currentPhase = GetCurrentWorldPhase();

        // 计算基于时间和相位的位置
        float cycleTime = pathLength / speed; // 一个完整周期的时间
        float elapsedTime = (Time.time - startTime);
        float t = ((elapsedTime / cycleTime) + currentPhase) % 1f; // 0-1之间

        if (pingPong)
        {
            // 往返模式
            t = Mathf.PingPong(t * 2f, 1f);
        }
        else if (!loop && t >= 1f)
        {
            // 非循环模式，到达终点后停止
            t = 1f;
        }

        // 计算当前位置
        Vector2 position = GetPositionAlongPath(t);
        transform.localPosition = position;
    }

    float GetCurrentWorldPhase()
    {
        if (worldManager == null)
        {
            return phaseA;
        }

        // 根据当前世界返回对应的相位
        switch (worldManager.currentWorld)
        {
            case WorldType.WorldA:
                return phaseA;
            case WorldType.WorldB:
                return phaseB;
            default:
                return phaseA;
        }
    }

    Vector2 GetPositionAlongPath(float t)
    {
        if (pathPoints == null || pathPoints.Length < 2)
        {
            return localStartPosition;
        }

        if (t <= 0f)
        {
            return pathPoints[0];
        }

        if (t >= 1f)
        {
            return pathPoints[pathPoints.Length - 1];
        }

        // 计算在哪个线段上
        float targetDistance = t * pathLength;
        float accumulatedDistance = 0f;

        for (int i = 0; i < segmentLengths.Length; i++)
        {
            if (accumulatedDistance + segmentLengths[i] >= targetDistance)
            {
                // 在这个线段上
                float segmentT = (targetDistance - accumulatedDistance) / segmentLengths[i];
                return Vector2.Lerp(pathPoints[i], pathPoints[i + 1], segmentT);
            }

            accumulatedDistance += segmentLengths[i];
        }

        // 安全返回
        return pathPoints[pathPoints.Length - 1];
    }

    void OnDrawGizmos()
    {
        if (pathPoints == null || pathPoints.Length < 2)
        {
            return;
        }

        // 绘制路径
        Gizmos.color = Color.cyan;
        
        Vector3 parentPos = transform.parent != null ? transform.parent.position : Vector3.zero;

        for (int i = 0; i < pathPoints.Length - 1; i++)
        {
            Vector3 start = parentPos + (Vector3)pathPoints[i];
            Vector3 end = parentPos + (Vector3)pathPoints[i + 1];
            Gizmos.DrawLine(start, end);
        }

        // 绘制路径点
        Gizmos.color = Color.yellow;
        foreach (var point in pathPoints)
        {
            Gizmos.DrawWireSphere(parentPos + (Vector3)point, 0.2f);
        }

        // 绘制当前位置（仅在运行时）
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.3f);
        }
    }
}
