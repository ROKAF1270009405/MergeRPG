using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BoxClickSpawner : MonoBehaviour
{
    public GameObject blockPrefab;
    public Transform blockParent;
    public Slider cooltimeSlider;
    public GridManager gridManager; // [추가] 그리드 매니저 연결용

    [Header("쿨타임 설정")]
    public int maxClickCount = 3;
    public float cooltimeDuration = 10f;

    private int currentClickCount = 0;
    private bool isCoolingDown = false;

    void Start()
    {
        if (cooltimeSlider != null)
            cooltimeSlider.gameObject.SetActive(false);

        // 만약 인스펙터에서 연결 안 했다면 자동으로 찾아보기
        if (gridManager == null)
            gridManager = FindFirstObjectByType<GridManager>();
    }

    // 버튼 클릭 시 호출될 함수 (public 확인!)
    public void SpawnBlock()
    {
        if (isCoolingDown) return;

        // 1. 그리드에서 빈 슬롯 찾기
        RectTransform emptySlot = GetEmptySlot();

        if (emptySlot != null)
        {
            // 2. 상자 소환 (변수 선언은 여기서 한 번만!)
            GameObject newBlock = Instantiate(blockPrefab, blockParent);

            // 3. 위치 및 크기 설정 (변수 선언 중복 방지)
            RectTransform blockRect = newBlock.GetComponent<RectTransform>();
            blockRect.position = emptySlot.position;
            blockRect.localScale = Vector3.one;

            // UI 레이어상 맨 앞으로 보내기
            newBlock.transform.SetAsLastSibling();
            newBlock.name = "Block_Level_1";

            // 4. 슬롯 상태 업데이트
            Slot slotScript = emptySlot.GetComponent<Slot>();
            if (slotScript != null)
            {
                slotScript.isFull = true;
                slotScript.storedItem = newBlock;
            }

            // 5. 클릭 횟수 및 쿨타임 관리
            currentClickCount++;
            if (currentClickCount >= maxClickCount)
            {
                StartCoroutine(StartCooldown());
            }
        }
        else
        {
            Debug.Log("인벤토리가 가득 찼습니다!");
        }
    }



    // 빈 슬롯을 찾는 로직
    RectTransform GetEmptySlot()
    {
        foreach (RectTransform slotRect in gridManager.allSlots)
        {
            Slot slotScript = slotRect.GetComponent<Slot>();
            if (slotScript != null && !slotScript.isFull)
            {
                return slotRect;
            }
        }
        return null; // 빈 슬롯 없음
    }

    IEnumerator StartCooldown()
    {
        isCoolingDown = true;
        currentClickCount = 0;

        if (cooltimeSlider != null)
        {
            cooltimeSlider.gameObject.SetActive(true);
            cooltimeSlider.maxValue = cooltimeDuration;
            float timer = 0;
            while (timer < cooltimeDuration)
            {
                timer += Time.deltaTime;
                cooltimeSlider.value = timer;
                yield return null;
            }
            cooltimeSlider.gameObject.SetActive(false);
        }
        isCoolingDown = false;
    }
}