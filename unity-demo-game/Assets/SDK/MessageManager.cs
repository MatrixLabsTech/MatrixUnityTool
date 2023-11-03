using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageManager : MonoBehaviour
{
    // Reference to the message GameObject you want to destroy
    public string msg;
    // Start is called before the first frame update
    void Start()
    {
        // Call a coroutine to destroy the messageObject after 5 seconds
        StartCoroutine(DestroyMessageAfterDelay(5f));

        // Get the first child GameObject
        Transform firstChild = transform.GetChild(0);

        // Check if the first child exists and has a Text component
        if (firstChild != null)
        {
            Text textComponent = firstChild.GetComponent<Text>();

            // Check if the first child has a Text component
            if (textComponent != null)
            {
                // Set the text of the Text component
                textComponent.text = msg;
            }
        }

    }

    // Coroutine to destroy the messageObject after a delay
    private IEnumerator DestroyMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Check if the messageObject exists before destroying it
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // Your update logic here
    }
}
