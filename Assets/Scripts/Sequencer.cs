using UnityEngine;

public class Sequencer : MonoBehaviour
{
    [System.Serializable]
    public class SequenceAction
    {
        public AnimationClip animationClip;
        public AudioClip audioClip;
        public GameObject targetObject;
        public Vector3 cameraStartPosition;
        public Vector3 cameraEndPosition;
        public bool waitForAnimation;
        public bool waitForAudio;
    }

    public SequenceAction[] sequenceActions;
    public Animator playerAnimator;
    public AudioSource audioSource;
    public Transform virtualCameraTransform;
    public float cameraMoveDuration = 2f;

    private int currentIndex;

    private void Start()
    {
        currentIndex = 0;
        ExecuteSequence();
    }

    public void ExecuteSequence()
    {
        if (currentIndex >= sequenceActions.Length)
        {
            Debug.Log("Sequence execution complete.");
            return;
        }

        SequenceAction currentAction = sequenceActions[currentIndex];
        StartCoroutine(ExecuteAction(currentAction));
    }

    private System.Collections.IEnumerator ExecuteAction(SequenceAction action)
    {
        Coroutine animationCoroutine = null;
        Coroutine audioCoroutine = null;

        if (action.animationClip != null)
        {
            animationCoroutine = StartCoroutine(PlayAnimation(action.animationClip));
        }

        if (action.audioClip != null)
        {
            audioCoroutine = StartCoroutine(PlayAudio(action.audioClip));
        }

        // Wait for both animation and audio to finish if necessary
        if (action.waitForAnimation && animationCoroutine != null)
        {
            yield return animationCoroutine;
        }

        if (action.waitForAudio && audioCoroutine != null)
        {
            yield return audioCoroutine;
        }

        // Rest of the code for other actions (game object, camera movement)

        currentIndex++;
        ExecuteSequence();
    }

    private System.Collections.IEnumerator PlayAnimation(AnimationClip animationClip)
    {
        playerAnimator.Play(animationClip.name);
        yield return new WaitForSeconds(animationClip.length);
    }

    private System.Collections.IEnumerator PlayAudio(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
        yield return new WaitForSeconds(audioClip.length);
    }
}