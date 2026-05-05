using System.Collections;
using UnityEngine;
using TMPro;

public class VendorDialogue : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject bubblePanel;
    [SerializeField] private TextMeshProUGUI speechText;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] voiceClips;

    [Header("Timing")]
    [SerializeField] private float minInterval = 8f;
    [SerializeField] private float maxInterval = 12f;
    [SerializeField] private float fadeTime = 0.25f;
    [SerializeField] private float extraVisibleTime = 0.5f;

    [Header("Phrases")]
    [TextArea]
    [SerializeField] private string[] phrases =
    {
        "Tá demorando muito.",
        "Vou morrer de tanto esperar.",
        "Dá para você se apressar com isso?",
        "Não tenho o dia todo.",
        "Você vai comprar ou só veio olhar?",
        "Meu tempo vale ouro, sabia?"
    };

    private Coroutine dialogueRoutine;
    private int lastIndex = -1;

    private void Start()
    {
        HideBubbleInstant();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        dialogueRoutine = StartCoroutine(DialogueLoop());
    }

    private IEnumerator DialogueLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));

            yield return ShowRandomPhrase();
        }
    }

    private IEnumerator ShowRandomPhrase()
    {
        if (phrases == null || phrases.Length == 0)
            yield break;

        int index = GetRandomIndexWithoutRepeat();

        speechText.text = phrases[index];
        bubblePanel.SetActive(true);

        yield return FadeIn();

        if (audioSource != null && voiceClips != null && index < voiceClips.Length && voiceClips[index] != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(voiceClips[index]);
        }

        float waitTime = extraVisibleTime;

        if (audioSource != null && index < voiceClips.Length && voiceClips[index] != null)
            waitTime = Mathf.Max(waitTime, voiceClips[index].length + extraVisibleTime);

        yield return new WaitForSeconds(waitTime);

        yield return FadeOut();
    }

    private int GetRandomIndexWithoutRepeat()
    {
        if (phrases.Length == 1)
            return 0;

        int index;
        do
        {
            index = Random.Range(0, phrases.Length);
        }
        while (index == lastIndex);

        lastIndex = index;
        return index;
    }

    private IEnumerator FadeIn()
    {
        if (canvasGroup == null)
            yield break;

        canvasGroup.alpha = 0f;

        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeTime);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOut()
    {
        if (canvasGroup == null)
        {
            bubblePanel.SetActive(false);
            yield break;
        }

        float startAlpha = canvasGroup.alpha;
        float t = 0f;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t / fadeTime);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        bubblePanel.SetActive(false);
    }

    private void HideBubbleInstant()
    {
        if (bubblePanel != null)
            bubblePanel.SetActive(false);

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
    }
}