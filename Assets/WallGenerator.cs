using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator : MonoBehaviour
{
    public GameObject wall;
    public int amount;
    public float xmin, xmax, ymin, ymax;

    private List<GameObject> walls = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateWalls(int amount) {

        for (int i = 0; i < walls.Count; i++) {
            GameObject go = walls[0];
            Destroy(go);
            walls.RemoveAt(0);
        }


        for (int i = 0; i < amount; i++) {
            float x = Random.Range(xmin, xmax);
            float y = Random.Range(ymin, ymax);

            GameObject go = Instantiate(wall, new Vector3(x, y, 0), Quaternion.identity);
            walls.Add(go);
        }
    }
}
