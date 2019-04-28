using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex 
{
    public Vector2Int position;
    public Vertex parent;

    public Vertex(Vector2Int pos, Vertex par) {
        position = pos;
        parent = par;
    }

    // override object.Equals
    public override bool Equals(object obj)
    {
        //       
        // See the full list of guidelines at
        //   http://go.microsoft.com/fwlink/?LinkID=85237  
        // and also the guidance for operator== at
        //   http://go.microsoft.com/fwlink/?LinkId=85238
        //

        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        // TODO: write your implementation of Equals() here
        Vertex other = (Vertex)obj;
        return position.Equals(other.position);
    }

    // override object.GetHashCode
    public override int GetHashCode()
    {
        // TODO: write your implementation of GetHashCode() here
        throw new System.NotImplementedException();
        return base.GetHashCode();
    }

    public List<Vector2Int> CreateRelativePath() {
        List<Vector2Int> relativePath = new List<Vector2Int>();
        Vertex currentVert = this;

        while (currentVert.parent != null) {
            Vector2Int difference = currentVert.parent.position - currentVert.position;
            difference.x = -difference.x;
            // We don't need to invert the y values because y is already inverted due to the way images are indexed (topleft is (0, 0))

            relativePath.Insert(0, difference);
            currentVert = currentVert.parent;
        }
        
        return relativePath;
    }

    public List<Vector2Int> CreatePath()
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vertex currentVert = this;

        while (currentVert.parent != null) { 

            path.Insert(0, currentVert.position);
            currentVert = currentVert.parent;
        }

        return path;
    }
}
