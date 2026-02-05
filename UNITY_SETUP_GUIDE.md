# Unity 世界切换平台游戏 - Unity 设置指南

## 已创建的脚本文件

所有C#脚本已成功创建！以下是完整的脚本列表：

### 玩家系统 (Assets/Scripts/Player/)
- ✅ PlayerController.cs - 玩家移动、跳跃和冲刺控制
- ✅ PlayerWorldSwitcher.cs - 世界切换逻辑
- ✅ DashEffect.cs - 冲刺视觉效果

### 世界系统 (Assets/Scripts/World/)
- ✅ WorldRules.cs - 世界规则ScriptableObject
- ✅ WorldManager.cs - 世界管理器（单例）
- ✅ WorldSpecificObject.cs - 世界特定对象组件
- ✅ WorldVisualController.cs - 视觉过渡效果
- ✅ Hazard.cs - 障碍物
- ✅ Checkpoint.cs - 检查点

### 管理器 (Assets/Scripts/Managers/)
- ✅ GameManager.cs - 游戏状态和暂停管理
- ✅ RespawnManager.cs - 重生管理
- ✅ InputManager.cs - 输入处理
- ✅ CameraController.cs - 相机跟随

### UI系统 (Assets/Scripts/UI/)
- ✅ DashCooldownUI.cs - 冲刺冷却显示
- ✅ WorldIndicatorUI.cs - 世界指示器
- ✅ DebugInfoUI.cs - 调试信息显示

---

## 下一步：在Unity中设置

### 步骤1：创建WorldRules资源

1. 在Unity编辑器的 Project 窗口中，右键点击 `Assets/` 文件夹（或 `Assets/ScriptableObjects/` 也可以）
2. 选择 `Create → Game → World Rules`
   - 如果没有看到该菜单，确认 `WorldRules.cs` 文件在 `Assets/Scripts/World/` 下且无编译错误
3. 创建两个WorldRules资源：
   - **WorldA_Rules**
     - Gravity Multiplier: 1.0
     - Speed Multiplier: 1.0
     - World Name: "世界A"
     - World Color: RGB(255, 179, 217) 粉色
   
   - **WorldB_Rules**
     - Gravity Multiplier: 0.6
     - Speed Multiplier: 0.6
     - World Name: "世界B"
     - World Color: RGB(212, 165, 255) 紫色

### 步骤2：配置物理层

1. 打开 `Edit → Project Settings → Tags and Layers`
2. 添加以下层：
   - Layer 6: `WorldA`
   - Layer 7: `WorldB`
   - Layer 8: `Player`
   - Layer 9: `Hazard`

3. 配置碰撞矩阵：`Edit → Project Settings → Physics 2D`
   - Player ✓ WorldA (玩家在世界A时碰撞)
   - Player ✓ WorldB (玩家在世界B时碰撞)
   - Player ✓ Hazard (玩家始终与障碍物碰撞)

### 步骤3：创建玩家预制体

1. 创建一个空对象，命名为 `Player`，Tag设为 `Player`
2. 添加以下组件：
   - `Rigidbody2D`
     - Body Type: Dynamic
     - Gravity Scale: 1
     - Constraints: Freeze Rotation Z
   - `BoxCollider2D` 或 `CapsuleCollider2D`
   - `SpriteRenderer` （添加玩家精灵图）
   - `AudioSource` （用于音效）
   - `TrailRenderer` （用于冲刺拖尾）

3. 创建子对象 `GroundCheck`：
   - 位置：玩家脚底位置
   - 用于检测地面

4. 添加脚本到Player：
   - `PlayerController`
   - `PlayerWorldSwitcher`
   - `DashEffect`

5. 配置 PlayerController 参数：
   - Base Speed: 5
   - Jump Force: 10
   - Base Dash Speed: 15
   - Dash Duration: 0.2
   - Dash Cooldown: 1.5
   - Ground Check: 拖入GroundCheck子对象
   - Ground Check Radius: 0.2
   - Ground Layer: 选择WorldA和WorldB层

### 步骤4：创建管理器对象

创建一个空对象 `GameManagers`，添加以下脚本：
- `WorldManager` - 配置WorldA_Rules和WorldB_Rules
- `GameManager`
- `RespawnManager` - 配置Player引用
- `InputManager` - 配置PlayerController和PlayerWorldSwitcher引用

### 步骤5：设置相机

1. 选择Main Camera
2. 添加 `CameraController` 脚本
3. 配置：
   - Target: 拖入Player对象
   - Smooth Speed: 0.125
   - Offset: (0, 2, -10)

4. 添加 `WorldVisualController` 脚本
5. 配置：
   - Main Camera: 拖入自身
   - World A Color: RGB(255, 179, 217)
   - World B Color: RGB(212, 165, 255)
   - Transition Duration: 0.2
   - Player: 拖入Player对象

### 步骤6：创建平台预制体

#### PlatformA（粉色平台）
1. 创建精灵对象，命名为 `PlatformA`
2. 设置Layer为 `WorldA`
3. 添加 `BoxCollider2D`
4. 添加 `WorldSpecificObject` 脚本
   - World Belonging: WorldA
5. SpriteRenderer颜色设为粉色 #FFB3D9
6. 保存为预制体

