using UnityEngine;

[CreateAssetMenu(fileName = "WorldRules", menuName = "Game/World Rules")]
public class WorldRules : ScriptableObject
{
    [Header("物理参数")]
    [Tooltip("重力倍率")]
    public float gravityMultiplier = 1f;
    
    [Tooltip("移动速度倍率")]
    public float speedMultiplier = 1f;
    
    [Header("世界信息")]
    [Tooltip("世界名称")]
    public string worldName;
    
    [Tooltip("世界颜色")]
    public Color worldColor = Color.white;
}
