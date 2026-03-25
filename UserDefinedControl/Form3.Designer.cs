namespace UserDefinedControl
{
    partial class Form3
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonControl2 = new UserDefinedControl.ButtonControl();
            this.buttonControl4 = new UserDefinedControl.ButtonControl();
            this.SuspendLayout();
            // 
            // buttonControl2
            // 
            this.buttonControl2.BackColor = System.Drawing.Color.Transparent;
            this.buttonControl2.ButtonFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonControl2.ButtonOffColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(196)))), ((int)(((byte)(222)))));
            this.buttonControl2.ButtonOnColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(130)))), ((int)(((byte)(180)))));
            this.buttonControl2.ButtonText = "int按钮切换";
            this.buttonControl2.ButtonTextOffColor = System.Drawing.Color.Black;
            this.buttonControl2.ButtonTextOnColor = System.Drawing.Color.White;
            this.buttonControl2.ControlAddress = "";
            this.buttonControl2.ControlValueTypeMode = UserDefinedControl.ButtonControl.ButtonControlValueTypeMode.Int;
            this.buttonControl2.InitValue = ((short)(0));
            this.buttonControl2.Location = new System.Drawing.Point(53, 31);
            this.buttonControl2.Mode = UserDefinedControl.ButtonControl.ButtonMode.Toggle;
            this.buttonControl2.Name = "buttonControl2";
            this.buttonControl2.ReadBoolAddressMethod = null;
            this.buttonControl2.ReadIntAddressMethod = null;
            this.buttonControl2.SignalB = false;
            this.buttonControl2.SignalBAddress = "";
            this.buttonControl2.SignalC = false;
            this.buttonControl2.Size = new System.Drawing.Size(120, 40);
            this.buttonControl2.TabIndex = 10;
            this.buttonControl2.TargetValue = ((short)(1));
            this.buttonControl2.WriteBoolAddressMethod = null;
            this.buttonControl2.WriteIntAddressMethod = null;
            // 
            // buttonControl4
            // 
            this.buttonControl4.BackColor = System.Drawing.Color.Transparent;
            this.buttonControl4.ButtonFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonControl4.ButtonOffColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(196)))), ((int)(((byte)(222)))));
            this.buttonControl4.ButtonOnColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.buttonControl4.ButtonText = "bool按钮切换";
            this.buttonControl4.ButtonTextOffColor = System.Drawing.Color.Black;
            this.buttonControl4.ButtonTextOnColor = System.Drawing.Color.White;
            this.buttonControl4.ControlAddress = "";
            this.buttonControl4.ControlValueTypeMode = UserDefinedControl.ButtonControl.ButtonControlValueTypeMode.Bool;
            this.buttonControl4.InitValue = ((short)(0));
            this.buttonControl4.Location = new System.Drawing.Point(53, 137);
            this.buttonControl4.Mode = UserDefinedControl.ButtonControl.ButtonMode.Toggle;
            this.buttonControl4.Name = "buttonControl4";
            this.buttonControl4.ReadBoolAddressMethod = null;
            this.buttonControl4.ReadIntAddressMethod = null;
            this.buttonControl4.SignalB = false;
            this.buttonControl4.SignalBAddress = "";
            this.buttonControl4.SignalC = false;
            this.buttonControl4.Size = new System.Drawing.Size(120, 40);
            this.buttonControl4.TabIndex = 12;
            this.buttonControl4.TargetValue = ((short)(1));
            this.buttonControl4.WriteBoolAddressMethod = null;
            this.buttonControl4.WriteIntAddressMethod = null;
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.buttonControl4);
            this.Controls.Add(this.buttonControl2);
            this.Name = "Form3";
            this.Text = "Form3";
            this.Load += new System.EventHandler(this.Form3_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private ButtonControl buttonControl2;
        private ButtonControl buttonControl4;
    }
}