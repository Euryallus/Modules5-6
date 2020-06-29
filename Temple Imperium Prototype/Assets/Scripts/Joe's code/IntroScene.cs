using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class IntroScene : MonoBehaviour
{
    //Set in inspector:
    [SerializeField]
    private TextMeshProUGUI[] textElements;
    [SerializeField]
    private TextMeshProUGUI textContinuePrompt;
    [SerializeField]
    private float animationSpeed;

    private TextMeshProUGUI activeTextElement;
    private int textElementIndex;
    private string fullText;
    private bool animatingText;

    private Coroutine animatingTextCoroutine;

    private void Start()
    {
        Time.timeScale = 1f;
        for (int i = 0; i < textElements.Length; i++)
        {
            textElements[i].gameObject.SetActive(false);
        }
        textContinuePrompt.gameObject.SetActive(false);

        StartAnimatingText(textElements[textElementIndex]);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
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
        if(activeTextElement != null)
        {
            activeTextElement.gameObject.SetActive(false);
        }

        activeTextElement = textElement;
        fullText = textElement.text;
        textElement.text = "";
        textElement.gameObject.SetActive(true);

        if (animatingText)
        {
            StopAnimatingText();
        }
        animatingTextCoroutine = StartCoroutine(AnimateText());
    }

    private void StopAnimatingText()
    {
        activeTextElement.text = fullText;
        StopCoroutine(animatingTextCoroutine);
        animatingText = false;
        textContinuePrompt.gameObject.SetActive(true);
    }

    private IEnumerator AnimateText()
    {
        animatingText = true;
        textContinuePrompt.gameObject.SetActive(false);

        bool triggerSound = true;

        int charIndex = 0;

        while (activeTextElement.text.Length < fullText.Length)
        {
            activeTextElement.text += fullText[charIndex];
            charIndex++;

            if(triggerSound)
                SoundEffectPlayer.instance.PlaySoundEffect2D("Typewriter Key", 0.6f, 0.98f, 1.02f);
            
            triggerSound = !triggerSound;

            yield return new WaitForSeconds(1f / animationSpeed);
        }

        SoundEffectPlayer.instance.PlaySoundEffect2D("Typewriter Ding", 0.8f, 0.98f, 1.02f);

        StopAnimatingText();
    }
}
