using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

//------------------------------------------------------\\
//  Used for the intro scene which shows some animated  \\
//  text to introcuce the game's story                  \\
//------------------------------------------------------\\
//      Written by Joe for proof of concept phase       \\
//------------------------------------------------------\\

public class IntroScene : MonoBehaviour
{
    //Set in inspector:
    [SerializeField]
    private TextMeshProUGUI[] textElements;         //All text elements that are used to show story text
    [SerializeField]
    private TextMeshProUGUI textContinuePrompt;     //Text telling the player how to continue with the story
    [SerializeField]
    private float animationSpeed;                   //How quickly text is animated

    private TextMeshProUGUI activeTextElement;  //Text element currently being shown/animated
    private int textElementIndex;               //Index of the current text element in the textElements array
    private string fullText;                    //Full text to be shown for the active text element
    private bool animatingText;                 //True = text is currently animating in, false = done animating
    private Coroutine animatingTextCoroutine;   //Coroutine used for text animation

    private void Start()
    {
        //Reset timeScale in case game was paused
        Time.timeScale = 1f;

        //Hide all text elements by default
        for (int i = 0; i < textElements.Length; i++)
        {
            textElements[i].gameObject.SetActive(false);
        }
        textContinuePrompt.gameObject.SetActive(false);

        //Start animating the first text element
        StartAnimatingText(textElements[textElementIndex]);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Pressing the space key either skips the animation,
            //  or moves to the next text element if animating is done
            if (animatingText)
            {
                StopAnimatingText();
            }
            else
            {
                ContinueStory();
            }
        }
    }

    private void ContinueStory()
    {
        if(textElementIndex < textElements.Length - 1)
        {
            //Start animating the next text element
            textElementIndex++;
            StartAnimatingText(textElements[textElementIndex]);
        }
        else
        {
            //Done, continue to game
            SceneManager.LoadScene("MAP");
        }
    }

    private void StartAnimatingText(TextMeshProUGUI textElement)
    {
        //If there was an element previously being shown, hide it
        //  to avoid overlapping text
        if(activeTextElement != null)
        {
            activeTextElement.gameObject.SetActive(false);
        }

        //Set fullText based on the text set in the editor,
        //  then reset the element's text to nothing ready to start animating
        activeTextElement = textElement;
        fullText = textElement.text;
        textElement.text = "";
        textElement.gameObject.SetActive(true);

        //Stop any existing animation
        if (animatingText)
        {
            StopAnimatingText();
        }
        //Start animating the new text
        animatingTextCoroutine = StartCoroutine(AnimateText());
    }

    private void StopAnimatingText()
    {
        //Stop animating and skip straight to showing the full text
        activeTextElement.text = fullText;
        StopCoroutine(animatingTextCoroutine);
        animatingText = false;
        //Show the continue prompt since the player can now continue to the next text element
        textContinuePrompt.gameObject.SetActive(true);
    }

    //Animates text with a typewriter effect, one letter at a time
    private IEnumerator AnimateText()
    {
        animatingText = true;
        textContinuePrompt.gameObject.SetActive(false);

        //triggerSound alternates bewteen true and false each time a character is added
        //  so sounds only play half of the time to avoid them playing too frequently
        bool triggerSound = true;

        //Used to add text one character at a time
        int charIndex = 0;

        //Continue adding text while the full text is not being shown
        while (activeTextElement.text.Length < fullText.Length)
        {
            //Add a character
            activeTextElement.text += fullText[charIndex];
            charIndex++;

            //Play a sound every other loop
            if(triggerSound)
                SoundEffectPlayer.instance.PlaySoundEffect2D("Typewriter Key", 0.6f, 0.98f, 1.02f);
            triggerSound = !triggerSound;

            //Wait for a set amount of time based on animationSpeed before continuing the animaion
            yield return new WaitForSeconds(1f / animationSpeed);
        }

        //All characters are added, animating is done
        SoundEffectPlayer.instance.PlaySoundEffect2D("Typewriter Ding", 0.8f, 0.98f, 1.02f);
        StopAnimatingText();
    }
}
