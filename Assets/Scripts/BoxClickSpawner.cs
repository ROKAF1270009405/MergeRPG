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

        bool success = gridManager.TrySpawnBlock(blockPrefab);
        if (success)
        {
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