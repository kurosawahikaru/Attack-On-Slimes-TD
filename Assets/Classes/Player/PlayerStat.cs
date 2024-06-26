using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerStat : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI MoneyDisplayText;
    [SerializeField] private int StartingMoney = 200;

    [SerializeField] private TextMeshProUGUI CurrentLifeTxt;
    [SerializeField] private int MaxLife = 10;

    [SerializeField] private TextMeshProUGUI CurrentGoalTxt;
    [SerializeField] private int MaxGoal = 20;

    [SerializeField] private GameObject GameOverUi;
    [SerializeField] private GameObject WinPopupUi;
    [SerializeField] private TextMeshProUGUI WinTxt;

    private int CurrentLife;
    private int CurrentMoney;
    private int CurrentGoal;


    // Start is called before the first frame update
    void Start()
    {
        WinPopupUi.SetActive(false);
        GameOverUi.SetActive(false);
        CurrentMoney = StartingMoney;
        MoneyDisplayText.SetText($"${CurrentMoney}");
        CurrentLife = MaxLife;
        CurrentLifeTxt.SetText($"{CurrentLife} / {MaxLife}");
        CurrentGoal = 0;
        CurrentGoalTxt.SetText($"{CurrentGoal} / {MaxGoal}");
    }

    public void AddMoney(int MoneyToAdd)
    {
        CurrentMoney += MoneyToAdd;
        MoneyDisplayText.SetText($"${CurrentMoney}");
    }

    public void AddLife(int LifeToAdd)
    {
        CurrentLife += LifeToAdd;
        CurrentLifeTxt.SetText($"{CurrentLife} / {MaxLife}");
    }

    public void AddGoal(int GoalToAdd)
    {
        CurrentGoal += GoalToAdd;
        CurrentGoalTxt.SetText($"{CurrentGoal} / {MaxGoal}");
    }

    public int GetMoney()
    {
        return CurrentMoney;
    }

    public int GetLife()
    {
        return CurrentLife;
    }

    public int GetGoal()
    {
        return CurrentGoal;
    }

    public bool WinCondition()
    {

        if (CurrentGoal >= MaxGoal)
        {
            return true;
        }
        return false;
    }

    public void WinGame()
    {
        Time.timeScale = 0;
        WinTxt.SetText($"Congratulation! You won {SceneManager.GetActiveScene().name}!");
        WinPopupUi.SetActive(true);
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        GameOverUi.SetActive( true );
    }


}
