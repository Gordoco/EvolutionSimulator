using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateBorder : MonoBehaviour
{
    private List<GameObject> Borders = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void initialize(int width, int chunkSize, GameObject BorderType)
    {
        int side = (width - 1) / 2;
        int left_x = -(chunkSize * (side - 1));
        int bottom_z = -(chunkSize * (side - 1));
        //Top and Bottom (duplicate corners)
        for (int i = 0; i < width - 1; i++)
        {
            Borders.Add(Instantiate(BorderType, new Vector3(left_x + (chunkSize * i), 0, bottom_z - (chunkSize/2)), Quaternion.identity));
            Borders.Add(Instantiate(BorderType, new Vector3(left_x + (chunkSize * i), 0, (side * chunkSize) + (chunkSize / 2)), Quaternion.identity));
        }
        //Left and Right (duplicate corners)
        for (int i = 0;  i < width - 1; i++)
        {
            Borders.Add(Instantiate(BorderType, new Vector3(left_x - (chunkSize/2), 0, bottom_z + (chunkSize * i)), Quaternion.identity));
            Borders.Add(Instantiate(BorderType, new Vector3((side * chunkSize) + (chunkSize/2), 0, bottom_z + (chunkSize * i)), Quaternion.identity));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
