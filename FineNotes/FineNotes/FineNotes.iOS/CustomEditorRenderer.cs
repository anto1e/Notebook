using FineNotes;
using FineNotes.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomEditor), typeof(CustomEditorRenderer))]
namespace FineNotes.iOS
{

    class CustomEditorRenderer : EditorRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            // Use tint color to change the cursor's color
            //Control.TintColor = UIColor.Blue;
        }
    }
}

