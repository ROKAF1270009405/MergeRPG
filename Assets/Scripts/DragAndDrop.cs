using UnityEngine;
using UnityEngine.InputSystem;

public class DragAndDrop : MonoBehaviour
{
    public bool isDragging = false;
    public int level = 1;
    public Color[] levelColors;

    private bool isMerging = false;
    private Vector3 offset;
    private Camera mainCamera;
    private Collider2D myCollider;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        mainCamera = Camera.main;
        myCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // [수정] 태어날 때는 Trigger를 꺼서 물리적으로 서로 밀어내게 합니다.
        if (myCollider != null)
        {
            myCollider.isTrigger = false;
        }

        UpdateVisual();
    }

    void Update()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        // 1. 클릭 감지 - 드래그 앤 드랍
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 mouseWorldPos = GetMouseWorldPosition();
            if (myCollider != null && myCollider.OverlapPoint(mouseWorldPos))
            {
                isDragging = true;
                offset = transform.position - mouseWorldPos;
                myCollider.isTrigger = true;
            }
        }
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;
            if (myCollider != null) myCollider.isTrigger = false;

            // [추가] 마우스를 놓는 순간 속도를 0으로 만들어 제자리에 딱 멈추게 함
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero; // 현재 이동 속도 제거 (Unity 6 기준)
                rb.angularVelocity = 0f;          // 회전 속도 제거
            }
        }

        // 2. 드래그 중
        if (isDragging && Mouse.current.leftButton.isPressed)
        {
            Vector3 targetPos = GetMouseWorldPosition() + offset;
            targetPos.z = 0;
            transform.position = targetPos;
        }

        // 3. 마우스 뗐을 때
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;
            if (myCollider != null) myCollider.isTrigger = false;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        float distanceToPlane = Mathf.Abs(mainCamera.transform.position.z);
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, distanceToPlane));
        worldPoint.z = 0;
        return worldPoint;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isMerging) return;

        DragAndDrop otherScript = other.GetComponent<DragAndDrop>();
        if (other.CompareTag("Block") && otherScript != null && !otherScript.isMerging)
        {
            if (otherScript.level == this.level)
            {
                if (gameObject.GetInstanceID() > other.gameObject.GetInstanceID())
                {
                    isMerging = true;
                    otherScript.isMerging = true;

                    Vector3 spawnPos = (transform.position + other.transform.position) / 2f;
                    spawnPos.z = 0;

                    // [수정된 부분] 이름 정리를 포함한 생성 로직
                    GameObject newBlock = Instantiate(gameObject, spawnPos, Quaternion.identity);

                    // (Clone) 글자를 없애고 깔끔하게 이름 짓기
                    newBlock.name = "Block_Level_" + (this.level + 1);

                    DragAndDrop newScript = newBlock.GetComponent<DragAndDrop>();
                    if (newScript != null)
                    {
                        newScript.level = this.level + 1;
                        newScript.isMerging = false;
                        newScript.isDragging = false;
                        newScript.UpdateVisual();
                    }

                    Destroy(gameObject);
                    Destroy(other.gameObject);
                }
            }
        }
    }

    public void UpdateVisual()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (levelColors != null && levelColors.Length > 0)
        {
            int colorIndex = Mathf.Clamp(level - 1, 0, levelColors.Length - 1);
            spriteRenderer.color = levelColors[colorIndex];
        }
    }
}