using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JamCoordinator : MonoBehaviour
{
    [SerializeField]
    JamGrid _grid;

    // Timing Stuff
    [SerializeField]
    [Range(0.1f, 10)] private float timeScale = 1; // speed up/slowdown time
    [SerializeField]
    private float stepTime = 3;
    [SerializeField]
    private float stepPause = 0.25f; // the delay between full steps

    public float StepDuration { get { return stepTime / timeScale; } }
    public float StepDelay { get { return stepPause / timeScale; } }

    private static JamCoordinator _instance;
    public static JamCoordinator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<JamCoordinator>();
            }
            return _instance;
        }
    }
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Assert(_grid != null, "Grid not assigned to coordinator");
        StartCoroutine(StepLoop());
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
            }
        }
        // Post Evaluate
        foreach (JamGridEntity entity in list) {
            if (entity.GetActor() is not null) {
                entity.GetActor().PostEvaluate();
            }
        }
    }

    IEnumerator StepLoop()
    {
        // add a pause before starting the loop
        yield return new WaitForSeconds(1);

        // while there are entities to move, step loop
        List<JamGridEntity> entities = _grid.GetAllEntities();
        while (entities.Count > 0)
        {
            // Pre Evaluate
            foreach (JamGridEntity entity in entities)
            {
                if (entity.GetActor() is not null)
                {
                    entity.GetActor().PreEvaluate();
                }
            }
            // Step
            foreach (JamGridEntity entity in entities)
            {
                if (entity.GetActor() is not null)
                {
                    entity.GetActor().Step();
                }
            }
            // Post Evaluate
            foreach (JamGridEntity entity in entities)
            {
                if (entity.GetActor() is not null)
                {
                    entity.GetActor().PostEvaluate();
                }
            }

            // report that we're done
            yield return new WaitForSeconds(StepDuration);

            Debug.Log("Step done!");

            // add a beat between steps
            yield return new WaitForSeconds(StepDelay);

            // grab the entities
            entities = _grid.GetAllEntities();
        }

        Debug.LogWarning("No entities left to step! Closing loop...");
    }
}
