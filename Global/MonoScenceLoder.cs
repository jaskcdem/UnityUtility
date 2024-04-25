using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

internal class MonoScenceLoder : MonoSingleton<MonoScenceLoder>
{
    internal MonoScenceLoder AddCallBacks(params UnityAction<Scene, LoadSceneMode>[] callBacks)
    {
        foreach (var item in callBacks) SceneManager.sceneLoaded += item;
        return Instance;
    }

    internal static void LoadScence(string name, LoadSceneMode mode = LoadSceneMode.Single) => SceneManager.LoadScene(name, mode);
    internal static void LoadScence(int buildIndex, LoadSceneMode mode = LoadSceneMode.Single) => SceneManager.LoadScene(buildIndex, mode);
    internal IEnumerator LoadScenceAsync(string name, LoadSceneMode mode = LoadSceneMode.Single)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(name, mode);
        if (ao == null) yield break;
        yield return ao;
    }
    internal IEnumerator LoadScenceAsync(int buildIndex, LoadSceneMode mode = LoadSceneMode.Single)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(buildIndex, mode);
        if (ao == null) yield break;
        yield return ao;
    }
}