#### PlatformB（紫色平台）
1. 创建精灵对象，命名为 `PlatformB`
2. 设置Layer为 `WorldB`
3. 添加 `BoxCollider2D`
4. 添加 `WorldSpecificObject` 脚本
   - World Belonging: WorldB
5. SpriteRenderer颜色设为紫色 #D4A5FF
6. 保存为预制体

#### PlatformBoth（白色平台）
1. 创建精灵对象，命名为 `PlatformBoth`
2. 添加 `BoxCollider2D`
3. 添加 `WorldSpecificObject` 脚本
   - World Belonging: Both
4. SpriteRenderer颜色设为白色
5. 保存为预制体

### 步骤7：创建障碍物预制体

1. 创建精灵对象，命名为 `Hazard`
2. 设置Layer为 `Hazard`
3. 添加 `BoxCollider2D` 或 `CircleCollider2D`
   - Is Trigger: ✓ (勾选)
4. 添加 `Hazard` 脚本
5. SpriteRenderer颜色设为红色
6. 保存为预制体

### 步骤8：创建检查点预制体

1. 创建精灵对象，命名为 `Checkpoint`
2. 添加 `BoxCollider2D`
   - Is Trigger: ✓ (勾选)
3. 添加 `Checkpoint` 脚本
4. 添加 `ParticleSystem` 组件（可选）
5. 保存为预制体

### 步骤9：创建UI Canvas

1. 创建Canvas (`UI → Canvas`)
2. Canvas Scaler设为 `Scale With Screen Size`

#### 创建世界指示器
1. 在Canvas下创建 `Image`，命名为 `WorldIndicator`
2. 位置：左上角
3. 添加 `WorldIndicatorUI` 脚本

#### 创建冲刺冷却显示
1. 在Canvas下创建 `Image`，命名为 `DashCooldown`
2. Image Type: Filled
3. Fill Method: Radial 360
4. 添加 `DashCooldownUI` 脚本
5. 配置Player Controller引用

#### 创建调试信息
1. 在Canvas下创建 `Text`，命名为 `DebugInfo`
2. 位置：右上角
3. 添加 `DebugInfoUI` 脚本
4. 配置Player Controller和Rigidbody2D引用

### 步骤10：创建测试场景

1. 创建新场景 `TutorialLevel`
2. 添加GameManagers对象
3. 添加Player
4. 添加Main Camera（带CameraController和WorldVisualController）
5. 使用平台预制体搭建简单关卡：
   - 起点：PlatformBoth
   - 几个PlatformA
   - 几个PlatformB
   - 添加Checkpoint
   - 添加Hazard
6. 测试所有机制

---

## 控制测试清单

启动游戏后，测试以下功能：

- [ ] A/D键左右移动
- [ ] 空格键跳跃
- [ ] 鼠标左键切换世界
- [ ] 世界切换时背景颜色平滑过渡（粉色↔紫色）
- [ ] 世界切换时平台显示/隐藏正确
- [ ] 在世界B移动和跳跃速度变慢
- [ ] 在世界B跳得更高
- [ ] 鼠标右键冲刺
- [ ] 冲刺有冷却时间
- [ ] 空中可以冲刺一次
- [ ] 落地后空中冲刺重置
- [ ] 触碰障碍物会死亡
- [ ] 死亡后重生到检查点
- [ ] R键快速重生
- [ ] ESC键暂停游戏
- [ ] UI正确显示当前世界和冲刺冷却

---

## 常见问题解决

### 问题1：玩家穿过平台
- 检查物理层和碰撞矩阵设置
- 确保平台的Collider2D已启用
- 确保Player的Rigidbody2D设置为Dynamic

### 问题2：世界切换不工作
- 检查WorldManager是否正确配置了两个WorldRules
- 确保WorldSpecificObject脚本已正确附加到平台
- 检查Console是否有错误信息

### 问题3：玩家不跳跃
- 检查Ground Layer设置
- 调整Ground Check位置和半径
- 查看GroundCheck是否正确引用

### 问题4：冲刺没有效果
- 检查Dash参数是否合理
- 确保冷却时间已过
- 查看Console是否有错误

### 问题5：UI不显示
- 确保Canvas和EventSystem存在
- 检查UI脚本的引用是否正确设置
- 确保Text/Image组件已正确配置

---

## 推荐的开发顺序

1. ✅ **基础移动** - 先让玩家能移动和跳跃
2. ✅ **世界切换** - 实现基本的世界切换（无视觉效果）
3. ✅ **平台显示切换** - 让平台根据世界显示/隐藏
4. ✅ **视觉反馈** - 添加背景颜色过渡
5. ✅ **冲刺系统** - 实现冲刺功能
6. ✅ **重生系统** - 添加障碍物和检查点
7. ✅ **UI系统** - 添加所有UI元素
8. 📝 **关卡设计** - 创建完整的教学关卡
9. 📝 **音效** - 添加所有音效
10. 📝 **优化和测试** - 调整参数，修复bug

---

## 下一步建议

1. **首先测试基础功能**：在Unity中设置最基本的场景，测试移动和世界切换
2. **逐步添加功能**：不要一次性添加所有功能，按顺序测试
3. **调整参数**：根据游戏手感调整速度、跳跃力、重力等参数
4. **设计关卡**：使用现有的预制体创建有趣的关卡
5. **添加美术资源**：后续可以替换精灵图和添加动画

---

**所有脚本文件已就绪！现在可以在Unity编辑器中开始设置和测试了。**
