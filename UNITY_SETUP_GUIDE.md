# Unity 世界切换平台游戏 - Unity 设置指南

## 已创建的脚本文件

所有 C# 脚本已创建完成，当前包含以下脚本：

### 玩家系统 (Assets/Scripts/Player/)
- ✅ PlayerController.cs - 移动、跳跃、冲刺、地面检测
- ✅ PlayerWorldSwitcher.cs - 世界切换与物理参数更新
- ✅ DashEffect.cs - 冲刺拖尾与粒子特效

### 世界系统 (Assets/Scripts/World/)
- ✅ WorldRules.cs - 世界规则 ScriptableObject
- ✅ WorldManager.cs - 世界管理器（单例）
- ✅ WorldSpecificObject.cs - 世界归属与显示/碰撞切换
- ✅ WorldPlatformVisuals.cs - 平台颜色自动设置
- ✅ WorldVisualController.cs - 相机背景色过渡与切换特效
- ✅ Hazard.cs - 障碍物（触发死亡）
- ✅ Checkpoint.cs - 检查点

### 管理器 (Assets/Scripts/Managers/)
- ✅ GameManager.cs - 游戏状态与暂停
- ✅ RespawnManager.cs - 重生与掉落检测
- ✅ InputManager.cs - 输入桥接（默认旧输入系统）
- ✅ CameraController.cs - 相机跟随

### UI 系统 (Assets/Scripts/UI/)
- ✅ DashCooldownUI.cs - 冲刺冷却显示
- ✅ WorldIndicatorUI.cs - 世界指示器
- ✅ DebugInfoUI.cs - 调试信息

---

## 在 Unity 中设置

### 步骤0：确认输入系统模式（重要）
当前脚本默认使用旧输入系统（`Input` API）。

- 推荐：`Edit → Project Settings → Player → Active Input Handling` 设为 **Input Manager (Old)** 或 **Both**。
- 如果设为 **Input System (New)**，`InputManager.cs`、`GameManager.cs`、`RespawnManager.cs` 的按键检测将失效。
- 如需切换到新输入系统，请先按“输入系统（可选）”章节配置。

### 步骤1：创建 WorldRules 资源

1. 在 Project 窗口右键 `Assets/`（或新建 `Assets/ScriptableObjects/`）
2. `Create → Game → World Rules`
3. 创建两个资源：
   - **WorldA_Rules**
     - Gravity Multiplier: 1.0
     - Speed Multiplier: 1.0
     - World Name: 世界A
     - World Color: RGB(255, 179, 217) #FFB3D9
   - **WorldB_Rules**
     - Gravity Multiplier: 0.6
     - Speed Multiplier: 0.6
     - World Name: 世界B
     - World Color: RGB(212, 165, 255) #D4A5FF

### 步骤2：配置物理层

1. `Edit → Project Settings → Tags and Layers`
2. 添加层：
   - Layer 6: `WorldA`
   - Layer 7: `WorldB`
   - Layer 8: `Player`
   - Layer 9: `Hazard`

3. `Edit → Project Settings → Physics 2D` 中配置碰撞矩阵：
   - Player ✓ WorldA
   - Player ✓ WorldB
   - Player ✓ Hazard

### 步骤3：创建玩家预制体

1. 新建对象 `Player`，Tag 设为 `Player`，Layer 设为 `Player`
2. 添加组件：
   - `Rigidbody2D`（Dynamic，Freeze Rotation Z）
   - `BoxCollider2D` 或 `CapsuleCollider2D`
   - `SpriteRenderer`
   - `AudioSource`
   - `TrailRenderer`（用于冲刺拖尾）

3. GroundCheck（两种方式任选其一）：
   - 方式A：创建子对象 `GroundCheck`（在玩家脚底）并拖入 PlayerController
   - 方式B：不手动创建，`PlayerController` 会自动生成

4. 添加脚本：
   - `PlayerController`
   - `PlayerWorldSwitcher`
   - `DashEffect`

