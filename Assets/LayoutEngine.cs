using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class LayoutEngine : MonoBehaviour
{
    public ResearchNode Root;
    public float Spacing = 1.0f;
    public float ClusterScale = 2.0f;
    private const float Pi = Mathf.PI;
    public GameObject TreeNodePrefab;
    public Canvas Canvas;
    

    // Start is called before the first frame update
    public void Generate()
    {
        Layout(Root, Vector2.zero, Spacing);
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
        if (n == 0) return;

        // Updated this to restrict the angle children spawn in to 90 degrees
        float angleStep = Pi / (2 * n);
        float childAngle = 0;

        if (parentNode != null)
        {
            childAngle = Mathf.Atan2(center.y - parentNode.Position.y, center.x - parentNode.Position.x);
            RectTransform parentTransform = parentNode.GetComponent<RectTransform>();

            // Moved these to their own lines to make it easier to read
            Vector2 lineStart = rectTransform.anchoredPosition + new Vector2(treeNode.Radius * Mathf.Cos(childAngle),treeNode.Radius * Mathf.Sin(childAngle));
            Vector2 lineEnd = parentTransform.anchoredPosition + new Vector2(parentNode.Radius * Mathf.Cos(childAngle),parentNode.Radius * Mathf.Sin(childAngle));

            DrawLine(lineStart, lineEnd, Canvas, treeNode.Name, parentNode.Name);
        } else {
            // Makes the root node spawn children in all directions
            angleStep = 2 * Pi
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
                adjustedRadius *= ClusterScale;
                // TODO: Make the children spawn away from the parent node in a 90 degree arc
            }

            float angle = i * angleStep + childStartAngle;
            float childX = center.x + adjustedRadius * Mathf.Cos(angle);
            float childY = center.y + adjustedRadius * Mathf.Sin(angle);
            Vector2 childCenter = new (childX, childY);

            Layout(child, childCenter, adjustedRadius / 2, treeNode);
        }
    }

    // TODO: This doesn't work at all
    private static void DrawLine(Vector2 start, Vector2 end, Component canvas, string fromObjectName, string toObjectName)
    {
        GameObject lineObject = new($"Line from {fromObjectName} to {toObjectName}");
        lineObject.transform.SetParent(canvas.transform);
        Image image = lineObject.AddComponent<Image>();
        image.color = Color.white;

        RectTransform rectTransform = lineObject.GetComponent<RectTransform>();
        Vector2 direction = (end - start).normalized;
        float distance = Vector2.Distance(start, end);
        rectTransform.anchorMin = new Vector2(0, 0.5f);
        rectTransform.anchorMax = new Vector2(0, 0.5f);
        rectTransform.sizeDelta = new Vector2(distance, 2);
        rectTransform.pivot = new Vector2(0, 0.5f);
        rectTransform.position = start + direction * distance * 0.5f;
        rectTransform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
    }
}
