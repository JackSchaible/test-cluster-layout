using TMPro;
using UnityEngine;

[ExecuteInEditMode]
public class Node : MonoBehaviour
{
    public TextMeshPro _text;

    public string Name
    {
        get => _text.text;
        set => _text.text = value;
    }

    public ResearchNode Cluster;
    public ResearchNode[] Children;
    public Vector2 Position;
    public float Radius;

    void Start()
    {
        _text.text = Name;
    }
}
