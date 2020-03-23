using UnityEngine;
using System.Collections;
using System;

public class SphereBehaviourScript : MonoBehaviour
{
    public Transform SphereFunction;
    public Transform SphereXAxis;
    public Transform SphereYAxis;
    public Transform SphereZAxis;

    int test = 0;
    int scale = 2;
    Transform[][] pointTab;

    void Start()
    {
        displayAxis();
        displayFunction();
    }
    public void displayAxis()
    {
        Debug.Log("displayAxis");
        for (int i = -scale * 50; i < scale * 50; i++)
        {
            Transform pointXAxis = Instantiate(SphereXAxis);
            Transform pointYAxis = Instantiate(SphereYAxis);
            Transform pointZAxis = Instantiate(SphereZAxis);

            pointXAxis.localPosition = (new Vector3(i, 0, 0)) / scale;
            pointYAxis.localPosition = (new Vector3(0, i, 0)) / scale;
            pointZAxis.localPosition = (new Vector3(0, 0, i)) / scale;

            pointXAxis.SetParent(transform, false);
            pointYAxis.SetParent(transform, false);
            pointZAxis.SetParent(transform, false);
        }
    }
    public void displayFunction()
    {
        Debug.Log("Displaying...");
        pointTab = new Transform[100 * scale][];
        for (int i = 0; i < scale * 100; i++)
        {
            pointTab[i] = new Transform[100*scale];
            for (int j = 0; j < scale * 100; j++)
            {
                pointTab[i][j] = null;
                pointTab[i][j] = Instantiate(SphereFunction);
                float x = (i-(50*scale));
                float z = (j-(50*scale));
                float y = ((float)Math.Pow(x, 2) + (float)Math.Pow(z, 2))/100;

                Vector3 position = new Vector3(x, y, z);
                if ((position.x >= (-50 * scale)) && (position.x <= (50 * scale)) &&
                    (position.y >= (-50 * scale)) && (position.y <= (50 * scale)) &&
                    (position.z >= (-50 * scale)) && (position.z <= (50 * scale)))
                {
                    pointTab[i][j].localPosition = (position) / scale;
                    pointTab[i][j].SetParent(transform, false);
                }
            }
        }
        Debug.Log("Dispalyed");
        test = 0;
    }
    public void destroyFunction()
    {
        if (test == 0)
        {
            test = 1;
            Debug.Log("Destroying...");
            for (int i = 0; i < (scale * 100); i++)
            {
                for (int j = 0; j < (scale * 100); j++)
                {
                    if (pointTab[i][j] != null)
                    {
                        pointTab[i][j].localPosition = (new Vector3(0, 0, 0)) / scale;
                    }
                }
            }
            Debug.Log("Destroyed");
        }
        else displayFunction();
    }
}