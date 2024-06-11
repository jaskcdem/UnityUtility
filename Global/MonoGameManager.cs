using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MonoGameManager : MonoSingleton<MonoGameManager>
{
    private List<MapObstacle> Obstacles;
    public GameObject Player; public const string PlayerTag = "Player";
    [Tooltip("camera collide check layer")] public const string CameraLayerName = "Wall";
    [Tooltip("camera collide player layer")] public const string PlayerLayerName = "Player";
    [Tooltip("camera collide mob layer")] public const string MobLayerName = "Mob";
    [Tooltip("skills collide check layer")] public const string SlopeLayerName = "Slope";
    [Tooltip("camera collide check radius")] public const float CameraRadius = 0.1f;

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

    /// <summary> <see cref="Physics.SphereCast"/></summary>
    public static bool CheckCameraCollide((Vector3 original, Vector3 direction) vecCam, out RaycastHit hitInfo, float maxDistance)
        => Physics.SphereCast(vecCam.original, CameraRadius, vecCam.direction, out hitInfo, maxDistance, LayerMask.NameToLayer(CameraLayerName));
}
