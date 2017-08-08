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
using System.Linq;
using System.Linq.Expressions;
using SFML;
using SFML.Graphics;
using SFML.System;

namespace ZombieSurvival_SFML
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
		
		public Renderer(Vector2u WindowSize)
		{
			textfont = new Font("Lato.ttf");
			textcolour = new Color(255,255,255);
			textbackcolour = new Color(50,50,50,200);
			crosshaircolour = new Color(255,255,255,200);
			windowsize = (Vector2i)WindowSize;
			clock = new Clock();
			minimappos = new IntRect((int)windowsize.X-100, 0, 100, 100);
			random = new Random(DateTime.Now.GetHashCode());
			rendertexture = new RenderTexture((uint)windowsize.X, (uint)windowsize.Y);
			
			floorbrightness = 255;
		}
		
		public void LoadSky(String Filename){
			sky = (new Texture(Filename));
			sky.Repeated = true;
		}
		
		public void LoadFloor(String Filename){
			floor = (new Texture(Filename));
			floor.Repeated = true;
		}
		
		public void LoadFPSOverlay1(String Filename){
			fps1 = (new Texture(Filename));
		}
		
		public void LoadFPSOverlay2(String Filename){
			fps2 = (new Texture(Filename));
		}
		
		//Point in triangle functions
		float sign (Vector2i p1, Vector2i p2, Vector2i p3){
			return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
		}

		bool PointInTriangle (Vector2i pt, Vector2i v1, Vector2i v2, Vector2i v3){
			bool b1, b2, b3;

			b1 = sign(pt, v1, v2) < 0.0f;
			b2 = sign(pt, v2, v3) < 0.0f;
			b3 = sign(pt, v3, v1) < 0.0f;

			return ((b1 == b2) && (b2 == b3));
		}
		
		//Distance between two points
		double PointDistance(Vector2i p1, Vector2i p2){
			return Math.Sqrt(Math.Pow((double)(p1.X-p2.X),2) + Math.Pow((double)(p1.Y-p2.Y),2));
		}
		
		double Max3(double a, double b, double c){
			if(a>=b && a>=c) return a;
			if(b>=a && b>=c) return b;
			if(c>=a && c>=b) return c;
			return a;
		}
		
		//Main render function
		public void Render(Player player, List<GameObject> objects, List<ObjectReference> objref, RenderWindow window){
			List<RenderObject> renderobjs = new List<Renderer.RenderObject>();
			
			//Generate view triangle with top vertex being player
			Vector2i tp = player.GetPosition();
			//Left and right vertices
			Vector2i tl = new Vector2i((int)(tp.X+Math.Cos(player.GetAngle()-player.FOV/2)*player.MAXVIEWDIST), (int)(tp.Y+Math.Sin(player.GetAngle()-player.FOV/2)*player.MAXVIEWDIST));
			Vector2i tr = new Vector2i((int)(tp.X+Math.Cos(player.GetAngle()+player.FOV/2)*player.MAXVIEWDIST), (int)(tp.Y+Math.Sin(player.GetAngle()+player.FOV/2)*player.MAXVIEWDIST));
			
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
			float xpos = (float)((player.GetAngleMod()/(2*Math.PI))*sky.Size.X);
			Sprite sprite = new Sprite(sky, new IntRect((int)xpos, 0, windowsize.X, windowsize.Y));
			
			//byte brightness = (byte)random.Next(200, 255);
			skybrightness += (random.Next(150, 255) - skybrightness)>>4;
			floorbrightness += (random.Next(200, 255) - floorbrightness)>>4;
			byte brightness = (byte)skybrightness;
			sprite.Color = new Color(brightness, brightness, brightness);
			rendertexture.Draw(sprite);
			
			sprite = new Sprite(floor, new IntRect((int)xpos, 0, windowsize.X, windowsize.Y));
			brightness = (byte)floorbrightness;
			sprite.Color = new Color(brightness, brightness, brightness);
			sprite.Position = new Vector2f(0, player.GetYView());
			rendertexture.Draw(sprite);
			
			float px,py;
			float scale;
			
			for(int n = 0; n<renderobjs.Count; n++){
				//for(int n = 0; n<0; n++){
				int objindex = renderobjs[n].index;
				bool draw = true;
				
				px = (float)tp.X;
				py = (float)tp.Y;
				
				Vector2i objpos = objects[objindex].GetPosition();
				
				//Distance from player to object
				float playerpointdist = (float)renderobjs[n].distance;
				
				//Angle between leftmost triangle vertex and player
				float line1angle = (float) Math.Atan2((float)tl.Y-py, (float)tl.X-px);
				
				//Angle between object and player
				float line2angle = (float) Math.Atan2(objpos.Y-py, objpos.X-px);
				
				//Difference between the two
				float langle = line2angle-line1angle;
				//Wrap
				if(langle<0) langle += (float) Math.PI*2;
				
				//Calculate image scale dependent on distance to player
				//scale = 1.0/(0.003*Math.Cos(player.FOV/2-langle)*playerpointdist);
				scale = (float)(1.0f/(0.003*Math.Cos(player.FOV/2-langle)*playerpointdist));
				
				//Grab image with draw widths and height
				//Texture im = objects[objindex].GetImage();
				Texture im = objref.Find(obj => obj.GetID() == objects[objindex].GetTextureReference()).GetTexture();
				float imh = im.Size.Y*scale/2;
				float imw = im.Size.X*scale/2;
				
				//Physical draw distance on the screen
				//As you get closer to objects, make them appear lower in the screen
				//to remove the effect of making it look like you're lying down
				float drawy;
				//drawy = player.GetYView()+1-(imh*(playerpointdist*0.5/player.GetMaxDistance()+0.2));
				//drawy = (player.GetYView()+1)-(imh*(0.8+0.2*playerpointdist/player.GetMaxDistance()) - imh*(1-objects[objindex].GetScale()));
				drawy = (float)((player.GetYView()+1)-(imh*(0.8+0.2*playerpointdist/player.GetMaxDistance())));
				
				//double drawy = player.GetYView()+1-imh;
				float drawx = (float)(langle*windowsize.X/player.FOV - (imw/2));
				
				sprite = new Sprite(im);
				sprite.Scale = new Vector2f(imw/(float)im.Size.X, imh/(float)im.Size.Y);
				sprite.Position = new Vector2f(drawx, drawy);
				//Decrease object brightness for further objects
				//byte brightness = (byte)(255*Math.Pow(1.0-playerpointdist/player.GetMaxDistance(), 2));
				brightness = (byte)(255*Math.Pow(1.0-playerpointdist/player.MAXVIEWDIST, 2));
				sprite.Color = new Color(brightness, brightness, brightness);
				//Draw image
				rendertexture.Draw(sprite);
			}
			
			RenderPlayerOverlays(objects, player);
			RenderMiniMap(minimappos, objects, player);
			
			Sprite frame = new Sprite(rendertexture.Texture);
			//frame.Origin = new Vector2f(frame.GetLocalBounds().Left + frame.GetLocalBounds().Width/2, frame.GetLocalBounds().Top + frame.GetLocalBounds().Height/2);
			//frame.Scale = new Vector2f(1.0f, -1.0f);
			frame.TextureRect = new IntRect(0,frame.TextureRect.Height, frame.TextureRect.Width, -frame.TextureRect.Height);
			//frame.Color = new Color(10, 255, 10);
			window.Draw(frame);
			
			
			
			//FPS
			fps += (1000.0f/clock.Restart().AsMilliseconds() - fps)*0.5f;
			Console.WriteLine("Avg FPS: " + fps);
		}
		
		void RenderPlayerOverlays(List<GameObject> objects, Player player){
			FloatRect txtsize;
			Text text;
			
			Sprite sprite;
			//if(player.GetCurrentOverlay() == 1) sprite = new Sprite(fps1);
			//else sprite = new Sprite(fps2);
			sprite = new Sprite(fps1);
			//if(player.GetCurrentOverlay() == 2) sprite.Scale = new Vector2f(0.9f, 1.0f);
			sprite.Scale = new Vector2f((float)player.GetHandMove(), 1.0f);
			sprite.Position = new Vector2f(windowsize.X/2, windowsize.Y/2+40-player.GetYViewMove());
			//Weird ass walk scaling
			//sprite.Scale = new Vector2f((float)(1+player.GetSineMove()*0.01), (float)(1+player.GetSineMove()*0.01));
			sprite.Origin = new Vector2f(sprite.GetLocalBounds().Left + sprite.GetLocalBounds().Width/2, sprite.GetLocalBounds().Top + sprite.GetLocalBounds().Height/2);
			rendertexture.Draw(sprite);
			
			//Generate inventory overlay
			string inv = "Inventory";
			List<int> pinv = player.GetInventory();
			for(int n = 0; n<pinv.Count; n++){
				string name = "";
				//Find object with same ID
				name = objects.Find(obj => obj.GetID() == pinv[n]).GetName();
				inv += "\n"+(n+1) + ": " + name;
			}
			
			if(player.GetShowInventory()){
				invtextbackcolour = new Color(20,180,20,150);
				text = new Text(inv, textfont, 16);
			}
			else{
				invtextbackcolour = new Color(180,20,20,150);
				text = new Text("Inventory", textfont, 16);
			}
			
			text.Color = textcolour;
			text.Origin = new Vector2f(text.GetLocalBounds().Left,0);
			//text.Position = new Vector2f(0,0);
			text.Position = new Vector2f(minimappos.Left-1, minimappos.Top + minimappos.Height+10);
			txtsize = text.GetLocalBounds();
			
			RectangleShape rect = new RectangleShape(new Vector2f(txtsize.Width+txtsize.Left+2, txtsize.Height+txtsize.Top+1));
			//rect.Position = new Vector2f(0,0);
			rect.Position = new Vector2f(minimappos.Left-2, minimappos.Top + minimappos.Height+10);
			rect.FillColor = rect.OutlineColor = invtextbackcolour;
			rendertexture.Draw(rect);
			rendertexture.Draw(text);
			
			//Draw HP and stamina
			//string hpstring = "HP: " + player.GetHP() + "/" + player.MAXHP + "\nStamina: " + player.GetStamina() + "/" + player.MAXSTAMINA;
			//string hpstring = "HP: " + player.MAXHP + "/" + player.MAXHP + "\nStamina: " + player.MAXSTAMINA + "/" + player.MAXSTAMINA;
			text.DisplayedString = "HP: \nStamina: ";
			text.Position = new Vector2f(0,0);
			text.Origin = new Vector2f(text.GetLocalBounds().Left, text.GetLocalBounds().Top);
			
			int xstart = (int)(text.GetLocalBounds().Left + text.GetLocalBounds().Width);
			rect.Position = new Vector2f(0,0);
			rect.Size = new Vector2f(text.GetLocalBounds().Left + text.GetLocalBounds().Width+105, text.GetLocalBounds().Top + text.GetLocalBounds().Height);
			rect.FillColor = textbackcolour;
			
			//text.DisplayedString = "HP: " + player.GetHP() + "/" + player.MAXHP + "\nStamina: " + player.GetStamina() + "/" + player.MAXSTAMINA;
			rendertexture.Draw(rect);
			rendertexture.Draw(text);
			
			//HP
			rect.Position = new Vector2f(xstart, 0);
			rect.Size = new Vector2f(player.GetHP()*100/player.MAXHP, text.GetLocalBounds().Height/2);
			rect.FillColor = new Color(0,200,0);
			rendertexture.Draw(rect);
			
			rect.Position = new Vector2f(xstart+player.GetHP()*100/player.MAXHP, 0);
			rect.Size = new Vector2f(100-player.GetHP()*100/player.MAXHP, text.GetLocalBounds().Height/2);
			rect.FillColor = new Color(200,0,0);
			rendertexture.Draw(rect);
			
			//Stamina
			rect.Position = new Vector2f(xstart, text.GetLocalBounds().Top+text.GetLocalBounds().Height/2);
			rect.Size = new Vector2f(player.GetStamina()*100/player.MAXSTAMINA, text.GetLocalBounds().Height/2);
			rect.FillColor = new Color(0,50,255);
			rendertexture.Draw(rect);
			
			rect.Position = new Vector2f(xstart+player.GetStamina()*100/player.MAXSTAMINA, text.GetLocalBounds().Top+text.GetLocalBounds().Height/2);
			rect.Size = new Vector2f(100-player.GetStamina()*100/player.MAXSTAMINA, text.GetLocalBounds().Height/2);
			rect.FillColor = new Color(0,0,100);
			rendertexture.Draw(rect);
			
			if(player.GetResetInventoryFull()>0){
				text = new Text("Inventory full", textfont, 16);
				txtsize = text.GetLocalBounds();
				text.Origin = new Vector2f(txtsize.Left + txtsize.Width/2, 0);
				text.Position = new Vector2f(windowsize.X/2, 0);
				text.Color = new Color(255,255,255,(byte)(255*player.GetResetInventoryFull()/player.RESETINVENTORYFULLMAX));
				
				rect = new RectangleShape(new Vector2f(txtsize.Width+txtsize.Left+1, txtsize.Height+txtsize.Top));
				rect.Position = new Vector2f(windowsize.X/2-txtsize.Width/2-txtsize.Left,0);
				rect.FillColor = rect.OutlineColor = new Color(20,20,20,(byte)(255*player.GetResetInventoryFull()/player.RESETINVENTORYFULLMAX));
				rendertexture.Draw(rect);
				rendertexture.Draw(text);
			}
			
			if(player.GetShowChar()){
				text = new Text(inv, textfont, 16);
				txtsize = text.GetLocalBounds();
				text.Origin = new Vector2f(txtsize.Left + txtsize.Width/2, 0);
				text.Position = new Vector2f(windowsize.X/2, 0);
				text.Color = textcolour;
				
				rect = new RectangleShape(new Vector2f(txtsize.Width+txtsize.Left+1, txtsize.Height+txtsize.Top+1));
				rect.Position = new Vector2f(windowsize.X/2-txtsize.Width/2-txtsize.Left,0);
				rect.FillColor = rect.OutlineColor = textbackcolour;
				rendertexture.Draw(rect);
				rendertexture.Draw(text);
				
				if(player.GetInvSubmenuPos() != -1){
					string txt = " 1: Drop\n 2: Combine\n 3: Discard";
					Text text2 = new Text(txt, textfont, 16);
					text2.Color = textcolour;
					FloatRect txtsize2 = text2.GetLocalBounds();
					text2.Origin = new Vector2f(txtsize2.Left, 0);
					text2.Position = new Vector2f(txtsize.Left+txtsize.Width/2+text.Position.X, (player.GetInvSubmenuPos()+1)*txtsize.Height/(1+player.GetInventory().Count)+text.Position.Y);
					
					rect.Position = text2.Position;
					rect.Size = new Vector2f(txtsize2.Left + txtsize2.Width, txtsize2.Top + txtsize2.Height);
					rendertexture.Draw(rect);
					rendertexture.Draw(text2);
				}
			}
			
		}
		
		void RenderMiniMap(IntRect size, List<GameObject> objects, Player player){
			CircleShape circle = new CircleShape();
			
			circle.Radius = 2.0f;
			circle.Origin = new Vector2f(circle.GetLocalBounds().Left + circle.GetLocalBounds().Width/2, circle.GetLocalBounds().Top + circle.GetLocalBounds().Height/2);
			circle.OutlineColor = new Color(0,0,0);
			circle.OutlineThickness = 1;
			RectangleShape rect = new RectangleShape(new Vector2f(size.Width+circle.Radius, size.Height+circle.Radius));
			rect.Position = new Vector2f(size.Left-circle.Radius, size.Top);
			rect.FillColor = new Color(200, 200, 200, 230);
			rect.OutlineThickness = 1;
			rect.OutlineColor = new Color(0,0,0);
			rendertexture.Draw(rect);
			
			int MINIMAPRANGE = player.GetMiniMapRange();
			
			for(int n = 0; n<objects.Count; n++){
				if(Math.Abs(player.GetPosition().X-objects[n].GetPosition().X) < MINIMAPRANGE && Math.Abs(player.GetPosition().Y-objects[n].GetPosition().Y)<MINIMAPRANGE && objects[n].GetVisible()){
					Color fillcol;
					
					if(objects[n].GetFixed()){
						fillcol = new Color(0, 200, 0);
					}
					else{
						fillcol = new Color(0, 0, 200);
					}
					
					int objx = (objects[n].GetPosition().X-player.GetPosition().X)*size.Width/(MINIMAPRANGE*2) + size.Width/2 + size.Left;
					int objy = (objects[n].GetPosition().Y-player.GetPosition().Y)*size.Height/(MINIMAPRANGE*2) + size.Height/2 + size.Top;
					
					circle.FillColor = fillcol;
					circle.Position = new Vector2f(objx, objy);
					rendertexture.Draw(circle);
				}
			}
			
			/*
			circle.Radius = 5.0f;
			circle.Origin = new Vector2f(circle.GetLocalBounds().Left + circle.GetLocalBounds().Width/2, circle.GetLocalBounds().Top + circle.GetLocalBounds().Height/2);
			circle.FillColor = new Color(200, 0, 0);
			circle.Position = new Vector2f(size.Left + size.Width/2, size.Top + size.Height/2);
			circle.SetPointCount(3);
			circle.Rotation = (float)(player.GetAngle()*180.0/Math.PI) + 90;
			rendertexture.Draw(circle);*/
			
			ConvexShape playershape = new ConvexShape(3);
			playershape.SetPoint(0, new Vector2f((float)(6*Math.Cos(player.GetAngle())), (float)(6*Math.Sin(player.GetAngle()))));
			playershape.SetPoint(1, new Vector2f((float)(4*Math.Cos(player.GetAngle()+Math.PI*120/180)), (float)(4*Math.Sin(player.GetAngle()+Math.PI*120/180))));
			playershape.SetPoint(2, new Vector2f((float)(4*Math.Cos(player.GetAngle()+Math.PI*240/180)), (float)(4*Math.Sin(player.GetAngle()+Math.PI*240/180))));
			playershape.FillColor = new Color(200,0,0);
			playershape.Position = new Vector2f(size.Left + size.Width/2, size.Top + size.Height/2);
			rendertexture.Draw(playershape);
		}
		
		Texture sky, floor, fps1, fps2;
		Color textcolour, textbackcolour, invtextbackcolour, crosshaircolour;
		Font textfont;
		Vector2i windowsize;
		IntRect minimappos;
		Random random;
		RenderTexture rendertexture;
		
		int skybrightness, floorbrightness;
		
		Clock clock;
		float fps;
	}
}
