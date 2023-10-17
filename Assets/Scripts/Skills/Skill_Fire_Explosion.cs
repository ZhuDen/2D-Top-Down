using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Skill_Fire_Explosion : MonoBehaviour
{
    public Skills NameSkill;
    public SkillsParameters skillsParameters;
    public GameObject MoveBallGM;

    public PlayerControl MyPlayerControl;

    public Button ButtonSkill;
    public TextMeshProUGUI TextCountdown;
    public Image ImageFillCountdown, SelectImage;
    public float Multipler, countDown;
    public bool IsReload;

    void Start()
    {
        // сделаем задержку для получения игрока, что бы сначала его менеджер нашёл
        Invoke("GetPlayerControl", 0.1f);
        Multipler = 1f / skillsParameters.Countdown;
        countDown = skillsParameters.Countdown;
        ButtonSkill.onClick.AddListener(SelectSkill);
    }

    public void GetPlayerControl()
    {
        MyPlayerControl = GameManager.Instance.MyPlayerControl;
    }

    private void OnEnable()
    {
        GameManager.OnClickMouse += OnClickMouseUse;
    }

    private void OnDisable()
    {
        GameManager.OnClickMouse -= OnClickMouseUse;
    }

    private void Update()
    {
        if (IsReload)
        {
            countDown -= Time.deltaTime;
            ImageFillCountdown.fillAmount = Multipler * countDown;
            TextCountdown.text = Mathf.RoundToInt(countDown).ToString();
            if (countDown <= 0)
            {
                IsReload = false;
                ImageFillCountdown.enabled = false;
                TextCountdown.enabled = false;
                countDown = skillsParameters.Countdown;
            }
        }
    }

    public void StartReload()
    {
        IsReload = true;
        ImageFillCountdown.enabled = true;
        TextCountdown.enabled = true;
    }

    public void SelectSkill()
    {
        if (IsReload) return;
        SelectImage.enabled = true;
        MyPlayerControl.SelectedSkill = NameSkill;
    }


    private void OnClickMouseUse()
    {
        if (IsReload) return;
        if (MyPlayerControl.SelectedSkill == NameSkill)
        {
            Vector2 posMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameObject ball = Instantiate(MoveBallGM, MyPlayerControl.SpawnSkills.position, MoveBallGM.transform.rotation);
            ball.GetComponent<MoveAndExplosion>().StartMove(new Vector3(posMouse.x, posMouse.y, 0));
            StartReload();
        }
    }

}
