using UnityEngine;

[DisallowMultipleComponent]
public class WorldPlatformVisuals : MonoBehaviour
{
    [Header("归属 (可从 WorldSpecificObject 自动读取)")]
    [SerializeField] private WorldBelonging worldBelonging = WorldBelonging.Both;

    [Header("组件引用")]
    [SerializeField] private SpriteRenderer targetRenderer;

    [Header("填充色 (保留粉/紫，但更显眼)")]
    [SerializeField] private Color worldAFill = new Color(1f, 0.45f, 0.75f, 1f);
    [SerializeField] private Color worldBFill = new Color(0.72f, 0.52f, 1f, 1f);
    [SerializeField] private Color bothFill = Color.white;


    private void Awake()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<SpriteRenderer>();
        }

        WorldSpecificObject worldSpecificObject = GetComponent<WorldSpecificObject>();
        if (worldSpecificObject != null)
        {
            worldBelonging = worldSpecificObject.WorldBelonging;
        }
    }

    private void Start()
    {
        ApplyVisuals();
    }

    private void ApplyVisuals()
    {
        if (targetRenderer == null)
        {
            Debug.LogWarning("WorldPlatformVisuals 找不到 SpriteRenderer。", this);
            return;
        }

        targetRenderer.color = GetFillColor();

    }

    private Color GetFillColor()
    {
        switch (worldBelonging)
        {
            case WorldBelonging.WorldA:
                return worldAFill;
            case WorldBelonging.WorldB:
                return worldBFill;
            default:
                return bothFill;
        }
    }

}
