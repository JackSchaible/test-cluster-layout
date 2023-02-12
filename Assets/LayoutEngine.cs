using System.Collections.Generic;
using UnityEngine;
using Vectrosity;
using Vector2 = UnityEngine.Vector2;

[ExecuteInEditMode]
public class LayoutEngine : MonoBehaviour
{
    public ResearchNode Root;
    public float Spacing = 1.0f;
    //public float Padding = 1.2f;
    //public float EndPadding = 1.0f;
    //public float EndClusterPadding = 1.0f;
    public float ClusterScale = 2.0f;
    public float LineWidth = 2.0f;
    private const float Pi = Mathf.PI;
    public GameObject TreeNodePrefab;
    public Canvas Canvas;

    

    // Start is called before the first frame update
    public void Generate()
    {
        Layout(Root, Vector2.zero, Spacing);
    }

    public void Clean()
    {
        foreach (Transform child in Canvas.transform)
        {
            DestroyImmediate(child.gameObject);
        }
    }

    private void Layout(ResearchNode node, Vector2 center, float radius, Node parentNode = null)
    {
        GameObject treeNodeObject = Instantiate(TreeNodePrefab, Canvas.transform);
        treeNodeObject.name = node.Name;
        Node treeNode = treeNodeObject.GetComponent<Node>();
        treeNode.Position = center;
        treeNode.Cluster = node.Cluster;
        treeNode.Children = node.Children;
        treeNode.Name = node.Name;
        treeNode.Radius = Spacing;

        RectTransform rectTransform = treeNodeObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = center;
        rectTransform.sizeDelta = new Vector2(2 * treeNode.Radius, 2 * treeNode.Radius);

        ResearchNode[] children = node.Children;
        int n = children.Length;
        

        // Updated this to restrict the angle children spawn in to 90 degrees
        float angleStep = Pi / (2 * n);
        float childAngle = 0;

        if (parentNode != null)
        {
            // Padding value calculations
            float Padding = Spacing * 1.25f;

            childAngle = Mathf.Atan2(center.y - parentNode.Position.y, center.x - parentNode.Position.x);
            RectTransform parentTransform = parentNode.GetComponent<RectTransform>();

            float EndPadding = (parentTransform.anchoredPosition - center).magnitude + (Padding * 1.2f) ;
            float EndClusterPadding = (parentTransform.anchoredPosition - center).magnitude;


            // Moved these to their own lines to make it easier to read
            Vector2 parentPos = parentTransform.anchoredPosition;
            float endPadding = treeNode.Cluster == parentNode.Cluster ? EndClusterPadding : EndPadding;

            Vector2 lineStart = new Vector2(endPadding * Mathf.Cos(childAngle), endPadding * Mathf.Sin(childAngle));
            Vector2 lineEnd = new Vector2(Padding * Mathf.Cos(childAngle),Padding * Mathf.Sin(childAngle));
            
            DrawLine(lineStart, lineEnd, parentPos, Canvas, treeNode.Name, parentNode.Name);
        } else {
            // Makes the root node spawn children in all directions
            angleStep = 2 * Pi / n;
        }
        
        // Starting point for children angles. Allows us to "walk" out from starting point with every increment of i
        // 
        float childStartAngle = childAngle - (angleStep * (Mathf.Ceil(n / 2)));

        // Draw children
        for (int i = 0; i < n; i++)
        {
            ResearchNode child = children[i];
            float adjustedRadius = radius;
            if (child.Cluster != node.Cluster)
            {
                adjustedRadius *= 2 * ClusterScale;
            }
            else
            {
                adjustedRadius *= ClusterScale;
            }

            float angle = i * angleStep + childStartAngle;
            float childX = center.x + adjustedRadius * Mathf.Cos(angle);
            float childY = center.y + adjustedRadius * Mathf.Sin(angle);
            Vector2 childCenter = new (childX, childY);

            Layout(child, childCenter, radius, treeNode);
        }
    }

    private void DrawLine(Vector2 start, Vector2 end,Vector2 parentPos, Canvas canvas, string fromObjectName, string toObjectName)
    {
        VectorLine line = new ($"Line from {fromObjectName} to {toObjectName}",
            new List<Vector2>(), LineWidth);
        line.SetCanvas(canvas);
        line.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        line.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        line.rectTransform.anchoredPosition = parentPos;
        line.points2.Add(start);
        line.points2.Add(end);
        line.Draw();
    }
}
