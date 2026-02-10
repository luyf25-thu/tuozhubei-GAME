using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Editor tool for automatically creating moving platform prefabs.
/// Generates PhaseShiftRail prefab with proper components and configuration.
/// This is an Editor-only script and will not be included in builds.
/// </summary>
#if UNITY_EDITOR
public class MovingPlatformPrefabCreator
{
    private const string PREFAB_FOLDER = "Assets/Prefabs/MovingPlatform";
    private const string PREFAB_NAME = "Phase Shift Rail";

    [MenuItem("Tools/Create Level Moving Platforms")]
    public static void CreateMovingPlatformPrefabs()
    {
        // Ensure folder exists
        if (!AssetDatabase.IsValidFolder(PREFAB_FOLDER))
        {
            string parentFolder = "Assets/Prefabs";
            if (!AssetDatabase.IsValidFolder(parentFolder))
            {
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            }
            AssetDatabase.CreateFolder(parentFolder, "MovingPlatform");
        }

        // Create Phase Shift Rail prefab
        CreatePhaseShiftRailPrefab();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("✓ Moving platform prefabs created successfully!");
    }

    private static void CreatePhaseShiftRailPrefab()
    {
        string prefabPath = $"{PREFAB_FOLDER}/{PREFAB_NAME}.prefab";

        // Check if prefab already exists
        GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (existingPrefab != null)
        {
            Debug.LogWarning($"Prefab '{PREFAB_NAME}' already exists. Updating components...");
            // Update existing prefab instead of creating new one
            UpdatePrefabComponents(existingPrefab);
            return;
        }

        // Create temporary GameObject
        GameObject platformObj = new GameObject(PREFAB_NAME);

        // Add SpriteRenderer
        SpriteRenderer spriteRenderer = platformObj.AddComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(1f, 0.84f, 0.2f, 1f); // Golden yellow
        
        // Create simple platform sprite if none exists
        Sprite platformSprite = CreatePlatformSprite();
        spriteRenderer.sprite = platformSprite;
        spriteRenderer.sortingOrder = 2;

        // Add BoxCollider2D (NOT trigger - platforms must be solid)
        BoxCollider2D boxCollider = platformObj.AddComponent<BoxCollider2D>();
        boxCollider.size = new Vector2(2f, 0.5f);
        boxCollider.isTrigger = false; // CRITICAL: Must be false for platform collision

        // Add Rigidbody2D for physics
        Rigidbody2D rb = platformObj.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic; // Kinematic so it moves but doesn't respond to physics
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Add PhaseShiftMover script
        PhaseShiftMover mover = platformObj.AddComponent<PhaseShiftMover>();
        
        // Set default path (horizontal movement)
        Vector3[] defaultPath = new Vector3[]
        {
            Vector3.zero,
            Vector3.right * 3f
        };
        
        // Serialize the path via reflection
        SerializedObject so = new SerializedObject(mover);
        SerializedProperty pathProp = so.FindProperty("path");
        if (pathProp != null)
        {
            pathProp.arraySize = defaultPath.Length;
            for (int i = 0; i < defaultPath.Length; i++)
            {
                SerializedProperty element = pathProp.GetArrayElementAtIndex(i);
                element.vector3Value = defaultPath[i];
            }
        }
        
        // Set movement speed
        SerializedProperty speedProp = so.FindProperty("speed");
        if (speedProp != null)
        {
            speedProp.floatValue = 3f;
        }
        
        // Set phase offset
        SerializedProperty phaseOffsetProp = so.FindProperty("phaseOffset");
        if (phaseOffsetProp != null)
        {
            phaseOffsetProp.floatValue = 0.5f; // Offset platform in World B
        }
        
        so.ApplyModifiedProperties();

        // Create prefab
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(platformObj, prefabPath);
        Debug.Log($"✓ Created prefab: {PREFAB_NAME}");

        // Cleanup temporary object
        Object.DestroyImmediate(platformObj);
    }

    private static void UpdatePrefabComponents(GameObject prefab)
    {
        // Ensure components are present with correct settings
        SpriteRenderer spriteRenderer = prefab.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = prefab.AddComponent<SpriteRenderer>();
        }
        spriteRenderer.color = new Color(1f, 0.84f, 0.2f, 1f);

        BoxCollider2D boxCollider = prefab.GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            boxCollider = prefab.AddComponent<BoxCollider2D>();
            boxCollider.size = new Vector2(2f, 0.5f);
        }
        boxCollider.isTrigger = false; // CRITICAL: Ensure solid

        Rigidbody2D rb = prefab.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = prefab.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;
        }

        PhaseShiftMover mover = prefab.GetComponent<PhaseShiftMover>();
        if (mover == null)
        {
            mover = prefab.AddComponent<PhaseShiftMover>();
        }

        Debug.Log($"✓ Updated prefab: {prefab.name}");
    }

    /// <summary>
    /// Creates a simple platform sprite if needed.
    /// Returns existing sprite if available.
    /// </summary>
    private static Sprite CreatePlatformSprite()
    {
        // Try to load existing platform sprite
        string[] guids = AssetDatabase.FindAssets("Platform");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.Contains("Sprites") || path.Contains("sprite"))
            {
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (sprite != null)
                {
                    return sprite;
                }
            }
        }

        // Create simple yellow square sprite as fallback
        Texture2D platformTexture = new Texture2D(64, 16);
        Color[] pixels = new Color[64 * 16];
        
        // Fill with golden yellow
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = new Color(1f, 0.84f, 0.2f, 1f);
        }
        
        platformTexture.SetPixels(pixels);
        platformTexture.Apply();
        
        Sprite platformSprite = Sprite.Create(
            platformTexture,
            new Rect(0, 0, 64, 16),
            new Vector2(0.5f, 0.5f),
            32f
        );
        
        return platformSprite;
    }
}
#endif
