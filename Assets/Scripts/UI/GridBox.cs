using UnityEditor.SearchService;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GridBox : MonoBehaviour
{
    [SerializeField]
    SceneAsset gridScene;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SceneManager.LoadScene(gridScene.name, LoadSceneMode.Additive);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
