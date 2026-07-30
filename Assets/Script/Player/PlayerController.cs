using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 5f;
    public bool canMove = true; // 이동 허용 여부

    [Header("채집 & 타격 설정 (3x3 사각형 영역)")]
    [Tooltip("3x3 사각형 가로/세로 크기 (1타일이 1이면 3.0 정도가 3x3 영역)")]
    public Vector2 areaSize = new Vector2(3f, 3f);
    public int attackDamage = 1;
    public LayerMask resourceLayer;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;
    private Vector2 lastLookDirection = Vector2.down;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        canMove = true; // 게임 시작 시 무조건 이동 가능하도록 보장
    }

    void FixedUpdate()
    {
        // canMove가 false면 플레이어 이동을 멈춤
        if (!canMove)
        {
            rb.linearVelocity = Vector2.zero; // Unity 6 (2022 이하면 rb.velocity = Vector2.zero)
            return;
        }

        rb.linearVelocity = movement * moveSpeed;
    }

    // 키보드/패드 이동 입력
    public void OnMove(InputValue value)
    {
        if (!canMove)
        {
            movement = Vector2.zero;
            UpdateAnimator(lastLookDirection, 0f);
            return;
        }

        movement = value.Get<Vector2>();

        if (movement.magnitude > 0.01f)
        {
            lastLookDirection = Get4WayDirection(movement);
            UpdateAnimator(lastLookDirection, movement.magnitude);
        }
        else
        {
            UpdateAnimator(lastLookDirection, 0f);
        }
    }

    void Update()
    {
        // 마우스 좌클릭 시 (canMove 여부 상관없이 마우스 입력은 받고 내부에서 처리)
        if (Input.GetMouseButtonDown(0))
        {
            // 제작창이 켜져 있을 때(!canMove)는 땅이나 자원을 때리지 않음
            if (!canMove) return;

            HandleTouchOrClick();
        }
    }

    // 마우스/터치 선택 처리
    private void HandleTouchOrClick()
    {
        if (Camera.main == null) return;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 targetPos = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
        Vector2 playerPos = (Vector2)transform.position;

        Bounds attackBounds = new Bounds(playerPos, areaSize);

        if (attackBounds.Contains(targetPos))
        {
            Vector2 direction = targetPos - playerPos;
            if (direction.magnitude > 0.1f)
            {
                lastLookDirection = Get4WayDirection(direction);
                UpdateAnimator(lastLookDirection, 0f);
            }

            Collider2D hit = Physics2D.OverlapPoint(targetPos, resourceLayer);
            if (hit != null)
            {
                Resource resource = hit.GetComponent<Resource>();
                if (resource != null)
                {
                    resource.TakeDamage(attackDamage);
                }
            }
        }
    }

    private Vector2 Get4WayDirection(Vector2 dir)
    {
        dir.Normalize();
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            return dir.x > 0 ? Vector2.right : Vector2.left;
        }
        else
        {
            return dir.y > 0 ? Vector2.up : Vector2.down;
        }
    }

    private void UpdateAnimator(Vector2 dir, float speed)
    {
        if (animator != null)
        {
            animator.SetFloat("InputX", dir.x);
            animator.SetFloat("InputY", dir.y);
            animator.SetFloat("Speed", speed);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, areaSize);
    }
}