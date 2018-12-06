using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum MathOperations
{
    Addition,
    Subtraction,
    Multiplication
}

public class Riddle
{
    public List<int> Numbers;
    public MathOperations Operation;
    public List<int> Options;
    public int answer;

    public Riddle()
    {
        Numbers = new List<int>();
        Options = new List<int>();

    }
}

public class RiddleAR : MonoBehaviour 
{
    [SerializeField]
    TMP_Text UserName;

    [SerializeField]
    TMP_Text Score;

    [SerializeField]
    TMP_Text Questiom;

    [SerializeField]
    TMP_Text RiddleHealth;

    [SerializeField]
    Image HealthBar;


    [SerializeField]
    List<Button> Options;

    SkeletonEditor.PlayerController controller;

    int RiddleStrength;//300,500,700,1000

    int maxStrength;

    void Start () 
    {
        controller = GetComponent<SkeletonEditor.PlayerController>();
        UserName.text = PlayerPrefs.GetString(PlayerPrefKeys.DisplayName,"user");

    }

    private void OnEnable()
    {
        int playerscore = PlayerPrefs.GetInt(PlayerPrefKeys.SCORE, 0);
        if (playerscore <= 50)
        {
            RiddleStrength = 300;
        }
        else if (playerscore > 50 && playerscore <= 150)
        {
            RiddleStrength = 500;
        }
        else if (playerscore > 150 && playerscore <= 250)
        {
            RiddleStrength = 700;
        }
        else if (playerscore > 250 && playerscore <= 150)
        {
            RiddleStrength = 1000;
        }
        else
        {
            RiddleStrength = 1000;

        }
        maxStrength = RiddleStrength;
        RiddleHealth.text = RiddleStrength.ToString();
        HealthBar.fillAmount = RiddleStrength / maxStrength;
        Score.text = PlayerPrefs.GetInt(PlayerPrefKeys.SCORE, 0).ToString();
        ShowRiddleQ();
    }

    void ShowRiddleQ()
    {
        Riddle ridd = GenrateRiddle();
        string opString = "+";
        switch (ridd.Operation)
        {
            case MathOperations.Addition:
                opString = "+";
                break;
            case MathOperations.Subtraction:
                opString = "-";
                break;
            case MathOperations.Multiplication:
                opString = "*";
                break;
        }

        Questiom.text = ridd.Numbers[0].ToString() + " " + opString + " " + ridd.Numbers[1].ToString() + " = ?";

        for (int i = 0; i < 4; i++)
        {
            Options[i].GetComponentInChildren<TMP_Text>().text = ridd.Options[i].ToString();

            if (ridd.Options[i] == ridd.answer)
            {
                Options[i].onClick.RemoveAllListeners();
                Options[i].onClick.AddListener(OnRightAnswerPressed);
            }
            else
            {
                Options[i].onClick.RemoveAllListeners();

                Options[i].onClick.AddListener(OnWrongAnswerPressed);
            }
        }
    }

    public Riddle GenrateRiddle()
    {
        Riddle r = new Riddle();
        int EquationLength = 2;//length of equation
        for (int i = 0; i < EquationLength; i++)
        {
            r.Numbers.Add(Random.Range(1,10));
        }

        r.Operation = (MathOperations)Random.Range(0, 3);
        switch (r.Operation)
        {
            case MathOperations.Addition:
                r.answer = r.Numbers[0] + r.Numbers[1];
                break;
            case MathOperations.Subtraction:
                r.answer = r.Numbers[0] - r.Numbers[1];
                break;
            case MathOperations.Multiplication:
                r.answer = r.Numbers[0] * r.Numbers[1];
                break;
        }

        int AnswerOption = Random.Range(0,4);

        for (int i = 0; i < 4; i++)
        {
            if(i == AnswerOption)
            {
                r.Options.Add(r.answer);
            }
            else
            {
                r.Options.Add(CreateWrongAnswer(r.answer,r.Options));
            }
        }

        return r;
    }

    int CreateWrongAnswer(int rightAnswer, List<int> alreadyCreatedAnswers)
    {
        bool WrongAnswerCreated = false;
        int wrongAnswer = 0;
        while (!WrongAnswerCreated)
        {
            int offset = Random.Range(1,7);
            MathOperations operationToDo = (MathOperations)Random.Range(0,2);
            switch (operationToDo)
            {
                case MathOperations.Addition:
                    wrongAnswer = rightAnswer + offset;
                    break;
                default:
                    wrongAnswer = rightAnswer - offset;
                    break;
            }
            WrongAnswerCreated |= !alreadyCreatedAnswers.Contains(wrongAnswer);
        }
        return wrongAnswer;
    }

    void OnRightAnswerPressed()
    {
        SoundManager.Instance.PlayClickSound();
        SoundManager.Instance.PlayRightAnswerdSound();
        controller.GetHit();
        RiddleStrength -= 100;
        RiddleHealth.text = RiddleStrength.ToString();
        HealthBar.fillAmount = (float)RiddleStrength / (float)maxStrength;

        if (RiddleStrength <= 0)
        {
            controller.Die();
            SoundManager.Instance.PlayRiddleSolvedSound();
            Invoke("DelayedRiddleSolved",2);
        }
        else
        {
            ShowRiddleQ();
        }
    }

    void OnWrongAnswerPressed()
    {
        //player health can be added
        SoundManager.Instance.PlayClickSound();
        SoundManager.Instance.PlayWrongAnswerdSound();
        controller.Attack();
        Invoke("DelayedRiddleFailed", 2);
    }

    void DelayedRiddleSolved()
    {
        EventManager.TriggerEvent(EventNames.OnRiddleSolved, this);

    }

    void DelayedRiddleFailed()
    {
        EventManager.TriggerEvent(EventNames.OnRiddleFailed, null);

    }
}
