using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class SaveWP : MonoBehaviour
{
    public const string SavePath = @"Assets/WP.txt", NodeNumberStr = "NodeNumber:";
    public enum WPColumn
    {
        Position, BLink, NeiborsCnt, NeiborsIndex
    }

    void Start()
    {
        IEnumerable<GameObject> gos = GameObject.FindGameObjectsWithTag("WP").OrderBy(x => x.name);
        StreamWriter sw = new(SavePath, false);
        int nNode = gos.Count();
        sw.WriteLine($"{NodeNumberStr} {nNode}");
        for (int i = 0; i < nNode; i++)
        {
            StringBuilder builder = new();
            builder.Append($"{gos.ElementAt(i).transform.position} ".Replace(", ", ","));
            if (gos.ElementAt(i).TryGetComponent(out BaseWayPoint wp))
            {
                builder.Append($"{wp.bLink} {wp.Neibors.Count} ");
                var goList = gos.ToList();
                var query = goList.Where(g => wp.Neibors.Contains(g)).OrderBy(x => x.name);
                foreach (var g in query) builder.Append($"{goList.IndexOf(g)} ");
            }
            sw.WriteLine(builder);
        }
        sw.Close();
    }
}
