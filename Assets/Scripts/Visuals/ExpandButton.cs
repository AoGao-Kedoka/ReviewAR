using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ExpandButton : MonoBehaviour
{
    enum Status { Collapsed, Expanded};
    private Status PanelStatus = Status.Collapsed;
    [SerializeField]
    private RectTransform ReviewsPanel;
    [SerializeField]
    private TextMeshProUGUI TextComponent;
    public void ButtonPressed()
    {
        if (PanelStatus== Status.Collapsed)
        {
            this.PanelStatus = Status.Expanded;
            this.ReviewsPanel.gameObject.SetActive(true);
            this.TextComponent.text = "<";
        }
        else if (PanelStatus== Status.Expanded)
        {
            this.PanelStatus = Status.Collapsed;
            this.ReviewsPanel.gameObject.SetActive(false);
            this.TextComponent.text = ">";
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        this.ReviewsPanel.gameObject.SetActive(false);  
      //  this.TextComponent = this.GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
