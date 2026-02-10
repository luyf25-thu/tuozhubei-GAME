// LevelBuilderFromJson.cs
// Put this file under Assets/Scripts/Level/
//
// Usage:
// 1) Create an empty GameObject named "LevelRoot" in your scene.
// 2) Add this component to LevelRoot.
// 3) Assign the JSON TextAsset and all prefabs in the inspector.
// 4) Right-click the component header -> Build Level.

using System;
using UnityEngine;

[ExecuteAlways]
public class LevelBuilderFromJson : MonoBehaviour
{
    [Header("JSON input (TextAsset)")]
    public TextAsset levelJson;

    [Header("Prefab Library")]
    public GameObject platformA;
    public GameObject platformB;
    public GameObject platformBoth;

    public GameObject spikeBoth;
    public GameObject spikeA;
    public GameObject spikeB;

    public GameObject pitKillzone;          // trigger collider area
    public GameObject laserA;
    public GameObject laserB;

    public GameObject sawA;
    public GameObject sawB;

    public GameObject thornA;
    public GameObject thornB;

    public GameObject checkpoint;

    public GameObject echoShard;
    public GameObject echoCrystalDashReset;
    public GameObject mirrorRuneKey;
    public GameObject keyDoorMirrorRune;

    public GameObject phaseShiftRail;       // moving platform prefab (add PhaseShiftMover)
    public GameObject worldLockZone;        // optional
    public GameObject wallMarker;           // optional
    public GameObject oneWayDropBtoA;       // optional

    [Header("Build Settings")]
    public bool clearChildrenBeforeBuild = true;
    public string builtParentName = "__BUILT_LEVEL__";

    [ContextMenu("Build Level")]
    public void BuildLevel()
    {
        if (levelJson == null)
        {
            Debug.LogError("[LevelBuilder] levelJson not assigned.");
            return;
        }

        if (clearChildrenBeforeBuild)
            ClearBuilt();

        // Validate prefab connections before building
        if (!ValidatePrefabs())
        {
            Debug.LogError("[LevelBuilder] Build cancelled due to missing prefabs. Check Console for details.");
            return;
        }

        LevelData data = JsonUtility.FromJson<LevelData>(levelJson.text);
        if (data == null || data.rooms == null)
        {
            Debug.LogError("[LevelBuilder] Invalid JSON format.");
            return;
        }

        Transform parent = GetOrCreateParent();

        foreach (var room in data.rooms)
        {
            Vector2 origin = new Vector2(room.origin.x, room.origin.y);
            GameObject roomGO = new GameObject(room.id);
            roomGO.transform.SetParent(parent, false);
            roomGO.transform.localPosition = origin;

            if (room.objects == null) continue;

            foreach (var obj in room.objects)
                BuildObject(roomGO.transform, obj);
        }

        Debug.Log($"[LevelBuilder] Built {data.rooms.Length} rooms.");
    }

