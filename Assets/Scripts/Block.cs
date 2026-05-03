using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour
{
    public int level = 1; // 현재 블록 레벨
    public Vector2 blockPosition = Vector2.zero;
    
    // 레벨이 오를 때 호출할 함수 (나중에 이미지 변경 등을 위해)
    public void SetLevel(int newLevel)
    {
        level = newLevel;
        // 여기서 레벨에 따라 이미지를 바꾸는 로직을 넣으면 좋습니다!
        // GetComponent<Image>().sprite = 레벨별이미지;
    }
    public void SetPosition(Vector2 newPosition)
    {
        blockPosition = newPosition;
    }
}