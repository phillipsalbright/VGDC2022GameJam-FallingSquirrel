using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Vector2 movementInput = new Vector2(0, 0);
    private Rigidbody2D rb;
    private enum PlayerState { gliding, normal, diving }
    private PlayerState currentState = PlayerState.normal;
    private Coroutine cr = null;
    [SerializeField] private float jumpForce = 400f;
    private Vector2 velocityRef = Vector3.zero;
    private float smoothDamp = .05f;
    [SerializeField] private float normalGravityMultiplier = -9f;
    [SerializeField] private float glidingGravityMultiplier = -4f;
    [SerializeField] private float divingGravityMultiplier = -14f;
    [SerializeField] private float normalMoveSpeed = 5f;
    [SerializeField] private float glidingMoveSpeed = 7f;
    [SerializeField] private float divingMoveSpeed = 2f;
    private float currentSpeed;
    private float gravity;
    private SpriteRenderer s;
    private bool grounded = false;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask whatIsGround;
    private bool jumpPressed;
    private int playerNum;
    private bool inSap;
    private bool stunned;
    private float timeStunned = 0;
    private Coroutine flashingAnim = null;
    

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody2D>();
        gravity = normalGravityMultiplier;
        currentSpeed = normalMoveSpeed;
        s = this.GetComponent<SpriteRenderer>();
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        movementInput.x = ctx.ReadValue<Vector2>().x;
    }

    public void OnGlide(InputAction.CallbackContext ctx)
    {
        Debug.Log(ctx.action.triggered);
        if (ctx.action.triggered)
        {
            if (cr != null)
            {
                StopCoroutine(cr);
            }
            cr = StartCoroutine(ChangeState(PlayerState.gliding));
        } else if (currentState == PlayerState.gliding)
        {
            if (cr != null)
            {
                StopCoroutine(cr);
            }
            StartCoroutine(ChangeState(PlayerState.normal));
        }
    }

    public void OnDive(InputAction.CallbackContext ctx)
    {
        if (ctx.action.triggered)
        {
            if (cr != null)
            {
                StopCoroutine(cr);
            }
            cr = StartCoroutine(ChangeState(PlayerState.diving));
        } else if (currentState == PlayerState.diving)
        {
            if (cr != null)
            {
                StopCoroutine(cr);
            }
            StartCoroutine(ChangeState(PlayerState.normal));
        }
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.action.triggered)
        {
            jumpPressed = true;
        }
    }

    private IEnumerator ChangeState(PlayerState state)
    {
        if (grounded || stunned)
        {
            state = PlayerState.normal;
        }
        //set animator stuff
        currentState = state;
        yield return new WaitForSeconds(.5f);
        switch (state)
        {
            case PlayerState.diving:
                gravity = divingGravityMultiplier;
                currentSpeed = divingMoveSpeed;
                s.color = new Color(1, 0, 0);
                break;
            case PlayerState.gliding:
                gravity = glidingGravityMultiplier;
                currentSpeed = glidingMoveSpeed;
                s.color = new Color(0, 0, 1);
                break;
            case PlayerState.normal:
                gravity = normalGravityMultiplier;
                currentSpeed = normalMoveSpeed;
                s.color = new Color(1, 1, 1);

                break;
        }
        Debug.Log(state);
    }

    private void FixedUpdate()
    {
        Vector2 targetVelocity = new Vector2(movementInput.x * currentSpeed, rb.velocity.y);
        if (inSap)
        {
            targetVelocity = targetVelocity / 2;
        }
        if (stunned)
        {
            targetVelocity = targetVelocity / 2;
        }
        rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref velocityRef, smoothDamp);
        Vector2 targetGravityVel = new Vector2(rb.velocity.x, gravity);
        rb.velocity = Vector2.SmoothDamp(rb.velocity, targetGravityVel, ref velocityRef, .1f);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, .15f, whatIsGround);
        bool isGrounded = false;
        //Debug.Log(colliders.Length);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != this.gameObject)
            {
                if (!grounded)
                {
                    if (cr != null)
                    {
                        StopCoroutine(cr);
                    }
                    cr = StartCoroutine(ChangeState(PlayerState.normal));

                }
                isGrounded = true;
                break;
            }
        }
        grounded = isGrounded;
        if (jumpPressed && grounded && !inSap && currentState == PlayerState.normal)
        {
            jumpPressed = false;
            rb.velocity = new Vector2(rb.velocity.x, 20);
        } else if (jumpPressed)
        {
            jumpPressed = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.layer)
        {
            case 6:
                FindObjectOfType<GameManager>().CupCollected(playerNum);
                break;
            case 8:
                inSap = true;
                break;
            case 9:
                timeStunned = 0;
                rb.velocity = new Vector2(rb.velocity.x, 130);
                stunned = true;
                if (flashingAnim == null)
                {
                    flashingAnim = StartCoroutine(FlashingAnim());
                }
                ChangeState(PlayerState.normal);
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            inSap = false;
        }
    }

    public void SetPlayerNum(int playerNum)
    {
        this.playerNum = playerNum;
    }

    IEnumerator FlashingAnim()
    {
        Color c1 = new Color(1, 1, 1, 1);
        Color c2 = new Color(1, 1, 1, .2f);
        while (timeStunned < 2)
        {
            s.color = c2;
            yield return new WaitForSeconds(.08f);
            timeStunned += .08f;
            s.color = c1;
            yield return new WaitForSeconds(.08f);
            timeStunned += .08f;
        }
        s.color = c1;
        stunned = false;
    }
}