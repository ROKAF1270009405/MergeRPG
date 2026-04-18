using UnityEngine;
using UnityEngine.EventSystems;

public class BlockDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        // CanvasGroup이 없으면 자동으로 추가해줌
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f; // 잡았을 때 살짝 투명하게
        canvasGroup.blocksRaycasts = false; // 중요! 드래그 중엔 마우스가 이 물체를 통과하게 함
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 마우스의 이동량(delta)만큼 상자를 이동시킴 (Canvas 스케일에 맞춰서)
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        GameObject target = eventData.pointerCurrentRaycast.gameObject;

        if (target != null)
        {
            Block targetBlock = target.GetComponentInParent<Block>();
            // 드래그 중인 나(this)에게서도 Block 컴포넌트를 가져와야 레벨을 알 수 있습니다!
            Block myBlock = GetComponent<Block>();

            if (targetBlock != null && targetBlock != myBlock && myBlock != null)
            {
                // 두 블록의 레벨이 같은지 비교
                if (targetBlock.level == myBlock.level)
                {
                    MergeWith(targetBlock);
                    return;
                }
            }
        }

        ReturnToSlot();
    }

    void MergeWith(Block target)
    {
        Debug.Log("합치기! 레벨 업!");
        target.SetLevel(target.level + 1); // 타겟 레벨 상승
        Destroy(this.gameObject); // 드래그하던 블록은 삭제
    }

    void ReturnToSlot()
    {
        // 일단은 제자리에 가만히 있게 하거나, 
        // 나중에 Slot 좌표를 저장해뒀다가 거기로 튕겨 돌아가게 만듭니다.
    }
}