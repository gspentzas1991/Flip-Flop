using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    /// <summary>
    /// Keeps track of which dialogue must be displayed
    /// </summary>
    public int dialogueStep = 1;
    [SerializeField] Text QdialogueText;
    [SerializeField] Text EdialogueText;
    [SerializeField] Image QdialogueBubble;
    [SerializeField] Image EdialogueBubble;
    private List<DialogueItem> dialogueList = new List<DialogueItem>();


    // Start is called before the first frame update
    void Start()
    {
        dialogueList.Add( new DialogueItem(Character.Q, 1, "Hi buddy, You must be the new intern!"));
        dialogueList.Add(new DialogueItem(Character.Q, 2, "My name is Q and that guy over there is E"));
        dialogueList.Add(new DialogueItem(Character.E, 3, "...sup"));
        dialogueList.Add(new DialogueItem(Character.Q, 4, "Welcome to the Matching Factory! (tm). We're sure you're excited for your first day"));
        dialogueList.Add(new DialogueItem(Character.Q, 5, "Now I know this looks daunting, but we'll be right here to help you out. Can you spot a gray ball along with the other shapes?"));
        dialogueList.Add(new DialogueItem(Character.Q, 6, "That's your cursor. You can move it up with the W key, or down with the S key"));
        dialogueList.Add(new DialogueItem(Character.E, 7, "...sounds pretty limited..."));
        dialogueList.Add(new DialogueItem(Character.Q, 8, "Oh ho ho, but this is where WE come in. Shout at us using the Q and E keys, and we will rotate this whole machine for you"));
        dialogueList.Add(new DialogueItem(Character.Q, 9, "That way you will be able to move any direction you want! Your job is to use that cursor and create as many matches of 3 shapes as you can"));
        dialogueList.Add(new DialogueItem(Character.E, 10, "...not sure how the factory is making money..."));
        dialogueList.Add(new DialogueItem(Character.Q, 11, "One last thing! As you continue matching shapes, you will fill a special bomb meter. It's that circle in the upper left!"));
        dialogueList.Add(new DialogueItem(Character.E, 12, "...wait...there's an actual bomb in here?"));
        dialogueList.Add(new DialogueItem(Character.Q, 13, "When this circle turns yellow, you can press the Spacebar key to make 5 random shapes EXPLODE!"));
        dialogueList.Add(new DialogueItem(Character.E, 14, "...we should start wearing helmets"));
        dialogueList.Add(new DialogueItem(Character.Q, 15, "Good luck buddy! Me and E will be right here when you call us. Now let's match some shapes!"));
        DisplayDialogue(dialogueList[0]);

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.isInDialogue)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                dialogueStep++;
                if (dialogueStep>=dialogueList.Count)
                {
                    ExitDialogue();
                }
                else
                {
                    DisplayDialogue(dialogueList[dialogueStep]);
                }
            }
        }
    }

    private void DisplayDialogue(DialogueItem dialogue)
    {
        QdialogueBubble.gameObject.SetActive(false);
        EdialogueBubble.gameObject.SetActive(false);
        if (dialogue.Character == Character.Q)
        {
            QdialogueBubble.gameObject.SetActive(true);
            QdialogueText.text = dialogue.DialogueText;
        }
        else
        {
            EdialogueBubble.gameObject.SetActive(true);
            EdialogueText.text = dialogue.DialogueText;
        }
    }

    private void ExitDialogue()
    {
        QdialogueBubble.gameObject.SetActive(false);
        EdialogueBubble.gameObject.SetActive(false);
        GameManager.Instance.isInDialogue = false;
    }
}
