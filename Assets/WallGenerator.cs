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

        for (int i = 0; i < walls.Count; i++) {
            GameObject go = walls[0];
            Destroy(go);
            walls.RemoveAt(0);
        }


        for (int i = 0; i < wallsPerLine; i++) {
            for(float y = ymin; y < ymax; y += 2* wall.GetComponent<BoxCollider2D>().size.y * 1.5f)
            {
                float x = Random.Range(xmin, xmax);

                GameObject go = Instantiate(wall, new Vector3(x, y, 0), Quaternion.identity);
                walls.Add(go);
            }
        }
    }
}
