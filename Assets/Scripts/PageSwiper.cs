using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class PageSwiper : MonoBehaviour, IDragHandler, IEndDragHandler {

    public TMP_Text iconText;
    public Vector3 panelLocation;
    public Vector3 bottomPanelLocation;
    public float percentThreshold = 0.6f;
    public float easing = 100f;
    public int totalPages = 5;
    public int currentPage = 1;
    public int previousPage = 1;
    private Vector3 updatedLocation;

    private float activeTabWidth = 418f * 0.3f;
    private float inActiveTabWidth;
    private float inActiveTabWidthPos;
    private float tabHeight;
    private float bufferWidth;
    private float buttonStartPosx;
    private float buttonStartPosy;

    // private GameObject button;
    private GameObject navBar;
    private GameObject bottomPanel;
    private GameObject icons;

    private Vector3 newPosActive;
    

    void Awake(){
        navBar = GameObject.Find("navBar");
        bottomPanel = GameObject.Find("bottomTab");
        icons = GameObject.Find("icons");
        // iconText = GetComponent<TMP_Text>();
    }

    // Start is called before the first frame update
    void Start(){
        inActiveTabWidth = (418f * 0.7f)/(totalPages -1);
        inActiveTabWidthPos = (Screen.width * 0.7f)/(totalPages -1);
        tabHeight = inActiveTabWidth;
        bufferWidth = activeTabWidth - inActiveTabWidth;
        panelLocation = transform.position;
        updatedLocation = panelLocation;
        bottomPanelLocation = bottomPanel.transform.position;
        var button = navBar.transform.GetChild(0).gameObject;
        buttonStartPosx = button.transform.position.x * 1.11f;
        buttonStartPosy = button.transform.position.y;
        changeTab();
    }


    public void OnDrag(PointerEventData data){
        float difference = data.pressPosition.x - data.position.x;
        transform.position = panelLocation - new Vector3(difference, 0, 0);
        bottomPanel.transform.position = bottomPanelLocation + new Vector3(difference* 0.3f, 0, 0);
    }


    public void OnEndDrag(PointerEventData data){
        float percentage = (data.pressPosition.x - data.position.x) / Screen.width;

        if(Mathf.Abs(percentage) >= percentThreshold){
            Vector3 newLocation = panelLocation;
            if(percentage > 0 && currentPage < totalPages){
                currentPage++;
                newLocation += new Vector3(-Screen.width, 0, 0);
            }else if(percentage < 0 && currentPage > 1){
                currentPage--;
                newLocation += new Vector3(Screen.width, 0, 0);
            }
            StartCoroutine(SmoothMove(transform.position, newLocation, easing));
            panelLocation = newLocation;
            changeTab();
        }else{
            StartCoroutine(SmoothMove(transform.position, panelLocation, easing));
            // bottomPanel.transform.position = bottomPanelLocation;
            StartCoroutine(SmoothMoveObject(bottomPanel.transform.position, bottomPanelLocation, easing, bottomPanel));
        }
    }

    public void NavigationBarClick(PointerEventData data){

        var activeTabMin = newPosActive.x - 30;
        var activeTabMax = newPosActive.x + 30;

        var clickPosition = data.position.x;
        var index = currentPage-1;

        Vector3 newLocation = panelLocation;

        if(clickPosition < activeTabMin && currentPage > 1 ){
            index = (int)(clickPosition / inActiveTabWidthPos);
        }
        if(clickPosition > activeTabMax && currentPage < totalPages){
            index = (int)((clickPosition - inActiveTabWidthPos * 3) / inActiveTabWidthPos + 2);
        }

        if(index != currentPage-1){
            newLocation += new Vector3((Screen.width) * (currentPage - (int)index - 1), 0, 0);
            currentPage = (int)index +1;

            StartCoroutine(SmoothMove(panelLocation, newLocation, easing));
            panelLocation = newLocation;
            changeTab();
        }
    }

    private void changeTab(){

        for(int i=0; i< totalPages; i++){
            var button = navBar.transform.GetChild(i).gameObject;
            var icon = icons.transform.GetChild(i).gameObject;
            var rectTransform = button.GetComponent<RectTransform>();
            var iconScale = icon.GetComponent<RectTransform>();
            var buttonPosition = button.transform.position;
            var iconPosition = icon.transform.position;

            
            if(i == currentPage -1){

                rectTransform.sizeDelta = new Vector2(activeTabWidth, tabHeight);
                var iconS = iconScale.sizeDelta + new Vector2(inActiveTabWidthPos* 0.4f, inActiveTabWidthPos* 0.4f);
                newPosActive = new Vector3(buttonStartPosx + (inActiveTabWidthPos* i), buttonPosition.y, 0);
                var textPosition = new Vector3(buttonStartPosx + (inActiveTabWidthPos* i), buttonPosition.y * 0.5f, 0);
                var newPosIcon = new Vector3(buttonStartPosx + (inActiveTabWidthPos* i), buttonPosition.y * 2.2f, 0);
                
                
                StartCoroutine(SmoothMoveObject(button.transform.position, newPosActive, easing, button));
                StartCoroutine(SmoothMoveIcon(iconS, newPosIcon, easing, icon));
                StartCoroutine(SmoothMoveIcon(iconS - new Vector2(inActiveTabWidthPos* 0.3f, inActiveTabWidthPos* 0.3f), newPosIcon - new Vector3(0, buttonPosition.y * 0.5f, 0), easing, icon));
                iconText.text = "page" + (i+1).ToString();
                iconText.transform.position = textPosition;
            
                // button.transform.position = newPosActive;

                // icon.transform.position = newPosIcon;
            }
            else{
                rectTransform.sizeDelta = new Vector2(inActiveTabWidth, tabHeight);
                iconScale.sizeDelta = new Vector2(55, 55);
                
                var newPos = new Vector3(buttonStartPosx + (inActiveTabWidthPos* i ), buttonPosition.y, 0) + new Vector3(inActiveTabWidthPos * 0.4f, 0, 0);
                var newPosReverse = new Vector3(buttonStartPosx * 0.6f + (inActiveTabWidthPos* i), buttonPosition.y, 0);
                
                // Vector3.Lerp(rectTransform.sizeDelta, newScale, 0.3f);
                if(i == previousPage -1){
                    icon.transform.position = new Vector3(buttonStartPosx + (inActiveTabWidthPos* i), buttonPosition.y, 0);
                }
                // 

                if(i > currentPage - 1){
                    StartCoroutine(SmoothMoveObject(button.transform.position, newPos, easing, button));
                    StartCoroutine(SmoothMoveObject(icon.transform.position, newPos, easing, icon));
                    // button.transform.position = newPos;
                    // icon.transform.position = newPos;
                }
                else{
                    StartCoroutine(SmoothMoveObject(button.transform.position, newPosReverse, easing, button));
                    StartCoroutine(SmoothMoveObject(icon.transform.position, newPosReverse, easing, icon));
                    // button.transform.position = newPosReverse;
                    // icon.transform.position = newPosReverse;
                }
                
            }
        }
        previousPage = currentPage;
        StartCoroutine(SmoothMoveObject(bottomPanel.transform.position, newPosActive, easing, bottomPanel));
        // bottomPanel.transform.position = newPosActive;
        bottomPanelLocation = newPosActive;
    }


    IEnumerator SmoothMove(Vector3 startpos, Vector3 endpos, float seconds){
        float t = 0f;
        while(t <= 1.0){
            t += Time.deltaTime / seconds;
            transform.position = Vector3.Lerp(startpos, endpos, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
        transform.position = endpos;
    }

    IEnumerator SmoothMoveObject(Vector3 startpos, Vector3 endpos, float seconds, GameObject obj){
        float t = 0f;
        while(t <= 1.0){
            t += Time.deltaTime / seconds;
            obj.transform.position = Vector3.Lerp(startpos, endpos, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
        obj.transform.position = endpos;
    }

    IEnumerator SmoothMoveIcon(Vector2 endscale, Vector3 endpos, float seconds, GameObject icon){
        var iconScale = icon.GetComponent<RectTransform>();
        float t = 0f;
        while(t <= 1.0){
            iconScale.sizeDelta = Vector2.Lerp(iconScale.sizeDelta, endscale, Mathf.SmoothStep(0f, 1f, t/0.2f));
            icon.transform.position = Vector3.Lerp(icon.transform.position, endpos, Mathf.SmoothStep(0f, 1f, t/0.2f));
            t += Time.deltaTime;
            yield return null;
        }
    icon.transform.position = endpos;
    iconScale.sizeDelta = endscale;
    }
}