using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class DialogueItem
    {
        /// <summary>
        /// Which character says the dialogue
        /// </summary>
        public Character Character;
        /// <summary>
        /// The dialogueStep in which the dialogueItem will appear
        /// </summary>
        public int DialogueStep;
        /// <summary>
        /// The actual text of the dialogue
        /// </summary>
        public string DialogueText;

        public DialogueItem(Character _character,int _dialogueStep, string _dialogueText)
        {
            Character = _character;
            DialogueStep = _dialogueStep;
            DialogueText = _dialogueText;
        }
    }
}
