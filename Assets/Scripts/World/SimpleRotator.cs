using UnityEngine;

/// <summary>
/// 简单的旋转动画组件
/// 用于pickup物品的视觉效果
/// </summary>
public class SimpleRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 90f; // 度/秒
    public Vector3 rotationAxis = Vector3.forward; // 旋转轴（z轴=2D旋转）

    [Header("Bobbing (Optional)")]
    public bool enableBobbing = true;
    public float bobbingSpeed = 2f;
    public float bobbingAmount = 0.3f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.localPosition;
    }

    void Update()
    {
        // 旋转
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);

        // 上下浮动（可选）
        if (enableBobbing)
        {
            float newY = startPosition.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmount;
            transform.localPosition = new Vector3(startPosition.x, newY, startPosition.z);
        }
    }
}
