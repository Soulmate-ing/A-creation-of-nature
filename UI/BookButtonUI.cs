using UnityEngine;
using UnityEngine.UI;

public class BookButtonUI : MonoBehaviour
{
    public GameObject Button;
    private void Start()
    {
        Button.SetActive(false); 
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Button.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Button.SetActive(false);
       }
    }
}