# 关卡构建系统使用指南

## 概述

本项目整合了一个完整的JSON驱动关卡构建系统，可以快速创建大型、复杂的2D平台游戏关卡。系统包含：
- 自动化关卡生成器（从JSON生成Unity场景物体）
- 完整的预制体库（平台、危险物、收集品、移动平台、特殊区域）
- 示例关卡：镜潮城（MirrorTideCity）

## 快速开始（5分钟）

### 第一步：生成所有预制体

在Unity编辑器中，依次执行以下菜单命令：

1. `Tools > Create Level Hazards` - 创建所有危险物预制体
2. `Tools > Create Level Pickups` - 创建所有收集品和门预制体
3. `Tools > Create Level Zones` - 创建所有特殊区域预制体

> ℹ️ 这会在 `Assets/Prefabs/` 下自动生成所有需要的预制体。

### 第二步：创建移动平台预制体

**手动创建** PhaseShiftRail 预制体（因为它需要平台的SpriteRenderer）：

1. 在场景中创建空物体，命名 `Phase Shift Rail`
2. 添加 `SpriteRenderer` 组件（设置sprite为简单方块）
3. 添加 `BoxCollider2D` 组件（非trigger）
4. 添加 `PhaseShiftMover` 脚本
5. 拖拽到 `Assets/Prefabs/MovingPlatform/` 文件夹，保存为预制体
6. 删除场景中的临时物体

### 第三步：设置关卡构建器

1. 在你的游戏场景中创建空物体，命名 `LevelRoot`
2. 添加 `LevelBuilderFromJson` 组件
3. 在Inspector中设置：
   - **levelJson** = `Assets/Levels/MirrorTideCity_LevelData.json`
   - **platformA** = `Assets/Prefabs/Platform/platform A.prefab`
   - **platformB** = `Assets/Prefabs/Platform/platform B.prefab`
   - **platformBoth** = `Assets/Prefabs/Platform/platform Both.prefab`
   - **checkpoint** = `Assets/Prefabs/Checkpoint/Check Point.prefab`
   - 其他预制体按照名称对应连接（自动生成的都在对应文件夹）

### 第四步：生成关卡

1. 在Inspector中右键 `LevelBuilderFromJson` 组件头
2. 选择 `Build Level`
3. 稍等片刻，关卡会自动生成在 `__BUILT_LEVEL__` 子物体下

> ✅ 完成！现在你可以在Scene视图中看到完整的镜潮城关卡。

---

## 预制体库说明

### 平台（Platform）
- **platform A** - 只在World A存在的平台
- **platform B** - 只在World B存在的平台
- **platform Both** - 两个世界都存在的平台

### 危险物（Hazard）
| 预制体 | 说明 | 生效世界 |
|--------|------|----------|
| Spike Both | 尖刺 | 两个世界 |
| Spike A | 尖刺 | 只World A |
| Spike B | 尖刺 | 只World B |
| Laser A | 激光束 | 只World A |
| Laser B | 激光束 | 只World B |
| Saw A | 旋转锯齿 | 只World A |
| Saw B | 旋转锯齿 | 只World B |
| Thorn A | 荆棘 | 只World A |
| Thorn B | 荆棘 | 只World B |
| Pit Killzone | 掉落死亡区 | 两个世界 |

### 收集品（Pickup）
- **Echo Shard** - 普通收集品（加分）
- **Echo Crystal - Dash Reset** - 重置dash冷却，恢复空中dash
- **Mirror Rune Key** - 钥匙（用于开门），只在World B可见

### 门（Door）
- **Key Door - Mirror Rune** - 需要Mirror Rune钥匙才能通过

### 移动平台（Moving Platform）
- **Phase Shift Rail** - 在两个世界中以不同相位移动的平台

### 特殊区域（Zone）
- **World Lock Zone** - 进入后锁定世界切换
- **Wall Marker** - 墙壁装饰标记（纯视觉）
- **One Way Drop (B to A)** - 在World B是固体，在World A可穿过

---

## 关卡数据格式（JSON）

### 基本结构

```json
{
  "meta": {
    "name": "关卡名称",
    "roomSize": { "w": 32, "h": 18 },
    "worlds": {
      "A": { "gravityMultiplier": 1.0, "speedMultiplier": 1.0 },
      "B": { "gravityMultiplier": 0.6, "speedMultiplier": 0.6 }
    }
  },
  "rooms": [
    {
      "id": "房间ID",
      "origin": { "x": 0, "y": 0 },
      "objects": [...]
    }
  ]
}
```

### 物体类型

#### 平台
```json
{
  "type": "Platform",
  "kind": "Both", // 或 "A" 或 "B"
  "pos": { "x": 5, "y": 2 },
  "size": { "w": 10, "h": 1 },
  "note": "说明文字"
}
```

#### 危险物
```json
{
  "type": "Hazard",
  "kind": "SpikeBoth", // 或 "SpikeA", "LaserB" 等
  "pos": { "x": 12, "y": 3 },
  "size": { "w": 4, "h": 1 }
}
```

#### 移动平台
```json
{
  "type": "MovingPlatform",
  "kind": "PhaseShiftRail",
  "pos": { "x": 6, "y": 5 },
  "size": { "w": 6, "h": 1 },
  "path": [
    { "x": 6, "y": 5 },
    { "x": 22, "y": 5 }
  ],
  "speed": 2.2,
  "worldPhase": {
    "A": 0.3,  // World A的相位（0-1）
    "B": 0.8   // World B的相位（0-1）
  }
}
```

