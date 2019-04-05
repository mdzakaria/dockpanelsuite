using System.Drawing;
using System.Windows.Forms;

namespace WeifenLuo.WinFormsUI.Util
{
    /// <summary>
    /// Show tooltip with image
    /// </summary>
    public class ImageToolTip : ToolTip
    {
        #region Constructors

        /// <summary>
        /// Show tooltip with image
        /// </summary>
        public ImageToolTip()
        {
            Popup += ImageTip_Popup;
            Draw += ImageTip_Draw;

            OwnerDraw = true;
            //Must be set otherwise will not draw the image properly
            IsBalloon = false;
            //this.ShowAlways = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Image to be shown
        /// </summary>
        public Image ToolTipImage { get; set; }

        #endregion

        #region Methods

        private void ImageTip_Draw(object sender, DrawToolTipEventArgs e)
        {
            if (ToolTipImage != null)
                e.Graphics.DrawImage(ToolTipImage, 0, 0);
        }

        private void ImageTip_Popup(object sender, PopupEventArgs e)
        {
            //Creates the popup and sets its dimensions from the resource name
            string ToolText = null;

            ToolText = GetToolTip(e.AssociatedControl);
            if (!string.IsNullOrEmpty(ToolText))
            {
                if (ToolTipImage != null)
                    e.ToolTipSize = new Size(ToolTipImage.Width, ToolTipImage.Height);
                else
                    e.Cancel = true;
            }
        }

        #endregion
    } //End Class
}