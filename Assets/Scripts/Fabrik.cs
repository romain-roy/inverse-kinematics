using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fabrik : MonoBehaviour
{
    public List<Vector3> p;
    public Vector3 t;
    public float tol = 0;

    private float[] d, r, lambda;

    void Start()
    {
    }

    void Update()
    {
        FabrikAlgo();
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
        t = point;
        t.z = 0;
    }

    void FabrikAlgo()
    {
        int n = p.Count - 1;

        d = new float[n];
        r = new float[n];
        lambda = new float[n];

        // The distance between each join

        for (int i = 0; i < n; i++)
            d[i] = (p[i + 1] - p[i]).magnitude;

        // The distance between root and target

        float dist = (p[0] - t).magnitude;

        // Check whether the target is within reach

        float sommeDistances = 0f;
        for (int i = 0; i < d.Length; i++)
            sommeDistances += d[i];

        if (dist > sommeDistances)
        {
            // The target is unreachable

            for (int i = 0; i < n; i++)
            {
                // Find the distance ri between the target t and the joint position pi

                r[i] = (t - p[i]).magnitude;
                lambda[i] = d[i] / r[i];

                // Find the new joint positions pi

                p[i + 1] = (1 - lambda[i]) * p[i] + lambda[i] * t;
            }
        }
        else
        {
            // The target is reachable; thus, set as b the initial position of the joint p1

            Vector3 b = p[0];

            // Check whether the distance between the end effector pn and the target t is greater than a tolerance

            float difA = (p[n] - t).magnitude;
            int it = 0;

            while (difA > tol && it < 5)
            {
                it++;
                
                // STAGE 1: FORWARD REACHING
                // Set the end effector pn as target t

                p[n] = t;

                for (int i = n - 1; i >= 0; i--)
                {
                    // Find the distance ri between the new joint position pi+1 and the joint pi

                    r[i] = (p[i + 1] - p[i]).magnitude;
                    lambda[i] = d[i] / r[i];

                    // Find the new joint positions pi

                    p[i] = (1 - lambda[i]) * p[i + 1] + lambda[i] * p[i];
                }

                // STAGE 2: BACKWARD REACHING
                // Set the root p1 its initial position

                p[0] = b;

                for (int i = 0; i < n; i++)
                {
                    // Find the distance ri between the new joint position pi and the joint pi+1

                    r[i] = (p[i + 1] - p[i]).magnitude;
                    lambda[i] = d[i] / r[i];

                    // Find the new joint position pi

                    p[i + 1] = (1 - lambda[i]) * p[i] + lambda[i] * p[i + 1];
                }

                difA = (p[n] - t).magnitude;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < p.Count - 1; i++)
        {
            Gizmos.DrawLine(p[i], p[i + 1]);
        }
        for (int i = 0; i < p.Count; i++)
        {
            Gizmos.DrawSphere(p[i], 0.1f);
        }
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(t, 0.1f);
    }
}
