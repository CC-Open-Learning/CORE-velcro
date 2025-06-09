using UnityEngine;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    /// <summary>
    ///  Image dialogs have a few customizations available:
    ///     - Canvas Dim: The background of the information dialog can be dimmed to bring focus to the UI and block interaction with other UI
    ///     - Title: The title element can be toggled on or off
    ///     - Description: The description can be bolded or not
    ///     - NoteImage: The note image may be left empty
    ///     
    /// Intended use of this class is to connect ImageDialog.HandleDisplayUniqueUI() to a UnityEvent in your DLX. Methods changing
    /// content, canvas dim, and show/hide have been exposed if DLX would prefer to serialize and control individual stages of
    /// the image dialog
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class ImageDialog : BaseDialog
    {
        private VisualElement dialogImage;
        private VisualElement noteImage;

        private Label subDescriptionLabel;
        private Label noteLabel;

        private const string DimmedBackgroundClass = "image-dialog-canvas";


        private void Start()
        {
            InitializeDialog(DimmedBackgroundClass);

            subDescriptionLabel = Root.Q<VisualElement>("SubDescriptionContainer").Q<Label>("SubDescriptionLabel");
            noteLabel = Root.Q<VisualElement>("NoteContainer").Q<Label>("NoteLabel");
            dialogImage = Root.Q<VisualElement>("ImageContainer").Q<VisualElement>("ImageBackground").Q<VisualElement>("Image");
            noteImage = Root.Q<VisualElement>("NoteContainer").Q<VisualElement>("NoteImage");
        }

        /// <summary>
        /// This method is intended to be a single access public method to populate the image dialog with text, 
        /// buttons, sprites, and display it
        /// </summary>
        /// <param name="imageDialogSO"></param>
        public void HandleDisplayUI(ImageDialogSO imageDialogSO)
        {
            SetContent(imageDialogSO);
            Show();
        }

        /// <summary>
        /// Image dialogs have multiple customizations available:
        ///     - SubDescription: The subdescription may be left empty
        ///     - Note: The note may be left empty
        ///     - DialogImage: The dialog image may be left empty
        ///     - NoteImage: The note image may be left empty
        ///     
        /// </summary>
        /// <param name="imageDialogSO"></param>
        public void SetContent(ImageDialogSO imageDialogSO)
        {
            base.SetContent(imageDialogSO);

            subDescriptionLabel.SetElementText(imageDialogSO.SubDescription);
            noteLabel.SetElementText(imageDialogSO.Note);
            dialogImage.SetElementSprite(imageDialogSO.DialogImage);
            noteImage.SetElementSprite(imageDialogSO.NoteImage);
        }
    }
}