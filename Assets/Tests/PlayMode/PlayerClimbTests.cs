using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerClimbTests
{
    [UnityTest]
    public IEnumerator ClimbIncreasesVerticalVelocityWhileTouchingWall()
    {
        GameObject player = new GameObject("Player");
        player.transform.position = Vector3.zero;
        Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        player.AddComponent<BoxCollider2D>();
        PlayerController controller = player.AddComponent<PlayerController>();

        Transform groundCheck = new GameObject("GroundCheck").transform;
        groundCheck.SetParent(player.transform);
        groundCheck.localPosition = new Vector3(0f, -2f, 0f);

        SetPrivateField(controller, "groundCheck", groundCheck);
        SetPrivateField(controller, "groundCheckRadius", 0.05f);
        SetPrivateField(controller, "groundLayer", (LayerMask)(1 << 2));
        SetPrivateField(controller, "rb", rb);

        GameObject wall = new GameObject("Wall");
        wall.layer = 2;
        wall.transform.position = new Vector3(0.6f, 0f, 0f);
        BoxCollider2D wallCollider = wall.AddComponent<BoxCollider2D>();
        wallCollider.size = new Vector2(1f, 3f);

        yield return null;

        controller.SetMovementInput(1f);

        yield return null;

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        Assert.Greater(rb.velocity.y, 0f);
        Assert.IsTrue(controller.IsWallClinging);
    }

    [UnityTest]
    public IEnumerator ClimbStartsSlidingAfterDistanceBudget()
    {
        GameObject player = new GameObject("Player");
        player.transform.position = Vector3.zero;
        Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        player.AddComponent<BoxCollider2D>();
        PlayerController controller = player.AddComponent<PlayerController>();

        Transform groundCheck = new GameObject("GroundCheck").transform;
        groundCheck.SetParent(player.transform);
        groundCheck.localPosition = new Vector3(0f, -2f, 0f);

        SetPrivateField(controller, "groundCheck", groundCheck);
        SetPrivateField(controller, "groundCheckRadius", 0.05f);
        SetPrivateField(controller, "groundLayer", (LayerMask)(1 << 2));
        SetPrivateField(controller, "rb", rb);
        SetPrivateField(controller, "maxClimbDistance", 0.2f);
        SetPrivateField(controller, "climbSpeedStart", 5f);

        GameObject wall = new GameObject("Wall");
        wall.layer = 2;
        wall.transform.position = new Vector3(0.6f, 0f, 0f);
        BoxCollider2D wallCollider = wall.AddComponent<BoxCollider2D>();
        wallCollider.size = new Vector2(1f, 3f);

        yield return null;

        controller.SetMovementInput(1f);

        yield return null;

        for (int i = 0; i < 40; i++)
        {
            yield return new WaitForFixedUpdate();
        }

        Assert.Less(rb.velocity.y, 0f);
    }

    [UnityTest]
    public IEnumerator CannotReclimbSameWallUntilLockClears()
    {
        GameObject player = new GameObject("Player");
        player.transform.position = Vector3.zero;
        Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        player.AddComponent<BoxCollider2D>();
        PlayerController controller = player.AddComponent<PlayerController>();

        Transform groundCheck = new GameObject("GroundCheck").transform;
        groundCheck.SetParent(player.transform);
        groundCheck.localPosition = new Vector3(0f, -0.5f, 0f);

        SetPrivateField(controller, "groundCheck", groundCheck);
        SetPrivateField(controller, "groundCheckRadius", 0.05f);
        SetPrivateField(controller, "groundLayer", (LayerMask)(1 << 2));
        SetPrivateField(controller, "rb", rb);
        SetPrivateField(controller, "maxClimbDistance", 0.2f);
        SetPrivateField(controller, "climbSpeedStart", 5f);
        SetPrivateField(controller, "wallLockDuration", 2f);

        GameObject wall = new GameObject("Wall");
        wall.layer = 2;
        wall.transform.position = new Vector3(0.6f, 0f, 0f);
        BoxCollider2D wallCollider = wall.AddComponent<BoxCollider2D>();
        wallCollider.size = new Vector2(1f, 3f);

        yield return null;

        controller.SetMovementInput(1f);

        yield return null;

        yield return new WaitForFixedUpdate();

          Assert.IsTrue(controller.IsWallClinging);

        controller.Jump();

        yield return null;

        controller.SetMovementInput(1f);

        yield return null;
        yield return new WaitForFixedUpdate();

        Assert.IsFalse(controller.IsWallClinging);
    }

    [UnityTest]
    public IEnumerator CannotReclimbSameWallAfterLettingGo()
    {
        GameObject player = new GameObject("Player");
        player.transform.position = Vector3.zero;
        Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        player.AddComponent<BoxCollider2D>();
        PlayerController controller = player.AddComponent<PlayerController>();

        Transform groundCheck = new GameObject("GroundCheck").transform;
        groundCheck.SetParent(player.transform);
        groundCheck.localPosition = new Vector3(0f, -0.5f, 0f);

        SetPrivateField(controller, "groundCheck", groundCheck);
        SetPrivateField(controller, "groundCheckRadius", 0.05f);
        SetPrivateField(controller, "groundLayer", (LayerMask)(1 << 2));
        SetPrivateField(controller, "rb", rb);
        SetPrivateField(controller, "wallLockDuration", 2f);

        GameObject wall = new GameObject("Wall");
        wall.layer = 2;
        wall.transform.position = new Vector3(0.6f, 0f, 0f);
        BoxCollider2D wallCollider = wall.AddComponent<BoxCollider2D>();
        wallCollider.size = new Vector2(1f, 3f);

        yield return null;

        controller.SetMovementInput(1f);

        yield return null;
        yield return new WaitForFixedUpdate();

        Assert.IsTrue(controller.IsWallClinging);

        controller.SetMovementInput(0f);

        yield return null;
        yield return new WaitForFixedUpdate();

        controller.SetMovementInput(1f);

        yield return null;
        yield return new WaitForFixedUpdate();

        Assert.IsFalse(controller.IsWallClinging);
    }

    [UnityTest]
    public IEnumerator LockClearsWhenTouchingDifferentWall()
    {
        GameObject player = new GameObject("Player");
        player.transform.position = Vector3.zero;
        Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        player.AddComponent<BoxCollider2D>();
        PlayerController controller = player.AddComponent<PlayerController>();

        Transform groundCheck = new GameObject("GroundCheck").transform;
        groundCheck.SetParent(player.transform);
        groundCheck.localPosition = new Vector3(0f, -0.5f, 0f);

        SetPrivateField(controller, "groundCheck", groundCheck);
        SetPrivateField(controller, "groundCheckRadius", 0.05f);
        SetPrivateField(controller, "groundLayer", (LayerMask)(1 << 2));
        SetPrivateField(controller, "rb", rb);
        SetPrivateField(controller, "wallLockDuration", 2f);
        SetPrivateField(controller, "wallCheckDistance", 1.0f);

        GameObject wallA = new GameObject("WallA");
        wallA.layer = 2;
        wallA.transform.position = new Vector3(0.6f, 0f, 0f);
        BoxCollider2D wallACollider = wallA.AddComponent<BoxCollider2D>();
        wallACollider.size = new Vector2(1f, 3f);

        GameObject wallB = new GameObject("WallB");
        wallB.layer = 2;
        wallB.transform.position = new Vector3(-0.6f, 0f, 0f);
        BoxCollider2D wallBCollider = wallB.AddComponent<BoxCollider2D>();
        wallBCollider.size = new Vector2(1f, 3f);

        yield return null;

        controller.SetMovementInput(1f);

        yield return null;
        yield return new WaitForFixedUpdate();

        Assert.IsTrue(controller.IsWallClinging);

        controller.Jump();

        yield return null;

        wallACollider.enabled = false;
        wallB.transform.position = new Vector3(0.6f, 0f, 0f);
        rb.position = new Vector2(0.05f, 0f);
        rb.velocity = Vector2.zero;
        controller.SetMovementInput(1f);

        yield return null;
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        Assert.IsNotNull(controller.CurrentWallCollider);
        Assert.AreEqual(wallBCollider, controller.CurrentWallCollider);
        Assert.IsTrue(controller.IsWallClinging);
    }

    private static void SetPrivateField(object target, string fieldName, object value)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(field, $"Missing field '{fieldName}' on {target.GetType().Name}.");
        field.SetValue(target, value);
    }
}
