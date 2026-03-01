using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private static readonly int AnimatorStateParam = Animator.StringToHash("State");

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

    [Header("攀爬参数")]
    [SerializeField] private float maxClimbDistance = 3.5f;
    [SerializeField] private float climbSpeedStart = 4.5f;
    [SerializeField] private float slideSpeedStart = 1.0f;
    [SerializeField] private float slideSpeedMax = 2.5f;
    [SerializeField] private float climbDistanceEpsilon = 0.05f;
    [SerializeField] private float wallCheckDistance = 0.1f;
    [SerializeField] private float wallCheckHeight = 1.0f;
    [SerializeField] private float wallJumpHorizontalSpeed = 4.0f;
    [SerializeField] private float wallJumpVerticalMultiplier = 1.4f;
    [SerializeField] private float wallLockDuration = 1.0f;
    
    [Header("组件引用")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private DashEffect dashEffect;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip dashSound;

    [Header("动画参数")]
    [SerializeField] private bool useSpriteSequenceAnimation = true;
    [SerializeField] private bool useBuiltInInputFallback = true;
    [SerializeField] private string idleFramesFolder = "Animations/Clips/cat idling";
    [SerializeField] private string runningFramesFolder = "Animations/Clips/cat running";
    [SerializeField] private string jumpingFramesFolder = "Animations/Clips/cat jumping";
    [SerializeField] private float spriteSequenceFps = 10f;
    [SerializeField] private Vector2 spritePivot = new Vector2(0.5f, 0.08f);
    [SerializeField] private float spritePixelsPerUnit = 100f;
    [SerializeField] private bool trimTransparentBounds = true;
    [SerializeField] private byte alphaThreshold = 8;
    [SerializeField] private bool forceBottomPivot = true;
    [SerializeField] private int bottomCropPixels = 10;
    [SerializeField] private float runningInputThreshold = 0.1f;
    [SerializeField] private float runningVelocityThreshold = 0.05f;
    [SerializeField] private string idleAnimationStateName = "idle animation";
    [SerializeField] private string runningAnimationStateName = "running animation";
    [SerializeField] private string jumpingAnimationStateName = "jumping animation";
    [SerializeField] private bool enableAnimationDebugLogs;
    
    // 状态变量
    private float movementInput;
    private float currentSpeedMultiplier = 1f;
    private bool isGrounded;
    private bool wasGrounded;
    private bool loggedGroundCheckMissing;
    private string lastLocomotionAnimationStateName;
    private Sprite[] idleFrames;
    private Sprite[] runningFrames;
    private Sprite[] jumpingFrames;
    private PlayerState lastSpriteAnimationState;
    private int spriteFrameIndex;
    private float spriteFrameTimer;
    private bool loggedMissingIdleFrames;
    private bool loggedMissingRunningFrames;
    private bool loggedMissingJumpingFrames;
    private PlayerState lastLoggedAnimationState;
    private bool animatorStateParameterChecked;
    private bool animatorHasStateParameter;
    private bool loggedMissingAnimatorStateParameter;
    
    // 冲刺状态
    private bool isDashing;
    private float dashTimer;
    private float dashCooldownTimer;
    private bool airDashUsed;

    // 攀爬状态
    private bool isWallClinging;
    private float wallAttachY;
    private float lastClingY;
    private float climbDistance;
    private float currentSlideSpeed;
    private int wallDirection;
    private Collider2D currentWallCollider;
    private Collider2D lastWallCollider;
    private Collider2D lockedWallCollider;
    private float wallLockTimer;
    
    // 状态枚举
    public enum PlayerState { Idle, Running, Jumping, Falling, Dashing, Climbing }
    private PlayerState currentState;

    void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        LoadSpriteSequenceFramesIfNeeded();

        ResolveGroundCheck();
        ResolveGroundLayer();
        AdjustGroundCheckPosition();
    }

    void Update()
    {
        UpdateMovementInputFallback();

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

        UpdateWallState();

        if (isWallClinging)
        {
            isGrounded = false;
        }
        
        // 落地时重置空中冲刺
        if (isGrounded && !wasGrounded)
        {
            airDashUsed = false;
            ClearWallLock();
        }

        if (wallLockTimer > 0f)
        {
            wallLockTimer -= Time.deltaTime;
            if (wallLockTimer <= 0f)
            {
                ClearWallLock();
            }
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
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            if (isWallClinging)
            {
                ApplyWallClimbMovement();
            }
            else
            {
                // 应用移动
                float targetSpeed = movementInput * baseSpeed * currentSpeedMultiplier;
                rb.velocity = new Vector2(targetSpeed, rb.velocity.y);
            }
            
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

    private void UpdateMovementInputFallback()
    {
        if (!useBuiltInInputFallback)
        {
            return;
        }

        float rawAxis = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(rawAxis) > 0.01f)
        {
            movementInput = rawAxis;
            return;
        }

        if (Keyboard.current != null)
        {
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            {
                movementInput = 1f;
                return;
            }

            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            {
                movementInput = -1f;
                return;
            }
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            movementInput = 1f;
            return;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            movementInput = -1f;
            return;
        }

        movementInput = 0f;
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
        if (isWallClinging && !isDashing)
        {
            if (IsLockedWall(currentWallCollider))
            {
                return;
            }

            float jumpScale = Mathf.Clamp01(1f - (climbDistance / maxClimbDistance));
            if (jumpScale <= 0f)
            {
                return;
            }

            float verticalSpeed = jumpForce * wallJumpVerticalMultiplier * jumpScale;
            rb.velocity = new Vector2(-wallDirection * wallJumpHorizontalSpeed, verticalSpeed);
            ExitWallCling(true);
            if (audioSource && jumpSound)
            {
                audioSource.PlayOneShot(jumpSound);
            }
            return;
        }

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
        float horizontalVelocity = rb != null ? Mathf.Abs(rb.velocity.x) : 0f;
        bool hasRunningInput = Mathf.Abs(movementInput) > runningInputThreshold;
        bool hasHorizontalVelocity = horizontalVelocity > runningVelocityThreshold;

        if (isDashing)
        {
            currentState = PlayerState.Dashing;
        }
        else if (isWallClinging)
        {
            currentState = PlayerState.Climbing;
        }
        else if (!isGrounded)
        {
            currentState = rb.velocity.y > 0 ? PlayerState.Jumping : PlayerState.Falling;
        }
        else if (hasRunningInput || hasHorizontalVelocity)
        {
            currentState = PlayerState.Running;
        }
        else
        {
            currentState = PlayerState.Idle;
        }
    }

    private void UpdateAnimation()
    {
        UpdateAnimatorStateParameter();

        if (enableAnimationDebugLogs && lastLoggedAnimationState != currentState)
        {
            lastLoggedAnimationState = currentState;
            Debug.Log($"Animation State => {currentState}", this);
        }

        if (TryUpdateSpriteSequenceAnimation())
        {
            return;
        }

        string targetAnimationStateName = GetLocomotionAnimationStateName(currentState);
        if (string.IsNullOrEmpty(targetAnimationStateName) || targetAnimationStateName == lastLocomotionAnimationStateName)
        {
            return;
        }

        lastLocomotionAnimationStateName = targetAnimationStateName;
        if (animator == null)
        {
            return;
        }

        animator.Play(targetAnimationStateName);
    }

    private void UpdateAnimatorStateParameter()
    {
        if (animator == null)
        {
            return;
        }

        if (!animatorStateParameterChecked)
        {
            animatorStateParameterChecked = true;
            AnimatorControllerParameter[] parameters = animator.parameters;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].type == AnimatorControllerParameterType.Int && parameters[i].nameHash == AnimatorStateParam)
                {
                    animatorHasStateParameter = true;
                    break;
                }
            }
        }

        if (!animatorHasStateParameter)
        {
            if (!loggedMissingAnimatorStateParameter)
            {
                loggedMissingAnimatorStateParameter = true;
                Debug.LogWarning("Animator 缺少 int 参数 'State'，已跳过 SetInteger。请执行 Tools > Player > Force Setup Animator。", this);
            }
            return;
        }

        animator.SetInteger(AnimatorStateParam, GetAnimatorStateCode(currentState));
    }

    private static int GetAnimatorStateCode(PlayerState state)
    {
        if (state == PlayerState.Running)
        {
            return 1;
        }

        if (state == PlayerState.Jumping || state == PlayerState.Falling)
        {
            return 2;
        }

        return 0;
    }

    private string GetLocomotionAnimationStateName(PlayerState state)
    {
        if (state == PlayerState.Jumping || state == PlayerState.Falling)
        {
            return jumpingAnimationStateName;
        }

        if (state == PlayerState.Running)
        {
            return runningAnimationStateName;
        }

        return idleAnimationStateName;
    }

    private void LoadSpriteSequenceFramesIfNeeded()
    {
        if (!useSpriteSequenceAnimation)
        {
            return;
        }

        if (idleFrames == null || idleFrames.Length == 0)
        {
            idleFrames = LoadSpritesFromAssetsFolder(idleFramesFolder);
            if ((idleFrames == null || idleFrames.Length == 0) && !loggedMissingIdleFrames)
            {
                loggedMissingIdleFrames = true;
                Debug.LogWarning($"Idle 序列帧加载失败，目录：Assets/{idleFramesFolder}", this);
            }
        }

        if (runningFrames == null || runningFrames.Length == 0)
        {
            runningFrames = LoadSpritesFromAssetsFolder(runningFramesFolder);
            if ((runningFrames == null || runningFrames.Length == 0) && !loggedMissingRunningFrames)
            {
                loggedMissingRunningFrames = true;
                Debug.LogWarning($"Running 序列帧加载失败，目录：Assets/{runningFramesFolder}", this);
            }
        }

        if (jumpingFrames == null || jumpingFrames.Length == 0)
        {
            jumpingFrames = LoadSpritesFromAssetsFolder(jumpingFramesFolder);
            if ((jumpingFrames == null || jumpingFrames.Length == 0) && !loggedMissingJumpingFrames)
            {
                loggedMissingJumpingFrames = true;
                Debug.LogWarning($"Jumping 序列帧加载失败，目录：Assets/{jumpingFramesFolder}", this);
            }
        }
    }

    private bool TryUpdateSpriteSequenceAnimation()
    {
        if (!useSpriteSequenceAnimation || spriteRenderer == null)
        {
            return false;
        }

        Sprite[] frames = GetFramesForState(currentState);
        if (frames == null || frames.Length == 0)
        {
            return false;
        }

        if (lastSpriteAnimationState != currentState)
        {
            lastSpriteAnimationState = currentState;
            spriteFrameIndex = 0;
            spriteFrameTimer = 0f;
            spriteRenderer.sprite = frames[spriteFrameIndex];
            if (enableAnimationDebugLogs)
            {
                Debug.Log($"Sprite Sequence => {currentState}, frames={frames.Length}", this);
            }
            return true;
        }

        float frameDuration = 1f / Mathf.Max(1f, spriteSequenceFps);
        spriteFrameTimer += Time.deltaTime;

        while (spriteFrameTimer >= frameDuration)
        {
            spriteFrameTimer -= frameDuration;
            spriteFrameIndex = (spriteFrameIndex + 1) % frames.Length;
            spriteRenderer.sprite = frames[spriteFrameIndex];
        }

        return true;
    }

    private Sprite[] GetFramesForState(PlayerState state)
    {
        if (state == PlayerState.Jumping || state == PlayerState.Falling)
        {
            return jumpingFrames;
        }

        if (state == PlayerState.Running)
        {
            return runningFrames;
        }

        return idleFrames;
    }

    private Sprite[] LoadSpritesFromAssetsFolder(string assetsRelativeFolder)
    {
        if (string.IsNullOrWhiteSpace(assetsRelativeFolder))
        {
            return null;
        }

        string fullFolder = Path.Combine(Application.dataPath, assetsRelativeFolder.Replace('/', Path.DirectorySeparatorChar));
        if (!Directory.Exists(fullFolder))
        {
            return null;
        }

        string[] files = Directory.GetFiles(fullFolder, "*.png", SearchOption.TopDirectoryOnly);
        if (files.Length == 0)
        {
            return null;
        }

        List<string> orderedFiles = new List<string>(files);
        orderedFiles.Sort((a, b) => CompareFrameFileNames(Path.GetFileNameWithoutExtension(a), Path.GetFileNameWithoutExtension(b)));

        List<Sprite> sprites = new List<Sprite>(orderedFiles.Count);
        foreach (string file in orderedFiles)
        {
            byte[] bytes = File.ReadAllBytes(file);
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!texture.LoadImage(bytes))
            {
                continue;
            }

            texture.name = Path.GetFileNameWithoutExtension(file);
            Vector2 clampedPivot = new Vector2(Mathf.Clamp01(spritePivot.x), Mathf.Clamp01(spritePivot.y));
            if (forceBottomPivot)
            {
                clampedPivot.y = 0f;
            }

            float ppu = Mathf.Max(1f, spritePixelsPerUnit);
            Rect spriteRect = trimTransparentBounds ? CalculateOpaqueRect(texture, alphaThreshold) : new Rect(0f, 0f, texture.width, texture.height);
            if (bottomCropPixels > 0)
            {
                float maxCrop = Mathf.Max(0f, spriteRect.height - 1f);
                float crop = Mathf.Clamp(bottomCropPixels, 0f, maxCrop);
                spriteRect.y += crop;
                spriteRect.height -= crop;
            }

            Sprite sprite = Sprite.Create(texture, spriteRect, clampedPivot, ppu);
            sprite.name = texture.name;
            sprites.Add(sprite);
        }

        return sprites.ToArray();
    }

    private static int CompareFrameFileNames(string nameA, string nameB)
    {
        int numberA = ExtractLeadingNumber(nameA);
        int numberB = ExtractLeadingNumber(nameB);
        if (numberA != numberB)
        {
            return numberA.CompareTo(numberB);
        }

        return string.Compare(nameA, nameB, System.StringComparison.OrdinalIgnoreCase);
    }

    private static int ExtractLeadingNumber(string fileName)
    {
        int value = 0;
        bool hasDigit = false;
        for (int i = 0; i < fileName.Length; i++)
        {
            if (!char.IsDigit(fileName[i]))
            {
                if (hasDigit)
                {
                    break;
                }

                continue;
            }

            hasDigit = true;
            value = (value * 10) + (fileName[i] - '0');
        }

        return hasDigit ? value : int.MaxValue;
    }

    private static Rect CalculateOpaqueRect(Texture2D texture, byte threshold)
    {
        Color32[] pixels = texture.GetPixels32();
        int width = texture.width;
        int height = texture.height;

        int minX = width;
        int minY = height;
        int maxX = -1;
        int maxY = -1;

        for (int y = 0; y < height; y++)
        {
            int rowOffset = y * width;
            for (int x = 0; x < width; x++)
            {
                if (pixels[rowOffset + x].a <= threshold)
                {
                    continue;
                }

                if (x < minX) minX = x;
                if (y < minY) minY = y;
                if (x > maxX) maxX = x;
                if (y > maxY) maxY = y;
            }
        }

        if (maxX < minX || maxY < minY)
        {
            return new Rect(0f, 0f, width, height);
        }

        return new Rect(minX, minY, (maxX - minX) + 1, (maxY - minY) + 1);
    }

    // 用于调试的可视化
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        DrawWallCheckGizmos();
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.6f);
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        DrawWallCheckGizmos();
    }

    private void UpdateWallState()
    {
        if (isDashing)
        {
            ExitWallCling(true);
            return;
        }

        if (isGrounded)
        {
            ExitWallCling(true);
            return;
        }

        if (!TryGetWallCandidate(out int detectedWallDirection, out Collider2D detectedWallCollider))
        {
            ExitWallCling(true);
            return;
        }

        if (lockedWallCollider != null && detectedWallCollider != lockedWallCollider)
        {
            ClearWallLock();
        }

        if (IsLockedWall(detectedWallCollider))
        {
            ExitWallCling(true);
            return;
        }

        if (Mathf.Abs(movementInput) < 0.1f || Mathf.Sign(movementInput) != detectedWallDirection)
        {
            ExitWallCling(true);
            return;
        }

        if (!isWallClinging || wallDirection != detectedWallDirection)
        {
            EnterWallCling(detectedWallDirection, detectedWallCollider);
        }

        float currentY = rb.position.y;
        if (currentY > lastClingY)
        {
            float deltaY = currentY - lastClingY;
            climbDistance = Mathf.Clamp(climbDistance + deltaY, 0f, maxClimbDistance);
        }

        lastClingY = currentY;
    }

    private void EnterWallCling(int direction, Collider2D wallCollider)
    {
        isWallClinging = true;
        wallDirection = direction;
        currentWallCollider = wallCollider;
        lastWallCollider = wallCollider;
        wallAttachY = rb.position.y;
        lastClingY = wallAttachY;
        climbDistance = 0f;
        currentSlideSpeed = slideSpeedStart;
    }

    private void ExitWallCling(bool lockWall = false)
    {
        if (isWallClinging && lockWall)
        {
            Collider2D colliderToLock = currentWallCollider != null ? currentWallCollider : lastWallCollider;
            if (colliderToLock != null)
            {
                lockedWallCollider = colliderToLock;
                wallLockTimer = wallLockDuration;
            }
        }

        isWallClinging = false;
        wallDirection = 0;
        currentWallCollider = null;
        lastClingY = rb.position.y;
    }

    private void ApplyWallClimbMovement()
    {
        float effectiveMax = Mathf.Max(0.01f, maxClimbDistance);
        float epsilon = Mathf.Clamp(climbDistanceEpsilon, 0f, effectiveMax);
        if (climbDistance >= effectiveMax - epsilon)
        {
            climbDistance = effectiveMax;
        }

        float climbRatio = Mathf.Clamp01(climbDistance / effectiveMax);
        float climbSpeed = Mathf.Lerp(climbSpeedStart, 0f, climbRatio);

        if (climbSpeed > 0f)
        {
            currentSlideSpeed = slideSpeedStart;
            rb.velocity = new Vector2(0f, climbSpeed);
        }
        else
        {
            currentSlideSpeed = Mathf.MoveTowards(currentSlideSpeed, slideSpeedMax, (slideSpeedMax - slideSpeedStart) * Time.fixedDeltaTime);
            rb.velocity = new Vector2(0f, -currentSlideSpeed);
        }
    }

    private bool TryGetWallCandidate(out int direction, out Collider2D wallCollider)
    {
        bool right = IsWallAtDirection(1, out Collider2D rightCollider);
        bool left = IsWallAtDirection(-1, out Collider2D leftCollider);

        bool rightUnlocked = right && !IsLockedWall(rightCollider);
        bool leftUnlocked = left && !IsLockedWall(leftCollider);

        if (rightUnlocked)
        {
            direction = 1;
            wallCollider = rightCollider;
            return true;
        }

        if (leftUnlocked)
        {
            direction = -1;
            wallCollider = leftCollider;
            return true;
        }

        if (lockedWallCollider != null)
        {
            if (right && rightCollider != lockedWallCollider)
            {
                ClearWallLock();
                direction = 1;
                wallCollider = rightCollider;
                return true;
            }

            if (left && leftCollider != lockedWallCollider)
            {
                ClearWallLock();
                direction = -1;
                wallCollider = leftCollider;
                return true;
            }
        }

        direction = 0;
        wallCollider = null;
        return false;
    }

    private bool IsWallAtDirection(int direction, out Collider2D wallCollider)
    {
        Vector2 position = rb != null ? rb.position : (Vector2)transform.position;
        float effectiveWallCheckDistance = GetEffectiveWallCheckDistance();
        Vector2 originUpper = position + Vector2.up * (wallCheckHeight * 0.5f);
        Vector2 originLower = position + Vector2.down * (wallCheckHeight * 0.5f);
        Vector2 dir = new Vector2(direction, 0f);

        RaycastHit2D hitUpper = Physics2D.Raycast(originUpper, dir, effectiveWallCheckDistance, groundLayer);
        RaycastHit2D hitLower = Physics2D.Raycast(originLower, dir, effectiveWallCheckDistance, groundLayer);

        if (hitUpper.collider != null && !hitUpper.collider.isTrigger)
        {
            wallCollider = hitUpper.collider;
            return true;
        }

        if (hitLower.collider != null && !hitLower.collider.isTrigger)
        {
            wallCollider = hitLower.collider;
            return true;
        }

        wallCollider = null;
        return false;
    }

    private bool IsLockedWall(Collider2D wallCollider)
    {
        return wallCollider != null && wallCollider == lockedWallCollider;
    }

    private void ClearWallLock()
    {
        lockedWallCollider = null;
        wallLockTimer = 0f;
    }

    private float GetEffectiveWallCheckDistance()
    {
        float distance = wallCheckDistance;
        Collider2D collider2D = GetComponent<Collider2D>();
        if (collider2D != null)
        {
            float halfWidth = collider2D.bounds.extents.x;
            float autoDistance = halfWidth + 0.05f;
            distance = Mathf.Max(distance, autoDistance);
        }

        return Mathf.Max(distance, 0.01f);
    }

    private void DrawWallCheckGizmos()
    {
        Vector2 position = rb != null ? rb.position : (Vector2)transform.position;
        Vector2 originUpper = position + Vector2.up * (wallCheckHeight * 0.5f);
        Vector2 originLower = position + Vector2.down * (wallCheckHeight * 0.5f);

        Gizmos.color = new Color(0f, 1f, 1f, 0.6f);
        Gizmos.DrawLine(originUpper, originUpper + Vector2.right * wallCheckDistance);
        Gizmos.DrawLine(originUpper, originUpper + Vector2.left * wallCheckDistance);
        Gizmos.DrawLine(originLower, originLower + Vector2.right * wallCheckDistance);
        Gizmos.DrawLine(originLower, originLower + Vector2.left * wallCheckDistance);
    }

    public bool IsGrounded => isGrounded;
    public bool IsDashing => isDashing;
    public bool IsWallClinging => isWallClinging;
    public Collider2D CurrentWallCollider => currentWallCollider;
    public PlayerState CurrentState => currentState;
}
