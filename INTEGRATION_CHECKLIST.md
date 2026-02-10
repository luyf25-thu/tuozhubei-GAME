# 关卡系统整合完成 ✅

## 已完成的工作

### 1. 核心脚本（7个新脚本）

✅ **Assets/Scripts/Level/LevelBuilderFromJson.cs**  
关卡构建器，从JSON自动生成Unity场景物体

✅ **Assets/Scripts/World/PhaseShiftMover.cs**  
移动平台系统，支持双世界不同相位

✅ **Assets/Scripts/World/Pickup.cs**  
收集品系统（分数、dash重置、钥匙）

✅ **Assets/Scripts/World/KeyDoor.cs**  
钥匙门系统，自动检测并打开

✅ **Assets/Scripts/World/WorldLockZone.cs**  
世界切换锁定区域

✅ **Assets/Scripts/World/OneWayDropPlatform.cs**  
单向掉落平台（B固体，A穿透）

✅ **Assets/Scripts/World/SimpleRotator.cs**  
收集品旋转动画

### 2. 预制体生成工具（3个编辑器脚本）

✅ **Assets/Scripts/Level/HazardPrefabCreator.cs**  
一键生成9种危险物预制体

✅ **Assets/Scripts/Level/PickupPrefabCreator.cs**  
一键生成3种收集品和1种门预制体

✅ **Assets/Scripts/Level/ZonePrefabCreator.cs**  
一键生成3种特殊区域预制体

### 3. 示例关卡数据

✅ **Assets/Levels/MirrorTideCity_LevelData.json**  
镜潮城完整关卡数据（17个房间）

### 4. 系统扩展

✅ **GameManager.cs** 已更新：
- `AddScore(int)` - 分数系统
- `CollectKey(string)` - 钥匙收集
- `HasKey(string)` - 钥匙检查
- `currentWorld` 属性 - 代理WorldManager

✅ **WorldManager.cs** 已更新：
- `LockWorldSwitching(bool)` - 锁定/解锁世界切换
- `SwitchWorld(WorldType)` - 强制切换到指定世界

✅ **PlayerController.cs** 已更新：
- `ResetDashCooldown()` - 重置dash冷却
- `Die()` - 死亡触发

### 5. 文档

✅ **LEVEL_SYSTEM_GUIDE.md**  
完整的关卡系统使用指南（6000+字）

✅ **Game_Implementation_Plan.md** 已更新  
添加关卡系统章节

---

## 接下来在Unity中的操作

### 步骤1：生成预制体（3分钟）

在Unity编辑器菜单依次执行：

1. `Tools > Create Level Hazards`  
   生成9种危险物预制体

2. `Tools > Create Level Pickups`  
   生成3种收集品和1种门预制体

3. `Tools > Create Level Zones`  
   生成3种特殊区域预制体

### 步骤2：手动创建移动平台预制体（2分钟）

1. 场景中创建空物体 `Phase Shift Rail`
2. 添加组件：
   - SpriteRenderer（设置简单方块sprite）
   - BoxCollider2D（非trigger）
   - PhaseShiftMover脚本
3. 保存到 `Assets/Prefabs/MovingPlatform/Phase Shift Rail.prefab`
4. 删除场景中的临时物体

### 步骤3：设置关卡构建器（3分钟）

1. 在游戏场景创建空物体 `LevelRoot`
2. 添加 `LevelBuilderFromJson` 组件
3. 在Inspector连接所有预制体：
   - **levelJson** = `MirrorTideCity_LevelData`
   - **platformA** = `platform A`
   - **platformB** = `platform B`
   - **platformBoth** = `platform Both`
   - **checkpoint** = `Check Point`
   - **spikeBoth** = `Spike Both`
   - **spikeA** = `Spike A`
   - **spikeB** = `Spike B`
   - （以此类推，连接所有生成的预制体）

### 步骤4：生成关卡（1秒）

