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
        if (grid == null) {
            GameObject[] objs = gridScene.GetRootGameObjects();
            Debug.Log(objs.Count());
            foreach (GameObject obj in objs) {
                JamGrid gridObj = obj.GetComponent<JamGrid>();
                if (gridObj != null) grid = gridObj;
            }
        }
        Debug.Assert(grid != null, "Could not find jam grid in scene"); 
        return grid;
    }

    public RectTransform GetDisplay()
    {
        Debug.Assert(display != null, "Could not find grid display"); 
        return display;
    }
}
