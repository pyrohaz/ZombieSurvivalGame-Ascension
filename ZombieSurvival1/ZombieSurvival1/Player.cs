/*
 * Created by SharpDevelop.
 * User: harri
 * Date: 17/07/2017
 * Time: 16:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ZombieSurvival1
{
	/// <summary>
	/// Description of Player.
	/// </summary>
	
	public class Player
	{
		public Player()
		{
			PMAX = new int[2];
			PMIN = new int[2];
			jumpphase = Math.PI;
			inventory = new List<int>();
		}
		
		public void SetPosition(double X, double Y){
			x = X;
			y = Y;
		}
		
		public void SetBounds(int[] min, int[] max){
			PMAX[0] = max[0];
			PMAX[1] = max[1];
			PMIN[0] = min[0];
			PMIN[1] = min[1];
			
			maxdistance = Math.Sqrt(PMAX[0]*PMAX[0] + PMAX[1]*PMAX[1]);
		}
		
		public void SetYView(int YView){
			yview = YView;
		}
		
		public void GetPosition(out double X, out double Y){
			X = x;
			Y = y;
		}
		
		public Point GetPosition(){
			return new Point((int)x,(int)y);
		}
		
		public void GetMapBounds(out int[] min, out int[] max){
			min = PMIN;
			max = PMAX;
		}
		
		public int GetYView(){
			return yview + yviewmove;
		}
		
		public int GetYViewMove(){
			return yviewmove;
		}
		
		public double GetAngle(){
			return angle;
		}
		
		public double GetAngleMod(){
			/*double mangle = angle;
			while(mangle<0) mangle += 2*Math.PI;
			while(mangle>2*Math.PI) mangle -= 2*Math.PI;
			
			return mangle;*/
			return angle;
		}
		
		public double GetMaxDistance(){
			return maxdistance;
		}
		
		public List<int> GetInventory(){
			return inventory;
		}
		
		public bool GetShowInventory(){
			return showinv;
		}
		
		public int GetResetInventoryFull(){
			int iinvfull = invfull;
			if(invfull>0) invfull--;
			return iinvfull;
		}
		
		public bool GetShowChar(){
			return showchar;
		}
		
		public int GetInvSubmenuPos(){
			if(invsubmenupos != -1 && inventory.Count>0) return invsubmenupos;
			return -1;
		}
		
		public int GetCurrentOverlay(){
			return overlay;
		}
		
		double ObjectDistance(GameObject obj){
			return Math.Sqrt(Math.Pow(obj.GetPosition().X-x,2) + Math.Pow(obj.GetPosition().Y-y,2));
		}
		
		void InventoryMenuHandler(ref List<GameObject> objects){
			if(invsubmenupos2 == 0){
				//Drop
				objects.Find(obj => obj.GetID() == inventory[invsubmenupos]).SetPosition(new Point((int)(x+Math.Cos(angle)*10), (int)(y+Math.Sin(angle)*10)));
				objects.Find(obj => obj.GetID() == inventory[invsubmenupos]).SetVisible(true);
				inventory.RemoveAt(invsubmenupos);
			}
			else if(invsubmenupos2 == 1){
				//Combine
			}
			else if(invsubmenupos2 == 2){
				//Discard
				inventory.RemoveAt(invsubmenupos);
			}
			
			invsubmenupos = -1;
			invsubmenupos2 = -1;
		}
		
		public void MoveHandler(ref List<GameObject> objects){
			double MOVE_SPEED = 3;
			double lpx = x, lpy = y;
			double rundbl = 0.0;
			
			if(jumpphase<Math.PI) jumpphase += Math.PI/12;
			else jumpphase = Math.PI;
			
			yviewmove = (int)(3*Math.Sin(movephase) + 40*Math.Sin(jumpphase));
			
			
			//Only allow player movement when not on the showchar screen
			if(!showchar){
				if(run) rundbl = 1.0;
				
				if(ud != 0){
					x += (double)ud*Math.Cos(angle)*(MOVE_SPEED + rundbl*5);
					y += (double)ud*Math.Sin(angle)*(MOVE_SPEED + rundbl*5);
				}
				
				if(lr != 0){
					x -= (double)lr*Math.Cos(angle-Math.PI/2)*(MOVE_SPEED + rundbl*5);
					y -= (double)lr*Math.Sin(angle-Math.PI/2)*(MOVE_SPEED + rundbl*5);
				}
				
				if(ud != 0 || lr != 0) movephase += 0.5 + rundbl/3;
				else movephase = 0;
				
				//Ensure player isn't hitting any objects
				bool hit = false;
				for(int n = 0; n<objects.Count; n++){
					if(ObjectDistance(objects[n])<10 && objects[n].GetVisible()){
						hit = true;
						break;
					}
				}
				
				if(x>PMAX[0] || x<PMIN[0] || hit) x = lpx;
				if(y>PMAX[1] || y<PMIN[1] || hit) y = lpy;
				
				ItemHandler(objects);
			}
			else{
				//Do object related menu stuff
				if(invsubmenupos != -1 && invsubmenupos2 != -1){
					InventoryMenuHandler(ref objects);
				}
			}
		}
		
		public void KeyHandler(KeyEventArgs e, bool down){
			if(down){
				switch(e.KeyCode){
						case Keys.W: ud = 1; break;
						case Keys.S: ud = -1; break;
						case Keys.A: lr = -1; break;
						case Keys.D: lr = 1; break;
						//case Keys.Q: select = true; break;
						case Keys.ShiftKey: run = true; break;
						case Keys.F1: showinv = !showinv; break;
					case Keys.D1:
						if(invsubmenupos == -1) invsubmenupos = 0;
						else invsubmenupos2 = 0;
						break;
					case Keys.D2:
						if(invsubmenupos == -1) invsubmenupos = 1;
						else invsubmenupos2 = 1;
						break;
					case Keys.D3:
						if(invsubmenupos == -1) invsubmenupos = 2;
						else invsubmenupos2 = 2;
						break;
					case Keys.D4:
						if(invsubmenupos == -1) invsubmenupos = 3;
						else invsubmenupos2 = 3;
						break;
					case Keys.D5:
						if(invsubmenupos == -1) invsubmenupos = 4;
						else invsubmenupos2 = 4;
						break;
					case Keys.I:
						showchar = !showchar;
						invsubmenupos = -1;
						invsubmenupos2 = -1;
						break;
						case Keys.Space: if(!showchar) jumpphase = 0; break;
				}
			}
			else{
				switch(e.KeyCode){
					case Keys.W:
						case Keys.S: ud = 0; break;
					case Keys.A:
						case Keys.D: lr = 0; break;
						case Keys.ShiftKey: run = false; break;
						//case Keys.Q: select = false; break;
				}
			}
		}
		
		public void MouseHandler(int mousedx, int mousedy, int ymax){
			if(!showchar){
				if(mousedx > 0) angle += 0.04;
				else if(mousedx<0) angle -= 0.04;
				
				while(angle<0) angle += 2*Math.PI;
				while(angle>2*Math.PI) angle -= 2*Math.PI;
				
				if(mousedy > 0)  yview -= 10;
				else if(mousedy < 0)  yview += 10;
				
				if(yview>ymax) yview = ymax;
				if(yview<0) yview = 0;
			}
		}
		
		public void MouseClickHandler(MouseEventArgs e, bool down){
			if(e.Button == MouseButtons.Left){
				if(!down){
					select = false;
					overlay = 1;
				}
				else{
					select = true;
					overlay = 2;
				}
			}
		}
		
		void ItemHandler(List<GameObject> objects){
			for(int n = 0; n<objects.Count; n++){
				double dist = ObjectDistance(objects[n]);
				
				if(dist<20 && select && !objects[n].GetFixed() && objects[n].GetVisible()){
					//Pick up object
					if(inventory.Count < 3){
						objects[n].SetVisible(false);
						inventory.Add(objects[n].GetID());
						select = false;
					}
					else{
						invfull = 5;
					}
				}
			}
		}
		
		double x, y, angle;
		int ud = 0, lr = 0, yview, yviewmove;
		double movephase, jumpphase;
		double maxdistance;
		
		int[] PMAX, PMIN;
		
		bool showinv = false, showchar = false, run = false, select = false;
		int invsubmenupos = -1, invsubmenupos2 = -1, overlay = 1;
		List<int> inventory;
		int invfull = 0;
		
		public double FOV = 70.0*Math.PI/180.0;
		public double MAXVIEWDIST = 2000;
	}
}
