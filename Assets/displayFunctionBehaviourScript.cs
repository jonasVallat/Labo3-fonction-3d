using UnityEngine;
using System;
using UnityEngine.UI;

public class displayFunctionBehaviourScript : MonoBehaviour
{
    public Transform SphereFunction;
    public Transform SphereXAxis;
    public Transform SphereYAxis;
    public Transform SphereZAxis;
    public InputField inputField;
    public Button btnSphere;
    public Button btnSurface;
    public Button btnMesh;
    public Dropdown dropDown;
    public Transform blueCylinder;
    public Transform redCylinder;
    public Transform greenCylinder;
    public Transform greenCube;
    public Transform redCube;
    public Transform blueCube;
    public GameObject meshObject;
    public Material MeshMaterial;
    public Vector3 cameraInit;
    public Mesh mesh;

    bool sphereDisplayed = false;
    bool surfaceDisplayed = false;
    bool meshDisplayed = false;

    int scale = 1;
    Transform[][] pointTab;
    public Terrain terrain;
    float biggest = 0.0f;

    Vector3[] newVertices;
    Vector2[] newUV;

    private void Start()
    {
        inputField = GameObject.Find("Canvas/Image/InputField").GetComponent<InputField>();
        btnSphere = GameObject.Find("Canvas/Image/ButtonSphere").GetComponent<Button>();
        btnSurface = GameObject.Find("Canvas/Image/ButtonSurface").GetComponent<Button>();
        btnMesh = GameObject.Find("Canvas/Image/ButtonMesh").GetComponent<Button>();
        dropDown = GameObject.Find("Canvas/Image/Dropdown").GetComponent<Dropdown>();
        blueCylinder = GameObject.Find("BlueCylinder").GetComponent<Transform>();
        greenCylinder = GameObject.Find("GreenCylinder").GetComponent<Transform>();
        redCylinder = GameObject.Find("RedCylinder").GetComponent<Transform>();
        blueCube = GameObject.Find("BlueCube").GetComponent<Transform>();
        greenCube = GameObject.Find("GreenCube").GetComponent<Transform>();
        redCube = GameObject.Find("RedCube").GetComponent<Transform>();

        meshObject = new GameObject("MeshObject", typeof(MeshFilter), typeof(MeshRenderer));
        meshObject.transform.localScale = new Vector3(30, 30, 1);
        meshObject.GetComponent<MeshRenderer>().material = MeshMaterial;
        mesh = new Mesh();
        meshObject.GetComponent<MeshFilter>().mesh = mesh;

        btnSphere.interactable = false;
        btnSurface.interactable = false;
        btnMesh.interactable = false;

        inputField.text = "x*x+z*z";    //default input
        displayFunctionTerrain(false);
        createSpheres();                //instantiate spheres objects at start for future displays

        btnSphere.interactable = true;
        btnSurface.interactable = true;
        btnMesh.interactable = true;
    }
    public void displayFunctionMeshButton()     //"Mesh" button
    {
        if (sphereDisplayed)
        {
            displayFunctionSphere(false);
        }
        if (surfaceDisplayed)
        {
            displayFunctionTerrain(false);
        }
        displayFunctionMesh(true);
    }
    //display function from mesh (true=display, false=remove)
    public void displayFunctionMesh(bool display)       //display function with mesh
    {
        mesh.Clear();

        if (!display)       //only erases mesh content
        {
            meshDisplayed = false;
            return;
        }
        meshDisplayed = true;

        int sqrtVertices = 20;          //nb of vertices per side of the square that will form the function
        int half = sqrtVertices / 2;
        int nbVertices = sqrtVertices * sqrtVertices;
        int nbTriangles = (int)Math.Pow(sqrtVertices - 1, 2) * 6;
        Vector3[] vertices = new Vector3[nbVertices];
        Vector2[] uv = new Vector2[nbVertices];
        int[][] tabXY = new int[nbVertices][];
        int[] triangles = new int[nbTriangles];
        int index = 0;
        float x = 0;
        float y = 0;
        float z = 0;
        float biggest = 0;

        int dropDownValue = dropDown.value;

        // calculate each point of the function, both i and j represents an axe and the third is calculated from them
        for (int i = -half; i < half; i++)
        {
            for (int j = -half; j < half; j++)
            {
                if (dropDownValue == 0)     //y=...
                {
                    x = (float)(i * 1.7f / half);
                    y = Evaluate(i, j);
                    z = (float)(j * 50 / half);

                    biggest = Math.Max(biggest, Math.Abs(y));
                }
                else if (dropDownValue == 1)    //x=...
                {
                    x = Evaluate(j, i);
                    y = (float)(i * 1.7f / half);
                    z = (float)(j * 50 / half);

                    biggest = Math.Max(biggest, Math.Abs(x));
                }
                else    //z=...
                {
                    x = (float)(i * 1.7f / half);
                    y = (float)(j * 1.7f / half);
                    z = Evaluate(j, i);

                    biggest = Math.Max(biggest, Math.Abs(z));
                }
                vertices[index] = new Vector3(x, y, z);     //create vertice
                index++;
            }
        }
        //change values so they can't be too high
        for(int i = 0; i < vertices.Length; i++)
        {
            if (biggest != 0)
            {
                if (dropDownValue == 0)
                {
                    vertices[i].y *= 1.7f * 1.5f;   //1.7f = maxium value on axis, 1.5f = allows to go a little higher for aesthetic reasons
                    vertices[i].y /= biggest;
                }
                else if (dropDownValue == 1)
                {
                    vertices[i].x *= 1.7f * 1.5f;
                    vertices[i].x /= biggest;
                }
                else
                {
                    vertices[i].z *= 50 * 1.5f;     //1.7f = maxium value on Z axis
                    vertices[i].z /= biggest;
                }
            }
            uv[i] = new Vector2(vertices[i].x, vertices[i].y);  //create uv for each vertice
        }

        int step = 0;
        int squareCount = 1;
        index = 0;
        int v=1;

        //create triangles in the right order (there seems to be a problem here, the shader looks broken because of it)
        while ((index<triangles.Length)&(v<vertices.Length))
        {
            triangles[index] = v;   //create triangle
            switch (step)
            {
                case 0:
                    v += sqrtVertices;
                    break;
                case 1:
                    v -= sqrtVertices + 1;
                    break;
                case 3:
                    v += sqrtVertices + 1;
                    break;
                case 4:
                    v -= 1;
                    break;
                case 5:
                    squareCount++;
                    step = -1;
                    if (squareCount >= sqrtVertices)    //when a "row" of squares is finished, go to the next row
                    {
                        squareCount = 1;
                        v += 3;
                        v -= sqrtVertices;
                    }
                    else      //next square on same row
                    {
                        v += 2;
                        v -= sqrtVertices;
                    }
                    break;
            }
            index++;
            step++;
        }
        //update
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }
   
