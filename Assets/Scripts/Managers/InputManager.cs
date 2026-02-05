using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [Header("玩家引用")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerWorldSwitcher worldSwitcher;
    
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction switchWorldAction;
    private InputAction dashAction;

    private void Awake()
    {
        // 如果没有手动设置，尝试自动获取
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }
        
        if (worldSwitcher == null)
        {
            worldSwitcher = FindObjectOfType<PlayerWorldSwitcher>();
        }
    }

    private void Start()
    {
        // 注意：这个脚本需要 PlayerInputActions.inputactions 文件
        // 如果您使用的是传统输入系统，请使用下面的 Update 方法
    }

    private void Update()
    {
        // 使用传统输入系统（临时方案，建议后续切换到新输入系统）
        
        // 移动输入 (A/D)
        float horizontal = 0f;
        if (Input.GetKey(KeyCode.D))
            horizontal = 1f;
        else if (Input.GetKey(KeyCode.A))
            horizontal = -1f;
        
        if (playerController != null)
        {
            playerController.SetMovementInput(horizontal);
        }
        
        // 跳跃输入 (空格)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (playerController != null)
            {
                playerController.Jump();
            }
        }
        
        // 世界切换 (鼠标左键)
        if (Input.GetMouseButtonDown(0))
        {
            if (worldSwitcher != null)
            {
                worldSwitcher.SwitchWorld();
            }
        }
        
        // 冲刺 (鼠标右键)
        if (Input.GetMouseButtonDown(1))
        {
            if (playerController != null)
            {
                playerController.Dash();
            }
        }
    }

    // 以下方法用于新输入系统（需要配置 PlayerInputActions）
    // 取消注释以使用新输入系统
    
    /*
    private void OnEnable()
    {
        if (playerInput == null)
            return;
            
        moveAction = playerInput.actions["Movement"];
        jumpAction = playerInput.actions["Jump"];
        switchWorldAction = playerInput.actions["SwitchWorld"];
        dashAction = playerInput.actions["Dash"];
        
        jumpAction.performed += OnJump;
        switchWorldAction.performed += OnSwitchWorld;
        dashAction.performed += OnDash;
    }

    private void OnDisable()
    {
        if (playerInput == null)
            return;
            
        jumpAction.performed -= OnJump;
        switchWorldAction.performed -= OnSwitchWorld;
        dashAction.performed -= OnDash;
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (playerController != null)
        {
            playerController.Jump();
        }
    }

    private void OnSwitchWorld(InputAction.CallbackContext context)
    {
        if (worldSwitcher != null)
        {
            worldSwitcher.SwitchWorld();
        }
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        if (playerController != null)
        {
            playerController.Dash();
        }
    }
    */
}
