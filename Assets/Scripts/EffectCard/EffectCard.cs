using System.ComponentModel;
using UnityEngine;

public class EffectCard : MonoBehaviour
{

    [SerializeField]
    bool isTargeted = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActOn(JamGrid grid, int x, int y) {  }

    public bool CanBeUsedOn(JamGrid grid, int x, int y) { return true; }
    public void Select() {
        isTargeted = true;
    }
    public void Unselect() {
        isTargeted = false;
    }
}
