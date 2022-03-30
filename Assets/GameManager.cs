using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node
{
    public Node(bool _isWall, int _x, int _y)
    {
        isWall = _isWall;
        x = _x;
        y = _y;
    }

    public bool isWall;
    public Node parentNode;

    public int x, y, G, H;
    public int F { get { return G + H; } }
}

public class GameManager : MonoBehaviour
{
    public Vector2Int bottomLeft, topRight, startPos, targetPos;
    public List<Node> naviList;

    int sizeX, sizeY;
    int[] dx = { 0,0,1,-1 };
    int[] dy = { 1, -1, 0, 0 };
    Node[,] arr;
    Node start, target, cur;

    List<Node> OpenList, ClosedLIst;//priority queue 어케 쓰는지 모르니
    //일단 리스트로 ㄱ
    //여튼 이것도 다익스트라 비스무리하게 되겠지.
    public Transform startTR;


    public void pathFinding()
    {
        startPos = Vector2Int.RoundToInt(startTR.position);

        sizeX = topRight.x - bottomLeft.x + 1;
        sizeY = topRight.y - bottomLeft.y + 1;
        arr = new Node[sizeX, sizeY];

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                bool isWall = false;
                foreach (Collider2D col in Physics2D.OverlapCircleAll(new Vector2(i + bottomLeft.x, j + bottomLeft.y), 0.4f))
                    if (col.gameObject.layer == LayerMask.NameToLayer("Wall")) isWall = true;

                arr[i, j] = new Node(isWall, i + bottomLeft.x, j + bottomLeft.y);
            }
        }

        start = arr[startPos.x - bottomLeft.x, startPos.y - bottomLeft.y];
        target = arr[targetPos.x - bottomLeft.x, targetPos.y - bottomLeft.y];

        OpenList = new List<Node>() { start };
        ClosedLIst = new List<Node>();
        naviList = new List<Node>();

        while (OpenList.Count > 0)
        {
            cur = OpenList[0];
            for (int i = 1; i < OpenList.Count; i++)
            {
                if (OpenList[i].F <= cur.F && OpenList[i].H < cur.H)
                    cur = OpenList[i];
            }
            OpenList.Remove(cur);
            ClosedLIst.Add(cur);

            if (cur == target)
            {
                Node tmp = target;
                while (tmp != start)
                {
                    naviList.Add(tmp);
                    tmp = tmp.parentNode;
                }
                naviList.Add(start);
                naviList.Reverse();
                foreach (Node i in naviList)
                {
                    Debug.Log(i.x + ", " + i.y);
                }
                return;
            }

            for (int i = 0; i < 4; i++)
            {
                int nx = cur.x + dx[i];
                int ny = cur.y + dy[i];
                Debug.Log(nx + " " + ny);
                if (nx >= bottomLeft.x && nx < topRight.x + 1 && ny >= bottomLeft.y && ny < topRight.y + 1
                    && !arr[nx - bottomLeft.x, ny - bottomLeft.y].isWall
                    && !ClosedLIst.Contains(arr[nx - bottomLeft.x, ny - bottomLeft.y]))
                {
                    if (arr[cur.x - bottomLeft.x, ny - bottomLeft.y].isWall || arr[nx - bottomLeft.x, ny - bottomLeft.y].isWall)
                        continue;

                    Node nextNode = arr[nx - bottomLeft.x, ny - bottomLeft.y];
                    int MoveCost = cur.G + 10;

                    if (MoveCost < nextNode.G || !OpenList.Contains(nextNode))
                    {
                        nextNode.G = MoveCost;
                        nextNode.H = Mathf.Abs(nextNode.x - target.x) + Mathf.Abs(nextNode.y - target.y);
                        nextNode.parentNode = cur;

                        OpenList.Add(nextNode);
                    }
                }
            }
        }
        Debug.Log("not found");
    }


    private void OnDrawGizmos()
    {
        if (naviList.Count != 0)
        {
            for (int i = 0; i < naviList.Count - 1; i++)
                Gizmos.DrawLine(new Vector2(naviList[i].x, naviList[i].y), new Vector2(naviList[i + 1].x, naviList[i + 1].y));
        }
    }
}
