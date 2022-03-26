using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace Assets.Scripts.UI
{
    [RequireComponent(typeof(Image))]
    public class Tab : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
    {
        public TabGroup TabGroup;
        public Image Background;

        void Start()
        {
            Background = GetComponent<Image>();
            TabGroup.Subscribe(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            TabGroup.OnTabEnter(this);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            TabGroup.OnTabSelected(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TabGroup.OnTabExit(this);
        }
    }
}