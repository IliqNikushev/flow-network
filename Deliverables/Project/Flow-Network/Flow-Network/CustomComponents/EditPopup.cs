using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Flow_Network.CustomComponents
{
    public partial class EditPopup : UserControl
    {
        private Object valueObject;
        public Action OnFlowAltered = () => { };
        public Object Value
        {
            get { return valueObject; }
            set
            {
                if (this.valueObject == value) return;
                this.valueObject = value;

                if (this.valueObject == null)
                {
                    this.Hide();
                    return;
                }
                else
                {
                    if (!this.Visible) this.Show();
                }

                OnObjectChanged();
            }
        }

        protected virtual void OnObjectChanged() { }

        public EditPopup()
        {
            InitializeComponent();
            this.ParentChanged += (x, y) => this.Show();
        }

        public EditPopup(Object value)
            : this()
        {
            this.valueObject = value;
        }

        private bool isInitialized = false;

        public new void Show()
        {
            if (!isInitialized)
            {
                OnObjectChanged();
                isInitialized = true;
            }
            base.Show();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}