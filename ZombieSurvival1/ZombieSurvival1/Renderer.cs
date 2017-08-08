/*
 * Created by SharpDevelop.
 * User: harri
 * Date: 21/07/2017
 * Time: 23:21
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Threading;

namespace ZombieSurvival1
{
	
	/// <summary>
	/// Description of Renderer.
	/// </summary>
	public class Renderer
	{
		private class RenderObject{
			public RenderObject(){}
			public RenderObject(int Index, double Distance){
				index = Index;
				distance = Distance;
			}
			
			public int index;
			public double distance;
		}
		
		public Renderer(Size PanelSize)
		{
			//Seed random generator
			unchecked{rand = new Random((int)DateTime.UtcNow.Ticks);};
			//Create graphics bitmap + graphics
			bmp = new Bitmap(PanelSize.Width, PanelSize.Height, PixelFormat.Format32bppPArgb);
			
			g = Graphics.FromImage(bmp);
			//g.InterpolationMode = InterpolationMode.NearestNeighbor;
			g.InterpolationMode = InterpolationMode.Low;
			//g.InterpolationMode = InterpolationMode.HighQualityBicubic;
			g.CompositingQuality = CompositingQuality.HighSpeed;
			g.SmoothingMode = SmoothingMode.HighSpeed;
			g.TextRenderingHint = TextRenderingHint.SystemDefault;
			g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
			
			textbrush = new SolidBrush(Color.White);
			textbackbrush = new SolidBrush(Color.FromArgb(127, 50, 50, 50));
			textfont = new Font("Calibri", 12);
			crosshairpen = new Pen(Color.FromArgb(127, 255, 255, 255), 3);
			
			brightnesstable = new float[256];
			for(int n = 0; n<256; n++){
				brightnesstable[n] = (float)-(0.5*Math.Pow((double)(255-n)/256, 3));
			}
		}
		
		public void LoadSky(String Filename){
			sky = Image.FromFile(Filename);
		}
		
		public void LoadFloor(String Filename){
			floor = Image.FromFile(Filename);
		}
		
		public void LoadFPSOverlay1(String Filename){
			fps1 = Image.FromFile(Filename);
		}
		
		public void LoadFPSOverlay2(String Filename){
			fps2 = Image.FromFile(Filename);
		}
		
		public void Quit(){
			
		}
		
		//Point in triangle functions
		float sign (Point p1, Point p2, Point p3){
			return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
		}

		bool PointInTriangle (Point pt, Point v1, Point v2, Point v3){
			bool b1, b2, b3;

			b1 = sign(pt, v1, v2) < 0.0f;
			b2 = sign(pt, v2, v3) < 0.0f;
			b3 = sign(pt, v3, v1) < 0.0f;

			return ((b1 == b2) && (b2 == b3));
		}
		
		//Distance between two points
		double PointDistance(Point p1, Point p2){
			return Math.Sqrt(Math.Pow((double)(p1.X-p2.X),2) + Math.Pow((double)(p1.Y-p2.Y),2));
		}
		
		double Max3(double a, double b, double c){
			if(a>=b && a>=c) return a;
			if(b>=a && b>=c) return b;
			if(c>=a && c>=b) return c;
			return a;
		}
		
		//Main render function
		public void Render(Player player, List<GameObject> objects, ref Graphics panelg){
			List<RenderObject> renderobjs = new List<Renderer.RenderObject>();
			//Image Attribute matrix
			float[][] imgmatrix = {
				new float[]{1,0,0,0,0},
				new float[]{0,1,0,0,0},
				new float[]{0,0,1,0,0},
				new float[]{0,0,0,1,0},
				new float[]{0,0,0,0,1}
			};
			
			ImageAttributes imgattr = new ImageAttributes();
			imgattr.ClearColorMatrix();
			imgattr.SetGamma(1.0f);
			
			//Generate view triangle with top vertex being player
			Point tp = player.GetPosition();
			//Left and right vertices
			Point tl = new Point((int)(tp.X+Math.Cos(player.GetAngle()-player.FOV/2)*player.MAXVIEWDIST), (int)(tp.Y+Math.Sin(player.GetAngle()-player.FOV/2)*player.MAXVIEWDIST));
			Point tr = new Point((int)(tp.X+Math.Cos(player.GetAngle()+player.FOV/2)*player.MAXVIEWDIST), (int)(tp.Y+Math.Sin(player.GetAngle()+player.FOV/2)*player.MAXVIEWDIST));
			
			//Find all objects in view
			for(int n = 0; n<objects.Count; n++){
				if(objects[n].GetVisible() && PointInTriangle(objects[n].GetPosition(), tp, tl, tr)){
					renderobjs.Add(new Renderer.RenderObject(n, PointDistance(tp, objects[n].GetPosition())));
				}
			}
			
			//Sort objects by distance (bro Z buffering)
			//Insertion sort
			int i = 1, j;
			while(i<renderobjs.Count){
				j = i;
				while(j>0 && renderobjs[j-1].distance < renderobjs[j].distance){
					RenderObject obj = renderobjs[j-1];
					renderobjs[j-1] = renderobjs[j];
					renderobjs[j] = obj;
					j--;
				}
				i++;
			}
			
			
			//Draw sky
			float xpos = (float)((player.GetAngleMod()/(2*Math.PI))*sky.Width);
			float brightness = 0.0f;
			/*
			int[] mapmin, mapmax;
			player.GetMapBounds(out mapmin, out mapmax);
			//double xmax = Math.Max(mapmin[0]-tp.X, tp.X-mapmax[0]);
			//double ymax = Math.Max(mapmin[1]-tp.Y, tp.Y-mapmax[1]);
			
			//Darken sky as player gets closer to edge
			//float brightness = (float)-(0.6*Math.Pow((1-Math.Sqrt(xmax*xmax + ymax*ymax)/player.GetMaxDistance()), 4));
			//int index = (int)((Math.Sqrt(xmax*xmax + ymax*ymax))*127/player.GetMaxDistance());
			
			double xmin = Math.Min(tp.X-mapmin[0], mapmax[0]-tp.X);
			double ymin = Math.Min(tp.Y-mapmin[1], mapmax[1]-tp.Y);
			//int index = (int)((Math.Sqrt(xmin*xmin + ymin*ymin))*255/player.GetMaxDistance());
			int index = 0;
			if(xmin<ymin) index = (int)(255*xmin/(mapmax[0]-mapmin[0]));
			else index = (int)(ymin/(mapmax[1]-mapmin[1]));
			
			Debug.WriteLine(index);
			float brightness = brightnesstable[index];
			//Debug.WriteLine(brightness + " " + index);
			for(int m = 0; m<3; m++){
				imgmatrix[4][m] = brightness;
			}
			
			imgattr.SetColorMatrix(new ColorMatrix(imgmatrix), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
			 */
			
			//if(draw) g.DrawImage(im, new Rectangle((int)drawx, (int)drawy, (int)imw, (int)imh), 0, 0, im.Width, im.Height, GraphicsUnit.Pixel, imgattr);
			
			g.DrawImage(sky, 0, 0, new RectangleF(xpos, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
			//Wrap sky
			if(xpos>sky.Width/2) g.DrawImage(sky, (sky.Width-xpos)-1, 0, new RectangleF(0, 0, bmp.Width-(sky.Width-xpos), bmp.Height), GraphicsUnit.Pixel);
			
			
			//g.DrawImage(sky, new Rectangle(0,0,bmp.Width,bmp.Height), xpos, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, imgattr);
			/*int drawwidth = bmp.Width;
			if(sky.Width-xpos < bmp.Width) drawwidth = (int)(sky.Width-xpos);
			g.DrawImage(sky, new Rectangle(0,0,drawwidth,bmp.Height), xpos, 0, drawwidth, bmp.Height, GraphicsUnit.Pixel, imgattr);
			//Wrap sky
			if(xpos>sky.Width/2) g.DrawImage(sky, new Rectangle(sky.Width-(int)xpos-1, 0, bmp.Width-(sky.Width-(int)xpos), bmp.Height), 0, 0, (int)xpos-sky.Width/2, sky.Height, GraphicsUnit.Pixel, imgattr);
			 */
			
			g.DrawImage(floor, 0, player.GetYView(), new RectangleF(xpos, 0, bmp.Width, floor.Height-player.GetYView()), GraphicsUnit.Pixel);
			//Wrap floor
			if(xpos>floor.Width/2) g.DrawImage(floor, (floor.Width-xpos)-1, player.GetYView(), new RectangleF(0, 0, bmp.Width-(floor.Width-xpos), floor.Height-player.GetYView()), GraphicsUnit.Pixel);
			
			double px, py;
			double scale;
			
			
			for(int n = 0; n<renderobjs.Count; n++){
				int objindex = renderobjs[n].index;
				bool draw = true;
				
				px = (double)tp.X;
				py = (double)tp.Y;
				
				Point objpos = objects[objindex].GetPosition();
				
				//Distance from player to object
				double playerpointdist = renderobjs[n].distance;
				
				//Angle between leftmost triangle vertex and player
				double line1angle = Math.Atan2((double)tl.Y-py, (double)tl.X-px);
				
				//Angle between object and player
				double line2angle = Math.Atan2(objpos.Y-py, objpos.X-px);
				
				//Difference between the two
				double langle = line2angle-line1angle;
				//Wrap
				if(langle<0) langle += Math.PI*2;
				
				//Calculate image scale dependent on distance to player
				//scale = 1.0/(0.003*Math.Cos(player.FOV/2-langle)*playerpointdist);
				scale = 1.0/(0.003*Math.Cos(player.FOV/2-langle)*playerpointdist);
				
				//Grab image with draw widths and height
				Image im = objects[objindex].GetImage();
				double imh = im.Height*scale/2;
				double imw = im.Width*scale/2;
				
				//Physical draw distance on the screen
				//As you get closer to objects, make them appear lower in the screen
				//to remove the effect of making it look like you're lying down
				double drawy;
				//drawy = player.GetYView()+1-(imh*(playerpointdist*0.5/player.GetMaxDistance()+0.2));
				//drawy = (player.GetYView()+1)-(imh*(0.8+0.2*playerpointdist/player.GetMaxDistance()) - imh*(1-objects[objindex].GetScale()));
				drawy = (player.GetYView()+1)-(imh*(0.8+0.2*playerpointdist/player.GetMaxDistance()));
				
				//double drawy = player.GetYView()+1-imh;
				double drawx = langle*bmp.Width/player.FOV - (imw/2);
				
				//Change brightness for further objects
				brightness = (float)-(0.5*playerpointdist/player.GetMaxDistance());
				for(int m = 0; m<3; m++){
					imgmatrix[4][m] = brightness;
				}
				
				imgattr.SetColorMatrix(new ColorMatrix(imgmatrix), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
				
				//Draw image
				if(draw) g.DrawImage(im, new Rectangle((int)drawx, (int)drawy, (int)imw, (int)imh), 0, 0, im.Width, im.Height, GraphicsUnit.Pixel, imgattr);
			}
			
			//Draw FPS overlay + offset to allow for jumping
			if(player.GetCurrentOverlay() == 1) g.DrawImageUnscaled(fps1, 0, 40-player.GetYViewMove());
			else g.DrawImageUnscaled(fps2, 0, 40-player.GetYViewMove());
			
			RenderPlayerOverlays(objects, player);
			
			//FPS calculator
			long msnow = DateTime.Now.Ticks/TimeSpan.TicksPerMillisecond;
			framerate += ((1000/(msnow-msec))-framerate)*0.3;
			SizeF txtsize = g.MeasureString("FPS: " + framerate.ToString("N1"), textfont);
			g.FillRectangle(textbackbrush, bmp.Width-txtsize.Width, 0, txtsize.Width, txtsize.Height);
			g.DrawString("FPS: " + framerate.ToString("N1"), textfont, textbrush, bmp.Width-txtsize.Width, 0);
			
			//Draw entire graphics to panel
			panelg.DrawImageUnscaled(bmp, 0, 0);
			
			msec = DateTime.Now.Ticks/TimeSpan.TicksPerMillisecond;
		}
		
		void RenderPlayerOverlays(List<GameObject> objects, Player player){
			SizeF txtsize;
			
			//Generate inventory overlay
			string inv = "Inventory\n";
			List<int> pinv = player.GetInventory();
			for(int n = 0; n<pinv.Count; n++){
				string name = "";
				//Find object with same ID
				name = objects.Find(obj => obj.GetID() == pinv[n]).GetName();
				inv += (n+1) + ": " + name + "\n";
			}
			
			//Draw inventory overlay
			if(player.GetShowInventory()){
				invtextbackbrush = new SolidBrush(Color.FromArgb(127,20,80,20));
				txtsize = g.MeasureString(inv, textfont);
				g.FillRectangle(invtextbackbrush, new RectangleF(0, 0, txtsize.Width, txtsize.Height));
				g.DrawString(inv, textfont, textbrush, 0, 0);
			}
			else{
				invtextbackbrush = new SolidBrush(Color.FromArgb(127,80,20,20));
				txtsize = g.MeasureString("Inventory", textfont);
				g.FillRectangle(invtextbackbrush, new RectangleF(0, 0, txtsize.Width, txtsize.Height));
				g.DrawString("Inventory", textfont, textbrush, 0, 0);
			}
			
			if(player.GetResetInventoryFull()>0){
				txtsize = g.MeasureString("Inventory full!", textfont);
				g.FillRectangle(textbackbrush, (bmp.Width-txtsize.Width)/2, 0, txtsize.Width, txtsize.Height);
				g.DrawString("Inventory full!", textfont, textbrush, (bmp.Width-txtsize.Width)/2, 0);
			}
			
			//Draw character overlay
			if(player.GetShowChar()){
				txtsize = g.MeasureString(inv, textfont);
				g.FillRectangle(textbackbrush, (bmp.Width-txtsize.Width)/2, 50, txtsize.Width, txtsize.Height);
				g.DrawString(inv, textfont, textbrush, (bmp.Width-txtsize.Width)/2, 50);
				
				if(player.GetInvSubmenuPos() != -1){
					string txt = "1: Drop\n2: Combine\n3: Discard";
					SizeF txtsize2 = g.MeasureString(txt, textfont);
					g.FillRectangle(textbackbrush, (bmp.Width+txtsize.Width)/2, 50+(player.GetInvSubmenuPos()+1)*txtsize.Height/(1+player.GetInventory().Count), txtsize2.Width, txtsize2.Height);
					g.DrawString(txt, textfont, textbrush, (bmp.Width+txtsize.Width)/2, 50+(player.GetInvSubmenuPos()+1)*txtsize.Height/(1+player.GetInventory().Count));
				}
			}
			
			int CROSSWIDTH = 10;
			g.DrawLine(crosshairpen, bmp.Width/2, (bmp.Height-CROSSWIDTH)/2, bmp.Width/2, (bmp.Height+CROSSWIDTH)/2+1);
			g.DrawLine(crosshairpen, (bmp.Width-CROSSWIDTH)/2, bmp.Height/2, (bmp.Width+CROSSWIDTH)/2+1, bmp.Height/2);
		}
		
		Image sky, floor, fps1, fps2;
		Random rand;
		Bitmap bmp;
		Graphics g;
		SolidBrush textbrush, textbackbrush, invtextbackbrush;
		Pen crosshairpen;
		Font textfont;
		
		float[] brightnesstable;
		
		double framerate = 0.0;
		long msec = 0;
	}
}
