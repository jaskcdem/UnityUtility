using System.Collections;
using UnityEngine;
using UnityEngine.Events;

internal class ResourcesLoader : Singleton<ResourcesLoader>
{
    internal ResourcesLoader() { }
    internal ResourcesLoader(UnityAction<GameObject> callback)
    {
        LoadedCallBack = callback;
    }

    UnityAction<GameObject> LoadedCallBack = null;

    internal GameObject LoadGameObject(string path)
    {
        var obj = Resources.Load(path);
        return obj switch
        {
            GameObject go => go,
            _ => null,
        };
    }
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
    internal GameObject[] LoadAllGameObject(string path)
    {
        var objs = Resources.LoadAll(path);
        return objs switch
        {
            GameObject[] gos => gos,
            _ => null,
        };
    }
//www eee rrr
}
