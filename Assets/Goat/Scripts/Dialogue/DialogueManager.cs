using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Text nameText;
    public Text dialogueOneText;
    public Text dialogueTwoText;
    public Text dialogueThreeText;
    public Animator animator;

    public List<string> firstNames = new List<string>();
    public List<string> lastNames = new List<string>();
    public List<string> sentenceOne = new List<string>();
    public List<string> sentenceTwo = new List<string>();
    public List<string> sentenceThree = new List<string>();

    int nameNumber;
    int dialogueNumber;
    int listNameMax;
    int listDialogueMax;

    void Start()
    {
        // First names
        firstNames.Clear();
        firstNames.Add("Grogu");
        firstNames.Add("Shifu");
        firstNames.Add("Koli");

        // Last names
        lastNames.Clear();
        lastNames.Add("Hopsnop");
        lastNames.Add("Fropstop");
        lastNames.Add("Gorbluk");
        lastNames.Add("Druppok");

        // The first sentences 
        sentenceOne.Clear();
        sentenceOne.Add("Hello,");
        sentenceOne.Add("Good day,");
        sentenceOne.Add("HELLO, I WAS FIRST");

        // The second sentences
        sentenceTwo.Clear();
        sentenceTwo.Add("I want to pay for");
        sentenceTwo.Add("How much is this");
        sentenceTwo.Add("YES,thank you I want to pay");

        // The third sentences
        sentenceThree.Clear();
        sentenceThree.Add("Goodbye");
        sentenceThree.Add("Farewell");
        sentenceThree.Add("Bye");

    }
    public void StartDialogue(Dialogue dialogue)
    {
        animator.SetBool("IsOpen", true);

        nameNumber = Random.Range(0, listNameMax);

        if (firstNames.Count < lastNames.Count)
        {
            listNameMax = lastNames.Count;
        }
        else
        {
            listNameMax = firstNames.Count;
        }
        for (int i = 0; i < nameNumber; i++)
        {           
                nameText.text = firstNames[i] + " " + lastNames[i];            
        }

    }
    public void DisplayNextSentence()
    {
        dialogueNumber = Random.Range(0, listDialogueMax);

        if (sentenceOne.Count > sentenceTwo.Count || sentenceOne.Count > sentenceThree.Count)
        {
            listDialogueMax = sentenceOne.Count + 1;
        }
        if (sentenceTwo.Count > sentenceOne.Count || sentenceTwo.Count > sentenceThree.Count)
        {
            listDialogueMax = sentenceTwo.Count + 1;
        }
        else
        {
            listDialogueMax = sentenceThree.Count + 1;
        }

        for (int i = 0; i < dialogueNumber; i++)
        {
            dialogueOneText.text = sentenceOne[i];
            dialogueTwoText.text = sentenceTwo[i];
            dialogueThreeText.text = sentenceThree[i];
        }         
    }

    void EndDialogue()
    {
        animator.SetBool("IsOpen", false);

    }

}
