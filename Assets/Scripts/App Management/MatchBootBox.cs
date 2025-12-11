using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Make sure this runs before your other scripts in the scene.
[DefaultExecutionOrder(-1000)]
public class MatchBootBox : MonoBehaviour
{
    [SerializeField] private string coreSceneName = "Core";

    private bool _isBootstrapRun;
    private string _matchSceneName;

    private void Awake()
    {
        _matchSceneName = gameObject.scene.name;

        // If AppManager already exists, we were loaded by the proper flow (Core/App).
        if (AppManager.Instance != null)
        {
            // Optional log:
            LogCore.Log(LogType.General, $"MatchBootBox in scene '{_matchSceneName}' detected AppManager; disabling self.");
            // No bootstrapping, just get out of the way.
            gameObject.SetActive(false);
            return;
        }

        // Also check that this is effectively the "first/only" scene
        // (user hit Play with this scene open).
        var active = SceneManager.GetActiveScene();
        bool isOnlySceneLoaded = SceneManager.sceneCount == 1 &&
                                 active == gameObject.scene;

        if (!isOnlySceneLoaded)
        {
            // Safety: no AppManager, but multiple scenes loaded? Just log and bail.
            LogCore.Log(LogType.General,
                $"MatchBootBox in '{_matchSceneName}' found no AppManager, but multiple scenes are loaded. Not bootstrapping.");
            gameObject.SetActive(false);
            return;
        }

        // We are in bootstrap mode.
        _isBootstrapRun = true;

        LogCore.Log(LogType.General,
            $"[MatchBootBox] Bootstrap in scene '{_matchSceneName}'. " +
            $"Loading Core and suppressing local scene startup.");

        // Prevent other objects in this scene from running Start/Update/etc.
        DisableOtherRootObjectsInScene();

        // Keep this object alive across the Core load.
        DontDestroyOnLoad(gameObject);
    }

    private void DisableOtherRootObjectsInScene()
    {
        var scene = gameObject.scene;
        var roots = scene.GetRootGameObjects();
        foreach (var root in roots)
        {
            if (root == gameObject) continue;
            root.SetActive(false);
        }
    }

    private IEnumerator Start()
    {
        // If not in bootstrap mode, do nothing.
        if (!_isBootstrapRun)
            yield break;

        // Load Core additively.
        SceneManager.LoadScene(coreSceneName, LoadSceneMode.Additive);

        // Wait for AppManager to exist.
        yield return new WaitUntil(() => AppManager.Instance != null && !AppManager.Instance.SceneLoadingBusy);

        LogCore.Log(LogType.General,
            $"[MatchBootBox] Core loaded. Handing off to AppManager with MatchAPS('{_matchSceneName}').");

        // Kick the app state so it reloads this match scene "properly".
        AppManager.Instance.SetAppState(new MatchAPS(_matchSceneName));

        // Our bootstrap job is done.
        Destroy(gameObject);
    }
}
