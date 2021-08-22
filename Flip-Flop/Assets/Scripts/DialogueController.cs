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
    private List<DialogueItem> firstShiftDialogue = new List<DialogueItem>();
    private List<DialogueItem> secondShiftDialogue = new List<DialogueItem>();
    private List<DialogueItem> thirdShiftDialogue = new List<DialogueItem>();
    private List<DialogueItem> fourthShiftDialogue = new List<DialogueItem>();


    // Start is called before the first frame update
    void Start()
    {
        firstShiftDialogue.Add( new DialogueItem(Character.Q, 1, "Hi buddy, You must be the new intern!"));
        firstShiftDialogue.Add(new DialogueItem(Character.Q, 2, "My name is Q and that guy over there is E"));
        firstShiftDialogue.Add(new DialogueItem(Character.E, 3, "...sup"));
        firstShiftDialogue.Add(new DialogueItem(Character.Q, 4, "Welcome to the Matching Factory™! We're sure you're excited for your first day"));
        firstShiftDialogue.Add(new DialogueItem(Character.Q, 5, "Now I know this looks daunting, but we'll be right here to help you out. Can you spot a gray ball along with the other shapes?"));
        firstShiftDialogue.Add(new DialogueItem(Character.Q, 6, "That's your cursor. You can move it up and down using the W and S keys"));
        firstShiftDialogue.Add(new DialogueItem(Character.E, 7, "...sounds pretty limiting..."));
        firstShiftDialogue.Add(new DialogueItem(Character.Q, 8, "Oh ho ho, but this is where we come in. Shout at us using the Q and E keys, and we will rotate this whole machine for you!"));
        firstShiftDialogue.Add(new DialogueItem(Character.Q, 9, "Our job is to match three shapes together. We need to hit our shift target, before the end of our shift."));
        firstShiftDialogue.Add(new DialogueItem(Character.E, 10, "...not sure how the factory is making money..."));
        firstShiftDialogue.Add(new DialogueItem(Character.Q, 11, "One last thing! As you continue matching shapes, you will fill a special bomb meter. It's that circle in the upper left!"));
        firstShiftDialogue.Add(new DialogueItem(Character.E, 12, "...wait...there's an actual bomb in here?"));
        firstShiftDialogue.Add(new DialogueItem(Character.Q, 13, "When this circle turns yellow, you can press the F key to make 5 random shapes EXPLODE!"));
        firstShiftDialogue.Add(new DialogueItem(Character.E, 14, "...we should start wearing helmets"));
        firstShiftDialogue.Add(new DialogueItem(Character.Q, 15, "Good luck buddy! Me and E will be right here when you call us. Now let's match some shapes!"));

        secondShiftDialogue.Add(new DialogueItem(Character.Q, 1, "Great job pal, you're a natural at this!"));
        secondShiftDialogue.Add(new DialogueItem(Character.Q, 2, "Now things are going to get a bit tougher on this second shift"));
        secondShiftDialogue.Add(new DialogueItem(Character.Q, 3, "The boss wants us to hit higher targets in less time"));
        secondShiftDialogue.Add(new DialogueItem(Character.E, 4, "...should we start a union?"));
        secondShiftDialogue.Add(new DialogueItem(Character.Q, 5, "Let's give it our best pal!"));

        thirdShiftDialogue.Add(new DialogueItem(Character.Q, 1, "Good morning mate! Today E took a day off to go to his grandma's wedding"));
        thirdShiftDialogue.Add(new DialogueItem(Character.Q, 2, "So it's just the two of us. We will need to rotate the machine a bit more, but I'm sure we can handle it"));
        thirdShiftDialogue.Add(new DialogueItem(Character.Q, 3, "Let's have a great shift!"));

        fourthShiftDialogue.Add(new DialogueItem(Character.Q, 1, "Welcome back E! How was the wedding"));
        fourthShiftDialogue.Add(new DialogueItem(Character.E, 2, "...I'm hangover"));
        fourthShiftDialogue.Add(new DialogueItem(Character.Q, 3, "Sounds like a good time! Well friend-o today is our last shift and the deadline is pretty brutal"));
        fourthShiftDialogue.Add(new DialogueItem(Character.Q, 4, "But with the whole team back together, I'm sure we can do it!"));
        fourthShiftDialogue.Add(new DialogueItem(Character.E, 5, "...leave it to me!"));

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.isInDialogue)
        {
            List<DialogueItem> currentDialogue = new List<DialogueItem>();
            switch (GameManager.Instance.currentShift)
            {
                case 0:
                    currentDialogue = firstShiftDialogue;
                    break;
                case 1:
                    currentDialogue = secondShiftDialogue;
                    break;
                case 2:
                    currentDialogue = thirdShiftDialogue;
                    break;
                case 3:
                    currentDialogue = fourthShiftDialogue;
                    break;
            }
            //if both dialogueBubbles are inactive but we're in dialogue, then display the current dialogueItem
            if (QdialogueBubble.gameObject.activeInHierarchy == false && EdialogueBubble.gameObject.activeInHierarchy == false)
            {
                DisplayDialogue(currentDialogue[dialogueStep]);
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                dialogueStep++;
                if (dialogueStep>= currentDialogue.Count)
                {
                    ExitDialogue();
                }
                else
                {
                    DisplayDialogue(currentDialogue[dialogueStep]);
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
        dialogueStep = 0;
        GameManager.Instance.isInDialogue = false;
    }
}
