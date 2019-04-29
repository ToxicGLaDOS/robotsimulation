using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator : MonoBehaviour
{
    public GameObject wall;
    public int wallsPerLine;
    public float xmin, xmax, ymin, ymax;

    private List<GameObject> walls = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        GenerateWalls();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateWalls() {

        int initalCount = walls.Count;
        for (int i = 0; i < initalCount; i++) {
            GameObject go = walls[0];
            walls.RemoveAt(0);
            Destroy(go);
            
        }
        
        for(float y = ymin; y < ymax; y += 2* wall.GetComponent<BoxCollider2D>().size.y * 1.5f)
        {
            for (int i = 0; i < wallsPerLine; i++)
            {
                float x = Random.Range(xmin, xmax);

                GameObject go = Instantiate(wall, new Vector3(x, y, 0), Quaternion.identity);
                walls.Add(go);
            }
        }
    }
}
