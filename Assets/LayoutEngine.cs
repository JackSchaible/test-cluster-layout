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
    private void Awake()
    {
        foreach (Transform child in Canvas.transform)
            DestroyImmediate(child.gameObject);
        
        Layout(Root, Vector2.zero, Spacing);
    }

    public void Layout(ResearchNode node, Vector2 center, float radius, Node parentNode = null)
    {
        GameObject treeNodeObject = Instantiate(TreeNodePrefab, Canvas.transform);
        Node treeNode = treeNodeObject.GetComponent<Node>();
        treeNode.Position = center;
        treeNode.Cluster = node.Cluster;
        treeNode.Radius = node.Radius;
        treeNode.Parents = node.Parents;
        treeNode.Children = node.Children;
        treeNode.Name = node.Name;

        RectTransform rectTransform = treeNodeObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = center;
        rectTransform.sizeDelta = new Vector2(2 * treeNode.Radius, 2 * treeNode.Radius);

        ResearchNode[] children = node.Children;
        int n = children.Length;
        if (n == 0) return;

        float angleStep = 2 * Pi / n;
        float childAngle = 0;
        if (parentNode != null)
        {
            childAngle = Mathf.Atan2(center.y - parentNode.Position.y, center.x - parentNode.Position.x);
            DrawLine(
                treeNode.Position + new Vector2(treeNode.Radius * Mathf.Cos(childAngle),
                    treeNode.Radius * Mathf.Sin(childAngle)),
                parentNode.Position + new Vector2(parentNode.Radius * Mathf.Cos(childAngle),
                    parentNode.Radius * Mathf.Sin(childAngle)), Canvas);
        }

        for (int i = 0; i < n; i++)
        {
            ResearchNode child = children[i];
            float adjustedRadius = radius;
            if (child.Cluster != node.Cluster)
            {
                adjustedRadius *= ClusterScale;
            }

            float angle = i * angleStep + childAngle;
            float childX = center.x + adjustedRadius * Mathf.Cos(angle);
            float childY = center.y + adjustedRadius * Mathf.Sin(angle);
            Vector2 childCenter = new (childX, childY);
            
            if (child.Children.Length == 0)
                DrawLine(childCenter, treeNode.Position, Canvas);
            
            Layout(child, childCenter, adjustedRadius / 2, treeNode);
        }
    }

    private static void DrawLine(Vector2 start, Vector2 end, Component canvas)
    {
        GameObject lineObject = new ("Line");
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
