using TestExample;
using UnityEngine;

[CreateAssetMenu(fileName = "Config", menuName = "Config")]
public class Config : ScriptableObject
{
    [field: SerializeField] public Player PlayerPrefab { get; private set; }
    [field: SerializeField] public Box BoxPrefab { get; private set; }
    [field: SerializeField] public GameStateController GameStateControllerPrefab { get; private set; }
}