1. 右键 `LevelBuilderFromJson` 组件
2. 选择 `Build Level`
3. 查看生成的 `__BUILT_LEVEL__` 子物体

---

## 预制体清单

### 必需连接的预制体（18个）

**平台**（3个）：
- platform A
- platform B  
- platform Both

**危险物**（9个）：
- Spike Both, Spike A, Spike B
- Laser A, Laser B
- Saw A, Saw B
- Thorn A, Thorn B
- Pit Killzone

**收集品**（3个）：
- Echo Shard
- Echo Crystal - Dash Reset
- Mirror Rune Key

**门**（1个）：
- Key Door - Mirror Rune

**移动平台**（1个）：
- Phase Shift Rail

**区域**（3个）：
- World Lock Zone
- Wall Marker
- One Way Drop (B to A)

**检查点**（1个）：
- Check Point

---

## 验证清单

### 在Unity编辑器中检查：

- [ ] 所有预制体已生成在对应文件夹
- [ ] Phase Shift Rail手动创建完成
- [ ] LevelBuilderFromJson所有槽已连接
- [ ] 右键组件可看到"Build Level"菜单
- [ ] 执行Build Level后__BUILT_LEVEL__出现
- [ ] 场景视图中可看到镜潮城关卡布局
- [ ] 全部房间正确定位（17个Room子物体）

### 运行时测试：

- [ ] 移动平台在两个世界中不同位置移动
- [ ] 危险物在对应世界中可见/隐藏
- [ ] 收集品可拾取并触发效果
- [ ] 钥匙门在拾取钥匙后可通过
- [ ] WorldLockZone进入时无法切换世界
- [ ] OneWayDrop在B固体、A穿透

---

## 常见问题

**Q: 找不到Tools菜单？**  
A: 确保HazardPrefabCreator等脚本在Assets/Scripts/Level文件夹，Unity会自动识别编辑器脚本。

**Q: 预制体生成失败？**  
A: 确保 `Assets/Prefabs/Hazard`、`Assets/Prefabs/Pickup` 等文件夹已存在。

**Q: Build Level没反应？**  
A: 检查Console是否有错误，确保levelJson TextAsset已连接。

**Q: 移动平台不动？**  
A: 检查PhaseShiftMover的pathPoints数组、speed参数，确保WorldManager存在。

---

## 快速参考

### 创建自定义关卡

1. 复制 `MirrorTideCity_LevelData.json`
2. 修改房间和物体数据
3. 在LevelBuilderFromJson中指定新JSON
4. Build Level

### 调试技巧

- 右键组件 → `Clear Built` 清除旧关卡
- 选中PhaseShiftRail查看路径Gizmos（青色线）
- 查看Zone范围的线框Gizmos
- 检查Console的警告信息

---

## 文件位置速查

```
d:\Vscodeproject\tuozhubei-GAME\
├── LEVEL_SYSTEM_GUIDE.md           ← 详细使用指南
├── INTEGRATION_CHECKLIST.md        ← 本文件
├── Game_Implementation_Plan.md     ← 项目总设计文档
├── Assets/
│   ├── Scripts/
│   │   ├── Level/
│   │   │   ├── LevelBuilderFromJson.cs
│   │   │   ├── HazardPrefabCreator.cs
│   │   │   ├── PickupPrefabCreator.cs
│   │   │   └── ZonePrefabCreator.cs
│   │   ├── World/
│   │   │   ├── PhaseShiftMover.cs
│   │   │   ├── Pickup.cs
│   │   │   ├── KeyDoor.cs
│   │   │   ├── WorldLockZone.cs
│   │   │   ├── OneWayDropPlatform.cs
│   │   │   └── SimpleRotator.cs
│   │   ├── Managers/
│   │   │   └── GameManager.cs (已更新)
│   │   └── Player/
│   │       └── PlayerController.cs (已更新)
│   └── Levels/
│       └── MirrorTideCity_LevelData.json
```

---

**整合完成！** 🎉

现在你可以在Unity中快速生成和测试镜潮城关卡了。
