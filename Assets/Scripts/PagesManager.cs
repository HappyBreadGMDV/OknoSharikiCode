using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PagesManager : MonoBehaviour
{
    [System.Serializable]
    public class PageButton
    {
        public Button buttonPage;
        public GameObject page;
    }

    public List<PageButton> PagesButtons = new List<PageButton>();

    private PageButton currentPage;

    void Start()
    {
        // Инициализируем кнопки и показываем первую страницу, если есть
        foreach (var pb in PagesButtons)
        {
            if (pb.buttonPage != null)
            {
                // Добавляем слушатель нажатия
                var captured = pb; // захват переменной для замыкания
                pb.buttonPage.onClick.AddListener(() => SwitchToPage(captured));
            }
        }

        // Показать первую страницу по умолчанию
        if (PagesButtons.Count > 0)
        {
            SwitchToPage(PagesButtons[0]);
        }
    }

    public void SwitchToPage(PageButton target)
    {
        if (target == null || target.page == null) return;

        // Деактивируем все страницы
        foreach (var pb in PagesButtons)
        {
            if (pb.page != null)
                pb.page.SetActive(false);
        }

        // Активируем нужную страницу
        target.page.SetActive(true);

        currentPage = target;
    }

    // Открыть страницу по индексу (если нужно)
    public void SwitchToPageByIndex(int index)
    {
        if (index >= 0 && index < PagesButtons.Count)
        {
            SwitchToPage(PagesButtons[index]);
        }
    }
}