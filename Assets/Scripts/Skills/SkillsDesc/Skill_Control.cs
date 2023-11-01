using UnityEngine;
using UnityEngine.UI;

public class Skill_Control : MonoBehaviour
{
    public Skills NameSkill;
    public TypeSkills TypeSkill;
    public SkillsParameters skillsParameters;
    public GameObject PrefabSkillGM;

    public PlayerControl MyPlayerControl;

    public Button ButtonSkill;
    public Text TextCountdown, TextNeedMana, TextButtonKey;
    public Image ImageFillCountdown, SelectImage;
    public float Multipler, countDown;
    public bool IsReload;
    public KeyCode MyKeyCode;

    void Start()
    {
        // сделаем задержку для получения игрока, что бы сначала его менеджер нашёл
        Invoke("GetPlayerControl", 0.1f);
        Multipler = 1f / skillsParameters.Countdown;
        countDown = skillsParameters.Countdown;

        if (TypeSkill == TypeSkills.AmedByPoint)
            ButtonSkill.onClick.AddListener(SelectSkill);

        if (TypeSkill == TypeSkills.NonDirectional)
            ButtonSkill.onClick.AddListener(OnClickUseNonDirectionalSkill);

        TextNeedMana.text = skillsParameters.NeedMana.ToString();
    }

    public void GetPlayerControl ()
    {
        MyPlayerControl = GameManager.Instance.MyPlayerControl;
    }

    

    private void OnEnable()
    {
        GameManager.OnClickMouse += OnClickMouseUse;
        PlayerControl.OnDeselectSkill += OnDeselectSkill;
        GetComponent<SkillKeyboard>().OnReplaceKey += OnReplaceKey;
    }

    private void OnDisable()
    {
        GameManager.OnClickMouse -= OnClickMouseUse;
        PlayerControl.OnDeselectSkill -= OnDeselectSkill;
        GetComponent<SkillKeyboard>().OnReplaceKey -= OnReplaceKey;
    }

    private void Update()
    {
        if(Input.GetKeyDown(MyKeyCode))
        {
            SelectSkill();
        }

        if (IsReload)
        {
            countDown -= Time.deltaTime;
            ImageFillCountdown.fillAmount = Multipler * countDown;
            TextCountdown.text = Mathf.RoundToInt(countDown).ToString();
            if(countDown <= 0)
            {
                IsReload = false;
                ImageFillCountdown.enabled = false;
                TextCountdown.enabled = false;
                countDown = skillsParameters.Countdown;
            }
        }
    }

    public void OnReplaceKey (KeyCode _keyCode)
    {
        MyKeyCode = _keyCode;
        TextButtonKey.text = _keyCode.ToString();
    }

    public void StartReload ()
    {
        IsReload = true;
        ImageFillCountdown.enabled = true;
        TextCountdown.enabled = true;
    }

    public void SelectSkill ()
    {
        if (MyPlayerControl.SelectedSkill == NameSkill) return;

        if (TypeSkill == TypeSkills.AmedByPoint)
        {
            MyPlayerControl.DeselectAllSkills();
            SelectImage.enabled = true;
            MyPlayerControl.SelectedSkill = NameSkill;
        }
        if (TypeSkill == TypeSkills.NonDirectional)
        {
            OnClickUseNonDirectionalSkill();
        }
        //  if (IsReload) return;
      
    }


    private void OnClickMouseUse()
    {
        if (MyPlayerControl.SelectedSkill == NameSkill)
        {
            if (TypeSkill == TypeSkills.AmedByPoint)
            {
                SpawnSkill();
            }
        }
    }

    public void OnClickUseNonDirectionalSkill ()
    {
        SpawnSkill();
    }

    public void SpawnSkill ()
    {
        if (IsReload)
        {
            UISpawner.Instance.SpawnTipReloadSkill();
            return;
        }

        if (MyPlayerControl.playerStats.Mana < skillsParameters.NeedMana)
        {
            UISpawner.Instance.SpawnTipNeedMana();
            return;
        }
        else
        {
            MyPlayerControl.playerStats.UpdateMana(skillsParameters.NeedMana, PlayerStats.TypeSummation.Minus);
        }

       /* if (TypeSkill == TypeSkills.AmedByPoint)
        {
            MyPlayerControl.playerAnimatorControl.SetElectricSkill();
            Vector2 posMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameObject skill = Instantiate(PrefabSkillGM, MyPlayerControl.SpawnSkills.position, PrefabSkillGM.transform.rotation);
            skill.GetComponent<MoveAndExplosion>().StartMove(new Vector3(posMouse.x, posMouse.y, 0), Random.Range(skillsParameters.DamageMin, skillsParameters.DamageMax));
        }
        if(TypeSkill == TypeSkills.NonDirectional)
        {
            GameObject skill = Instantiate(PrefabSkillGM, MyPlayerControl.transform.position, PrefabSkillGM.transform.rotation);
            MyPlayerControl.transform.position = new Vector3(MyPlayerControl.transform.position.x + Random.Range(-8f, 8f), MyPlayerControl.transform.position.y + Random.Range(-8f, 8f), 0);
        }*/
        StartReload();
    }

    private void OnDeselectSkill()
    {
        SelectImage.enabled = false;
    }

}
