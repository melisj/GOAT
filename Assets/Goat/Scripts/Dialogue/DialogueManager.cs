using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Text nameText;
    public Text dialogueText;  
    public Animator animator;
    public List<string> firstNames = new List<string>();
    public List<string> lastNames = new List<string>();

    int NameNumber;
    int listMax;
    
    void Start()
    {
        firstNames.Clear();
        firstNames.Add("Grogu");
        firstNames.Add("Shifu");
        firstNames.Add("Koli");

        lastNames.Clear();
        lastNames.Add("Hopsnop");
        lastNames.Add("Fropstop");
        lastNames.Add("Gorbluk");
        lastNames.Add("Druppok");

    }
    void Update()
    {
        if (firstNames.Count < lastNames.Count)
        {
            listMax = lastNames.Count + 1;
        }
        else
            listMax = firstNames.Count + 1;

       NameNumber = Random.Range(0, listMax);
      
    }
    public void StartDialogue(Dialogue dialogue)
    {
        animator.SetBool("IsOpen", true);

        for (int i = 0; i < NameNumber; i++)
        {           
                nameText.text = firstNames[i] + " " + lastNames[i];            
        }

        
    }

    public void DisplayNextSentence()
    {
        
        dialogueText.text = "Test";
      
      
        
    }

    void EndDialogue()
    {
        animator.SetBool("IsOpen", false);

    }

}
