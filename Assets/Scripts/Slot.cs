using UnityEngine;

public class Slot : MonoBehaviour
{
    [Header("슬롯 상태")]
    public bool isFull = false;     // 현재 칸이 찼는지 여부
    public GameObject storedItem;   // 이 칸에 들어있는 아이템(블록)
    public Vector2 slotNum = Vector2.zero;
    // 나중에 아이템을 이 슬롯에 놓을 때 호출할 함수
    public void SetItem(GameObject item)
    {
        storedItem = item;
        isFull = (item != null);
    }
}