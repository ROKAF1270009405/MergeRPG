using UnityEngine;
using UnityEngine.EventSystems;

//드래그 로직
//블럭에 부여되는 드래그 기능
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
        transform.SetParent(canvas.transform);
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
        Block myBlock = GetComponent<Block>();
        Slot currentSlot = myBlock.currentSlot;
        
        if (target != null)
        {
            Slot targetSlot = target.GetComponentInParent<Slot>();

            if (targetSlot != null)
            {
                if (targetSlot.isFull)
                {
                    //머지 성공
                    Block targetBlock = targetSlot.storedItem.GetComponent<Block>();
                    
                    if (targetBlock != null && targetBlock.level == myBlock.level && targetBlock != myBlock)
                    {
                        targetBlock.SetLevel(targetBlock.level + 1);
                        if (currentSlot != null)
                        {
                            currentSlot.SetItem(null);
                            Destroy(this.gameObject);
                            Debug.Log("합치기! 레벨 업!");
                            return;
                        }
                    }
                }
                else
                {
                    //이동
                    if(currentSlot != null) currentSlot.SetItem(null);
                    
                    targetSlot.SetItem(this.gameObject);
                    this.transform.SetParent(targetSlot.transform);
                    rectTransform.anchoredPosition = Vector2.zero;
                    return;
                }
            }
        }
        transform.SetParent(currentSlot.transform);
        ReturnToSlot();
    }

    void ReturnToSlot()
    {
        rectTransform.anchoredPosition = Vector2.zero;
        
        // 일단은 제자리에 가만히 있게 하거나, 
        // 나중에 Slot 좌표를 저장해뒀다가 거기로 튕겨 돌아가게 만듭니다.
    }
}