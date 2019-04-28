using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using UnityEngine.UI;

public class GridMap : MonoBehaviour
{

    public int width, height;

    public System.Drawing.Color wallColor;

    public UnityEngine.UI.Image uiImage;
    Bitmap image;
    // Start is called before the first frame update
    void Start()
    {
        image = new Bitmap(width, height);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                image.SetPixel(x, y, System.Drawing.Color.Blue);
            }
        }

        List<Vector2Int> path = BFS(new Vector2Int(0, 0), new Vector2Int(10, 10));

    }

    // Update is called once per frame
    void Update()
    {
        MemoryStream ms = new MemoryStream();
        image.Save(ms, ImageFormat.Png);
        var buffer = new byte[ms.Length];
        ms.Position = 0;
        ms.Read(buffer, 0, buffer.Length);
        Texture2D t = new Texture2D(width, height);
        t.LoadImage(buffer);
        uiImage.sprite = Sprite.Create(t, uiImage.sprite.rect, uiImage.sprite.pivot);

    }

    public void SetPixel(int x, int y, System.Drawing.Color color) {
        image.SetPixel(x, y, color);
    }

    public List<Vector2Int> BFS(Vector2Int start, Vector2Int end) {
        Queue<Vertex> queue = new Queue<Vertex>();
        List<Vertex> discovered = new List<Vertex>();

        // Add the first node to the queue
        Vertex begin = new Vertex(start, null);
        queue.Enqueue(begin);

        // While the queue is not empty
        while (queue.Count > 0)
        {
            Vertex vertex = queue.Dequeue();
            if (vertex.position == end)
            {
                return vertex.CreatePath();
            }

            foreach (Vertex w in Adjacent(vertex, discovered))
            {
                if (!discovered.Contains(w)) {
                    discovered.Add(w);
                    queue.Enqueue(w);
                }

            }
        }

        return null;
    }

    private List<Vertex> Adjacent(Vertex v, List<Vertex> discovered) {
        List<Vertex> adj = new List<Vertex>();
        Vertex right = new Vertex(v.position + new Vector2Int(1, 0), v);
        Vertex up = new Vertex(v.position + new Vector2Int(0, -1), v);
        Vertex left = new Vertex(v.position + new Vector2Int(-1, 0), v);
        Vertex down = new Vertex(v.position + new Vector2Int(0, 1), v);

        // For each right,left,up,down we check to make sure it hasn't been discovered yet, then check to make sure its inbounds, then make sure it isn't a wall.
        if (!discovered.Contains(right) && right.position.x < width && image.GetPixel(right.position.x, right.position.y) != wallColor) {
            adj.Add(right);
        }
        if (!discovered.Contains(up) && up.position.y >= 0 && image.GetPixel(right.position.x, right.position.y) != wallColor) {
            adj.Add(up);
        }
        if (!discovered.Contains(left) && left.position.x >= 0 && image.GetPixel(right.position.x, right.position.y) != wallColor) {
            adj.Add(left);
        }
        if (!discovered.Contains(down) && down.position.y < height && image.GetPixel(right.position.x, right.position.y) != wallColor) {
            adj.Add(down);
        }
        return adj;
    }

    public static List<Vector2> bresenham(int x1, int y1, int x2, int y2)
    {
        List<Vector2> points = new List<Vector2>();
        int m_new = 2 * (y2 - y1);
        int slope_error_new = m_new - (x2 - x1);

        for (int x = x1, y = y1; x <= x2; x++)
        {

            points.Add(new Vector2(x, y));

            // Add slope to increment angle formed 
            slope_error_new += m_new;

            // Slope error reached limit, time to 
            // increment y and update slope error. 
            if (slope_error_new >= 0)
            {
                y++;
                slope_error_new -= 2 * (x2 - x1);
            }
        }

        return points;
    }
}
