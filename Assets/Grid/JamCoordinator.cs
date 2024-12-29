using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class JamCoordinator : MonoBehaviour
{
    [SerializeField]
    JamGrid _grid;

    // Timing Stuff
    [SerializeField]
    [Range(0, 10)] private float timeScale = 1; // speed up/slowdown time
    public float TimeScale { get => timeScale; }
    [SerializeField]
    private float stepTime = 3;
    [SerializeField]
    private float stepPause = 0.25f; // the delay between full steps
    [SerializeField]
    private float initPause = 1f; // the delay between full steps

    public float StepDuration { get { return timeScale == 0 ? Mathf.Infinity : stepTime / timeScale; } }
    public float StepDelay { get { return timeScale == 0 ? Mathf.Infinity : stepPause / timeScale; } }

    // Scoring stuff
    [SerializeField]
    private int score = 0;
    public int Score { get => score; }
    private int stepCount = 0;
    public int StepCount { get => score; }

    // Scoring stuff
    [SerializeField]
    public bool GameOver = false;

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

        // Setup Timer
        stepTimer = stepTime + stepPause + initPause;
    }
    

    [SerializeField]
    float stepTimer = 0;
    void FixedUpdate()
    {
        stepTimer -= Time.fixedDeltaTime * timeScale;
    }

    // Update is called once per frame
    public void SetTimescale(float newTimeScale)
    {
        timeScale = newTimeScale;
        if (timeScale < 0) timeScale = 0;
    }

    IEnumerator StepLoop()
    {

        // initial pause
        while (stepTimer > stepPause + stepTime){
            yield return null;
        }

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
            stepCount++;
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
            while (stepTimer > stepPause){
                yield return null;
            }

            // Debug.Log("Step done!");

            // add a beat between steps
            while (stepTimer > 0){
                yield return null;
            }

            stepTimer += stepTime + stepPause;

            // grab the entities
            entities = _grid.GetAllEntities();
        }

        // Debug.LogWarning("No entities left to step! Closing loop...");
    }
}
