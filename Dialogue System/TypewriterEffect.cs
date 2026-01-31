using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>Makes the text print out slowly rather than instantly. Can be modified to run faster/slower in general and with specific chars.</summary>
public class TypewriterEffect : MonoBehaviour
{
    #region Member Variables 

    const float defaultPrintSpeed = 50.0f;
    float printingSpeed;

    public bool isRunning { get; private set; }

    private readonly List<Punctuation> punctuations = new List<Punctuation>()
    {
        new Punctuation(new HashSet<char>(){'.', '!', '?'}, 0.4f),
        new Punctuation(new HashSet<char>(){',', ';', ':'}, 0.3f)
    };

    private Coroutine typingCoroutine;

    #endregion

    #region Run & Stop

    /// <summary>Runs the typingCoroutine responsible for printing text into the dialogue box.</summary>
    public void Run(string dialogueText, TMP_Text textLabel, float printSpeed = defaultPrintSpeed, DialogueCharacters character = DialogueCharacters.bink)
    {
        printingSpeed = printSpeed > 0 ? printSpeed : defaultPrintSpeed;

        typingCoroutine = StartCoroutine(TypeText(dialogueText, textLabel, character));
    }

    /// <summary>Stops the typingCoroutine. No more text will be printed.</summary>
    public void Stop()
    {
        StopCoroutine(typingCoroutine);
        isRunning = false;
    }

    #endregion

    #region Text Printing

    /// <summary>Runs through the text of the Dialogue Object at a given print speed.</summary>
    /// <param string name="dialogueText">The string to be printed out.</param>
    /// <param TMP_Text name="textLabel">Where the text is being written to. Resides in the Dialogue UI.</param>
    /// <returns>yield return WaitForSeconds of waitTime if char is punctuation, else yield return null.</returns>
    IEnumerator TypeText(string dialogueText, TMP_Text textLabel, DialogueCharacters character = DialogueCharacters.bink)
    {
        isRunning = true;
        textLabel.text = string.Empty;

        float time = 0.0f;
        int charIndex = 0;

        while (charIndex < dialogueText.Length)
        {
            int lastCharIndex = charIndex;

            time += Time.unscaledDeltaTime * printingSpeed;
            charIndex = Mathf.FloorToInt(time);
            charIndex = Mathf.Clamp(charIndex, 0, dialogueText.Length);

            for (int i = lastCharIndex; i < charIndex; i++)
            {
                bool isLast = i >= dialogueText.Length - 1;

                // Replaces a sprite command with the sprite.
                if (dialogueText[i] == '<')
                {
                    string sprite = string.Empty;
                    for (int s = i; s < dialogueText.Length; s++)
                    {
                        sprite += dialogueText[s];
                        if (dialogueText[s] == '>')
                        {
                            i = s;
                            charIndex = s;
                            time = s;

                            break;
                        }
                    }
                    textLabel.text += sprite;
                    yield return null;
                }

                textLabel.text = dialogueText.Substring(0, i + 1);

                if (IsPunctuation(dialogueText[i], out float waitTime) && !isLast && !IsPunctuation(dialogueText[i + 1], out _))
                {
                    if (AudioManager.instance != null)
                        AudioManager.PlaySound(DialogueType.Punctuation, true);

                    yield return new WaitForSecondsRealtime(waitTime);
                }
            }

            switch (character)
            {
                case DialogueCharacters.merchant:
                    AudioManager.PlaySound(DialogueType.Merchant, true, false, 0.75f);
                    break;

                case DialogueCharacters.merchantHivemind:
                    AudioManager.PlaySound(DialogueType.MerchantHivemind, true, false, 0.75f);
                    break;

                case DialogueCharacters.unknown:
                    AudioManager.PlaySound(DialogueType.Unknown, true, false, 0.75f);
                    break;

                case DialogueCharacters.bink:
                default:
                    AudioManager.PlaySound(DialogueType.Bink, true, false, 0.75f);
                    break;
            }

            yield return null;
        }

        isRunning = false;
    }

    #endregion

    #region Check for Punctuation

    /// <summary>Checks an input char for whether it falls within a Punctuation set. If so, output the WaitTime float, else return false.</summary>
    /// <param char name="character">The character to be checked against the Punctuations.</param>
    /// <param out float name="waitTime">Outputs the WaitTime float of the Punctuation set.</param>
    /// <returns>True if given character is within a Punctuation set with a specified waitTime.</returns>
    bool IsPunctuation(char character, out float waitTime)
    {
        foreach (Punctuation punctuationCategory in punctuations)
        {
            if (punctuationCategory.Punctuations.Contains(character))
            {
                waitTime = punctuationCategory.WaitTime;
                return true;
            }
        }

        waitTime = 0.0f;
        return false;
    }

    #endregion

    #region Punctuation Struct

    /// <summary>Keeps track of specific punctuation to change the speed at which the dialogue prints.</summary>
    private readonly struct Punctuation
    {
        public readonly HashSet<char> Punctuations;
        public readonly float WaitTime;

        /// <summary>Creates a new HashSet of chars with a specified float for the wait time of those characters.</summary>
        /// <param HashSet<char> name="punctuations">A list of chars for this punctuation set.</param>
        /// <param float name="waitTime">The wait time those characters will cause in TypeText.</param>
        /// <returns>Void</returns>
        public Punctuation(HashSet<char> punctuations, float waitTime)
        {
            Punctuations = punctuations;
            WaitTime = waitTime;
        }
    }

    #endregion
}
