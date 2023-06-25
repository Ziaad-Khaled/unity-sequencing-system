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
        if (action.animationClip != null)
        {
            playerAnimator.Play(action.animationClip.name);

            if (action.waitForAnimation)
            {
                yield return new WaitForSeconds(action.animationClip.length);
            }
        }

        if (action.audioClip != null)
        {
            audioSource.clip = action.audioClip;
            audioSource.Play();

            if (action.waitForAudio)
            {
                yield return new WaitForSeconds(action.audioClip.length);
            }
        }

        if (action.targetObject != null)
        {
            //action.targetObject.SetActive(!action.targetObject.activeSelf);
        }

        if (action.cameraStartPosition != Vector3.zero && action.cameraEndPosition != Vector3.zero)
        {
            float elapsedTime = 0f;

            while (elapsedTime < cameraMoveDuration)
            {
                virtualCameraTransform.position = Vector3.Lerp(action.cameraStartPosition, action.cameraEndPosition, elapsedTime / cameraMoveDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure the final position is set precisely
            virtualCameraTransform.position = action.cameraEndPosition;
        }

        currentIndex++;
        ExecuteSequence();
    }
}
