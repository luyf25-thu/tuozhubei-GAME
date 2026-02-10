using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 在Unity编辑器中一键创建所有需要的Pickup和Door预制体
/// 使用方式：菜单Tools > Create Level Pickups
/// </summary>
public class PickupPrefabCreator : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/Create Level Pickups")]
    public static void CreateAllPickupPrefabs()
    {
        // 确保文件夹存在
        string pickupPath = "Assets/Prefabs/Pickup/";
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs/Pickup"))
        {
            AssetDatabase.CreateFolder("Assets/Prefabs", "Pickup");
        }

        string doorPath = "Assets/Prefabs/Door/";
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs/Door"))
        {
            AssetDatabase.CreateFolder("Assets/Prefabs", "Door");
        }

        // 创建Pickup预制体
        CreatePickupPrefab("Echo Shard", pickupPath, PickupType.EchoShard, new Color(0.5f, 1f, 1f), WorldBelonging.Both);
        CreatePickupPrefab("Echo Crystal - Dash Reset", pickupPath, PickupType.DashReset, new Color(1f, 0.8f, 0.3f), WorldBelonging.Both);
        CreatePickupPrefab("Mirror Rune Key", pickupPath, PickupType.Key, new Color(1f, 1f, 0.5f), WorldBelonging.WorldB);

        // 创建Door预制体
        CreateDoorPrefab("Key Door - Mirror Rune", doorPath, new Color(0.7f, 0.7f, 0.9f));

        Debug.Log("所有Pickup和Door预制体创建完成！");
        AssetDatabase.Refresh();
    }

    enum PickupType
    {
        EchoShard,      // 普通收集品
        DashReset,      // 重置dash冷却
        Key             // 钥匙
    }

    static void CreatePickupPrefab(string name, string path, PickupType type, Color color, WorldBelonging belonging)
    {
        // 创建GameObject
        GameObject pickupObj = new GameObject(name);

        // 添加SpriteRenderer
        SpriteRenderer sr = pickupObj.AddComponent<SpriteRenderer>();
        sr.sprite = CreatePickupSprite(type);
        sr.color = color;
        sr.sortingOrder = 10;

        // 添加CircleCollider2D
        CircleCollider2D collider = pickupObj.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.4f;

        // 添加Pickup脚本（先检查是否存在，如果不存在就创建简单版本）
        var pickupScript = pickupObj.AddComponent<Pickup>();

        // 添加WorldSpecificObject（用于控制在哪个世界中可见/可拾取）
        WorldSpecificObject worldObj = pickupObj.AddComponent<WorldSpecificObject>();
        var field = typeof(WorldSpecificObject).GetField("worldBelonging", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(worldObj, belonging);
        }

        // 添加简单的旋转动画
        var rotator = pickupObj.AddComponent<SimpleRotator>();

        // 保存为预制体
        string prefabFullPath = path + name + ".prefab";
        PrefabUtility.SaveAsPrefabAsset(pickupObj, prefabFullPath);
        
        // 清理场景中的临时对象
        DestroyImmediate(pickupObj);
        
        Debug.Log($"创建Pickup预制体: {prefabFullPath}");
    }

    static void CreateDoorPrefab(string name, string path, Color color)
    {
        // 创建GameObject
        GameObject doorObj = new GameObject(name);

        // 添加SpriteRenderer
        SpriteRenderer sr = doorObj.AddComponent<SpriteRenderer>();
        sr.sprite = CreateDoorSprite();
        sr.color = color;
        sr.sortingOrder = 5;

        // 添加BoxCollider2D（默认是固体，需要钥匙才能打开）
        BoxCollider2D collider = doorObj.AddComponent<BoxCollider2D>();
        collider.isTrigger = false; // 初始是固体
        collider.size = new Vector2(1, 2);

        // 添加Door脚本
        doorObj.AddComponent<KeyDoor>();

        // Door通常只在World A中存在碰撞（根据JSON）
        WorldSpecificObject worldObj = doorObj.AddComponent<WorldSpecificObject>();
        var field = typeof(WorldSpecificObject).GetField("worldBelonging", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(worldObj, WorldBelonging.WorldA);
        }

        // 保存为预制体
        string prefabFullPath = path + name + ".prefab";
        PrefabUtility.SaveAsPrefabAsset(doorObj, prefabFullPath);
        
        // 清理场景中的临时对象
        DestroyImmediate(doorObj);
        
        Debug.Log($"创建Door预制体: {prefabFullPath}");
    }

    static Sprite CreatePickupSprite(PickupType type)
    {
        int size = 32;
        Texture2D tex = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];

        float centerX = size / 2f;
        float centerY = size / 2f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - centerX;
                float dy = y - centerY;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);

                bool visible = false;

                switch (type)
                {
                    case PickupType.EchoShard:
                        // 小菱形
                        visible = Mathf.Abs(dx) + Mathf.Abs(dy) < size / 3f;
                        break;

                    case PickupType.DashReset:
                        // 星形
                        float angle = Mathf.Atan2(dy, dx);
                        float starRadius = size / 3f * (1f + 0.4f * Mathf.Sin(angle * 5f));
                        visible = dist < starRadius;
                        break;

                    case PickupType.Key:
                        // 钥匙形状（圆圈+柄）
                        bool circle = dist < size / 4f;
                        bool handle = x > centerX - 2 && x < centerX + 2 && y < centerY && y > centerY - size / 3f;
                        visible = circle || handle;
                        break;
                }

                pixels[y * size + x] = visible ? Color.white : Color.clear;
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();
        
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 16);
    }

    static Sprite CreateDoorSprite()
    {
        int width = 32;
        int height = 64;
        Texture2D tex = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // 门框
                bool isFrame = x < 4 || x >= width - 4 || y < 4 || y >= height - 4;
                
                // 门板纹理
                bool isDoorPanel = (x / 8 + y / 8) % 2 == 0;
                
                Color color = isFrame ? Color.white : (isDoorPanel ? new Color(0.8f, 0.8f, 0.8f) : new Color(0.6f, 0.6f, 0.6f));
                pixels[y * width + x] = color;
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();
        
        return Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 16);
    }
#endif
}
