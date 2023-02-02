using UnityEngine;

[CreateAssetMenu]
public class ResearchNode : ScriptableObject
{
    public string Name;
    public ResearchNode Cluster;
    public ResearchNode[] Parents;
    public ResearchNode[] Children;
    public float Radius;
}
