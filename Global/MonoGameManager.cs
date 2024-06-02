using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MonoGameManager : MonoSingleton<MonoGameManager>
{
    private List<MapObstacle> Obstacles;
    public GameObject Player; public const string PlayerTag = "Player";

    void Start()
    {
        if (Player == null) Player = GameObject.FindGameObjectWithTag(PlayerTag);
        Obstacles = new List<MapObstacle>();
        GameObject[] gos = GameObject.FindGameObjectsWithTag(MapObstacle.TagStr);
        if (gos != null || gos.Length > 0)
        {
            //Debug.Log(gos.Length);
            foreach (GameObject go in gos)
                Obstacles.Add(go.GetComponent<MapObstacle>());
            Debug.Assert(Obstacles.Count == gos.Length, $"Do your all Obstacles having tag `{MapObstacle.TagStr}` and Component `{nameof(MapObstacle)}`?");
        }
    }

    public GameObject GetPlayer() => Player;

    public List<MapObstacle> GetObstacles() => Obstacles;
}
