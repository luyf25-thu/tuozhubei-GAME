using System.Text;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("移动参数")]
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    
    [Header("冲刺参数")]
    [SerializeField] private float baseDashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1.5f;
    
    [Header("地面检测")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool enableGroundDebug;
    
    [Header("组件引用")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private DashEffect dashEffect;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip dashSound;
    
    // 状态变量
    private float movementInput;
    private float currentSpeedMultiplier = 1f;
    private bool isGrounded;
    private bool wasGrounded;
    private bool loggedGroundCheckMissing;
    
    // 冲刺状态
    private bool isDashing;
    private float dashTimer;
    private float dashCooldownTimer;
    private bool airDashUsed;
    
    // 状态枚举
    public enum PlayerState { Idle, Running, Jumping, Falling, Dashing }
    private PlayerState currentState;

    void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        ResolveGroundCheck();
        ResolveGroundLayer();
        AdjustGroundCheckPosition();
    }

    void Update()
    {
        // 地面检测
        wasGrounded = isGrounded;
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

            if (enableGroundDebug && isGrounded != wasGrounded)
            {
                Collider2D[] hits = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius, groundLayer);
                StringBuilder log = new StringBuilder();
                log.AppendLine($"Grounded change: {wasGrounded} -> {isGrounded}");
                log.AppendLine($"GroundCheck pos={groundCheck.position} radius={groundCheckRadius} layerMask={groundLayer.value}");

                if (hits.Length == 0)
                {
                    log.AppendLine("Hits: (none)");
                }
                else
                {
                    log.AppendLine("Hits:");
                    foreach (Collider2D hit in hits)
                    {
                        string layerName = LayerMask.LayerToName(hit.gameObject.layer);
                        log.AppendLine($"- {hit.name} (layer {layerName})");
                    }
                }

                Debug.Log(log.ToString(), this);
            }
        }
        else
        {
            isGrounded = false;
            if (!loggedGroundCheckMissing)
            {
                loggedGroundCheckMissing = true;
                Debug.LogWarning("GroundCheck 未设置，无法检测落地状态。请在 PlayerController 上设置 GroundCheck。", this);
            }
        }
        
        // 落地时重置空中冲刺
        if (isGrounded && !wasGrounded)
        {
            airDashUsed = false;
        }
        
        // 更新冲刺计时器
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
        
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0)
            {
                isDashing = false;
            }
        }
        
        // 更新状态
        UpdateState();
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            // 应用移动
            float targetSpeed = movementInput * baseSpeed * currentSpeedMultiplier;
            rb.velocity = new Vector2(targetSpeed, rb.velocity.y);
            
            // 翻转角色
            if (movementInput > 0.1f)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (movementInput < -0.1f)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }

    public void SetMovementInput(float input)
    {
        movementInput = input;
    }

    private void ResolveGroundCheck()
    {
        if (groundCheck != null)
            return;

        Transform found = transform.Find("GroundCheck");
        if (found != null)
        {
            groundCheck = found;
            return;
        }

        GameObject groundCheckObject = new GameObject("GroundCheck");
        groundCheckObject.transform.SetParent(transform);
        groundCheckObject.transform.localPosition = new Vector3(0f, -0.5f, 0f);
        groundCheck = groundCheckObject.transform;
    }

    [ContextMenu("Recreate GroundCheck")]
    private void RecreateGroundCheck()
    {
        if (groundCheck != null)
        {
            if (Application.isPlaying)
            {
                Destroy(groundCheck.gameObject);
            }
            else
            {
                DestroyImmediate(groundCheck.gameObject);
            }
        }

        GameObject groundCheckObject = new GameObject("GroundCheck");
        groundCheckObject.transform.SetParent(transform);
        groundCheckObject.transform.localPosition = new Vector3(0f, -0.5f, 0f);
        groundCheck = groundCheckObject.transform;
        AdjustGroundCheckPosition();
    }

    private void AdjustGroundCheckPosition()
    {
        if (groundCheck == null)
            return;

        Collider2D collider2D = GetComponent<Collider2D>();
        if (collider2D == null)
            return;

        float bottomWorldY = collider2D.bounds.min.y;
        Vector3 localPosition = groundCheck.localPosition;
        float localBottomY = transform.InverseTransformPoint(new Vector3(0f, bottomWorldY, 0f)).y;

        groundCheck.localPosition = new Vector3(localPosition.x, localBottomY - (groundCheckRadius * 0.5f), localPosition.z);
    }

    private void ResolveGroundLayer()
    {
        if (groundLayer != 0)
            return;

        groundLayer = Physics2D.GetLayerCollisionMask(gameObject.layer);
    }

    public void Jump()
    {
        if (isGrounded && !isDashing)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            
            if (audioSource && jumpSound)
            {
                audioSource.PlayOneShot(jumpSound);
            }
        }
    }

    public void Dash()
    {
        // 检查冷却
        if (dashCooldownTimer > 0)
            return;
        
        // 检查空中冲刺
        if (!isGrounded && airDashUsed)
            return;
        
        // 执行冲刺
        Vector2 dashDirection = GetDashDirection();
        rb.velocity = dashDirection * baseDashSpeed * currentSpeedMultiplier;
        
        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;
        
        if (!isGrounded)
        {
            airDashUsed = true;
        }
        
        // 播放音效和特效
        if (audioSource && dashSound)
        {
            audioSource.PlayOneShot(dashSound);
        }
        
        if (dashEffect)
        {
            dashEffect.PlayDashEffect();
        }
    }

    private Vector2 GetDashDirection()
    {
        // 优先使用当前移动输入方向
        if (Mathf.Abs(movementInput) > 0.1f)
        {
            return new Vector2(Mathf.Sign(movementInput), 0);
        }
        else
        {
            // 使用玩家朝向
            return new Vector2(transform.localScale.x, 0);
        }
    }

    public void UpdateSpeedMultiplier(float multiplier)
    {
        currentSpeedMultiplier = multiplier;
    }

    public float GetDashCooldownPercent()
    {
        if (dashCooldownTimer <= 0)
            return 1f;
        
        return 1f - (dashCooldownTimer / dashCooldown);
    }

    private void UpdateState()
    {
        if (isDashing)
        {
            currentState = PlayerState.Dashing;
        }
        else if (!isGrounded)
        {
            currentState = rb.velocity.y > 0 ? PlayerState.Jumping : PlayerState.Falling;
        }
        else if (Mathf.Abs(movementInput) > 0.1f)
        {
            currentState = PlayerState.Running;
        }
        else
        {
            currentState = PlayerState.Idle;
        }
    }

    // 用于调试的可视化
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.6f);
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    public bool IsGrounded => isGrounded;
    public bool IsDashing => isDashing;
    public PlayerState CurrentState => currentState;
}
