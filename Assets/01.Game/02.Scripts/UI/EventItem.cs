using UnityEngine;

namespace BBG.PictureColoring
{
    public class EventItem:MonoBehaviour
    {
        [SerializeField] private string popupId;

        public void OnClicked()
        {
            PopupManager.Instance.Show(popupId);
        }

    }
}