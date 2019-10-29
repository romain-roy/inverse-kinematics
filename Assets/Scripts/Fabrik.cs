using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fabrik : MonoBehaviour
{
    public List<Vector3> jointPositions;
    public Vector3 target;
    public float tolerance = 0;

    private float[] d, r, lambda;

    void Update()
    {
        FabrikAlgo();
    }

    void FabrikAlgo()
    {
        int n = jointPositions.Count - 1;

        d = new float[n];
        r = new float[n];
        lambda = new float[n];

        // The distance between each join

        for (int i = 0; i < n; i++)
            d[i] = (jointPositions[i + 1] - jointPositions[i]).magnitude;

        // The distance between root and target

        float dist = (jointPositions[0] - target).magnitude;

        // Check whether the target is within reach

        float sommeDistances = 0f;
        for (int i = 0; i < d.Length; i++)
            sommeDistances += d[i];

        if (dist > sommeDistances)
        {
            // The target is unreachable

            for (int i = 0; i < n; i++)
            {
                // Find the distance between the target and the joint position

                r[i] = (target - jointPositions[i]).magnitude;
                lambda[i] = d[i] / r[i];

                // Find the new joint positions

                jointPositions[i + 1] = (1 - lambda[i]) * jointPositions[i] + lambda[i] * target;
            }
        }
        else
        {
            // The target is reachable; thus, set the initial position of the first joint

            Vector3 b = jointPositions[0];

            // Check whether the distance between the end effector and the target is greater than a tolerance

            float difA = (jointPositions[n] - target).magnitude;
            int it = 0;

            while (difA > tolerance && it < 5)
            {
                it++;

                // STAGE 1: FORWARD REACHING
                // Set the end effector as target

                jointPositions[n] = target;

                for (int i = n - 1; i >= 0; i--)
                {
                    // Find the distance between the new joint position and the previous joint

                    r[i] = (jointPositions[i + 1] - jointPositions[i]).magnitude;
                    lambda[i] = d[i] / r[i];

                    // Find the new joint positions

                    jointPositions[i] = (1 - lambda[i]) * jointPositions[i + 1] + lambda[i] * jointPositions[i];
                }

                // STAGE 2: BACKWARD REACHING
                // Set the root its initial position

                jointPositions[0] = b;

                for (int i = 0; i < n; i++)
                {
                    // Find the distance between the new joint position and the previous joint

                    r[i] = (jointPositions[i + 1] - jointPositions[i]).magnitude;
                    lambda[i] = d[i] / r[i];

                    // Find the new joint position

                    jointPositions[i + 1] = (1 - lambda[i]) * jointPositions[i] + lambda[i] * jointPositions[i + 1];
                }

                difA = (jointPositions[n] - target).magnitude;
            }
        }
    }

    void OnGUI()
    {
        Vector3 point = new Vector3();
        Event currentEvent = Event.current;
        Vector2 mousePos = new Vector2();
        Camera cam = Camera.main;
        mousePos.x = currentEvent.mousePosition.x;
        mousePos.y = cam.pixelHeight - currentEvent.mousePosition.y;
        point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));
        target = point;
        target.z = 0;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < jointPositions.Count - 1; i++)
        {
            Gizmos.DrawLine(jointPositions[i], jointPositions[i + 1]);
            Gizmos.DrawSphere(jointPositions[i], 0.1f);
        }
        Gizmos.DrawSphere(jointPositions[jointPositions.Count - 1], 0.1f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(target, 0.1f);
    }
}
