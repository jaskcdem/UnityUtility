using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AStar
{
    WayPointTerrain Terrain;
    List<PathNode> OpenList;
    Stack<Vector3> PathList;
    static public AStar Instance;

    public void Init(WayPointTerrain terrain)
    {
        Terrain = terrain;
        OpenList = new List<PathNode>();
        PathList = new Stack<Vector3>();
        Instance = this;
    }

    public Stack<Vector3> GetPath() => PathList;

    private PathNode GetBestNode()
    {
        PathNode rtn = null;
        float fMaxCost = 10000.0f;
        foreach (PathNode n in OpenList)
        {
            if (n.fTurtle < fMaxCost)
            {
                fMaxCost = n.fTurtle;
                rtn = n;
            }
        }
        OpenList.Remove(rtn);
        return rtn;
    }

    private void BuildPath(Vector3 startPos, Vector3 endPos, PathNode startNode, PathNode endNode)
    {
        PathList.Clear();
        PathList.Push(endPos);
        if (startNode == endNode) PathList.Push(startNode.Pos);
        else
        {
            PathNode pathNode = endNode;
            while (pathNode != null)
            {
                PathList.Push(pathNode.Pos);
                pathNode = pathNode.Parent;
            }
        }
        PathList.Push(startPos);
    }

    public bool PerformAStar(Vector3 startPos, Vector3 endPos)
    {
        PathNode sNode = Terrain.GetNodeFromPosition(startPos), eNode = Terrain.GetNodeFromPosition(endPos);
        if (sNode == null || eNode == null)
        {
            Debug.LogWarning("Can not find node in AStar map");
            return false;
        }
        else if (sNode == eNode)
        {
            // Build Path.
            BuildPath(startPos, endPos, sNode, eNode);
            return true;
        }

        OpenList.Clear();
        Terrain.ClearAStarInfo();
        PathNode neiborNode, currentNode;
        OpenList.Add(sNode);
        while (OpenList.Count > 0)
        {
            currentNode = GetBestNode();
            if (currentNode == null)
            {
                Debug.LogAssertion("Get Best Node error.");
                return false;
            }
            else if (currentNode == eNode)
            {
                // Build Path.
                BuildPath(startPos, endPos, sNode, eNode);
                return true;
            }

            int _NeiCnt = currentNode.Neibors.Count;
            Debug.Log(currentNode.Pos + " : " + _NeiCnt);
            for (int i = 0; i < _NeiCnt; i++)
            {
                neiborNode = currentNode.Neibors[i];
                switch (neiborNode.NodeState)
                {
                    case PathNodeState.NODE_CLOSED:
                        continue;
                    case PathNodeState.NODE_OPENED:
                        {
                            float fNewS = currentNode.fStone + (currentNode.Pos - neiborNode.Pos).magnitude;
                            if (fNewS < neiborNode.fStone)
                            {
                                neiborNode.fStone = fNewS;
                                neiborNode.fTurtle = neiborNode.fStone + neiborNode.fGold;
                                neiborNode.Parent = currentNode;
                            }
                            continue;
                        }
                }
                neiborNode.NodeState = PathNodeState.NODE_OPENED;
                neiborNode.fStone = currentNode.fStone + (currentNode.Pos - neiborNode.Pos).magnitude;
                neiborNode.fGold = (eNode.Pos - neiborNode.Pos).magnitude;
                neiborNode.fTurtle = neiborNode.fStone + neiborNode.fGold;
                neiborNode.Parent = currentNode;
                OpenList.Add(neiborNode);
                Debug.Log("Add open node");
            }

            currentNode.NodeState = PathNodeState.NODE_CLOSED;
        }

        return false;
    }
}
