/*
 * Created by SharpDevelop.
 * User: harri
 * Date: 23/06/2017
 * Time: 19:32
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace GameMapGenT1
{
	public struct MapObject{
		public Point point;
		public int index;
		public bool fixedobj;
	};
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		List<int> controldists = new List<int>();
		int panelcontroldist;
		
		Random random;
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, panel, new object[] { true });
			
			random = new Random(DateTime.Now.GetHashCode());
			
			TabPage tb = new TabPage("Player");
			tabControl_img.TabPages.Add(tb);
			
			panelcontroldist = label1.Left-panel.Right;
			
			//Go through each object and get distance to right hand side
			for(int n = 0; n<this.Controls.Count; n++){
				if(this.Controls[n].Name != "panel"){
					int dists = this.Right - this.Controls[n].Left;
					controldists.Add(dists);
				}
			}
			
		}
		
		List<int> imghash = new List<int>();
		List<Color> imgcols = new List<Color>();
		List<MapObject> mapobjs = new List<MapObject>();
		List<string> objectfilenames = new List<string>();
		
		int xsize = 1000, ysize = 1000;
		
		void AddImage(string fn){
			//Ensure image doesn't exist
			bool exists = false;
			for(int n = 0; n<imghash.Count; n++){
				if(fn.GetHashCode() == imghash[n]){
					exists = true;
				}
			}
			
			if(exists == false){
				string nfn = fn.Substring(fn.LastIndexOf('\\')+1, fn.LastIndexOf('.')-fn.LastIndexOf('\\')-1);
				string nfn2 = fn.Substring(fn.LastIndexOf('\\')+1, fn.Length-fn.LastIndexOf('\\')-1);
				objectfilenames.Add(nfn2);
				Image img = Image.FromFile(fn);
				Image imgsc = (Image)(new Bitmap(img, new Size(200,200)));
				
				double r = 0, g = 0, b = 0;
				double div = 0;
				//Grab average colour
				for(int y = 0; y<200; y++){
					for(int x = 0; x<200; x++){
						Color c = ((Bitmap)imgsc).GetPixel(x,y);
						if(c.A != 0){
							r += (double)c.R;
							g += (double)c.G;
							b += (double)c.B;
							div += 1.0;
						}
					}
				}
				
				r /= div;
				g /= div;
				b /= div;
				
				Color ic = Color.FromArgb(255,(int)r,(int)g,(int)b);
				Graphics gfx = Graphics.FromImage(imgsc);
				Brush ebrush = new SolidBrush(ic);
				gfx.FillEllipse(ebrush, 0, 0, 20, 20);
				imgcols.Add(ic);
				
				TabPage tb = new TabPage(nfn);
				imghash.Add(fn.GetHashCode());
				tb.BackgroundImage = imgsc;
				tb.BackgroundImageLayout = ImageLayout.Center;
				Label lbl = new Label();
				TextBox textbox = new TextBox();
				lbl.Text = "Object name: ";
				lbl.Left = 0;
				lbl.Top = 0;
				textbox.Left = lbl.Right;
				textbox.Top = lbl.Top;
				tb.Controls.Add(lbl);
				tb.Controls.Add(textbox);
				tabControl_img.TabPages.Add(tb);
			}
		}
		
		void Button_addClick(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			
			ofd.InitialDirectory = "\\";
			ofd.Filter = "PNG files (*.png)|*.png";
			ofd.FilterIndex = 0;
			ofd.RestoreDirectory = true;
			ofd.Multiselect = true;
			
			if(ofd.ShowDialog() == DialogResult.OK){
				foreach(string s in ofd.FileNames){
					AddImage(s);
				}
			}
		}
		
		void Button_setsizeClick(object sender, EventArgs e)
		{
			bool success = false;
			
			success = int.TryParse(textBox_xsize.Text, out xsize) && int.TryParse(textBox_ysize.Text, out ysize);
			
			if(success){
				//ResizePanel();
			}
		}
		
		void ResizePanel(){
			int maxwidth = (label1.Left - panelcontroldist);
			int maxheight = (this.Bottom)-(this.Top) - 70;
			
			//Go through each object and move to right hand size apart from main panel
			for(int n = 0; n<this.Controls.Count; n++){
				if(this.Controls[n].Name != "panel"){
					this.Controls[n].Left = this.Right - controldists[n];
				}
			}
			
			if(xsize<=ysize){
				int desheight = maxheight;
				int deswidth = desheight*xsize/ysize;
				
				if(desheight>maxheight && deswidth<maxwidth){
					//desheight = maxheight;
					//deswidth = desheight*xsize/ysize;
					desheight = deswidth*xsize/ysize;
				}
				else if(desheight<maxheight && deswidth>maxwidth){
					deswidth = desheight*ysize/xsize;
					//desheight = deswidth*ysize/xsize;
				}
				/*else if(desheight>maxheight && deswidth>maxwidth){
					//Find desired with least error
					if(Math.Abs(deswidth-maxwidth)<Math.Abs(desheight-maxheight)){
						deswidth = maxwidth;
						desheight = deswidth*ysize/xsize;
					}
					else{
						desheight = maxheight;
						deswidth = desheight*xsize/ysize;
					}
				}*/
				
				Debug.WriteLine(deswidth + " " + desheight + "       " + maxwidth + " " + maxheight);
				
				panel.Width = deswidth;
				panel.Height = desheight;
			}
			else{
				panel.Width = label1.Left - panelcontroldist;
				panel.Height = panel.Width*ysize/xsize;
				
			}
		}
		
		bool AddObject(Point e){
			MapObject obj = new MapObject();
			if(tabControl_img.TabCount>0){
				//Ensure point isn't already taken
				bool taken = false;
				obj.point = e;
				obj.index = tabControl_img.SelectedIndex;
				
				for(int n = 0; n<mapobjs.Count; n++){
					if(obj.point == mapobjs[n].point) taken = true;
				}
				
				//Make sure only one player is present
				for(int n = 0; n<mapobjs.Count; n++){
					if(mapobjs[n].index == 0 && tabControl_img.SelectedIndex == 0) taken = true;
				}
				
				if(!taken){
					obj.fixedobj = checkBox_fixed.Checked;
					mapobjs.Add(obj);
					label_obj.Text = "Objects: " + mapobjs.Count;
					return true;
				}
			}
			
			return false;
		}
		
		void PanelMouseClick(object sender, MouseEventArgs e)
		{
			Point m = new Point(e.X*xsize/panel.Width, e.Y*ysize/panel.Height);
			
			if(e.Button == MouseButtons.Left){
				if(AddObject(m)) DrawObjects(false);
			}
			else{
				//Remove point
				//Find point closest to mouse click
				int mindist = int.MaxValue;
				int pos = -1;
				for(int n = 0; n<mapobjs.Count; n++){
					int dist = PointDist(m, mapobjs[n].point);
					if(dist<mindist){
						mindist = dist;
						pos = n;
					}
				}
				
				if(mindist<10 && pos>=0 && pos<mapobjs.Count){
					//Only delete close to click points
					mapobjs.RemoveAt(pos);
				}
				
				DrawObjects(true);
			}
		}
		
		int PointDist(Point p1, Point p2){
			return (int)Math.Sqrt(Math.Pow(p1.X-p2.X, 2) + Math.Pow(p1.Y-p2.Y, 2));
		}
		
		int FastPointDist(Point p1, Point p2){
			return Math.Abs(p1.X-p2.X)+Math.Abs(p1.Y-p2.Y);
		}
		
		void DrawObjects(bool clear){
			Graphics g = panel.CreateGraphics();
			if(clear) g.Clear(Color.White);
			for(int n = 0; n<mapobjs.Count; n++){
				Brush brush;
				MapObject obj = mapobjs[n];
				
				if(obj.index>0) brush = new SolidBrush(imgcols[obj.index-1]);
				else brush = new SolidBrush(Color.Black);
				
				obj.point.X = obj.point.X*panel.Width/xsize;
				obj.point.Y = obj.point.Y*panel.Height/ysize;
				
				g.FillEllipse(brush, obj.point.X-3, obj.point.Y-3, 6, 6);
				g.DrawString(obj.index.ToString(), new Font("Calibri",8), brush, obj.point.X, obj.point.Y);
				
			}
		}
		
		int playerindex = 0;
		void Button_exportClick(object sender, EventArgs e)
		{
			bool hasplayer = false;
			
			for(int n = 0; n<mapobjs.Count; n++){
				if(mapobjs[n].index == 0){
					hasplayer = true;
					playerindex = n;
				}
			}
			
			if(hasplayer){
				SaveFileDialog sfd = new SaveFileDialog();
				
				sfd.InitialDirectory = "\\";
				//sfd.Filter = "TXT files (*.txt)|*.txt";
				sfd.Filter = "XML files (*.xml)|*.xml";
				sfd.FilterIndex = 0;
				sfd.RestoreDirectory = true;
				if(sfd.ShowDialog() == DialogResult.OK){
					/*System.IO.StreamWriter file = new System.IO.StreamWriter(sfd.FileName);
				
				string s = "";
				for(int n = 0; n<mapobjs.Count; n++){
					//s += mapobjs[n].index + " " + mapobjs[n].point.X*xsize/400 + " " + mapobjs[n].point.Y*ysize/400 + "\r\n";
					s += mapobjs[n].index + " " + mapobjs[n].point.X + " " + mapobjs[n].point.Y + "\r\n";
				}
				
				file.Write(s);
				file.Close();*/
					
					GenerateXML(sfd.FileName);
				}
			}
			else{
				MessageBox.Show("Player hasn't been placed.");
			}
		}
		
		void GenerateXML(string filename){
			XmlTextWriter xtw = new XmlTextWriter(filename, Encoding.UTF8);
			xtw.Formatting = Formatting.Indented;
			xtw.Indentation = 4;
			
			XmlWriter xw = XmlWriter.Create(xtw);
			xw.WriteStartDocument();
			
			xw.WriteStartElement("map");
			
			//Map options
			xw.WriteStartElement("mapoptions");
			
			xw.WriteStartElement("mapsizex");
			xw.WriteString(xsize.ToString());
			xw.WriteEndElement();
			xw.WriteStartElement("mapsizey");
			xw.WriteString(ysize.ToString());
			xw.WriteEndElement();
			
			xw.WriteStartElement("playerboundxmin");
			xw.WriteString(textBox_xmin.Text);
			xw.WriteEndElement();
			xw.WriteStartElement("playerboundxmax");
			xw.WriteString(textBox_xmax.Text);
			xw.WriteEndElement();
			xw.WriteStartElement("playerboundymin");
			xw.WriteString(textBox_ymin.Text);
			xw.WriteEndElement();
			xw.WriteStartElement("playerboundymax");
			xw.WriteString(textBox_ymax.Text);
			xw.WriteEndElement();
			
			xw.WriteStartElement("playerstartx");
			xw.WriteString(mapobjs[playerindex].point.X.ToString());
			xw.WriteEndElement();
			xw.WriteStartElement("playerstarty");
			xw.WriteString(mapobjs[playerindex].point.Y.ToString());
			xw.WriteEndElement();
			xw.WriteEndElement();
			
			//Map references
			xw.WriteStartElement("mapreferences");
			for(int n = 1; n<tabControl_img.TabCount; n++){
				xw.WriteStartElement("reference");
				xw.WriteStartElement("id");
				xw.WriteString(n.ToString());
				xw.WriteEndElement();
				
				xw.WriteStartElement("filename");
				xw.WriteString(objectfilenames[n-1]);
				xw.WriteEndElement();
				
				xw.WriteStartElement("objectname");
				xw.WriteString(tabControl_img.TabPages[n].Controls[1].Text);
				xw.WriteEndElement();
				xw.WriteEndElement();
			}
			xw.WriteEndElement();
			
			//Write map objects
			xw.WriteStartElement("mapobjects");
			for(int n = 0; n<mapobjs.Count; n++){
				if(mapobjs[n].index != 0){
					xw.WriteStartElement("mapobject");
					
					xw.WriteStartElement("objectid");
					xw.WriteString((n+1).ToString());
					xw.WriteEndElement();
					xw.WriteStartElement("objectreference");
					xw.WriteString(mapobjs[n].index.ToString());
					xw.WriteEndElement();
					xw.WriteStartElement("objectx");
					xw.WriteString(mapobjs[n].point.X.ToString());
					xw.WriteEndElement();
					xw.WriteStartElement("objecty");
					xw.WriteString(mapobjs[n].point.Y.ToString());
					xw.WriteEndElement();
					xw.WriteStartElement("objectfixed");
					if(mapobjs[n].fixedobj) xw.WriteString("1");
					else xw.WriteString("0");
					
					xw.WriteEndElement();
					
					xw.WriteEndElement();
				}
			}
			
			xw.WriteEndElement();
			xw.WriteEndElement();
			xw.WriteEndDocument();
			
			xw.Close();
			xtw.Close();
		}
		
		void Button_addlineClick(object sender, EventArgs e)
		{
			Form prompt = new Form();
			prompt.Width = 450;
			prompt.Height = 150;
			
			Label lbl1, lbl2, lbl3, lbl4, lbl5;
			TextBox tb1, tb2, tb3, tb4, tb5;
			Button btn;
			
			lbl1 = new Label();
			lbl2 = new Label();
			lbl3 = new Label();
			lbl4 = new Label();
			lbl5 = new Label();
			tb1 = new TextBox();
			tb2 = new TextBox();
			tb3 = new TextBox();
			tb4 = new TextBox();
			tb5 = new TextBox();
			btn = new Button();
			
			lbl1.Text = "X Start: ";
			lbl2.Text = "X End: ";
			lbl3.Text = "Y Start: ";
			lbl4.Text = "Y End: ";
			lbl5.Text = "Objects: ";
			
			lbl1.Left = 0;
			lbl1.Top = 0;
			tb1.Left = lbl1.Right;
			tb1.Top = lbl1.Top;
			lbl2.Left = tb1.Right+10;
			lbl2.Top = lbl1.Top;
			tb2.Left = lbl2.Right;
			tb2.Top = lbl1.Top;
			
			lbl3.Left = 0;
			lbl3.Top = tb1.Bottom + 10;
			tb3.Left = lbl3.Right;
			tb3.Top = lbl3.Top;
			lbl4.Left = tb3.Right+10;
			lbl4.Top = lbl3.Top;
			tb4.Left = lbl4.Right;
			tb4.Top = lbl4.Top;
			
			lbl5.Left = 0;
			lbl5.Top = lbl3.Bottom + 10;
			tb5.Left = lbl5.Right;
			tb5.Top = lbl5.Top;
			
			btn.Left = lbl2.Left;
			btn.Top = tb5.Top;
			btn.Text = "Done";
			
			tb1.Text = tb2.Text = tb3.Text = tb4.Text = tb5.Text = "0";
			
			prompt.Controls.Add(lbl1);
			prompt.Controls.Add(lbl2);
			prompt.Controls.Add(lbl3);
			prompt.Controls.Add(lbl4);
			prompt.Controls.Add(lbl5);
			prompt.Controls.Add(tb1);
			prompt.Controls.Add(tb2);
			prompt.Controls.Add(tb3);
			prompt.Controls.Add(tb4);
			prompt.Controls.Add(tb5);
			prompt.Controls.Add(btn);
			
			prompt.Text = "Object Line";
			
			btn.Click += delegate {
				int objects;
				double xs, ys, xe, ye;
				if(int.TryParse(tb5.Text, out objects) && double.TryParse(tb1.Text, out xs) && double.TryParse(tb2.Text, out xe) &&
				   double.TryParse(tb3.Text, out ys) && double.TryParse(tb4.Text, out ye)){
					double xstep = (xe-xs)/(double)objects, ystep = (ye-ys)/(double)objects;
					bool draw = false;
					for(int n = 0; n<objects; n++){
						if(xs<xsize && ys<ysize && xs>=0 && ys>=0){
							draw = AddObject(new Point((int)xs, (int)ys));
							xs += xstep;
							ys += ystep;
						}
					}
					
					if(draw) DrawObjects(false);
				}
				
				prompt.Close();
			};
			
			prompt.ShowDialog();
		}
		
		void Button_removeallClick(object sender, EventArgs e)
		{
			mapobjs.Clear();
			DrawObjects(true);
		}
		
		
		void Button_randomClick(object sender, EventArgs e)
		{
			Form prompt = new Form();
			prompt.Text = "Random";
			
			Label lbl = new Label();
			TextBox tb = new TextBox();
			Button button = new Button();
			
			lbl.Text = "Amount: ";
			lbl.Left = 0;
			lbl.Top = 0;
			
			tb.Left = lbl.Right;
			tb.Top = 0;
			
			button.Text = "Done";
			button.Left = 0;
			button.Top = lbl.Bottom;
			
			prompt.Controls.Add(lbl);
			prompt.Controls.Add(tb);
			prompt.Controls.Add(button);
			
			prompt.Width = 300;
			prompt.Height = 100;
			
			button.Click += delegate {
				int num;
				int xmin, xmax, ymin, ymax;
				bool success = true;
				
				success &= int.TryParse(textBox_xmin.Text, out xmin);
				success &= int.TryParse(textBox_xmax.Text, out xmax);
				success &= int.TryParse(textBox_ymin.Text, out ymin);
				success &= int.TryParse(textBox_ymax.Text, out ymax);
				success &= int.TryParse(tb.Text, out num);
				
				if(success){
					GenerateRandomObjects(num);
					prompt.Close();
				}
				else{
					MessageBox.Show("Int parse fail");
				}
			};
			
			prompt.ShowDialog();
		}
		
		void GenerateRandomObjects(int num){
			for(int n = 0; n<num; n++){
				int xp = 0, yp = 0;
				bool ok = false;
				
				do{
					//xp = (random.Next()%(xmax-xmin))+xmin;
					//yp = (random.Next()%(ymax-ymin))+ymin;
					xp = random.Next()%xsize;
					yp = random.Next()%ysize;
					
					//Ensure object is atleast 10 away
					bool close = false;
					for(int m = 0; m<mapobjs.Count; m++){
						//if(PointDist(new Point(xp, yp), mapobjs[m].point)<10){
						if(FastPointDist(new Point(xp, yp), mapobjs[m].point)<10){
							close = true;
							break;
						}
					}
					
					if(!close){
						ok = true;
						AddObject(new Point(xp,yp));
					}
				}
				while(!ok);
			}
			
			DrawObjects(false);
		}
		
		void MainFormResize(object sender, EventArgs e)
		{
			//Resize panel
			//ResizePanel();
		}
		
		void Button_redrawClick(object sender, EventArgs e)
		{
			DrawObjects(false);
		}
	}
}
