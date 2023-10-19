using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/IconData", order = 1)]
public class IconData : ScriptableObject
{
    public List<Icon> Icon = new List<Icon>();
}

[System.Serializable]
public class Icon
{
    public int ID;
    public string Name;
    public string Decription;
    public Sprite Image;
}
