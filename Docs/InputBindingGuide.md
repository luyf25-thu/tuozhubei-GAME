# 输入绑定指南（截图占位）

本指南说明如何在 Unity 编辑器中将 `PlayerInput` 的 Action 绑定到本项目的脚本（`InputManager`、`PlayerController`、`PlayerWorldSwitcher`）。每一步后建议截一张图保存为文档证据（占位位置已标注）。

---

## 前提

- 已安装 Unity 新输入系统包（Package Manager）。
- 场景中有挂载以下脚本的对象：`InputManager`、`PlayerController`、`PlayerWorldSwitcher`、`RespawnManager`、`GameManager`。
- 已创建并关联好 `.inputactions` 资源（包含 Actions: Movement, Jump, Dash, SwitchWorld, QuickRespawn, Pause）。

---

## 步骤（逐步截图占位）

1. 选择含 `PlayerInput` 的 GameObject（例如 `Player`）。

   - 操作：在 Hierarchy 选中目标。
   - 截图占位：`screenshots/01_PlayerInput_Inspector.png`

2. 在 Inspector 的 `PlayerInput` 组件中，将 `Behavior` 设置为 `Invoke Unity Events`。

   - 操作：展开 `PlayerInput` -> `Behavior` 下拉 -> 选择 `Invoke Unity Events`。
   - 截图占位：`screenshots/02_Behavior_InvokeUnityEvents.png`

3. 绑定 `Movement`：

   - 操作：在 `PlayerInput` 的 Actions 列表中展开 `Movement`，在 `Performed` 点击 `+` 添加监听器，拖入包含 `InputManager` 的对象，函数选择 `InputManager.OnMove(InputAction.CallbackContext)`。
   - 建议：同时在 `Canceled` 也绑定 `OnMove`（以便松开时传入 0）。
   - 截图占位：`screenshots/03_Bind_Movement.png`

4. 绑定按键动作（Jump / Dash / SwitchWorld）：

   - `Jump` (Space) -> 绑定到 `PlayerController.Jump()`（对象：Player 或 PlayerController 所在物体）。
   - `Dash` (Mouse Right) -> 绑定到 `PlayerController.Dash()`。
   - `SwitchWorld` (Mouse Left) -> 绑定到 `PlayerWorldSwitcher.SwitchWorld()`。
   - 截图占位：`screenshots/04_Bind_Jump_Dash_Switch.png`

5. 绑定快速重生与暂停：

   - `QuickRespawn` (R) -> 绑定到 `InputManager.QuickRespawn()`。
   - `Pause` (Esc) -> 绑定 to `InputManager.TogglePause()`。
   - 截图占位：`screenshots/05_Bind_QuickRespawn_Pause.png`

6. 检查 `InputManager` Inspector 引用：

   - 操作：选中 `InputManager` 所在 GameObject，确认 `playerController`、`worldSwitcher` 等字段已正确拖入。
   - 截图占位：`screenshots/06_InputManager_Refs.png`

7. 检查 `InputActions` 资源设置：

   - `Movement` 类型建议为 `Value` 并返回 `Vector2` 或 `Axis`/`Float`（若为单轴）。
   - `Jump/Dash/SwitchWorld/QuickRespawn/Pause` 类型应为 `Button`。
   - 截图占位：`screenshots/07_InputActions_Resource.png`

8. 运行并测试（Play 模式）：

   - 测试項：A/D 移动、Space 跳跃、鼠标左切换世界、鼠标右冲刺、R 快速重生、Esc 暂停。
   - 若某项無响应：停止 Play，檢查對應 Action 是否指向正確物件與方法，或檢查 `InputManager` 引用是否为空。
   - 截圖占位（运行中）：`screenshots/08_Play_Test.png`

---

## 故障排查快速清單

- 方法不可選：確認方法為 `public` 或簽名兼容 UnityEvent（已在 `InputManager` 中提供 `OnMove(InputAction.CallbackContext)` 等）。
- Movement 值異常：檢查 `Movement` 的綁定是否為 `Vector2`，並確認 `OnMove` 能正確讀取值。
- R/ESC 無响应：確認 `QuickRespawn`/`Pause` Action 已綁定到 `InputManager.QuickRespawn` / `InputManager.TogglePause`。
- 腳本引用為 null：在 Inspector 中將 `PlayerController`、`PlayerWorldSwitcher` 拖入 `InputManager` 對應欄位。

---

## 文件與位置

- 本指南：`Docs/InputBindingGuide.md`
- 关联文档：`readme.md`、`Game_Implementation_Plan.md`

---

如果你願意，我可以把上述佔位截圖模板生成在 `Docs/screenshots/` 下的空文件並提交，或者直接生成一份 PDF。你想先讓我創建佔位截圖文件夾嗎？
