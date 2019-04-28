using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System;
using UnityEngine.UI;

public class GridMap : MonoBehaviour
{

    public int width, height;


    public UnityEngine.Color emptyUColor;
    public UnityEngine.Color wallUColor;
    public UnityEngine.Color unseenUColor;
    public UnityEngine.Color robotUColor;
    public UnityEngine.Color goalUColor;

    private System.Drawing.Color emptyColor;
    private System.Drawing.Color wallColor;
    private System.Drawing.Color unseenColor;
    private System.Drawing.Color robotColor;
    private System.Drawing.Color goalColor;

    public UnityEngine.UI.Image uiImage;

    public enum PixelStates { EMPTY, WALL, UNSEEN, ROBOT, GOAL};
    Bitmap image;


    // Start is called before the first frame update
    void Start()
    {
        emptyColor = System.Drawing.Color.FromArgb((int)(emptyUColor.r*255), (int)(emptyUColor.g*255), (int)(emptyUColor.b*255));
        wallColor = System.Drawing.Color.FromArgb((int)(wallUColor.r*255), (int)(wallUColor.g*255), (int)(wallUColor.b*255));
        unseenColor = System.Drawing.Color.FromArgb((int)(unseenUColor.r*255), (int)(unseenUColor.g*255), (int)(unseenUColor.b*255));
        robotColor = System.Drawing.Color.FromArgb((int)(robotUColor.r*255), (int)(robotUColor.g*255), (int)(robotUColor.b*255));
        goalColor = System.Drawing.Color.FromArgb((int)(goalUColor.r*255), (int)(goalUColor.g*255), (int)(goalUColor.b*255));

        image = new Bitmap(width, height);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                SetPixel(x, y, PixelStates.UNSEEN);
            }
        }
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

    public void SetPixel(int x, int y, PixelStates state) {
        System.Drawing.Color color;
        switch (state)
        {
            case PixelStates.EMPTY:
                color = emptyColor;
                break;
            case PixelStates.WALL:
                color = wallColor;
                break;
            case PixelStates.UNSEEN:
                color = unseenColor;
                break;
            case PixelStates.ROBOT:
                color = robotColor;
                break;
            case PixelStates.GOAL:
                color = goalColor;
                break;
            default:
                throw new System.Exception("You can't use a pixel state that isn't implemented");
        }

        if (x > 0 && x < width && y > 0 && y < height && image.GetPixel(x,y) != robotColor)
        {
            image.SetPixel(x, y, color);
        }
    } 
    public static List<Vector2Int> bresenham(int x, int y, int x2, int y2)
    {
        List<Vector2Int> points = new List<Vector2Int>();
        int w = x2 - x;
        int h = y2 - y;
        int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
        if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
        if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
        if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
        int longest = Mathf.Abs(w);
        int shortest = Mathf.Abs(h);
        if (!(longest > shortest))
        {
            longest = Mathf.Abs(h);
            shortest = Mathf.Abs(w);
            if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
            dx2 = 0;
        }
        int numerator = longest >> 1;
        for (int i = 0; i <= longest; i++)
        {
            points.Add(new Vector2Int(x, y));
            numerator += shortest;
            if (!(numerator < longest))
            {
                numerator -= longest;
                x += dx1;
                y += dy1;
            }
            else
            {
                x += dx2;
                y += dy2;
            }
        }

        return points;
    }

    public List<Vector2Int> BFS(Vector2Int start, Vector2Int end) {
        //long startTime = DateTime.Now.Ticks;
        Queue<Vertex> queue = new Queue<Vertex>();
        HashSet<Vector2Int> discovered = new HashSet<Vector2Int>();

        // Add the first node to the queue
        Vertex begin = new Vertex(start, null);
        queue.Enqueue(begin);
        int vertexCount = 0;
        // While the queue is not empty
        while (queue.Count > 0)
        {
            Vertex vertex = queue.Dequeue();
            vertexCount++;
            if (vertex.position == end)
            {
                //print("Vertex count: " + vertexCount);
                //print("total time: " + ((DateTime.Now.Ticks - startTime) / 10000000.0f));
                List<Vector2Int> relativePath = vertex.CreatePath();
                return relativePath;

            }

            foreach (Vertex w in Adjacent(vertex, discovered))
            {
                discovered.Add(w.position);
                queue.Enqueue(w);

            }
        }

        return null;
    }

    private List<Vertex> Adjacent(Vertex v, HashSet<Vector2Int> discovered) {
        List<Vertex> adj = new List<Vertex>();
        Vector2Int right = v.position + new Vector2Int(1, 0);
        Vector2Int up = v.position + new Vector2Int(0, -1);
        Vector2Int left = v.position + new Vector2Int(-1, 0);
        Vector2Int down = v.position + new Vector2Int(0, 1);

        // For each right,left,up,down we check to make sure it hasn't been discovered yet, then check to make sure its inbounds, then make sure it isn't a wall.
        if (!discovered.Contains(right) && right.x < width && image.GetPixel(right.x, right.y) != wallColor) {
            adj.Add(new Vertex(right, v));
        }
        if (!discovered.Contains(up) && up.y >= 0 && image.GetPixel(up.x, up.y) != wallColor) {
            adj.Add(new Vertex(up, v));
        }
        if (!discovered.Contains(left) && left.x >= 0 && image.GetPixel(left.x, left.y) != wallColor) {
            adj.Add(new Vertex(left, v));
        }
        if (!discovered.Contains(down) && down.y < height && image.GetPixel(down.x, down.y) != wallColor) {
            adj.Add(new Vertex(down, v));
        }
        return adj;
    }
}
