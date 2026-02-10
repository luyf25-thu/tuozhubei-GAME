using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 在Unity编辑器中一键创建所有需要的Zone预制体
/// 使用方式：菜单Tools > Create Level Zones
/// </summary>
public class ZonePrefabCreator : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/Create Level Zones")]
    public static void CreateAllZonePrefabs()
    {
        string zonePath = "Assets/Prefabs/Zone/";
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs/Zone"))
        {
            AssetDatabase.CreateFolder("Assets/Prefabs", "Zone");
        }

        // 创建Zone预制体
        CreateWorldLockZone("World Lock Zone", zonePath);
        CreateWallMarkerZone("Wall Marker", zonePath);
        CreateOneWayDropZone("One Way Drop (B to A)", zonePath);

        Debug.Log("所有Zone预制体创建完成！");
        AssetDatabase.Refresh();
    }

    static void CreateWorldLockZone(string name, string path)
    {
        GameObject zoneObj = new GameObject(name);

        // 添加BoxCollider2D（触发器）
        BoxCollider2D collider = zoneObj.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(5, 5);

        // 添加SpriteRenderer（半透明紫色，编辑器可见）
        SpriteRenderer sr = zoneObj.AddComponent<SpriteRenderer>();
        sr.sprite = CreateSquareSprite();
        sr.color = new Color(0.8f, 0.5f, 1f, 0.2f);
        sr.sortingOrder = -5;

        // 添加WorldLockZone脚本
        zoneObj.AddComponent<WorldLockZone>();

        // 保存为预制体
        string prefabFullPath = path + name + ".prefab";
        PrefabUtility.SaveAsPrefabAsset(zoneObj, prefabFullPath);
        DestroyImmediate(zoneObj);
        
        Debug.Log($"创建Zone预制体: {prefabFullPath}");
    }

    static void CreateWallMarkerZone(string name, string path)
    {
        GameObject zoneObj = new GameObject(name);

        // 添加SpriteRenderer（装饰性墙壁标记）
        SpriteRenderer sr = zoneObj.AddComponent<SpriteRenderer>();
        sr.sprite = CreateSquareSprite();
        sr.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        sr.sortingOrder = -2;

        // 不添加Collider，纯装饰性

        // 保存为预制体
        string prefabFullPath = path + name + ".prefab";
        PrefabUtility.SaveAsPrefabAsset(zoneObj, prefabFullPath);
        DestroyImmediate(zoneObj);
        
        Debug.Log($"创建Zone预制体: {prefabFullPath}");
    }

    static void CreateOneWayDropZone(string name, string path)
    {
        GameObject zoneObj = new GameObject(name);

        // 添加BoxCollider2D
        BoxCollider2D collider = zoneObj.AddComponent<BoxCollider2D>();
        collider.isTrigger = false; // 在某些世界中是固体
        collider.size = new Vector2(4, 2);

        // 添加SpriteRenderer
        SpriteRenderer sr = zoneObj.AddComponent<SpriteRenderer>();
        sr.sprite = CreateSquareSprite();
        sr.color = new Color(1f, 0.9f, 0.5f, 0.4f);
        sr.sortingOrder = -3;

        // 添加OneWayDropPlatform脚本
        zoneObj.AddComponent<OneWayDropPlatform>();

        // 在B世界是固体，在A世界是穿透
        WorldSpecificObject worldObj = zoneObj.AddComponent<WorldSpecificObject>();
        var field = typeof(WorldSpecificObject).GetField("worldBelonging", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(worldObj, WorldBelonging.WorldB);
        }


        // 保存为预制体
        string prefabFullPath = path + name + ".prefab";
        PrefabUtility.SaveAsPrefabAsset(zoneObj, prefabFullPath);
        DestroyImmediate(zoneObj);
        
        Debug.Log($"创建Zone预制体: {prefabFullPath}");
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
