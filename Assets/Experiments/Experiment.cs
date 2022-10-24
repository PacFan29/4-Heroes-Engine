using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class Experiment : MonoBehaviour
{
    public PathCreator rail;
    public float offset = 0.3f;

    public EndOfPathInstruction endOfPathInstruction;
    public float speed = 5;
    float distanceTravelled;
    // Start is called before the first frame update
    void Start()
    {
        if (rail != null)
        {
            // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
            rail.pathUpdated += OnPathChanged;
            OnPathChanged();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (rail != null)
        {
            distanceTravelled -= speed * Time.deltaTime;
            transform.position = rail.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction) + (transform.up * 2f);
            transform.rotation = rail.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);

            if (transform.position - (transform.up * 2f) == rail.path.GetPoint(rail.path.NumPoints - 1)) {
                Debug.Log("End");
            } else if (transform.position - (transform.up * 2f) == rail.path.GetPoint(0)) {
                Debug.Log("Start");
            }
        }
    }

    void OnPathChanged() {
        distanceTravelled = rail.path.GetClosestDistanceAlongPath(transform.position);
    }
}
