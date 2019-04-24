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

    public static List<Vector2> bresenham(int x1, int y1, int x2, int y2)
    {
        List<Vector2> points = new List<Vector2>();
        int m_new = 2 * (y2 - y1);
        int slope_error_new = m_new - (x2 - x1);

        for (int x = x1, y = y1; x <= x2; x++)
        {
            print("(" + x + "," + y + ")\n");

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
