using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WayPointTerrain
{
    public List<PathNode> NodeList;
    public GameObject[] Walls;
    float NeiborCubeFixdist = 0.9f;

    public void Init()
    {
        Walls = GameObject.FindGameObjectsWithTag(AStarConst.WallStr);
        NodeList = new List<PathNode>();
        LoadWP();
    }

    public void ClearAStarInfo()
    {
        foreach (PathNode node in NodeList)
        {
            node.Parent = null;
            node.fTurtle = node.fStone = node.fGold = 0.0f;
            node.NodeState = PathNodeState.NODE_NONE;
        }
    }

    public PathNode GetNodeFromPosition(Vector3 pos)
    {
        PathNode retNode = null, node;
        int iCnt = NodeList.Count;
        float maxCost = 100000.0f;
        for (int i = 0; i < iCnt; i++)
        {
            node = NodeList[i];
            if (Physics.Linecast(pos, node.Pos, 1 << LayerMask.NameToLayer(AStarConst.WallStr))) continue;
            Vector3 vRay = node.Pos - pos;
            vRay.y = 0.0f; // Optional
            float mag = vRay.magnitude;
            if (mag < maxCost)
            {
                maxCost = mag;
                retNode = node;
            }
        }
        return retNode;
    }

    public void LoadWP()
    {
        StreamReader sr = new(SaveWP.SavePath);
        if (sr == null) return;

        string sAll = sr.ReadToEnd();
        string[] sLines = sAll.Split('\n');
        int iLine = sLines.Length, iLineIndex = 0, iNodeCount = 0;
        //Get Node Number
        while (iLineIndex < iLine)
        {
            string s = sLines[iLineIndex];
            iLineIndex++;
            string[] sArr = s.Split(' ');
            if (sArr.Length == 0) continue;
            if (sArr[0] == SaveWP.NodeNumberStr)
            {
                iNodeCount = int.Parse(sArr[1]);
                break;
            }
        }
        //Init NodeList
        NodeList.Clear();
        for (int i = 0; i < iNodeCount; i++)
        {
            PathNode node = new()
            {
                bLink = false,
                Neibors = new List<PathNode>(),
                Parent = null,
                Pos = Vector3.zero,
            };
            NodeList.Add(node);
        }

        for (int i = 0; i < iNodeCount; i++)
        {
            PathNode currentNode = NodeList[i];
            string s = sLines[iLineIndex];
            iLineIndex++;
            string[] sArr = s.Split(' ');
            currentNode.Pos = GameUtility.StringToVector3(sArr[(int)SaveWP.WPColumn.Position]);
            if (sArr[(int)SaveWP.WPColumn.BLink].Equals(bool.TrueString))
                currentNode.bLink = true;
            int _NeiCnt = int.Parse(sArr[(int)SaveWP.WPColumn.NeiborsCnt]);
            int _NeiStartIndex = (int)SaveWP.WPColumn.NeiborsIndex;
            for (int j = 0; j < _NeiCnt; j++)
            {
                int _NeiId = int.Parse(sArr[_NeiStartIndex + j]);
                currentNode.Neibors.Add(NodeList[_NeiId]);
            }
        }

        for (int i = 0; i < NodeList.Count; i++)
            Debug.Log($"{NodeList[i].Pos}  {NodeList[i].Neibors.Count}");
    }

    public void DrawPathNodes()
    {
        PathNode currentNode, neiborNode;
        for (int i = 0; i < NodeList.Count; i++)
        {
            currentNode = NodeList[i];
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(currentNode.Pos, 0.3f);

            for (int j = 0; j < currentNode.Neibors.Count; j++)
            {
                neiborNode = NodeList[i].Neibors[j];
                Vector3 vRay = neiborNode.Pos - currentNode.Pos;
                float fDist = vRay.magnitude;
                vRay.Normalize();
                Gizmos.color = Color.green;
                Gizmos.DrawLine(currentNode.Pos, neiborNode.Pos);
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(currentNode.Pos + vRay * fDist * NeiborCubeFixdist, Vector3.one * 0.3f);
            }
        }
    }

}