5. 配置 PlayerController：
   - Base Speed: 5
   - Jump Force: 10
   - Base Dash Speed: 15
   - Dash Duration: 0.2
   - Dash Cooldown: 1.5
   - Ground Check: 拖入 `GroundCheck`
   - Ground Check Radius: 0.2
   - Ground Layer: 选择 `WorldA`、`WorldB`
   - Jump Sound / Dash Sound: 可选

6. 配置 PlayerWorldSwitcher：
   - Switch Sound: 可选

7. 配置 DashEffect：
   - Trail: 拖入 `TrailRenderer`
   - Dash Particles: 可选

### 步骤4：创建管理器对象

1. 创建空对象 `GameManagers`
2. 添加脚本：
   - `WorldManager`（配置 WorldA_Rules / WorldB_Rules）
   - `GameManager`（PauseMenuUI 可选）
   - `RespawnManager`（配置 Player / PlayerRb、DeathSound 可选）
   - `InputManager`（配置 PlayerController / PlayerWorldSwitcher）

3. 可选：给 `GameManagers` 添加 `AudioSource`（供 RespawnManager / Hazard 音效使用）

4. RespawnManager 建议参数：
   - Fall Out Min Y: -20
   - Auto Create Out Of Bounds: 勾选
   - Out Of Bounds Size: (200, 200)

### 步骤5：设置相机

1. 选择 Main Camera
2. 添加 `CameraController`
   - Target: Player
   - Smooth Speed: 0.125
   - Offset: (0, 2, -10)

3. 添加 `WorldVisualController`
   - Main Camera: Main Camera
   - Player: Player
   - World A Color: #FFB3D9
   - World B Color: #D4A5FF
   - Transition Duration: 0.2
   - Switch Effect: 可选

### 步骤6：创建平台预制体

#### PlatformA（粉色平台）
1. 创建精灵对象 `PlatformA`，Layer 设为 `WorldA`
2. 添加 `BoxCollider2D`
3. 添加 `WorldSpecificObject`（World Belonging: WorldA）
4. 添加 `WorldPlatformVisuals`（自动设置颜色）
5. 保存为预制体

#### PlatformB（紫色平台）
1. 创建精灵对象 `PlatformB`，Layer 设为 `WorldB`
2. 添加 `BoxCollider2D`
3. 添加 `WorldSpecificObject`（World Belonging: WorldB）
4. 添加 `WorldPlatformVisuals`
5. 保存为预制体

#### PlatformBoth（白色平台）
1. 创建精灵对象 `PlatformBoth`
2. 添加 `BoxCollider2D`
3. 添加 `WorldSpecificObject`（World Belonging: Both）
4. 添加 `WorldPlatformVisuals`
5. 保存为预制体

### 步骤7：创建障碍物预制体

1. 创建精灵对象 `Hazard`，Layer 设为 `Hazard`
2. 添加 `BoxCollider2D` 或 `CircleCollider2D`
   - Is Trigger: ✓
3. 添加 `Hazard` 脚本
4. 可选：添加 `AudioSource` + Hit Sound
5. 保存为预制体

### 步骤8：创建检查点预制体

1. 创建精灵对象 `Checkpoint`
2. 添加 `BoxCollider2D`
   - Is Trigger: ✓
3. 添加 `Checkpoint` 脚本
4. 可选：添加 `AudioSource` + Activation Sound
5. 可选：添加 `ParticleSystem` 作为激活特效
6. 保存为预制体

### 步骤9：创建 UI Canvas

1. 创建 Canvas (`UI → Canvas`)
2. Canvas Scaler 设为 `Scale With Screen Size`

#### 世界指示器
1. 创建 `Image`，命名 `WorldIndicator`
2. 位置：左上角
3. 添加 `WorldIndicatorUI` 脚本

