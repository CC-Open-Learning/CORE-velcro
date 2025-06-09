using UnityEngine;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    /// <summary>
    ///  Information dialogs have a few customizations available:
    ///     - Canvas Dim: The background of the information dialog can be dimmed to bring focus to the UI and block interaction with other UI
    ///     - Title: The title element can be toggled on or off
    ///     - Description: The description can be bolded or not
    ///     
    /// Intended use of this class is to connect InformationDialog.HandleDisplayUniqueUI() to a UnityEvent in your DLX. Methods changing
    /// content, canvas dim, and show/hide have been exposed if DLX would prefer to serialize and control individual stages of
    /// the information dialog
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class InformationDialog : BaseDialog
    {
        private const string DimmedBackgroundClass = "information-dialog-canvas";

        private void Start()
        {
            InitializeDialog(DimmedBackgroundClass);
        }

        /// <summary>
        /// This method is intended to be a single access public method to populate the information dialog with text, 
        /// buttons, and display it
        /// </summary>
        /// <param name="informationDialogSO"></param>
        public void HandleDisplayUI(InformationDialogSO informationDialogSO)
        {
            SetContent(informationDialogSO);    //Sets unique properties for info dialog
            Show();
        }

        /// <summary>
        /// Information dialogs only have one customization available:
        ///     - Canvas Dim: The background of the information dialog can be dimmed to bring focus to the UI and block interaction with other UI
        ///     - Title: The title element can be toggled on or off
        ///     - Description: The description can be bolded or not
        /// </summary>
        /// <param name="informationDialogSO"></param>
        public void SetContent(InformationDialogSO informationDialogSO)
        {
            base.SetContent(informationDialogSO);
        }
    }
}