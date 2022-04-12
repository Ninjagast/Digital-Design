using UnityEngine;
using UnityEngine.EventSystems;

//todo: Need to resize the SheetArea when tab button gets pressed. For obvious reasons
namespace GlobalScripts
{
    public class SheetAreaController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            GameManager.Current.MouseOverSheet = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            GameManager.Current.MouseOverSheet = false;
        }
    }
}
