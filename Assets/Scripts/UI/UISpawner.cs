using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UISpawner : MonoBehaviour
{
    public static UISpawner Instance;
    public Transform ParentSkills, ParentTips;

    public Text TextCountHP, TextCountMana;
    public Text TextRegenHP, TextRegenMana;
    public Image ImageFillHP, ImageFillMana;

    public GameObject TipsReloadGM, TipsNeedMana;
    public bool IsReloadTips;

    public Image ImageIconCharacter;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void SpawnSkills (GameObject[] _skills)
    {
        for (int i = 0; i < _skills.Length; i++)
        {
            GameObject skill = Instantiate(_skills[i], ParentSkills);
            skill.GetComponent<SkillKeyboard>().OnSetKey(KeySettings.Instance.ActionKeys[i]);
        }

    }

    public void SpawnTipReloadSkill ()
    {
        if (IsReloadTips) return;
        IsReloadTips = true;
        GameObject tip = Instantiate(TipsReloadGM, ParentTips);
        tip.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0f), 0.3f, 1, 0);
        Invoke("ReloadTips", 1.6f);
    }

    public void SpawnTipNeedMana()
    {
        if (IsReloadTips) return;
        IsReloadTips = true;
        GameObject tip = Instantiate(TipsNeedMana, ParentTips);
        tip.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0f), 0.3f, 1, 0);
        Invoke("ReloadTips", 1.6f);
    }

    public void ReloadTips ()
    {
        IsReloadTips = false;
    }

    public void SetIconCharacter (Sprite _icon)
    {
        ImageIconCharacter.sprite = _icon;
        ImageIconCharacter.enabled = true;
    }
}
