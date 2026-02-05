using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("跟随目标")]
    [SerializeField] private Transform target;
    
    [Header("跟随参数")]
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset = new Vector3(0, 2, -10);
    
    [Header("边界限制")]
    [SerializeField] private bool useBounds = false;
    [SerializeField] private Vector2 minBounds;
    [SerializeField] private Vector2 maxBounds;
    
    [Header("跟随选项")]
    [SerializeField] private bool followX = true;
    [SerializeField] private bool followY = true;

    private void Start()
    {
        // 如果没有设置目标，尝试查找玩家
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    private void LateUpdate()
    {
        if (target == null)
            return;
        
        // 计算目标位置
        Vector3 desiredPosition = target.position + offset;
        
        // 根据设置决定是否跟随X/Y轴
        if (!followX)
            desiredPosition.x = transform.position.x;
        if (!followY)
            desiredPosition.y = transform.position.y;
        
        // 平滑跟随
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        
        // 应用边界限制
        if (useBounds)
        {
            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minBounds.x, maxBounds.x);
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minBounds.y, maxBounds.y);
        }
        
        // 确保Z轴位置（相机深度）
        smoothedPosition.z = offset.z;
        
        transform.position = smoothedPosition;
    }

    // 在编辑器中可视化边界
    private void OnDrawGizmosSelected()
    {
        if (!useBounds)
            return;
        
        Gizmos.color = Color.yellow;
        
        Vector3 bottomLeft = new Vector3(minBounds.x, minBounds.y, 0);
        Vector3 bottomRight = new Vector3(maxBounds.x, minBounds.y, 0);
        Vector3 topLeft = new Vector3(minBounds.x, maxBounds.y, 0);
        Vector3 topRight = new Vector3(maxBounds.x, maxBounds.y, 0);
        
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetBounds(Vector2 min, Vector2 max)
    {
        useBounds = true;
        minBounds = min;
        maxBounds = max;
    }
}
