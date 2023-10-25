using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public enum TypeSummation
    {
        Plus,
        Minus
    }

    public Sprite MyIcon;
    public int MaxHP, MaxMana;

    private int CountHP, CountMana;
    public int CountRegenMana, CountRegenHP;

    private Text TextCountHP, TextCountMana;
    public Text TextRegenHP, TextRegenMana;
    private Image ImageFillHP, ImageFillMana;
    private float MultiplerFillMana, MultiplerFillHP;
    private float TmRegen;
    public int HP
    {
        get
        {
            return CountHP;
        }
    }

    public int Mana
    {
        get
        {
            return CountMana;
        }
    }

    private void Start()
    {
        TextCountHP = UISpawner.Instance.TextCountHP;
        TextRegenHP = UISpawner.Instance.TextRegenHP;
        TextCountMana = UISpawner.Instance.TextCountMana;
        TextRegenMana = UISpawner.Instance.TextRegenMana;
        ImageFillHP = UISpawner.Instance.ImageFillHP;
        ImageFillMana = UISpawner.Instance.ImageFillMana;

        CountHP = MaxHP;
        CountMana = MaxMana;

        UpdatedMultiplers();
        UpdateUIStats();

        UISpawner.Instance.SetIconCharacter(MyIcon);
    }

    private void Update()
    {
        TextRegenHP.text = string.Format("+{0}", CountRegenHP);
        TextRegenMana.text = string.Format("+{0}", CountRegenMana);


        TmRegen += Time.deltaTime;
        if (TmRegen >= 1f)
        {
            if (CountHP < MaxHP)
                CountHP += CountRegenHP;
            if (CountMana < MaxMana)
                CountMana += CountRegenMana;

            if (CountHP > MaxHP)
            {
                CountHP = MaxHP;
            }
            if (CountMana > MaxMana)
            {
                CountMana = MaxMana;
            }

            UpdateUIStats();
            TmRegen = 0f;
        }



    }

    public void UpdateHP (int count, TypeSummation typeSummation)
    {
        if(typeSummation == TypeSummation.Plus)
            CountHP += count;
        if (typeSummation == TypeSummation.Minus)
            CountHP -= count;

        if (CountHP > MaxHP)
        {
            CountHP = MaxHP;
        }

        if(CountHP < 0)
        {
            CountHP = 0;
        }

        UpdateUIStats();
    }

    public void UpdateMana(int count, TypeSummation typeSummation)
    {
        if (typeSummation == TypeSummation.Plus)
            CountMana += count;
        if (typeSummation == TypeSummation.Minus)
            CountMana -= count;

        if (CountMana > MaxMana)
        {
            CountMana = MaxMana;
        }

        if (CountMana < 0)
        {
            CountMana = 0;
        }
        UpdateUIStats();
    }

    public void UpdateUIStats ()
    {
        TextCountHP.text = string.Format("{0}/{1}", CountHP, MaxHP);
        ImageFillHP.fillAmount = MultiplerFillHP * CountHP;

        TextCountMana.text = string.Format("{0}/{1}", CountMana, MaxMana);
        ImageFillMana.fillAmount = MultiplerFillMana * CountMana;
    }

    public void UpdatedMultiplers ()
    {
        MultiplerFillMana = 1f / MaxMana;
        MultiplerFillHP = 1f / MaxHP;
    }
}
