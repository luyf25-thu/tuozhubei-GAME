using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 在Unity编辑器中一键创建所有需要的Hazard预制体
/// 使用方式：将此脚本放在Editor文件夹外，在菜单Tools > Create Level Hazards运行
/// </summary>
public class HazardPrefabCreator : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/Create Level Hazards")]
    public static void CreateAllHazardPrefabs()
    {
        string prefabPath = "Assets/Prefabs/Hazard/";
        if (!AssetDatabase.IsValidFolder(prefabPath))
        {
            Debug.LogError($"文件夹不存在: {prefabPath}");
            return;
        }

        // Spike类型
        CreateHazardPrefab("Spike Both", prefabPath, WorldBelonging.Both, new Color(1f, 0.5f, 0f), HazardShape.Spike);
        CreateHazardPrefab("Spike A", prefabPath, WorldBelonging.WorldA, new Color(1f, 0.7f, 0.85f), HazardShape.Spike);
        CreateHazardPrefab("Spike B", prefabPath, WorldBelonging.WorldB, new Color(0.83f, 0.65f, 1f), HazardShape.Spike);

        // Laser类型
        CreateHazardPrefab("Laser A", prefabPath, WorldBelonging.WorldA, new Color(1f, 0.2f, 0.2f), HazardShape.Laser);
        CreateHazardPrefab("Laser B", prefabPath, WorldBelonging.WorldB, new Color(0.6f, 0.2f, 1f), HazardShape.Laser);

        // Saw类型
        CreateHazardPrefab("Saw A", prefabPath, WorldBelonging.WorldA, new Color(0.8f, 0.3f, 0.3f), HazardShape.Saw);
        CreateHazardPrefab("Saw B", prefabPath, WorldBelonging.WorldB, new Color(0.5f, 0.3f, 0.8f), HazardShape.Saw);

        // Thorn类型
        CreateHazardPrefab("Thorn A", prefabPath, WorldBelonging.WorldA, new Color(0.6f, 0.9f, 0.4f), HazardShape.Thorn);
        CreateHazardPrefab("Thorn B", prefabPath, WorldBelonging.WorldB, new Color(0.4f, 0.6f, 0.9f), HazardShape.Thorn);

        // Pit Killzone
        CreateKillzonePrefab("Pit Killzone", prefabPath);

        Debug.Log("所有Hazard预制体创建完成！");
        AssetDatabase.Refresh();
    }

    enum HazardShape
    {
        Spike,
        Laser,
        Saw,
        Thorn
    }

    static void CreateHazardPrefab(string name, string path, WorldBelonging belonging, Color color, HazardShape shape)
    {
        // 创建GameObject
        GameObject hazardObj = new GameObject(name);

        // 添加SpriteRenderer
        SpriteRenderer sr = hazardObj.AddComponent<SpriteRenderer>();
        sr.sprite = CreateShapeSprite(shape);
        sr.color = color;
        sr.sortingOrder = 5;

        // 添加BoxCollider2D
        BoxCollider2D collider = hazardObj.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;

        // 添加Hazard脚本
        hazardObj.AddComponent<Hazard>();

        // 添加WorldSpecificObject脚本
        WorldSpecificObject worldObj = hazardObj.AddComponent<WorldSpecificObject>();
        
        // 使用反射设置worldBelonging（因为它是private SerializeField）
        var field = typeof(WorldSpecificObject).GetField("worldBelonging", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(worldObj, belonging);
        }

        // 保存为预制体
        string prefabFullPath = path + name + ".prefab";
        PrefabUtility.SaveAsPrefabAsset(hazardObj, prefabFullPath);
        
        // 清理场景中的临时对象
        DestroyImmediate(hazardObj);
        
        Debug.Log($"创建Hazard预制体: {prefabFullPath}");
    }

    static void CreateKillzonePrefab(string name, string path)
    {
        // 创建GameObject
        GameObject killzoneObj = new GameObject(name);

        // 添加BoxCollider2D（用于覆盖掉落区域）
        BoxCollider2D collider = killzoneObj.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(10, 2); // 默认大小，可在场景中调整

        // 添加SpriteRenderer（半透明红色，以便在编辑器中可见）
        SpriteRenderer sr = killzoneObj.AddComponent<SpriteRenderer>();
        sr.sprite = CreateSquareSprite();
        sr.color = new Color(1f, 0f, 0f, 0.2f);
        sr.sortingOrder = -10; // 在最底层

        // 添加Hazard脚本
        killzoneObj.AddComponent<Hazard>();

        // Killzone在两个世界都生效
        WorldSpecificObject worldObj = killzoneObj.AddComponent<WorldSpecificObject>();
        var field = typeof(WorldSpecificObject).GetField("worldBelonging", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(worldObj, WorldBelonging.Both);
        }

        // 保存为预制体
        string prefabFullPath = path + name + ".prefab";
        PrefabUtility.SaveAsPrefabAsset(killzoneObj, prefabFullPath);
        
        // 清理场景中的临时对象
        DestroyImmediate(killzoneObj);
        
        Debug.Log($"创建Killzone预制体: {prefabFullPath}");
    }

    static Sprite CreateShapeSprite(HazardShape shape)
    {
        // 创建简单的形状纹理
        int size = 64;
        Texture2D tex = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];

        switch (shape)
        {
            case HazardShape.Spike:
                // 三角形尖刺
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        float normalizedY = (float)y / size;
                        float normalizedX = (float)x / size;
                        
                        // 居中三角形
                        bool inTriangle = 
                            normalizedX > 0.5f - normalizedY * 0.5f &&
                            normalizedX < 0.5f + normalizedY * 0.5f;
                        
                        pixels[y * size + x] = inTriangle ? Color.white : Color.clear;
                    }
                }
                break;

            case HazardShape.Laser:
                // 横向矩形光束
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        bool inBeam = y >= size * 0.4f && y < size * 0.6f;
                        pixels[y * size + x] = inBeam ? Color.white : Color.clear;
                    }
                }
                break;

            case HazardShape.Saw:
                // 圆形锯齿
                float centerX = size / 2f;
                float centerY = size / 2f;
                float radius = size / 2.5f;
                
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        float dx = x - centerX;
                        float dy = y - centerY;
                        float dist = Mathf.Sqrt(dx * dx + dy * dy);
                        
                        bool inCircle = dist <= radius;
                        pixels[y * size + x] = inCircle ? Color.white : Color.clear;
                    }
                }
                break;

            case HazardShape.Thorn:
                // 多个小三角形
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        int cellX = x / 16;
                        int localX = x % 16;
                        int localY = y % 16;
                        
                        bool inThorn = localX > 8 - localY / 2 && localX < 8 + localY / 2;
                        pixels[y * size + x] = inThorn ? Color.white : Color.clear;
                    }
                }
                break;
        }

        tex.SetPixels(pixels);
        tex.Apply();
        
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 16);
    }

    static Sprite CreateSquareSprite()
    {
        int size = 64;
        Texture2D tex = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }
        
        tex.SetPixels(pixels);
        tex.Apply();
        
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 16);
    }
#endif
}
