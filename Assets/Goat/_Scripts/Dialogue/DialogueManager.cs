using Goat.AI;
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
    public NPC npc;
    private string inventoryList;

    public List<string> firstNames = new List<string>();
    public List<string> myDialogueOne = new List<string>();
    public List<string> myDialogueTwo = new List<string>();
    public List<string> myDialogueThree = new List<string>();
    public List<string> lastNames = new List<string>();
    public List<string> sentenceOne = new List<string>();
    public List<string> sentenceTwo = new List<string>();
    public List<string> sentenceThree = new List<string>();

    private int nameNumber;
    private int dialogueNumber;
    private int listNameMax;
    private int listDialogueMax;

    private bool senOneDone = false;
    private bool senTwoDone = false;
    private bool senThreeDone = false;

    private void Start()
    {
        Debug.LogFormat("{0}x{1,-10}{2,-20}", 10, "testitem", 357);
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

        myDialogueOne.Clear();
        myDialogueTwo.Clear();
        myDialogueThree.Clear();

        //check wich list is longest a use that one as max
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

        //check which list is longer and use that one as the max
        if (firstNames.Count < lastNames.Count)
        {
            listNameMax = lastNames.Count;
        }
        else
        {
            listNameMax = firstNames.Count;
        }
    }

    private void Update()
    {
        nameNumber = Random.Range(0, listNameMax);
        dialogueNumber = Random.Range(0, listDialogueMax);
    }

    public void StartDialogue(Dialogue dialogue)
    {
        animator.SetBool("IsOpen", true);
        //display the name
        for (int i = 0; i < nameNumber; i++)
        {
            nameText.text = firstNames[i] + " " + lastNames[i];
        }
    }

    public void EndDialogue()
    {
        animator.SetBool("IsOpen", false);
    }

    public void AnswerOne()
    {
        senOneDone = true;

        //display the sentences in the porper order and with the right randomness
        for (int i = 0; i < dialogueNumber; i++)
        {
            if (senOneDone == true)
            {
                dialogueOneText.text = sentenceOne[i];
            }
        }
    }

    public void AnswerTwo()
    {
        senTwoDone = true;
        for (int i = 0; i < dialogueNumber; i++)
        {
            dialogueTwoText.text = sentenceTwo[i] + inventoryList;
        }
    }

    public void AsnwerThree()
    {
        for (int i = 0; i < dialogueNumber; i++)
        {
            dialogueThreeText.text = sentenceThree[i];
            senThreeDone = true;
        }
    }

    private List<string> ShowInventory(NPC customer)
    {
        List<string> inventory = new List<string>();
        foreach (var item in customer.Inventory.Items)
        {
            inventory.Add(string.Format("{0}x{1,10}{2,20}", item.Value, item.Key, (item.Key.Price() * item.Value)));
        }

        return inventory;
    }
}