    //instantiate spheres objects at start for future displays
    public void createSpheres()
    {
        pointTab = new Transform[100 * scale][];
        for (int i = 0; i < pointTab.Length; i++)
        {
            pointTab[i] = new Transform[100 * scale];
            for (int j = 0; j < pointTab[i].Length; j++)
            {
                pointTab[i][j] = Instantiate(SphereFunction);
                pointTab[i][j].SetParent(transform, false);
            }
        }
    }
    //"Sphères" button
    public void displayFunctionSphereButton()
    {
        if (surfaceDisplayed)
        {
            displayFunctionTerrain(false);
        }
        if (meshDisplayed)
        {
            displayFunctionMesh(false);
        }
        displayFunctionSphere(true);
    }
    //display function from spheres (true=display, false=remove)
    public void displayFunctionSphere(bool display)
    {
        sphereDisplayed = display;

        float biggest = 0;
        float x;
        float z;
        float y;

        for (int i = 0; i < pointTab.Length; i++)
        {
            for (int j = 0; j < pointTab[i].Length; j++)
            {
                if (display)
                {

                    x = (i - (50 * scale));
                    z = (j - (50 * scale));

                    //convert text to value
                    y = Evaluate(x, z);

                    //exchange x, y, z depending on selected dropdown
                    if (Math.Abs(y) > biggest)
                    {
                        biggest = Math.Abs(y);
                    }

                    int dropDownValue = dropDown.value;
                    if (dropDownValue == 2)
                    {
                        float temp = x;
                        x = z;
                        z = y;
                        y = temp;
                    }
                    else if(dropDownValue == 1)
                    {
                        float temp = x;
                        x = y;
                        y = z;
                        z = temp;
                    }

                    pointTab[i][j].localPosition = (new Vector3(x, y, z)) / scale;
                }
                else pointTab[i][j].localPosition = new Vector3(0, 0, 0);       //move all spheres to (0,0,0) to "remove" them without deleting them
            }
        }
        //moves the spheres into visible range
        //i and j are used as axis values, the third value is calculated from them
        for (int i = 0; i < pointTab.Length; i++)
        {
            for (int j = 0; j < pointTab[i].Length; j++)
            {
                if (float.IsNaN(pointTab[i][j].localPosition.y))    //avoid occasional bug on the values by ignoring it (bug => value=NaN)
                {
                    pointTab[i][j].localPosition = new Vector3(0, 0, 0);
                }
                else
                {
                    int dropDownValue = dropDown.value;
                    float a;
                    //change values so they can't be too high
                    if (dropDownValue == 0)
                    {
                        a = ((pointTab[i][j].localPosition.y * 100) / biggest);
                    }
                    else if (dropDownValue == 1)
                    {
                        a = ((pointTab[i][j].localPosition.x * 100) / biggest);
                    }
                    else
                    {
                        a = ((pointTab[i][j].localPosition.z * 100) / biggest);
                    }
                    if (float.IsNaN(a)) //avoid occasionnal bug (value=NaN)
                    {
                        a = 0.0f;
                    }
                    //place sphere depending on dropdown (x=..., y=..., z=...)
                    if (dropDownValue == 0)
                    {
                        pointTab[i][j].localPosition = new Vector3(pointTab[i][j].localPosition.x, a, pointTab[i][j].localPosition.z);
                    }
                    else if (dropDownValue == 1)
                    {
                        pointTab[i][j].localPosition = new Vector3(a, pointTab[i][j].localPosition.y, pointTab[i][j].localPosition.z);
                    }
                    else
                    {
                        pointTab[i][j].localPosition = new Vector3(pointTab[i][j].localPosition.x, pointTab[i][j].localPosition.y, a);
                    }
                }
            }
        }
    }
    //"Terrain" button
    public void displayFunctionTerrainButton()
    {
        if (sphereDisplayed)
        {
            displayFunctionSphere(false);
        }
        if (meshDisplayed)
        {
            displayFunctionMesh(false);
        }
        displayFunctionTerrain(true);
    }
    //display function from terrains object (true=display, false=remove)
    public void displayFunctionTerrain(bool display)
    {
        surfaceDisplayed = display;
        if (!display)
        {
            terrain.transform.localPosition = new Vector3(1000, 1000, 1000);       //moves terrain to non-visible location

            //moves axis elements to default positions
            greenCylinder.rotation = Quaternion.Euler(0, 0, 0);
            redCylinder.rotation = Quaternion.Euler(0, 0, -90);
            blueCylinder.rotation = Quaternion.Euler(-90, 0, 0);

            greenCube.position = new Vector3(0, 50, 0);
            redCube.position = new Vector3(50, 0, 0);
            blueCube.position = new Vector3(0, 0, 50);
            greenCube.rotation = Quaternion.Euler(30, 0, 45);
            redCube.rotation = Quaternion.Euler(0, 30, 45);
            blueCube.rotation = Quaternion.Euler(45, 35, 0);

            return;
        }

        Vector3 size = terrain.terrainData.size;
        int h = terrain.terrainData.heightmapHeight;
        int w = terrain.terrainData.heightmapWidth;
        float[,] data = new float[h, w];
        data = terrain.terrainData.GetHeights(0, 0, w, h);
        float x, z;

        //caluate points of the function to display
        //i and j are used as axis values, the third value is calculated from them
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                x = ((float)(i - (w / 2))) / 100;
                z = ((float)(j - (h / 2))) / 100;

                //convert text to value
                data[j, i] = Evaluate(x, z);

                //next few points are identical to reduce the number of points to calculate
                for(int count=0; (j+1<h)&&(count < 3); count++)
                {
                    data[j + 1, i] = data[j, i];
                    ++j;
                }

                if (biggest < Math.Abs(data[j, i]))
                {
                    biggest = Math.Abs(data[j, i]);
                }
            }
        }
        //adapt values so the function stays in visile range
        float temp;
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                temp = data[j, i];
                if (biggest > 0.0f)
                {
                    temp *= 0.083f;     //0.083f => maximum value on axis
                    temp *= 1.2f;       //1.2f => allows to go a little higher for aesthetic reasons
                    temp /= biggest;  
                }
                temp += 0.083f;     //values start at the lowest negative (position of the terrain), that value corrects it
                data[j, i] = temp;
            }
        }
        terrain.terrainData.SetHeights(0, 0, data);     //update
        int dropDownValue = dropDown.value;

        //change axis positions depending on dropDown
        if (dropDownValue == 0)
        {
            greenCylinder.rotation = Quaternion.Euler(0, 0, 0);
            redCylinder.rotation = Quaternion.Euler(0, 0, -90);
            blueCylinder.rotation = Quaternion.Euler(-90, 0, 0);

            greenCube.position = new Vector3(0, 50, 0);
            redCube.position = new Vector3(50, 0, 0);
            blueCube.position = new Vector3(0, 0, 50);
            greenCube.rotation = Quaternion.Euler(30, 0, 45);
            redCube.rotation = Quaternion.Euler(0, 30, 45);
            blueCube.rotation = Quaternion.Euler(45, 35, 0);
        }
        else if (dropDownValue == 1)
        {
            redCylinder.rotation = Quaternion.Euler(0, 0, 0);
            blueCylinder.rotation = Quaternion.Euler(0, 0, -90);
            greenCylinder.rotation = Quaternion.Euler(-90, 0, 0);

            redCube.position = new Vector3(0, 50, 0);
            blueCube.position = new Vector3(50, 0, 0);
            greenCube.position = new Vector3(0, 0, 50);
            redCube.rotation = Quaternion.Euler(30, 0, 45);
            blueCube.rotation = Quaternion.Euler(0, 30, 45);
            greenCube.rotation = Quaternion.Euler(45, 35, 0);
        }
        else
        {
            blueCylinder.rotation = Quaternion.Euler(0, 0, 0);
            greenCylinder.rotation = Quaternion.Euler(0, 0, -90);
            redCylinder.rotation = Quaternion.Euler(-90, 0, 0);

            blueCube.position = new Vector3(0, 50, 0);
            greenCube.position = new Vector3(50, 0, 0);
            redCube.position = new Vector3(0, 0, 50);
            blueCube.rotation = Quaternion.Euler(30, 0, 45);
            greenCube.rotation = Quaternion.Euler(0, 30, 45);
            redCube.rotation = Quaternion.Euler(45, 35, 0);
        }
        terrain.transform.localPosition = new Vector3(-50, -50, -50);       //moves terrain to visible location
    }

    //evaluate string input
    public float Evaluate(float a, float b)
    {
        string expression = inputField.text;
        int dropDownValue = dropDown.value;
        
        if(dropDownValue==0)    //y=...
        {
            expression = expression.Replace("x", (" " + a.ToString() + " "));
            expression = expression.Replace("z", (" " + b.ToString() + " "));
        }
        else if(dropDownValue == 1)     //x=...
        {
            expression = expression.Replace("z", (" " + a.ToString() + " "));
            expression = expression.Replace("y", (" " + b.ToString() + " "));
        }
        else     //z=...
        {
            expression = expression.Replace("y", (" " + a.ToString() + " "));
            expression = expression.Replace("x", (" " + b.ToString() + " "));
        }
        System.Data.DataTable table = new System.Data.DataTable();
        table.Columns.Add("expression", string.Empty.GetType(), expression);
        System.Data.DataRow row = table.NewRow();
        table.Rows.Add(row);

        return float.Parse((string)row["expression"]);
    }
}