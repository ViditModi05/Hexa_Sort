using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class StackSpawner : MonoBehaviour
{
    [Header(" Elements ")]   
    [SerializeField] private Transform stackPositionParent;
    [SerializeField] private Hexagon hexagonPrefab;
    [SerializeField] private HexStack hexagonStackPrefab;

    [Header(" Settings ")]
    [NaughtyAttributes.MinMaxSlider(2,8)]
    [SerializeField] private Vector2Int minmaxHexcount;
    [SerializeField] private Color[] colors;
    private int stackCounter;
    void Start()
    {
        GenerateStacks();
        
    }

    private void GenerateStacks()
    {
        for(int i = 0; i < stackPositionParent.childCount; i++)
        GenerateStack(stackPositionParent.GetChild(i));
    }

    private void GenerateStack(Transform parent) 
    {
        HexStack hexStack = Instantiate(hexagonStackPrefab, parent.position, Quaternion.identity, parent);
        hexStack.name = $"Stack { parent.GetSiblingIndex()}";
      
        int amount = Random.Range(minmaxHexcount.x, minmaxHexcount.y);

        int firstColorHexagonCount = Random.Range(0 , amount);

        Color[] colorArray = GetRandomColors();

        for(int i= 0; i < amount; i++)
        {
            Vector3 hexagonLocalPos = Vector3.up * i * .2f;
            Vector3 spawnPosition = hexStack.transform.TransformPoint(hexagonLocalPos);
            Hexagon hexagonInstance = Instantiate(hexagonPrefab, spawnPosition, Quaternion.identity, hexStack.transform);
            hexagonInstance.Color = i < firstColorHexagonCount ? colorArray[0]: colorArray[1];

            hexagonInstance.Configure(hexStack);

            hexStack.Add(hexagonInstance);
        }

    }
 
    private Color[] GetRandomColors()
    {
        List<Color> colorlist = new List<Color>();
        colorlist.AddRange(colors);

        if(colorlist.Count <= 0)
        {
            Debug.LogError("No color found");
            return null;
        }

        Color firstColor = colorlist.OrderBy(x => Random.value).First();
        colorlist.Remove(firstColor);

        if(colorlist.Count <= 0)
        {
            Debug.LogError("Only one color was found");
            return null;
        }
        
         Color secondColor = colorlist.OrderBy(x => Random.value).First();

         return new Color[] { firstColor, secondColor };

    }

    private void Awake()
    {
        Application.targetFrameRate = 60;

        StackController.onStackPlaced += StackPlacedCallback;
    }

    private void OnDestroy()
    {
        StackController.onStackPlaced -= StackPlacedCallback;

    }

    private void StackPlacedCallback(GridCell gridCell)
    {
        stackCounter++;
        if(stackCounter >= 3)
        {
            stackCounter = 0;
            GenerateStacks();
        }
        
    }
}