    [ContextMenu("Validate Prefabs")]
    bool ValidatePrefabs()
    {
        bool allValid = true;
        System.Text.StringBuilder missing = new System.Text.StringBuilder();
        missing.AppendLine("[LevelBuilder] Missing Prefab Assignments:");

        // Check platforms
        if (platformA == null) { missing.AppendLine("  - Platform A"); allValid = false; }
        if (platformB == null) { missing.AppendLine("  - Platform B"); allValid = false; }
        if (platformBoth == null) { missing.AppendLine("  - Platform Both"); allValid = false; }

        // Check hazards
        if (spikeBoth == null) { missing.AppendLine("  - Spike Both"); allValid = false; }
        if (spikeA == null) { missing.AppendLine("  - Spike A"); allValid = false; }
        if (spikeB == null) { missing.AppendLine("  - Spike B"); allValid = false; }
        if (pitKillzone == null) { missing.AppendLine("  - Pit Killzone"); allValid = false; }
        if (laserA == null) { missing.AppendLine("  - Laser A"); allValid = false; }
        if (laserB == null) { missing.AppendLine("  - Laser B"); allValid = false; }
        if (sawA == null) { missing.AppendLine("  - Saw A"); allValid = false; }
        if (sawB == null) { missing.AppendLine("  - Saw B"); allValid = false; }
        if (thornA == null) { missing.AppendLine("  - Thorn A"); allValid = false; }
        if (thornB == null) { missing.AppendLine("  - Thorn B"); allValid = false; }

        // Check collectibles
        if (checkpoint == null) { missing.AppendLine("  - Checkpoint"); allValid = false; }
        if (echoShard == null) { missing.AppendLine("  - Echo Shard"); allValid = false; }
        if (echoCrystalDashReset == null) { missing.AppendLine("  - Echo Crystal Dash Reset"); allValid = false; }
        if (mirrorRuneKey == null) { missing.AppendLine("  - Mirror Rune Key"); allValid = false; }
        if (keyDoorMirrorRune == null) { missing.AppendLine("  - Key Door Mirror Rune"); allValid = false; }

        // Check special objects
        if (phaseShiftRail == null) { missing.AppendLine("  - Phase Shift Rail"); allValid = false; }
        if (worldLockZone == null) { missing.AppendLine("  - World Lock Zone (optional)"); }
        if (wallMarker == null) { missing.AppendLine("  - Wall Marker (optional)"); }
        if (oneWayDropBtoA == null) { missing.AppendLine("  - One Way Drop B to A (optional)"); }

        if (!allValid)
        {
            Debug.LogError(missing.ToString());
        }
        else
        {
            Debug.Log("[LevelBuilder] All required prefabs assigned ✓");
        }

        return allValid;
    }

    [ContextMenu("Clear Built")]
    public void ClearBuilt()
    {
        Transform p = transform.Find(builtParentName);
        if (p != null)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) DestroyImmediate(p.gameObject);
            else Destroy(p.gameObject);
#else
            Destroy(p.gameObject);
#endif
        }
    }

    Transform GetOrCreateParent()
    {
        Transform p = transform.Find(builtParentName);
        if (p != null) return p;
        GameObject go = new GameObject(builtParentName);
        go.transform.SetParent(transform, false);
        return go.transform;
    }

    void BuildObject(Transform room, ObjData obj)
    {
        if (room == null)
        {
            Debug.LogError($"[LevelBuilder] Room transform is null!");
            return;
        }

        GameObject prefab = ResolvePrefab(obj);
        if (prefab == null)
        {
            Debug.LogError($"[LevelBuilder] Missing prefab! Type: {obj.type}, Kind: {obj.kind}. Please assign it in Inspector.");
            return;
        }

        Vector3 pos = new Vector3(obj.pos.x, obj.pos.y, 0f);
        GameObject go = InstantiatePrefab(prefab, room, pos);
        if (go == null)
        {
            Debug.LogError($"[LevelBuilder] Failed to instantiate prefab! Type: {obj.type}, Kind: {obj.kind}");
            return;
        }

        if (obj.size != null && obj.size.w > 0 && obj.size.h > 0)
            go.transform.localScale = new Vector3(obj.size.w, obj.size.h, 1f);

        if (obj.type == "MovingPlatform" && obj.path != null && obj.path.Length >= 2)
        {
            var mover = go.GetComponent<PhaseShiftMover>();
            if (mover != null)
            {
                Vector2[] pts = new Vector2[obj.path.Length];
                for (int i = 0; i < obj.path.Length; i++)
                    pts[i] = new Vector2(obj.path[i].x, obj.path[i].y);

                mover.SetPathLocal(pts, obj.speed, obj.worldPhase.A, obj.worldPhase.B);
            }
            else
            {
                Debug.LogWarning("[LevelBuilder] PhaseShiftMover not found on moving platform prefab.");
            }
        }

        if (!string.IsNullOrEmpty(obj.note))
            go.name = $"{go.name}__{obj.note}";
    }

    GameObject InstantiatePrefab(GameObject prefab, Transform parent, Vector3 localPos)
    {
        if (prefab == null)
        {
            Debug.LogError($"[LevelBuilder] Cannot instantiate null prefab!");
            return null;
        }
        if (parent == null)
        {
            Debug.LogError($"[LevelBuilder] Parent transform is null!");
            return null;
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            // Check if this is a prefab asset or a scene GameObject
            UnityEditor.PrefabAssetType prefabType = UnityEditor.PrefabUtility.GetPrefabAssetType(prefab);
            
            if (prefabType == UnityEditor.PrefabAssetType.Regular || 
                prefabType == UnityEditor.PrefabAssetType.Variant ||
                prefabType == UnityEditor.PrefabAssetType.Model)
            {
                // It's a real prefab asset - use PrefabUtility
                var go = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab, parent);
                if (go == null)
                {
                    Debug.LogError($"[LevelBuilder] PrefabUtility.InstantiatePrefab returned null for prefab: {prefab.name}");
                    return null;
                }
                go.transform.localPosition = localPos;
                return go;
            }
            else
            {
                // It's a scene GameObject or not a prefab - use regular Instantiate
                Debug.LogWarning($"[LevelBuilder] '{prefab.name}' is not a prefab asset. Using Instantiate instead. Consider saving it as a prefab in the Project.");
                var inst = Instantiate(prefab, parent);
                inst.transform.localPosition = localPos;
                return inst;
            }
        }
