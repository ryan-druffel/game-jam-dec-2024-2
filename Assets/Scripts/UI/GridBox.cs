using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GridBox : MonoBehaviour
{
    [SerializeField]
    SceneAsset gridSceneAsset;
    Scene gridScene;
    JamGrid grid;
    JamCoordinator coordinator;
    [SerializeField]
    RectTransform display;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SceneManager.LoadScene(gridSceneAsset.name, LoadSceneMode.Additive);
        gridScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

        Debug.Assert(gridScene != null, "Could not find grid scene"); 
    }

    public JamGrid GetGrid()
    {
        if (grid == null) grid = GetTypeFromGridSceneRoot<JamGrid>();
        Debug.Assert(grid != null, "Could not find jam grid in scene"); 
        return grid;
    }

    public RectTransform GetDisplay()
    {
        Debug.Assert(display != null, "Could not find grid display"); 
        return display;
    }

    public JamCoordinator GetCoordinator()
    {
        
        if (coordinator == null) coordinator = GetTypeFromGridSceneRoot<JamCoordinator>();
        Debug.Assert(coordinator != null, "Could not find coordinator"); 
        return coordinator;
    }

    private T GetTypeFromGridSceneRoot<T>() {
        T query = default(T);
        GameObject[] objs = gridScene.GetRootGameObjects();
        foreach (GameObject obj in objs) {
            T queryObj = obj.GetComponent<T>();
            if (queryObj != null) query = queryObj;
        }
        return query;
    }
}
