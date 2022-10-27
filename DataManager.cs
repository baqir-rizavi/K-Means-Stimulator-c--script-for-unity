using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    [SerializeField] Transform point;
    [SerializeField] Material[] colorsMaterials;
    [SerializeField] int maxIterations = 5;
    [SerializeField] int size = 200;
    [SerializeField] int k = 3;

    float[] c1;
    float[] c2;
    float[] c3;

    struct DataPoint
    {
        public Transform trans;
        public int classNumber;
    }

    DataPoint[] points;
    Vector3[] meanPoints;
    //List<DataPoint>[] classes;


    void Awake()
    {

        //need more color materials
        if (colorsMaterials.Length < k)
        {
            Debug.LogError("need more color materials");

        } 


        // three colums from table
        c1 = new float[size]; 
        c2 = new float[size]; 
        c3 = new float[size];

        points = new DataPoint[size];
        meanPoints = new Vector3[k];
        //classes = new List<DataPoint>[k];

        //for (int i = 0; i < k; i++)
            //classes[i] = new List<DataPoint>()

        // random (temperary)
        for (int i = 0; i < size; i++)
            c1[i] = Random.Range(-50, 50);
        for (int i = 0; i < size; i++)
            c2[i] = Random.Range(-50, 50);
        for (int i = 0; i < size; i++)
            c3[i] = Random.Range(-50, 50);

        
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(summoner());
    }
    
    IEnumerator summoner()
    {
        // Init data points only of one class: 0
        for (int i = 0; i < size; i++)
        {
           points[i].trans = Instantiate(point, new Vector3(c1[i], c2[i], c3[i]), Quaternion.identity, transform);
           ChangeClass(points[i], 0);
           yield return new WaitForSeconds(0.13f);
        }
        
        yield return new WaitForSeconds(0.13f);

        // chosing random mean points
        for (int i = 0; i < k; i++)
        {
            meanPoints[i] = new Vector3(Random.Range(-50, 50), Random.Range(-50, 50), Random.Range(-50, 50));
        }

        // k means 
        bool update = true; 
        int it = 0;
        while (update && it < maxIterations)
        {    
            update = false;
            foreach (var dPoint in points)
            {   
                int minDistanceClass = GetMinDistanceClassFromMeanPoints(dPoint);
                if (dPoint.classNumber != minDistanceClass)
                {    
                    ChangeClass(dPoint, minDistanceClass);
                    yield return new WaitForSeconds(0.13f);
                    update = true;
                }
            }
            UpdateMeanPoints();
            it++;
        }
    
    }

    int GetMinDistanceClassFromMeanPoints(DataPoint dPoint)
    {
        float[] distances = new float[k];
        int i = 0;
        foreach (var mPoint in meanPoints)
        {
            distances[i] = Mathf.Abs(Vector3.Distance(mPoint, dPoint.trans.position));
            i++;
        }
        return System.Array.IndexOf(distances, distances.Min());
    }

    void ChangeClass(DataPoint p, int c)
    {
        p.classNumber = c;
        ChangeColor(p.trans, colorsMaterials[c]);
    }

    void ChangeColor(Transform trans, Material material)
    {
        Material[] mat = new Material[1];
        mat[0] = material;
        trans.gameObject.GetComponent<MeshRenderer>().materials = mat;
    }

    void UpdateMeanPoints()
    {
        Vector3[] accumulators = new Vector3[k];
        for (int i = 0; i < k; i++)
        {
            accumulators[i] = new Vector3 (0,0,0);
        }
        int[] counter = new int[k];

        foreach (var dPoint in points)
        {
            accumulators[dPoint.classNumber] += dPoint.trans.position;
            counter[dPoint.classNumber]++;
        }
        for (int i = 0; i < k; i++)
        {
            if (counter[i] != 0) // if any point in cluster
            {
                meanPoints[i] = accumulators[i] / (float)counter[i];
            }
        }

    }

}