#endif
        var inst2 = Instantiate(prefab, parent);
        if (inst2 == null)
        {
            Debug.LogError($"[LevelBuilder] Instantiate returned null for prefab: {prefab.name}");
            return null;
        }
        inst2.transform.localPosition = localPos;
        return inst2;
    }

    GameObject ResolvePrefab(ObjData obj)
    {
        switch (obj.type)
        {
            case "Platform": return ResolvePlatform(obj.kind);
            case "Hazard": return ResolveHazard(obj.kind);
            case "Checkpoint": return checkpoint;
            case "Pickup": return ResolvePickup(obj.kind);
            case "Door": return ResolveDoor(obj.kind);
            case "Zone": return ResolveZone(obj.kind);
            case "MovingPlatform": return phaseShiftRail;
            default: return null;
        }
    }

    GameObject ResolvePlatform(string kind)
    {
        switch (kind)
        {
            case "A": return platformA;
            case "B": return platformB;
            case "Both": return platformBoth;
            default: return null;
        }
    }

    GameObject ResolveHazard(string kind)
    {
        switch (kind)
        {
            case "SpikeBoth": return spikeBoth;
            case "SpikeA": return spikeA;
            case "SpikeB": return spikeB;

            case "PitBoth": return pitKillzone;

            case "LaserA": return laserA;
            case "LaserB": return laserB;

            case "SawA": return sawA;
            case "SawB": return sawB;

            case "ThornA": return thornA;
            case "ThornB": return thornB;

            default: return null;
        }
    }

    GameObject ResolvePickup(string kind)
    {
        switch (kind)
        {
            case "EchoShard": return echoShard;
            case "EchoCrystal_DashReset": return echoCrystalDashReset;
            case "MirrorRuneKey": return mirrorRuneKey;
            default: return null;
        }
    }

    GameObject ResolveDoor(string kind)
    {
        switch (kind)
        {
            case "KeyDoor_MirrorRune": return keyDoorMirrorRune;
            default: return null;
        }
    }

    GameObject ResolveZone(string kind)
    {
        switch (kind)
        {
            case "WorldLock": return worldLockZone;
            case "WallMarker": return wallMarker;
            case "OneWayDrop_BtoA": return oneWayDropBtoA;
            default: return null;
        }
    }

    // -------- JSON structs --------
    [Serializable] public class LevelData { public Meta meta; public Room[] rooms; }
    [Serializable] public class Meta { public string name; public int version; public RoomSize roomSize; }
    [Serializable] public class RoomSize { public float w; public float h; }

    [Serializable] public class Room
    {
        public string id;
        public Grid grid;
        public Origin origin;
        public RoomSize size;
        public string theme;
        public ObjData[] objects;
    }

    [Serializable] public class Grid { public int x; public int y; }
    [Serializable] public class Origin { public float x; public float y; }

    [Serializable] public class ObjData
    {
        public string type;
        public string kind;
        public Pos pos;
        public Size2 size;
        public string note;

        public PathPoint[] path;
        public float speed;
        public WorldPhase worldPhase;
    }

    [Serializable] public class Pos { public float x; public float y; }
    [Serializable] public class Size2 { public float w; public float h; }
    [Serializable] public class PathPoint { public float x; public float y; }
    [Serializable] public class WorldPhase { public float A; public float B; }
}
