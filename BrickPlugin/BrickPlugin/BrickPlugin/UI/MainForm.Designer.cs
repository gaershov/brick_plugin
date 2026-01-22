using BrickPlugin;
using BrickPlugin.Models;
using BrickPlugin.Services;

namespace BrickPlugin
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            tableLayoutPanel1 = new TableLayoutPanel();
            label_Length = new Label();
            textBox_LengthValue = new TextBox();
            label_UnitLength = new Label();
            label_Width = new Label();
            textBox_WidthValue = new TextBox();
            label_UnitWidth = new Label();
            label_Height = new Label();
            textBox_HeightValue = new TextBox();
            label_UnitHeight = new Label();
            label_Radius = new Label();
            textBox_HoleRadiusValue = new TextBox();
            label_UnitRadius = new Label();
            label_MaxRadiusHint = new Label();
            label_RadiusCount = new Label();
            textBox_HolesCountValue = new TextBox();
            label_MaxHolesHint = new Label();
            panel1 = new Panel();
            button_Build = new Button();
            tableLayoutPanel1.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 4;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.Controls.Add(label_Length, 0, 0);
            tableLayoutPanel1.Controls.Add(textBox_LengthValue, 1, 0);
            tableLayoutPanel1.Controls.Add(label_UnitLength, 2, 0);
            tableLayoutPanel1.Controls.Add(label_Width, 0, 1);
            tableLayoutPanel1.Controls.Add(textBox_WidthValue, 1, 1);
            tableLayoutPanel1.Controls.Add(label_UnitWidth, 2, 1);
            tableLayoutPanel1.Controls.Add(label_Height, 0, 2);
            tableLayoutPanel1.Controls.Add(textBox_HeightValue, 1, 2);
            tableLayoutPanel1.Controls.Add(label_UnitHeight, 2, 2);
            tableLayoutPanel1.Controls.Add(label_Radius, 0, 3);
            tableLayoutPanel1.Controls.Add(textBox_HoleRadiusValue, 1, 3);
            tableLayoutPanel1.Controls.Add(label_UnitRadius, 2, 3);
            tableLayoutPanel1.Controls.Add(label_MaxRadiusHint, 3, 3);
            tableLayoutPanel1.Controls.Add(label_RadiusCount, 0, 4);
            tableLayoutPanel1.Controls.Add(textBox_HolesCountValue, 1, 4);
            tableLayoutPanel1.Controls.Add(label_MaxHolesHint, 3, 4);
            tableLayoutPanel1.Dock = DockStyle.Top;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.Padding = new Padding(5);
            tableLayoutPanel1.RowCount = 5;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.Size = new Size(600, 220);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // label_Length
            // 
            label_Length.Anchor = AnchorStyles.Left;
            label_Length.AutoSize = true;
            label_Length.Font = new Font("Segoe UI", 10F);
            label_Length.Location = new Point(8, 16);
            label_Length.Name = "label_Length";
            label_Length.Size = new Size(107, 19);
            label_Length.TabIndex = 0;
            label_Length.Text = "Длина кирпича";
            // 
            // textBox_LengthValue
            // 
            textBox_LengthValue.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textBox_LengthValue.BackColor = Color.White;
            textBox_LengthValue.BorderStyle = BorderStyle.FixedSingle;
            textBox_LengthValue.Font = new Font("Segoe UI", 10F);
            textBox_LengthValue.Location = new Point(155, 13);
            textBox_LengthValue.Name = "textBox_LengthValue";
            textBox_LengthValue.Size = new Size(141, 25);
            textBox_LengthValue.TabIndex = 0;
            textBox_LengthValue.Text = "0";
            // 
            // label_UnitLength
            // 
            label_UnitLength.Anchor = AnchorStyles.Left;
            label_UnitLength.AutoSize = true;
            label_UnitLength.Font = new Font("Segoe UI", 9F);
            label_UnitLength.ForeColor = Color.Gray;
            label_UnitLength.Location = new Point(302, 18);
            label_UnitLength.Name = "label_UnitLength";
            label_UnitLength.Size = new Size(72, 15);
            label_UnitLength.TabIndex = 1;
            label_UnitLength.Text = "100-1000мм";
            // 
            // label_Width
            // 
            label_Width.Anchor = AnchorStyles.Left;
            label_Width.AutoSize = true;
            label_Width.Font = new Font("Segoe UI", 10F);
            label_Width.Location = new Point(8, 58);
            label_Width.Name = "label_Width";
            label_Width.Size = new Size(119, 19);
            label_Width.TabIndex = 2;
            label_Width.Text = "Ширина кирпича";
            // 
            // textBox_WidthValue
            // 
            textBox_WidthValue.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textBox_WidthValue.BackColor = Color.White;
            textBox_WidthValue.BorderStyle = BorderStyle.FixedSingle;
            textBox_WidthValue.Font = new Font("Segoe UI", 10F);
            textBox_WidthValue.Location = new Point(155, 55);
            textBox_WidthValue.Name = "textBox_WidthValue";
            textBox_WidthValue.Size = new Size(141, 25);
            textBox_WidthValue.TabIndex = 1;
            textBox_WidthValue.Text = "0";
            // 
            // label_UnitWidth
            // 
            label_UnitWidth.Anchor = AnchorStyles.Left;
            label_UnitWidth.AutoSize = true;
            label_UnitWidth.Font = new Font("Segoe UI", 9F);
            label_UnitWidth.ForeColor = Color.Gray;
            label_UnitWidth.Location = new Point(302, 60);
            label_UnitWidth.Name = "label_UnitWidth";
            label_UnitWidth.Size = new Size(60, 15);
            label_UnitWidth.TabIndex = 3;
            label_UnitWidth.Text = "50-500мм";
            // 
            // label_Height
            // 
            label_Height.Anchor = AnchorStyles.Left;
            label_Height.AutoSize = true;
            label_Height.Font = new Font("Segoe UI", 10F);
            label_Height.Location = new Point(8, 100);
            label_Height.Name = "label_Height";
            label_Height.Size = new Size(112, 19);
            label_Height.TabIndex = 4;
            label_Height.Text = "Высота кирпича";
            // 
            // textBox_HeightValue
            // 
            textBox_HeightValue.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textBox_HeightValue.BackColor = Color.White;
            textBox_HeightValue.BorderStyle = BorderStyle.FixedSingle;
            textBox_HeightValue.Font = new Font("Segoe UI", 10F);
            textBox_HeightValue.Location = new Point(155, 97);
            textBox_HeightValue.Name = "textBox_HeightValue";
            textBox_HeightValue.Size = new Size(141, 25);
            textBox_HeightValue.TabIndex = 2;
            textBox_HeightValue.Text = "0";
            // 
            // label_UnitHeight
            // 
            label_UnitHeight.Anchor = AnchorStyles.Left;
            label_UnitHeight.AutoSize = true;
            label_UnitHeight.Font = new Font("Segoe UI", 9F);
            label_UnitHeight.ForeColor = Color.Gray;
            label_UnitHeight.Location = new Point(302, 102);
            label_UnitHeight.Name = "label_UnitHeight";
            label_UnitHeight.Size = new Size(60, 15);
            label_UnitHeight.TabIndex = 5;
            label_UnitHeight.Text = "30-300мм";
            // 
            // label_Radius
            // 
            label_Radius.Anchor = AnchorStyles.Left;
            label_Radius.AutoSize = true;
            label_Radius.Font = new Font("Segoe UI", 10F);
            label_Radius.Location = new Point(8, 142);
            label_Radius.Name = "label_Radius";
            label_Radius.Size = new Size(121, 19);
            label_Radius.TabIndex = 6;
            label_Radius.Text = "Радиус отверстий";
            // 
            // textBox_HoleRadiusValue
            // 
            textBox_HoleRadiusValue.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textBox_HoleRadiusValue.BackColor = Color.White;
            textBox_HoleRadiusValue.BorderStyle = BorderStyle.FixedSingle;
            textBox_HoleRadiusValue.Font = new Font("Segoe UI", 10F);
            textBox_HoleRadiusValue.Location = new Point(155, 139);
            textBox_HoleRadiusValue.Name = "textBox_HoleRadiusValue";
            textBox_HoleRadiusValue.Size = new Size(141, 25);
            textBox_HoleRadiusValue.TabIndex = 3;
            textBox_HoleRadiusValue.Text = "0";
            // 
            // label_UnitRadius
            // 
            label_UnitRadius.Anchor = AnchorStyles.Left;
            label_UnitRadius.AutoSize = true;
            label_UnitRadius.Font = new Font("Segoe UI", 9F);
            label_UnitRadius.ForeColor = Color.Gray;
            label_UnitRadius.Location = new Point(302, 144);
            label_UnitRadius.Name = "label_UnitRadius";
            label_UnitRadius.Size = new Size(60, 15);
            label_UnitRadius.TabIndex = 7;
            label_UnitRadius.Text = "мин. 2мм";
            // 
            // label_MaxRadiusHint
            // 
            label_MaxRadiusHint.Anchor = AnchorStyles.Left;
            label_MaxRadiusHint.AutoSize = true;
            label_MaxRadiusHint.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            label_MaxRadiusHint.ForeColor = Color.DarkGreen;
            label_MaxRadiusHint.Location = new Point(449, 144);
            label_MaxRadiusHint.Name = "label_MaxRadiusHint";
            label_MaxRadiusHint.Size = new Size(81, 15);
            label_MaxRadiusHint.TabIndex = 8;
            label_MaxRadiusHint.Text = "Макс: 30.0 мм";
            // 
            // label_RadiusCount
            // 
            label_RadiusCount.Anchor = AnchorStyles.Left;
            label_RadiusCount.AutoSize = true;
            label_RadiusCount.Font = new Font("Segoe UI", 10F);
            label_RadiusCount.Location = new Point(8, 175);
            label_RadiusCount.Name = "label_RadiusCount";
            label_RadiusCount.Size = new Size(86, 38);
            label_RadiusCount.TabIndex = 9;
            label_RadiusCount.Text = "Количество отверстий";
            // 
            // textBox_HolesCountValue
            // 
            textBox_HolesCountValue.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textBox_HolesCountValue.BackColor = Color.White;
            textBox_HolesCountValue.BorderStyle = BorderStyle.FixedSingle;
            textBox_HolesCountValue.Font = new Font("Segoe UI", 10F);
            textBox_HolesCountValue.Location = new Point(155, 181);
            textBox_HolesCountValue.Name = "textBox_HolesCountValue";
            textBox_HolesCountValue.Size = new Size(141, 25);
            textBox_HolesCountValue.TabIndex = 4;
            textBox_HolesCountValue.Text = "0";
            // 
            // label_MaxHolesHint
            // 
            label_MaxHolesHint.Anchor = AnchorStyles.Left;
            label_MaxHolesHint.AutoSize = true;
            label_MaxHolesHint.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            label_MaxHolesHint.ForeColor = Color.DarkGreen;
            label_MaxHolesHint.Location = new Point(449, 186);
            label_MaxHolesHint.Name = "label_MaxHolesHint";
            label_MaxHolesHint.Size = new Size(76, 15);
            label_MaxHolesHint.TabIndex = 10;
            label_MaxHolesHint.Text = "Макс: 42 шт";
            // 
            // panel1
            // 
            panel1.Controls.Add(button_Build);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 220);
            panel1.Name = "panel1";
            panel1.Size = new Size(600, 50);
            panel1.TabIndex = 1;
            // 
            // button_Build
            // 
            button_Build.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button_Build.BackColor = SystemColors.ButtonHighlight;
            button_Build.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            button_Build.ForeColor = Color.Black;
            button_Build.Location = new Point(450, 12);
            button_Build.Name = "button_Build";
            button_Build.Size = new Size(140, 32);
            button_Build.TabIndex = 5;
            button_Build.Text = "Построить кирпич";
            button_Build.UseVisualStyleBackColor = false;
            button_Build.Click += button_Build_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(600, 270);
            Controls.Add(panel1);
            Controls.Add(tableLayoutPanel1);
            Name = "MainForm";
            Text = "Плагин \"Кирпич\" КОМПАС-3D";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label_Height;
        private System.Windows.Forms.Label label_Length;
        private System.Windows.Forms.Label label_Width;
        private System.Windows.Forms.Label label_UnitRadius;
        private System.Windows.Forms.Label label_UnitHeight;
        private System.Windows.Forms.Label label_UnitWidth;
        private System.Windows.Forms.Label label_UnitLength;
        private System.Windows.Forms.Label label_MaxRadiusHint;
        private System.Windows.Forms.Label label_MaxHolesHint;
        private System.Windows.Forms.TextBox textBox_LengthValue;
        private System.Windows.Forms.Label label_RadiusCount;
        private System.Windows.Forms.Label label_Radius;
        private System.Windows.Forms.TextBox textBox_WidthValue;
        private System.Windows.Forms.TextBox textBox_HeightValue;
        private System.Windows.Forms.TextBox textBox_HoleRadiusValue;
        private System.Windows.Forms.TextBox textBox_HolesCountValue;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button_Build;
    }
}