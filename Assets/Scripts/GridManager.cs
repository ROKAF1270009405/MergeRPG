using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [Header("설정")]
    public GameObject slotPrefab;
    public RectTransform gridParent; // GridZone 패널

    [Header("그리드 규격")]
    public int columns = 9;
    public int rows = 7;
    public float cellSize = 100f;
    public float spacing = 10f;

    public List<RectTransform> allSlots = new List<RectTransform>();

    void Start()
    {
        GenerateRhombusGrid();
    }

    // 인스펙터에서 값을 바꿀 때마다 실시간으로 확인하고 싶다면 이 기능을 쓰세요!
    // void OnValidate() { if(gridParent != null) GenerateRhombusGrid(); }

    public void GenerateRhombusGrid()
    {
        // 1. 기존 슬롯 청소
        for (int i = gridParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(gridParent.GetChild(i).gameObject);
        }
        allSlots.Clear();

        // 2. 전체 그리드의 실제 가로/세로 크기 계산
        float totalGridWidth = (columns * cellSize) + ((columns - 1) * spacing);
        float totalGridHeight = (rows * cellSize) + ((rows - 1) * spacing);

        // 3. 시작 위치(좌측 상단) 계산: 패널 중심(0,0)에서 전체 크기의 절반만큼 이동
        // 슬롯의 피벗이 중앙(0.5, 0.5)일 때를 가정한 계산입니다.
        float startX = (-totalGridWidth / 2f) + (cellSize / 2f);
        float startY = (totalGridHeight / 2f) - (cellSize / 2f);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                if (IsCorner(c, r)) continue;

                GameObject newSlot = Instantiate(slotPrefab, gridParent);
                RectTransform rect = newSlot.GetComponent<RectTransform>();

                // 슬롯 크기 결정
                rect.sizeDelta = new Vector2(cellSize, cellSize);

                // 위치 결정
                float posX = startX + (c * (cellSize + spacing));
                float posY = startY - (r * (cellSize + spacing));

                rect.anchoredPosition = new Vector2(posX, posY);
                allSlots.Add(rect);
            }
        }
    }

    bool IsCorner(int c, int r)
    {
        int cornerSize = 2;
        bool topLeft = (c + r) < cornerSize;
        bool topRight = ((columns - 1 - c) + r) < cornerSize;
        bool bottomLeft = (c + (rows - 1 - r)) < cornerSize;
        bool bottomRight = ((columns - 1 - c) + (rows - 1 - r)) < cornerSize;

        return topLeft || topRight || bottomLeft || bottomRight;
    }
}