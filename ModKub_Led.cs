using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModKub_Controls_Library {
    public partial class ModKub_Led : UserControl {
        public ModKub_Led()
        {
            InitializeComponent();

            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.OnColor = Color.FromArgb(255, 153, 255, 54);
            this.OffColor = Color.FromArgb(255, 200, 200, 200);
            this.ReadOnlyIndicator_Color = Color.FromArgb(255, 200, 0, 0);
            this.BackColor = Color.Transparent;

            this.Size = new Size(20, 20);
        }
        #region Public and Private Members

        private Color _OnColor;
        private Color _OffColor;

        private Color _ReadOnlyIndicator_Color;
        private bool _ReadOnlyIndicator = false;

        private bool _on = true;

        /// <summary>
        /// Gets or Sets the ON color of the LED light
        /// </summary>
        [DefaultValue(typeof(Color), "153, 255, 54")]
        public Color OnColor
        {
            get { return _OnColor; }
            set
            {
                _OnColor = value;
                this.Invalidate();  // Redraw the control
            }
        }

        /// <summary>
        /// Gets or Sets the OFF color of the LED light
        /// </summary>
        [DefaultValue(typeof(Color), "200, 200, 200")]
        public Color OffColor
        {
            get { return _OffColor; }
            set
            {
                _OffColor = value;
                this.Invalidate();  // Redraw the control
            }
        }

        /// <summary>
        /// Gets or Sets the ReadOnlyIndicator color of the LED light
        /// </summary>
        [DefaultValue(typeof(Color), "200, 200, 200")]
        public Color ReadOnlyIndicator_Color
        {
            get { return _ReadOnlyIndicator_Color; }
            set
            {
                _ReadOnlyIndicator_Color = value;
                this.Invalidate();  // Redraw the control
            }
        }


        /// <summary>
        /// Gets or Sets whether the light is turned on
        /// </summary>
        [DefaultValue(typeof(bool), "false")]
        public bool On
        {
            get { return _on; }
            set
            {
                _on = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Defines if LED show a "corona circle" of read, as a READONLY-indication
        /// </summary>
        [DefaultValue(typeof(bool), "false")]
        public bool ReadOnlyIndicator
        {
            get { return _ReadOnlyIndicator; }
            set
            {
                _ReadOnlyIndicator = value;
                this.Invalidate();
            }
        }

        #endregion

        #region Disabled Properties

        [Browsable(false)]
        public virtual AccessibleRole AccessibleRole
        {
            get { return base.AccessibleRole; }
            set { base.AccessibleRole = value; }
        }
        
        [Browsable(false)]
        public virtual bool AutoScroll
        {
            get { return base.AutoScroll; }
            set { base.AutoScroll = value; }


        }

        [Browsable(false)]
        public virtual bool AllowDrop
        {
            get { return base.AllowDrop; }
            set { base.AllowDrop = value; }
        }
       
        [Browsable(false)]
        public virtual Size AutoScrollMargin
        {
            get { return base.AutoScrollMargin; }
            set { base.AutoScrollMargin = value; }
        }

        [Browsable(false)]
        public virtual Size AutoScrollMinSize
        {
            get { return base.AutoScrollMinSize; }
            set { base.AutoScrollMinSize = value; }
        }

        [Browsable(false)]
        public virtual bool AutoSize
        {
            get { return base.AutoSize; }
            set { base.AutoSize = value; }
        }

        [Browsable(false)]
        public virtual System.Drawing.Image BackgroundImage
        {
            get { return base.BackgroundImage; }
            set { base.BackgroundImage = value; }
        }

        [Browsable(false)]
        public virtual RightToLeft RightToLeft
        {
            get { return base.RightToLeft; }
            set { base.RightToLeft = value; }
        }

        [Browsable(false)]
        public virtual bool UseWaitCursor
        {
            get { return base.UseWaitCursor; }
            set { base.UseWaitCursor = value; }
        }
        
        [Browsable(false)]
        public virtual Padding Padding
        {
            get { return base.Padding; }
            set { base.Padding = value; }
        }


        #endregion

        #region Transpanency Methods

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x20;
                return cp;
            }
        }

        protected override void OnMove(EventArgs e)
        {
            RecreateHandle();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        { }

        #endregion

        #region Drawing Methods

        /// <summary>
        /// Handles the Paint event for this UserControl
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            // Create an offscreen graphics object for double buffering

            using (Bitmap offScreenBmp = new Bitmap(this.ClientRectangle.Width, this.ClientRectangle.Height))
            {
                using (System.Drawing.Graphics g = Graphics.FromImage(offScreenBmp))
                {
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    // Draw the control
                    drawControl(g);
                    // Draw the image to the screen
                    e.Graphics.DrawImageUnscaled(offScreenBmp, 0, 0);
                }
            }
        }

        /// <summary>
        /// Renders the control to an image
        /// </summary>
        /// <param name="g"></param>
        private void drawControl(Graphics g)
        {
            Color lightColor = (this.On) ? this.OnColor : this.OffColor;

            Rectangle paddedRectangle = new Rectangle
                (this.Padding.Left,
                 this.Padding.Top,
                 this.Width - (this.Padding.Left + this.Padding.Right + 1)-1,
                 this.Height - (this.Padding.Top + this.Padding.Bottom + 1)-1
                );
            int width = (paddedRectangle.Width < paddedRectangle.Height) ? paddedRectangle.Width : paddedRectangle.Height;
            Rectangle drawRectangle = new Rectangle(paddedRectangle.X, paddedRectangle.Y, width, width);

            // Draw the background ellipse
            if (drawRectangle.Width < 1) drawRectangle.Width = 1;
            if (drawRectangle.Height < 1) drawRectangle.Height = 1;
            g.FillEllipse(new SolidBrush(lightColor), drawRectangle);

            // Draw the border
            //float w = drawRectangle.Width;
            //g.SetClip(this.ClientRectangle);
            //if (this.On)
            g.DrawEllipse(new Pen(this.OffColor, 1F), drawRectangle);

            if (ReadOnlyIndicator)
            {
                g.DrawEllipse(new Pen(ReadOnlyIndicator_Color, 2F), drawRectangle);
            }

        }

        #endregion

    }
}
