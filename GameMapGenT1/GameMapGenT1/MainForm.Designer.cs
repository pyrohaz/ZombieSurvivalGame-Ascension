/*
 * Created by SharpDevelop.
 * User: harri
 * Date: 23/06/2017
 * Time: 19:32
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace GameMapGenT1
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.panel = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.textBox_xsize = new System.Windows.Forms.TextBox();
			this.textBox_ysize = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.button_setsize = new System.Windows.Forms.Button();
			this.tabControl_img = new System.Windows.Forms.TabControl();
			this.button_add = new System.Windows.Forms.Button();
			this.label_obj = new System.Windows.Forms.Label();
			this.button_export = new System.Windows.Forms.Button();
			this.button_addline = new System.Windows.Forms.Button();
			this.button_removeall = new System.Windows.Forms.Button();
			this.textBox_xmin = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textBox_xmax = new System.Windows.Forms.TextBox();
			this.textBox_ymax = new System.Windows.Forms.TextBox();
			this.textBox_ymin = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.checkBox_fixed = new System.Windows.Forms.CheckBox();
			this.button_random = new System.Windows.Forms.Button();
			this.button_redraw = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// panel
			// 
			this.panel.BackColor = System.Drawing.Color.White;
			this.panel.Location = new System.Drawing.Point(13, 13);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(400, 400);
			this.panel.TabIndex = 0;
			this.panel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.PanelMouseClick);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(420, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(45, 23);
			this.label1.TabIndex = 1;
			this.label1.Text = "X Size: ";
			// 
			// textBox_xsize
			// 
			this.textBox_xsize.Location = new System.Drawing.Point(471, 10);
			this.textBox_xsize.Name = "textBox_xsize";
			this.textBox_xsize.Size = new System.Drawing.Size(58, 20);
			this.textBox_xsize.TabIndex = 2;
			this.textBox_xsize.Text = "1000";
			// 
			// textBox_ysize
			// 
			this.textBox_ysize.Location = new System.Drawing.Point(471, 36);
			this.textBox_ysize.Name = "textBox_ysize";
			this.textBox_ysize.Size = new System.Drawing.Size(58, 20);
			this.textBox_ysize.TabIndex = 4;
			this.textBox_ysize.Text = "1000";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(420, 39);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(45, 23);
			this.label2.TabIndex = 3;
			this.label2.Text = "Y Size: ";
			// 
			// button_setsize
			// 
			this.button_setsize.Location = new System.Drawing.Point(535, 8);
			this.button_setsize.Name = "button_setsize";
			this.button_setsize.Size = new System.Drawing.Size(75, 23);
			this.button_setsize.TabIndex = 5;
			this.button_setsize.Text = "Set Size";
			this.button_setsize.UseVisualStyleBackColor = true;
			this.button_setsize.Click += new System.EventHandler(this.Button_setsizeClick);
			// 
			// tabControl_img
			// 
			this.tabControl_img.Location = new System.Drawing.Point(419, 65);
			this.tabControl_img.Name = "tabControl_img";
			this.tabControl_img.SelectedIndex = 0;
			this.tabControl_img.Size = new System.Drawing.Size(291, 291);
			this.tabControl_img.TabIndex = 6;
			// 
			// button_add
			// 
			this.button_add.Location = new System.Drawing.Point(420, 362);
			this.button_add.Name = "button_add";
			this.button_add.Size = new System.Drawing.Size(75, 23);
			this.button_add.TabIndex = 7;
			this.button_add.Text = "Add";
			this.button_add.UseVisualStyleBackColor = true;
			this.button_add.Click += new System.EventHandler(this.Button_addClick);
			// 
			// label_obj
			// 
			this.label_obj.Location = new System.Drawing.Point(535, 39);
			this.label_obj.Name = "label_obj";
			this.label_obj.Size = new System.Drawing.Size(86, 23);
			this.label_obj.TabIndex = 9;
			this.label_obj.Text = "Objects: 0";
			// 
			// button_export
			// 
			this.button_export.Location = new System.Drawing.Point(420, 391);
			this.button_export.Name = "button_export";
			this.button_export.Size = new System.Drawing.Size(75, 23);
			this.button_export.TabIndex = 10;
			this.button_export.Text = "Export";
			this.button_export.UseVisualStyleBackColor = true;
			this.button_export.Click += new System.EventHandler(this.Button_exportClick);
			// 
			// button_addline
			// 
			this.button_addline.Location = new System.Drawing.Point(501, 362);
			this.button_addline.Name = "button_addline";
			this.button_addline.Size = new System.Drawing.Size(75, 23);
			this.button_addline.TabIndex = 11;
			this.button_addline.Text = "Line";
			this.button_addline.UseVisualStyleBackColor = true;
			this.button_addline.Click += new System.EventHandler(this.Button_addlineClick);
			// 
			// button_removeall
			// 
			this.button_removeall.Location = new System.Drawing.Point(501, 390);
			this.button_removeall.Name = "button_removeall";
			this.button_removeall.Size = new System.Drawing.Size(75, 23);
			this.button_removeall.TabIndex = 12;
			this.button_removeall.Text = "Remove All";
			this.button_removeall.UseVisualStyleBackColor = true;
			this.button_removeall.Click += new System.EventHandler(this.Button_removeallClick);
			// 
			// textBox_xmin
			// 
			this.textBox_xmin.Location = new System.Drawing.Point(693, 10);
			this.textBox_xmin.Name = "textBox_xmin";
			this.textBox_xmin.Size = new System.Drawing.Size(58, 20);
			this.textBox_xmin.TabIndex = 14;
			this.textBox_xmin.Text = "0";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(620, 13);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(67, 23);
			this.label3.TabIndex = 13;
			this.label3.Text = "X Bounds:";
			// 
			// textBox_xmax
			// 
			this.textBox_xmax.Location = new System.Drawing.Point(757, 10);
			this.textBox_xmax.Name = "textBox_xmax";
			this.textBox_xmax.Size = new System.Drawing.Size(58, 20);
			this.textBox_xmax.TabIndex = 15;
			this.textBox_xmax.Text = "1000";
			// 
			// textBox_ymax
			// 
			this.textBox_ymax.Location = new System.Drawing.Point(757, 36);
			this.textBox_ymax.Name = "textBox_ymax";
			this.textBox_ymax.Size = new System.Drawing.Size(58, 20);
			this.textBox_ymax.TabIndex = 18;
			this.textBox_ymax.Text = "1000";
			// 
			// textBox_ymin
			// 
			this.textBox_ymin.Location = new System.Drawing.Point(693, 36);
			this.textBox_ymin.Name = "textBox_ymin";
			this.textBox_ymin.Size = new System.Drawing.Size(58, 20);
			this.textBox_ymin.TabIndex = 17;
			this.textBox_ymin.Text = "0";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(620, 39);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(67, 23);
			this.label4.TabIndex = 16;
			this.label4.Text = "Y Bounds:";
			// 
			// checkBox_fixed
			// 
			this.checkBox_fixed.Location = new System.Drawing.Point(716, 65);
			this.checkBox_fixed.Name = "checkBox_fixed";
			this.checkBox_fixed.Size = new System.Drawing.Size(104, 24);
			this.checkBox_fixed.TabIndex = 20;
			this.checkBox_fixed.Text = "Fixed Object";
			this.checkBox_fixed.UseVisualStyleBackColor = true;
			// 
			// button_random
			// 
			this.button_random.Location = new System.Drawing.Point(582, 362);
			this.button_random.Name = "button_random";
			this.button_random.Size = new System.Drawing.Size(75, 23);
			this.button_random.TabIndex = 21;
			this.button_random.Text = "Random";
			this.button_random.UseVisualStyleBackColor = true;
			this.button_random.Click += new System.EventHandler(this.Button_randomClick);
			// 
			// button_redraw
			// 
			this.button_redraw.Location = new System.Drawing.Point(582, 390);
			this.button_redraw.Name = "button_redraw";
			this.button_redraw.Size = new System.Drawing.Size(75, 23);
			this.button_redraw.TabIndex = 22;
			this.button_redraw.Text = "Redraw";
			this.button_redraw.UseVisualStyleBackColor = true;
			this.button_redraw.Click += new System.EventHandler(this.Button_redrawClick);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(890, 430);
			this.Controls.Add(this.button_redraw);
			this.Controls.Add(this.button_random);
			this.Controls.Add(this.checkBox_fixed);
			this.Controls.Add(this.textBox_ymax);
			this.Controls.Add(this.textBox_ymin);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textBox_xmax);
			this.Controls.Add(this.textBox_xmin);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.button_removeall);
			this.Controls.Add(this.button_addline);
			this.Controls.Add(this.button_export);
			this.Controls.Add(this.label_obj);
			this.Controls.Add(this.button_add);
			this.Controls.Add(this.tabControl_img);
			this.Controls.Add(this.button_setsize);
			this.Controls.Add(this.textBox_ysize);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textBox_xsize);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.panel);
			this.Name = "MainForm";
			this.Text = "GameMapGenT1";
			this.Resize += new System.EventHandler(this.MainFormResize);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Button button_redraw;
		private System.Windows.Forms.Button button_random;
		private System.Windows.Forms.CheckBox checkBox_fixed;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textBox_ymin;
		private System.Windows.Forms.TextBox textBox_ymax;
		private System.Windows.Forms.TextBox textBox_xmax;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBox_xmin;
		private System.Windows.Forms.Button button_removeall;
		private System.Windows.Forms.Button button_addline;
		private System.Windows.Forms.Button button_export;
		private System.Windows.Forms.Label label_obj;
		private System.Windows.Forms.Button button_add;
		private System.Windows.Forms.TabControl tabControl_img;
		private System.Windows.Forms.Button button_setsize;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBox_ysize;
		private System.Windows.Forms.TextBox textBox_xsize;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Panel panel;
	}
}
