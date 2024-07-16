using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class MergeManager : MonoBehaviour
{
    public Score score;
    [Header("Elements")]
    private List<GridCell> updatedCells = new List<GridCell>();
    private void Awake()
    {
        StackController.onStackPlaced += StackPlacedCallBack;
    }

      private void OnDestroy()
    {
        StackController.onStackPlaced -= StackPlacedCallBack;
        
    }

    private void StackPlacedCallBack(GridCell gridCell)
    {
        StartCoroutine(StackPlacedCoroutine(gridCell));   
    }

    IEnumerator StackPlacedCoroutine(GridCell gridCell)
    {
        updatedCells.Add(gridCell);
        
        while(updatedCells.Count > 0)
        {
            yield return CheckForMerge(updatedCells[0]);
        }
    }

    IEnumerator CheckForMerge(GridCell gridCell)
    {
        updatedCells.Remove(gridCell);

        if(!gridCell.isOccupied)
        {
            yield break;
        }

        List<GridCell> neighborGridCells = GetNeighborGridCells(gridCell); 

        if (neighborGridCells.Count <= 0)
        {
            Debug.Log("No Neighbors for this Cell");
            yield break;
        }

        Color gridCellTopHexagonColor = gridCell.Stack.GetTopHexagonColor();


        List<GridCell> similarNeighborGridCells = GetSimilarNeighborGridCells(gridCellTopHexagonColor , neighborGridCells.ToArray());

        if(similarNeighborGridCells.Count <= 0)
        {
            Debug.Log("No Neighbors for this Cell");
            yield break;
            
        }

        updatedCells.AddRange(similarNeighborGridCells);

        List<Hexagon> hexagonsToAdd = GetHexagonsToAdd(gridCellTopHexagonColor, similarNeighborGridCells.ToArray());

        RemoveHexagonsFromStacks(hexagonsToAdd , similarNeighborGridCells.ToArray());
        
        MoveHexagons(gridCell, hexagonsToAdd);

        yield return new WaitForSeconds(.2f + (hexagonsToAdd.Count + 1) * .01f);


        yield return CheckForCompleteStack(gridCell, gridCellTopHexagonColor);   

    }

    private List<GridCell> GetNeighborGridCells(GridCell gridCell)
    {

        LayerMask gridCellMask = 1 << gridCell.gameObject.layer;
        List<GridCell> neighborGridCells = new List<GridCell>();

        Collider[] neighborGridCellColliders = Physics.OverlapSphere(gridCell.transform.position, 2 , gridCellMask);

        foreach( Collider gridCellCollider in neighborGridCellColliders )
        {
            GridCell neighborGridCell = gridCellCollider.GetComponent<GridCell>();

            if(!neighborGridCell.isOccupied)
            {
                continue;
            }

            if(neighborGridCell == gridCell)
            {
                continue;
            }

            neighborGridCells.Add(neighborGridCell);
        }

        return neighborGridCells;
    }

    private List<GridCell> GetSimilarNeighborGridCells(Color gridCellTopHexagonColor, GridCell[] neighborGridCells)
    {
        List<GridCell> similarNeighborGridCells = new List<GridCell>();

        foreach(GridCell neighborGridCell in neighborGridCells)
        {
            Color neighborGridCellTopHexagonColor = neighborGridCell.Stack.GetTopHexagonColor();

            if(gridCellTopHexagonColor == neighborGridCellTopHexagonColor)
            {
                similarNeighborGridCells.Add(neighborGridCell);

            }

        }

        return similarNeighborGridCells;
    }

    private List<Hexagon> GetHexagonsToAdd(Color gridCellTopHexagonColor, GridCell[] similarNeighborGridCells)
    {
       List<Hexagon> hexagonsToAdd = new List<Hexagon>();
        foreach(GridCell neighborCell in similarNeighborGridCells)
        {
            HexStack neighborCellHexStack = neighborCell.Stack;


            for( int i = neighborCellHexStack.Hexagons.Count - 1 ; i >= 0 ; i-- )
            {
                Hexagon hexagon = neighborCellHexStack.Hexagons[i];

                if(hexagon.Color != gridCellTopHexagonColor)
                {
                    break;
                }

                hexagonsToAdd.Add(hexagon);
                hexagon.SetParent(null);
            }
        }

        return hexagonsToAdd;
    }

    private void RemoveHexagonsFromStacks(List<Hexagon> hexagonsToAdd, GridCell[] similarNeighborGridCells)
    {
          foreach(GridCell neighborCell in similarNeighborGridCells)
        {
            HexStack stack = neighborCell.Stack;

            foreach(Hexagon hexagon in hexagonsToAdd )
            {
                if(stack.Contains(hexagon))
                {
                    stack.Remove(hexagon);
                }
                
            }
        }
    }

    private void MoveHexagons(GridCell gridCell, List<Hexagon> hexagonsToAdd )
    {
        float initialY = gridCell.Stack.Hexagons.Count * .2f;

        for( int i = 0 ; i < hexagonsToAdd.Count ; i++)
        {
            Hexagon hexagon = hexagonsToAdd[i];

            float targetY = initialY + i * .2f;
            Vector3 targetLocalPosition = Vector3.up * targetY;

            gridCell.Stack.Add(hexagon);
            hexagon.MoveToLocal(targetLocalPosition);
        }
    }

    private IEnumerator CheckForCompleteStack(GridCell gridCell, Color topColor)
    {
        if(gridCell.Stack.Hexagons.Count < 10)
        {
            yield break;
        }

        List<Hexagon> similarHexagons = new List<Hexagon>();
        for( int i = gridCell.Stack.Hexagons.Count - 1 ; i >= 0 ; i-- )
        {
            Hexagon hexagon = gridCell.Stack.Hexagons[i];

            if(hexagon.Color != topColor)
            {
                break;
            }

            similarHexagons.Add(hexagon);

        }

        int similarHexagonsCount = similarHexagons.Count;

        if(similarHexagons.Count < 10)
        {
            yield break;
        }

        float delay = 0;

        while(similarHexagons.Count > 0)
        {
            similarHexagons[0].SetParent(null);
            similarHexagons[0].Vanish(delay);
            //DestroyImmediate(similarHexagons[0].gameObject);

            delay += .01f;
            gridCell.Stack.Remove(similarHexagons[0]);
            

            similarHexagons.RemoveAt(0);
        }
        score.AddScore(10);
        updatedCells.Add(gridCell);

        yield return new WaitForSeconds(.2f + (similarHexagonsCount + 1) * .01f);
        

    }
}