#### 冲刺冷却显示
1. 创建 `Image`，命名 `DashCooldown`
2. Image Type: Filled
3. Fill Method: Radial 360
4. 添加 `DashCooldownUI` 脚本
5. 可选：子对象 `Text` 用于显示百分比

#### 调试信息
1. 创建 `Text`，命名 `DebugInfo`
2. 位置：右上角
3. 添加 `DebugInfoUI` 脚本

### 步骤10：创建测试场景

1. 新建场景 `TutorialLevel`
2. 放入 `GameManagers`
3. 放入 `Player`
4. 放入 Main Camera（带 CameraController / WorldVisualController）
5. 使用平台预制体搭建简单关卡
6. 放入 `Checkpoint` 与 `Hazard`
7. 运行并测试全部机制

---

## 输入系统（可选：切换到新输入系统）

如需使用 Unity 新输入系统：

1. `Package Manager` 安装 **Input System**
2. `Edit → Project Settings → Player → Active Input Handling` 设为 **Both**
3. 创建 `PlayerInputActions.inputactions`：
   - Action Map: Player
   - Movement (Value/Axis, Composite): A/D
   - Jump (Button): Space
   - SwitchWorld (Button): Mouse Left
   - Dash (Button): Mouse Right
4. 在 Player 上添加 `PlayerInput` 组件并绑定该 InputActions
5. 打开 `InputManager.cs`，启用新输入系统相关代码（取消注释）

注意：`GameManager.cs` 与 `RespawnManager.cs` 仍在使用旧输入系统，如需完全切换请同步修改。

---

## 控制测试清单

- [ ] A/D 左右移动
- [ ] 空格跳跃
- [ ] 鼠标左键切换世界
- [ ] 背景颜色平滑过渡（粉色 ↔ 紫色）
- [ ] 平台显示/碰撞随世界切换
- [ ] 世界B移速降低、重力降低
- [ ] 鼠标右键冲刺
- [ ] 冲刺冷却生效
- [ ] 空中冲刺一次，落地重置
- [ ] 碰到障碍物死亡
- [ ] 触发检查点后重生位置更新
- [ ] R 键快速重生
- [ ] ESC 暂停/继续
- [ ] UI 显示当前世界与冲刺冷却

---

## 常见问题解决

### 问题1：玩家穿过平台
- 检查层与 Physics2D 碰撞矩阵
- 确认平台 Collider2D 启用
- 确认 Player Rigidbody2D 为 Dynamic

### 问题2：世界切换不工作
- 检查 WorldManager 是否配置了 WorldA_Rules / WorldB_Rules
- 确认平台上有 `WorldSpecificObject`
- 查看 Console 是否有错误

### 问题3：玩家不跳跃
- 检查 Ground Layer 设置
- GroundCheck 位置是否正确
- 确认 `PlayerController` 的 GroundCheck 已绑定或已自动创建

### 问题4：冲刺没有效果
- 检查 Dash 参数与冷却
- 确认 `DashEffect` / `TrailRenderer` 引用
- 查看 Console 是否有错误

### 问题5：UI 不显示
- 确保 Canvas 和 EventSystem 存在
- 检查 UI 脚本引用
- 确认 Text / Image 组件存在

### 问题6：按键无反应
- 检查 Active Input Handling 是否为 Old 或 Both
- 如果启用了新输入系统，确认已配置 InputActions 并启用脚本代码

---

## 推荐的开发顺序

1. ✅ 基础移动
2. ✅ 世界切换
3. ✅ 平台显示切换
4. ✅ 视觉过渡
5. ✅ 冲刺系统
6. ✅ 重生系统
7. ✅ UI 系统
8. 📝 关卡设计
9. 📝 音效与特效
10. 📝 优化与测试

---

## 下一步建议

1. 先测试最小场景：玩家、平台、世界切换
2. 再添加冲刺与重生
3. 调整参数以匹配手感
4. 开始关卡设计与美术替换

---

所有脚本文件已就绪，可以开始在 Unity 中配置与测试。