using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ExpandButton : MonoBehaviour
{
    enum Status { Collapsed, Expanded};
    private Status PanelStatus;
    [SerializeField]
    private RectTransform ReviewsPanel;
    [SerializeField]
    private TextMeshProUGUI TextComponent;
    public void ButtonPressed()
    {
        if (PanelStatus== Status.Collapsed)
        {
            this.Enable();
        }
        else if (PanelStatus== Status.Expanded)
        {
            this.Disable();
        }
    }

    public void Enable()
    {
        this.PanelStatus = Status.Expanded;
        this.ReviewsPanel.gameObject.SetActive(true);
        this.TextComponent.text = "<";
    }
    public void Disable()
    {
        this.PanelStatus = Status.Collapsed;
        this.ReviewsPanel.gameObject.SetActive(false);
        this.TextComponent.text = ">";
    }

    // Start is called before the first frame update
    void Start()
    {
        this.Disable();
    }
}
