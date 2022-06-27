using UnityEngine;
using UnityEngine.EventSystems;

//todo: Not a perfect solution need to change this if I have time. (Give every tab a tab area which tracks if a tab needs to be closed)
namespace GlobalScripts.Creation
{
    public class SheetAreaController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private static SheetAreaController _current;
        public static SheetAreaController Current => _current;
       
        public GameObject sheet;
        public bool isOpenTab = false;

        private void Awake()
        {
            _current = this;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            GameManager.Current.mouseOverSheet = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            GameManager.Current.mouseOverSheet = false;
        }

        public void OnTabOpen(bool templateBar)
        {
            isOpenTab = true;
            if (templateBar)
            {
                sheet.GetComponent<RectTransform>().offsetMax = new Vector2(-350,sheet.GetComponent<RectTransform>().offsetMax.y);
            }
            else
            {
                sheet.GetComponent<RectTransform>().offsetMax = new Vector2(sheet.GetComponent<RectTransform>().offsetMax.x, -350);
            }
        }
        
        public void OnTabClose(bool templateBar)
        {
            isOpenTab = false;
            if (templateBar)
            {
                sheet.GetComponent<RectTransform>().offsetMax = new Vector2(-50,sheet.GetComponent<RectTransform>().offsetMax.y);
            }
            else
            {
                sheet.GetComponent<RectTransform>().offsetMax = new Vector2(sheet.GetComponent<RectTransform>().offsetMax.x, -50);
            }
        }
    }
}
