namespace 控件拖拽功能
{
    partial class Form2
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
            this.customDoubleUpDown1 = new UserC.CustomDoubleUpDown();
            this.intWrite = new UserC.CustomSwithBtn();
            this.intSwitch = new UserC.CustomSwithBtn();
            this.boolSwitch = new UserC.CustomSwithBtn();
            this.customInput2 = new UserC.CustomInput();
            this.customInput1 = new UserC.CustomInput();
            this.SuspendLayout();
            // 
            // customDoubleUpDown1
            // 
            this.customDoubleUpDown1.ControlName = null;
            this.customDoubleUpDown1.FBValue = 0D;
            this.customDoubleUpDown1.Location = new System.Drawing.Point(145, 350);
            this.customDoubleUpDown1.MaxValue = 10000D;
            this.customDoubleUpDown1.MinValue = 0D;
            this.customDoubleUpDown1.Name = "customDoubleUpDown1";
            this.customDoubleUpDown1.OutValue = 100D;
            this.customDoubleUpDown1.ReadEnable = true;
            this.customDoubleUpDown1.Size = new System.Drawing.Size(182, 67);
            this.customDoubleUpDown1.Step = 1D;
            this.customDoubleUpDown1.TabIndex = 6;
            this.customDoubleUpDown1.WriteAdr = "1214.PLC1.Val.Ctrl.item1.PDegree";
            // 
            // intWrite
            // 
            this.intWrite.BtnName = "intWrite";
            this.intWrite.IntWriteValue = 2;
            this.intWrite.Location = new System.Drawing.Point(401, 268);
            this.intWrite.Name = "intWrite";
            this.intWrite.ReadAddress = false;
            this.intWrite.ReadEnable = true;
            this.intWrite.Size = new System.Drawing.Size(90, 35);
            this.intWrite.TabIndex = 5;
            this.intWrite.TxtFont = new System.Drawing.Font("宋体", 9F);
            this.intWrite.TypeSel = UserC.CustomSwithBtn.ControlModeOptions.intWrite;
            this.intWrite.WriteAddress = "1200.PLC1.PowerCtrl.Fry.item2.PC_FryPowerCtrl";
            // 
            // intSwitch
            // 
            this.intSwitch.BtnName = "intSwitch";
            this.intSwitch.IntWriteValue = 1;
            this.intSwitch.Location = new System.Drawing.Point(401, 216);
            this.intSwitch.Name = "intSwitch";
            this.intSwitch.ReadAddress = false;
            this.intSwitch.ReadEnable = true;
            this.intSwitch.Size = new System.Drawing.Size(90, 35);
            this.intSwitch.TabIndex = 4;
            this.intSwitch.TxtFont = new System.Drawing.Font("宋体", 9F);
            this.intSwitch.TypeSel = UserC.CustomSwithBtn.ControlModeOptions.intSwith;
            this.intSwitch.WriteAddress = "1200.PLC1.PowerCtrl.Fry.item1.PC_FryPowerCtrl";
            // 
            // boolSwitch
            // 
            this.boolSwitch.BtnName = "boolSwitch";
            this.boolSwitch.IntWriteValue = 0;
            this.boolSwitch.Location = new System.Drawing.Point(401, 161);
            this.boolSwitch.Name = "boolSwitch";
            this.boolSwitch.ReadAddress = false;
            this.boolSwitch.ReadEnable = true;
            this.boolSwitch.Size = new System.Drawing.Size(90, 35);
            this.boolSwitch.TabIndex = 3;
            this.boolSwitch.TxtFont = new System.Drawing.Font("宋体", 9F);
            this.boolSwitch.TypeSel = UserC.CustomSwithBtn.ControlModeOptions.boolSwith;
            this.boolSwitch.WriteAddress = "1200.PLC1.PowerCtrl.ABB.item1.PC_Start";
            // 
            // customInput2
            // 
            this.customInput2.ControlName = null;
            this.customInput2.DataType = UserC.CustomInput.DataTypeOptions.intType;
            this.customInput2.DecimiPlace = 1;
            this.customInput2.Location = new System.Drawing.Point(182, 249);
            this.customInput2.Name = "customInput2";
            this.customInput2.OutValue = 0D;
            this.customInput2.ReadEnable = false;
            this.customInput2.Size = new System.Drawing.Size(105, 30);
            this.customInput2.Step = 1D;
            this.customInput2.TabIndex = 1;
            this.customInput2.WriteAdr = "1200.PLC1.PowerCtrl.Motor.item1.PC_CurrentSel";
            // 
            // customInput1
            // 
            this.customInput1.ControlName = null;
            this.customInput1.DataType = UserC.CustomInput.DataTypeOptions.flaotType;
            this.customInput1.DecimiPlace = 2;
            this.customInput1.Location = new System.Drawing.Point(57, 249);
            this.customInput1.Name = "customInput1";
            this.customInput1.OutValue = 0D;
            this.customInput1.ReadEnable = true;
            this.customInput1.Size = new System.Drawing.Size(105, 30);
            this.customInput1.Step = 1D;
            this.customInput1.TabIndex = 0;
            this.customInput1.WriteAdr = "1200.PLC1.Val.Ctrl.item1.PDegree";
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.customDoubleUpDown1);
            this.Controls.Add(this.intWrite);
            this.Controls.Add(this.intSwitch);
            this.Controls.Add(this.boolSwitch);
            this.Controls.Add(this.customInput2);
            this.Controls.Add(this.customInput1);
            this.Name = "Form2";
            this.Text = "Form2";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private UserC.CustomInput customInput1;
        private UserC.CustomInput customInput2;
        private UserC.CustomSwithBtn boolSwitch;
        private UserC.CustomSwithBtn intSwitch;
        private UserC.CustomSwithBtn intWrite;
        private UserC.CustomDoubleUpDown customDoubleUpDown1;
    }
}