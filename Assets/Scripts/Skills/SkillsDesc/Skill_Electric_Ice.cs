using UnityEngine;
using UnityEngine.UI;

public class Skill_Electric_Ice : MonoBehaviour
{
    public Skills NameSkill;
    public SkillsParameters skillsParameters;
    public GameObject MoveBallGM;

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
        ButtonSkill.onClick.AddListener(SelectSkill);
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
        if (IsReload) return;
        MyPlayerControl.DeselectAllSkills();
        SelectImage.enabled = true;
        MyPlayerControl.SelectedSkill = NameSkill;
    }


    private void OnClickMouseUse()
    {
        if (IsReload) return;
        if (MyPlayerControl.SelectedSkill == NameSkill)
        {
            MyPlayerControl.playerAnimatorControl.SetElectricSkill();
            Vector2 posMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameObject ball = Instantiate(MoveBallGM, MyPlayerControl.SpawnSkills.position, MoveBallGM.transform.rotation);
            ball.GetComponent<MoveAndExplosion>().StartMove(new Vector3(posMouse.x, posMouse.y, 0), Random.Range(skillsParameters.DamageMin, skillsParameters.DamageMax));
            StartReload();
        }
    }


    private void OnDeselectSkill()
    {
        SelectImage.enabled = false;
    }

}
