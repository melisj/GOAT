using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RepDialogue : MonoBehaviour
{
    public List<string> CustSentenceStart = new List<string>();
    public List<string> RepPlusAnswers = new List<string>();
    public List<string> RepMinAnswers = new List<string>();
    public List<string> CustSentenceGoodEnd = new List<string>();
    public List<string> CustSentenceBadEnd = new List<string>();


    public Text CusSentStart;
    public Text CusSentEnd;
    public Text AnswerGood;
    public Text AnswerBad;

    public int sequenceRand;
    public bool isAnswered;
    // Start is called before the first frame update
    void Start()
    {
        isAnswered = false;

        CusSentStart = GameObject.Find("SentenceOne").GetComponent<Text>();
        CusSentEnd = GameObject.Find("SentenceTwo").GetComponent<Text>();
        AnswerGood = GameObject.Find("AnswerGood").GetComponent<Text>();
        AnswerBad = GameObject.Find("AnswerBad").GetComponent<Text>();

        CustSentenceStart.Clear();
        // fill List
        CustSentenceStart.Add("Hello, Can I pay for this");
        CustSentenceStart.Add("Good day, I want to pay for this");
        CustSentenceStart.Add("HELLO, I WAS FIRST");

        CustSentenceGoodEnd.Clear();
        // fill List
        CustSentenceGoodEnd.Add("Goodbye");
        CustSentenceGoodEnd.Add("Farewell");
        CustSentenceGoodEnd.Add("I WANT TO SPEAK TO THE MANAGER. AND YOU WILL BE FIRED. YOU WILL HEAR FROM MY ATTORNEY");

        CustSentenceBadEnd.Clear();
        // fill List
        CustSentenceBadEnd.Add("Well , I ... you... You will hear from me");
        CustSentenceBadEnd.Add("Uhm yes goodbye I think");
        CustSentenceBadEnd.Add("Yes thank you I knew you would listen to such a nice lady as myself");

        RepPlusAnswers.Clear();
        RepPlusAnswers.Add("I will help you.");
        RepPlusAnswers.Add("Yes here you go.");
        RepPlusAnswers.Add("No Ma'am you were not first you can leave.");
        // fill List

        RepMinAnswers.Clear();
        RepMinAnswers.Add("You can leave");
        RepMinAnswers.Add("uhm, yes whatever..");
        RepMinAnswers.Add("Yes because you say so");
        // fill List
        
        sequenceRand = Random.Range(0, (CustSentenceStart.Count));

        DialogueStart();


    }

    // Update is called once per frame
    void Update()
    {
        /*
         * random sentence uit list custsenstart
         */


    }

    public void DialogueStart()
    {
        /* display Random CustSenStart
         * 
         * if isAnswered == true
         * display CustSenEnd
         */
        CusSentStart.text = CustSentenceStart[sequenceRand];
        AnswerGood.text = RepPlusAnswers[sequenceRand];
        AnswerBad.text = RepMinAnswers[sequenceRand];


    }
    public void GoodAnswers()
    {
        /*
         * Display good answer (based on CustsenStart) , Display bad answer (based on CutsenStart)
         * reputation ++ // reputation  --
         * isAnswerd = true
         */
        Debug.Log("Yesh");

        isAnswered = true;

        if (isAnswered == true)
        {
            CusSentEnd.text = CustSentenceGoodEnd[sequenceRand];

        }

    }
    public void BadAnswers()
    {
        isAnswered = true;
        if (isAnswered == true)
        {
            CusSentEnd.text = CustSentenceBadEnd[sequenceRand];

        }
    }

}

