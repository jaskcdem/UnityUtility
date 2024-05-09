using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine;

internal class MonoResourcesLoader : MonoSingleton<MonoResourcesLoader>
{
    UnityAction<GameObject> LoadedCallBack = null;

    internal T LoadObject<T>(string path)
    {
        var obj = Resources.Load(path);
        return obj switch
        {
            T go => go,
            _ => default,
        };
    }
    internal T[] LoadAllObjects<T>(string path)
    {
        var objs = Resources.LoadAll(path);
        return objs switch
        {
            T[] gos => gos,
            _ => null,
        };
    }

    internal GameObject LoadGameObject(string path) => LoadObject<GameObject>(path);
    internal IEnumerator LoadGameObjectAsync(string path)
    {
        var result = Resources.LoadAsync(path);
        if (result == null) yield break;
        yield return result;

        LoadedCallBack?.Invoke(result.asset switch
        {
            GameObject go => go,
            _ => null,
        });
    }
    internal GameObject[] LoadAllGameObject(string path) => LoadAllObjects<GameObject>(path);

    internal IEnumerator LoadAssetBundle(string sName, bool containStand = true)
    {
        List<AssetBundle> aBundles = new();
        if (containStand)
        {
            string sNameManifest = "StandaloneWindows", sPathManifest = "file://" + Application.streamingAssetsPath + "/" + sNameManifest;
            UnityWebRequest uwrManifest = UnityWebRequestAssetBundle.GetAssetBundle(sPathManifest);
            yield return uwrManifest.SendWebRequest();
            if (uwrManifest.result is not UnityWebRequest.Result.ProtocolError and not UnityWebRequest.Result.ConnectionError)
            {
                AssetBundle aBundle = DownloadHandlerAssetBundle.GetContent(uwrManifest);
                Debug.Log(aBundle.name);
                aBundles.Add(aBundle);
            }
        }

        string sPath = "file://" + Application.streamingAssetsPath + "/" + sName;
        Debug.Log(sPath);
        UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(sPath);
        yield return uwr.SendWebRequest(); ;

        if (uwr.result is not UnityWebRequest.Result.ProtocolError and not UnityWebRequest.Result.ConnectionError)
        {
            AssetBundle aBundle = DownloadHandlerAssetBundle.GetContent(uwr);
            string[] aNames = aBundle.GetAllAssetNames();
            foreach (string s in aNames)
            {
                Debug.Log($"download : {s}");
            }
            aBundles.Add(aBundle);
        }

        foreach (AssetBundle ab in aBundles)
        {
            ab.Unload(false);
        }
    }
}
