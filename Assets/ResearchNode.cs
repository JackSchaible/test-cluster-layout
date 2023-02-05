using UnityEngine;

[CreateAssetMenu]
public class ResearchNode : ScriptableObject
{
    public string Name;
    public ResearchNode Cluster;
    public ResearchNode[] Children;
}
