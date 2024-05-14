using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PathNodeState
{
    NODE_NONE = -1,
    NODE_OPENED,
    NODE_CLOSED
};

public static class AStarConst
{
    public const string WallStr = "Wall";
    public const string GroundStr = "Ground";
}

public class PathNode
{
    public GameObject NodeGo;
    public List<PathNode> Neibors;
    public bool bLink;

    /// <summary> node's position </summary>
    public Vector3 Pos;
    /// <summary> node's parent </summary>
    public PathNode Parent;
    /// <summary> dist/cost from start node </summary>
    public float fStone;
    /// <summary> dist/cost to end node </summary>
    public float fGold;
    /// <summary> total dist/cost from <see cref="fStone"/> and <see cref="fGold"/> </summary>
    public float fTurtle;

    public PathNodeState NodeState;
}
