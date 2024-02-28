using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SettingsWindow : MonoBehaviour
{
    [Header("Button index opens page on same index in pages")]
    [SerializeField] SettingsPage[] pages;
    [SerializeField] SettingsTab[] tabs;

    int currentIndex = 0;
    private bool changing = false;

    void OnEnable()
    {
        if (pages.Length != tabs.Length)
        {
            Debug.LogError("settings menu is misconfigured, pages and tabs mismatched");
            return;
        }

        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].gameObject.SetActive(false);
            tabs[i].tabEnabled = false;
            tabs[i].index = i;
            tabs[i].settingsWindow = this;
        }

        tabs[currentIndex].tabEnabled = true;
        pages[currentIndex].gameObject.SetActive(true);
    }

    public async void ChangeTab(int index)
    {
        if (changing)
        {
            return;
        }

        changing = true;

        tabs[currentIndex].tabEnabled = false;
        pages[currentIndex].GetComponent<Animator>().SetTrigger("Flip");
        tabs[index].tabEnabled = true;
        await Task.Delay(667);
        pages[currentIndex].gameObject.SetActive(false);

        tabs[index].tabEnabled = true;
        pages[index].gameObject.SetActive(true);
        currentIndex = index;

        changing = false;
    }
}
