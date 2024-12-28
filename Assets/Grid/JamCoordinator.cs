using System.Collections.Generic;
using UnityEngine;

public class JamCoordinator : MonoBehaviour
{

    [SerializeField]
    JamGrid _grid;

    // Timing Stuff
    [SerializeField]
    private float stepTime = 3.0f;
    [SerializeField]
    private float timer = 0;
    [SerializeField]
    private float timeScale = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Assert(_grid is not null, "Grid not assigned to coordinator");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime * timeScale;
        if (timer > stepTime) {
            timer -= stepTime;
            StepGridOccupants();
        }
    }

    // Update is called once per frame
    void SetTimescale(float newTimeScale)
    {
        timeScale = newTimeScale;
        if (timeScale < 0) timeScale = 0;
    }

    void StepGridOccupants() {
        // Get occupants
        List<JamGridEntity> list = _grid.GetAllEntities();
        Debug.Log(list.Count);
        // Pre Evaluate
        foreach (JamGridEntity entity in list) {
            if (entity.GetActor() is not null) {
                entity.GetActor().PreEvaluate();
            }
        }
        // Step
        foreach (JamGridEntity entity in list) {
            if (entity.GetActor() is not null) {
                entity.GetActor().Step();
                Debug.Log(entity.ID + ": (" + entity.Column + " ," + entity.Row + ")");
            }
        }
        // Post Evaluate
        foreach (JamGridEntity entity in list) {
            if (entity.GetActor() is not null) {
                entity.GetActor().PostEvaluate();
            }
        }
    }
}
