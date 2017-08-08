/*
 * Created by SharpDevelop.
 * User: harri
 * Date: 17/07/2017
 * Time: 16:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace ZombieSurvival1
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
			this.SuspendLayout();
			// 
			// panel
			// 
			this.panel.BackColor = System.Drawing.Color.White;
			this.panel.Location = new System.Drawing.Point(12, 12);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(640, 480);
			this.panel.TabIndex = 1;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(667, 505);
			this.Controls.Add(this.panel);
			this.Name = "MainForm";
			this.Text = "Ascension";
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Panel panel;
	}
}
