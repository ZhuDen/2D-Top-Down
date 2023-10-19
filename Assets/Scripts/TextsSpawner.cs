using UnityEngine;
using UnityEngine.UI;

public class TextsSpawner : MonoBehaviour
{
    public static TextsSpawner Instance;

    public GameObject PrefabTextDamage;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public void SpawnTextDamage (int _damage, Vector2 _position)
    {
        GameObject textDamage = Instantiate(PrefabTextDamage, _position, PrefabTextDamage.transform.rotation);
        textDamage.transform.GetChild(0).GetComponent<Text>().text = _damage.ToString();
    }
}
