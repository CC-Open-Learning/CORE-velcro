using UnityEngine;

namespace VARLab.Velcro
{
    [CreateAssetMenu(fileName = "ImageDialogSO", menuName = "ScriptableObjects/Dialogs/ImageDialogSO")]
    public class ImageDialogSO : BaseDialogSO
    {
        [TextArea(1, 10), Tooltip("The [SubDescription] label of the dialog")]
        public string SubDescription;

        [TextArea(1, 10), Tooltip("The [Note] label of the dialog")]
        public string Note;

        [Tooltip("The [Image] sprite of the dialog")]
        public Sprite DialogImage;

        [Tooltip("The [NoteImage] sprite of the dialog")]
        public Sprite NoteImage;
    }
}