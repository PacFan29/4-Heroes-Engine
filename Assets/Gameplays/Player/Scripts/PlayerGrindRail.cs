using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class PlayerGrindRail : MonoBehaviour
{
    public PathCreator rail;
    private EndOfPathInstruction endOfPathInstruction = EndOfPathInstruction.Stop;
    [HideInInspector] public float speed = 5;
    bool front;
    float distanceTravelled;

    PlayerInfo info;
    bool grind = true;
    // Start is called before the first frame update
    void OnEnable()
    {
        if (rail != null)
        {
            info = this.GetComponent<PlayerInfo>();
            rail.pathUpdated += OnPathChanged;
            OnPathChanged();

            front = Vector3.Angle(info.skin.forward, GetVector(Vector3.forward)) < 90;

            grind = true;
        }
    }

    void Update()
    {
        if (rail != null && grind)
        {
            distanceTravelled += speed * Time.deltaTime * (front ? 1f : -1f);
            transform.position = rail.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction) + (transform.up * 2f);
            transform.rotation = rail.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
            if (!front) transform.forward = -transform.forward;
            info.skin.forward = transform.forward;

            
            Vector3 addSpeed = transform.up * 0.625f;
            addSpeed.y = 0;
            if (Vector3.Angle(Vector3.up, transform.forward) > 90) {
                speed += addSpeed.magnitude;
            } else {
                speed -= addSpeed.magnitude;
                if (speed < 0) {
                    front = !front;
                    speed = -speed;
                }
            }
            if (info.SpeedC["frc"] < 0 && speed < info.SpeedC["top"]) {
                speed -= info.SpeedC["frc"];
                if (speed > info.SpeedC["top"]) {
                    speed = info.SpeedC["top"];
                }
            }

            if (transform.position - (transform.up * 2f) == rail.path.GetPoint(rail.path.NumPoints - 1) && front) {
                //終端
                grind = false;
                ExitFromRail();
            } else if (transform.position - (transform.up * 2f) == rail.path.GetPoint(0) && !front) {
                //始端
                grind = false;
                ExitFromRail();
            }

            if (!info.Grounded) {
                Debug.Log("Jumped!");
                grind = false;
                ExitFromRail(true);
            }
        }
    }

    void OnPathChanged() {
        distanceTravelled = rail.path.GetClosestDistanceAlongPath(transform.position);
    }

    void ExitFromRail(bool jumped = false) {
        if (jumped) {
            info.ForwardSetUp(transform.forward, speed);
        } else {
            info.VelocitySetUp(transform.forward * speed);
            this.transform.position += transform.forward * 2;
        }
        info.axisInput = true;
        info.activePhysics = true;
        this.enabled = false;
    }

    Vector3 GetVector(Vector3 direction) {
        return rail.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction) * direction;
    }
}
