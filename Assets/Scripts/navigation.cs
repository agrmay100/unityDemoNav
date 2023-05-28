using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
// using PageSwiper;

public class navigation : MonoBehaviour, IPointerClickHandler, IDragHandler, IEndDragHandler
{
    // Start is called before the first frame update
    PageSwiper pageSwiper;
    [SerializeField] GameObject panelHolder;
    public Vector3 panelLocation;
    public float percentThreshold;
    public float easing;
    public int totalPages;
    private int currentPage;
    private Vector3 updatedLocation;


    void Awake(){
        pageSwiper = panelHolder.GetComponent<PageSwiper>();
     
    }

    public void OnPointerClick(PointerEventData data)
    {

        if(!data.dragging){
            
            pageSwiper.NavigationBarClick(data);
        }
    }

    void Start(){
        // Debug.Log(totalPages);
    }
    
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Update the Text on the screen depending on current position of the touch each frame
            // m_Text.text = "Touch Position : " + touch.position;
            Debug.Log(touch.position);
        }
        // else
        // {
        //     m_Text.text = "No touch contacts";
        // }
    }

    public void OnDrag(PointerEventData data){
        pageSwiper.OnDrag(data);
    }

    public void OnEndDrag(PointerEventData data){
        pageSwiper.OnEndDrag(data);
    }
}
