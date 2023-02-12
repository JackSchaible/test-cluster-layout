using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vectrosity;
using Vector2 = UnityEngine.Vector2;

[ExecuteInEditMode]
public class LayoutEngine : MonoBehaviour
{
    public ResearchNode Root;
    public float BranchLength = 1.0f;
    public float ClusterBranchLengthModifier = 2.0f;
    public float BranchWidth = 2.0f;
    public float BerrySize = 30.0f;
    public float NonStraightBranchFactor = 0.4f;
    public GameObject TreeNodePrefab;
    public Canvas Canvas;

    private const float Pi = Mathf.PI;

    public void Generate() =>
        Layout(Root, Vector2.zero);

    public void Clean()
    {
        foreach (Transform child in Canvas.transform)
            DestroyImmediate(child.gameObject);
    }

    private void Layout(ResearchNode node, Vector2 center, Node parentNode = null)
    {
        GameObject treeNodeObject = Instantiate(TreeNodePrefab, Canvas.transform);
        treeNodeObject.name = node.Name;
        Node treeNode = treeNodeObject.GetComponent<Node>();
        treeNode.Position = center;
        treeNode.Cluster = node.Cluster;
        treeNode.Children = node.Children;
        treeNode.Name = node.Name;
        treeNode.Radius = BerrySize;

        RectTransform rectTransform = treeNodeObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = center;
        rectTransform.sizeDelta = new Vector2(2 * treeNode.Radius, 2 * treeNode.Radius);

        ResearchNode[] children = node.Children;
        int nChildren = children.Length;

        float childAngle = 0;
        if (parentNode != null)
        {
            childAngle = Mathf.Atan2(center.y - parentNode.Position.y, center.x - parentNode.Position.x);
            RectTransform parentTransform = parentNode.GetComponent<RectTransform>();

            float lineLength = (treeNode.Cluster == parentNode.Cluster ? BranchLength : BranchLength * ClusterBranchLengthModifier)
                               - BerrySize;

            Vector2 lineStart = treeNode.Radius * new Vector2(Mathf.Cos(childAngle), Mathf.Sin(childAngle));
            Vector2 lineEnd = new Vector2(Mathf.Cos(childAngle), Mathf.Sin(childAngle)) * lineLength;

            DrawLine(lineStart, lineEnd, parentTransform.anchoredPosition,
                Canvas, treeNode.Name, parentNode.Name);
        }

        ResearchNode[] nonClusterChildren = children.Where(c => c.Cluster != node.Cluster).ToArray();
        int nNonClusterChildren = nonClusterChildren.Length;
        if (nNonClusterChildren > 0 && parentNode != null)
            LayoutNonClusterChildren(nNonClusterChildren, childAngle, nonClusterChildren, node, center, treeNode);
        else if (nNonClusterChildren > 0)
            LayoutClusterChildren(nNonClusterChildren, childAngle, nonClusterChildren, node, center, treeNode, 0, true);

        ResearchNode[] clusterChildren = children.Where(c => c.Cluster == node.Cluster).ToArray();
        int nClusterChildren = clusterChildren.Length;
        if (nClusterChildren > 0)
            LayoutClusterChildren(nClusterChildren, childAngle, clusterChildren, node, center, treeNode, nNonClusterChildren);
    }

    private void LayoutNonClusterChildren(int nChildren, float childAngle, ResearchNode[] children,
        ResearchNode node, Vector3 center, Node treeNode)
    {
        // Starting point for children angles. Allows us to "walk" out from starting point with every increment of i
        // Updated this to restrict the angle children spawn in to 90 degrees
        float angleStep = Pi / (2 * nChildren);
        float childStartAngle = childAngle - angleStep * Mathf.Ceil(nChildren / 2) +
                                // Make it so we don't always spawn perfectly straight out
                                (NonStraightBranchFactor * (nChildren % 2 == 0 ? 1 : -1));

        DrawChildren(nChildren, children, node, center, treeNode, childStartAngle, angleStep);
    }

    private void LayoutClusterChildren(int nChildren, float childAngle, ResearchNode[] children,
        ResearchNode node, Vector3 center, Node treeNode, int nNonClusterChildren, bool isFirstSpawn = false)
    {
        // Starting point for children angles. Allows us to "walk" out from starting point with every increment of i
        // Updated this to restrict the angle children spawn in to 90 degrees
        float angleStep = 2 * Pi / (nChildren);
        float childStartAngle = childAngle - angleStep * nChildren + (nNonClusterChildren * angleStep / 2) + 2f;

        // Make bio spawn downwards
        if (isFirstSpawn)
            childStartAngle = 30 * Mathf.Deg2Rad;

        DrawChildren(nChildren, children, node, center, treeNode, childStartAngle, angleStep);
    }

    private void DrawChildren(int nChildren, ResearchNode[] children, ResearchNode node, Vector3 center,
        Node treeNode, float childStartAngle, float angleStep)
    {

        // Draw children
        for (int i = 0; i < nChildren; i++)
        {
            ResearchNode child = children[i];
            float adjustedRadius = BranchLength;
            if (child.Cluster != node.Cluster)
                adjustedRadius *= ClusterBranchLengthModifier;

            float angle = i * angleStep + childStartAngle;
            float childX = center.x + adjustedRadius * Mathf.Cos(angle);
            float childY = center.y + adjustedRadius * Mathf.Sin(angle);
            Vector2 childCenter = new(childX, childY);

            Layout(child, childCenter, treeNode);
        }
    }

    private void DrawLine(Vector2 start, Vector2 end, Vector2 parentPos, Canvas canvas, string toObjectName, string fromObjectName)
    {
        VectorLine line = new($"Line from {fromObjectName} to {toObjectName}",
            new List<Vector2>(), BranchWidth);
        line.SetCanvas(canvas);
        line.rectTransform.localScale = Vector3.one;
        line.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        line.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        line.rectTransform.anchoredPosition = parentPos;
        line.points2.Add(start);
        line.points2.Add(end);
        line.Draw();
    }
}