#### 收集品
```json
{
  "type": "Pickup",
  "kind": "EchoShard", // 或 "EchoCrystal_DashReset", "MirrorRuneKey"
  "pos": { "x": 12, "y": 9.5 },
  "note": "可选说明"
}
```

#### 门
```json
{
  "type": "Door",
  "kind": "KeyDoor_MirrorRune",
  "pos": { "x": 26, "y": 2 },
  "size": { "w": 1, "h": 2 }
}
```

#### 特殊区域
```json
{
  "type": "Zone",
  "kind": "WorldLock", // 或 "WallMarker", "OneWayDrop_BtoA"
  "pos": { "x": 10, "y": 7 },
  "size": { "w": 4, "h": 2 }
}
```

---

## 自定义关卡创建

### 方法一：手动编写JSON

1. 复制 `MirrorTideCity_LevelData.json` 作为模板
2. 修改 `meta.name` 为你的关卡名称
3. 删除所有 `rooms` 数组内容
4. 添加你自己的房间和物体
5. 保存到 `Assets/Levels/` 文件夹
6. 在LevelBuilderFromJson中指定新的JSON文件

### 方法二：使用外部关卡编辑器

推荐使用Tiled或其他2D关卡编辑器，然后转换为JSON格式。

---

## 系统原理

### PhaseShiftMover（移动平台）

移动平台在两个世界中沿相同路径移动，但有不同的相位偏移：

- **Phase 0.0** = 在起点
- **Phase 0.5** = 在路径中间
- **Phase 1.0** = 在终点（循环回起点）

例如：
- World A phase=0.3，World B phase=0.8
- 当玩家切换世界时，平台位置会瞬间跳变（因为相位不同）
- 这创造了独特的"相位转移"解谜玩法

### WorldSpecificObject（世界特定对象）

所有危险物、部分平台使用此脚本来控制在哪个世界中可见/可碰撞：

- `WorldBelonging.WorldA` - 只在World A生效
- `WorldBelonging.WorldB` - 只在World B生效
- `WorldBelonging.Both` - 两个世界都生效

### 键门系统

1. 玩家拾取钥匙 → `GameManager.CollectKey("MirrorRune")`
2. 门每帧检查 → `GameManager.HasKey("MirrorRune")`
3. 有钥匙 → 禁用Collider，玩家可通过

---

## 调试技巧

### 查看生成的关卡

1. 运行游戏前在Editor中执行`Build Level`
2. 在Hierarchy中展开 `LevelRoot > __BUILT_LEVEL__`
3. 每个房间是一个子物体，可单独查看

### 重新生成关卡

1. 右键组件 → `Clear Built`
2. 再次 → `Build Level`

### 查看移动平台路径

选中PhaseShiftRail预制体或实例，在Scene视图可看到青色路径线和黄色路径点。

### 查看Zone范围

所有Zone预制体在Scene视图中都有Gizmos线框显示范围。

---

## 常见问题

**Q: 为什么某些预制体没有生成？**  
A: 检查LevelBuilderFromJson的Inspector，确保所有预制体槽都已连接。缺失的会显示警告。

**Q: 移动平台不动？**  
A: 确保PhaseShiftMover脚本的path数组至少有2个点，且speed > 0。

**Q: 危险物在两个世界都可见？**  
A: 检查预制体上的WorldSpecificObject组件，worldBelonging设置是否正确。

**Q: 门打不开？**  
A: 确保玩家已拾取对应的钥匙（检查GameManager.HasKey返回值），且门的requiredKeyId与钥匙ID匹配。

---

## 扩展系统

### 添加新的危险物类型

1. 创建新的预制体
2. 添加 `Hazard` 和 `WorldSpecificObject` 脚本
3. 在 `LevelBuilderFromJson.ResolveHazard()` 中添加对应的case

### 添加新的Pickup类型

1. 在 `Pickup.PickupItemType` 枚举中添加新类型
2. 在 `Pickup.CollectItem()` 中添加对应逻辑
3. 创建新预制体并在`LevelBuilderFromJson.ResolvePickup()`中映射

---

## 文件清单

### 核心脚本
- `LevelBuilderFromJson.cs` - 关卡构建器
- `PhaseShiftMover.cs` - 移动平台逻辑
- `Pickup.cs` - 收集品基类
- `KeyDoor.cs` - 键门逻辑
- `WorldLockZone.cs` - 世界锁定区域
- `OneWayDropPlatform.cs` - 单向掉落平台
- `SimpleRotator.cs` - 简单旋转动画

### 工具脚本（仅编辑器）
- `HazardPrefabCreator.cs` - 危险物预制体生成器
- `PickupPrefabCreator.cs` - 收集品预制体生成器
- `ZonePrefabCreator.cs` - 区域预制体生成器

### 数据文件
- `MirrorTideCity_LevelData.json` - 示例关卡数据

---

## 性能建议

- 单个房间建议不超过100个物体
- 移动平台路径点建议不超过10个
- 大型关卡可分多个房间（origin偏移）

---

祝你创作出精彩的关卡！如有问题，检查Unity Console的警告和错误日志。
