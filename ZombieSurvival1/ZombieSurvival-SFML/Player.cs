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
using SFML;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace ZombieSurvival_SFML
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
			
			hp = MAXHP;
			stamina = MAXSTAMINA;
			minimaprange = 1000;
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
			
			maxdistance = Math.Sqrt((double)PMAX[0]*(double)PMAX[0] + (double)PMAX[1]*(double)PMAX[1]);
		}
		
		public void SetYView(int YView){
			yview = YView;
		}
		
		public void GetPosition(out double X, out double Y){
			X = x;
			Y = y;
		}
		
		public Vector2i GetPosition(){
			return new Vector2i((int)x,(int)y);
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
		
		public double GetHandMove(){
			return handmove;
		}
		
		public double GetSineMove(){
			return sinemove;
		}
		
		public int GetHP(){
			return hp;
		}
		
		public int GetStamina(){
			return stamina;
		}
		
		public int GetMiniMapRange(){
			return minimaprange;
		}
		
		double ObjectDistance(GameObject obj, Vector2i pos){
			return Math.Sqrt(Math.Pow(obj.GetPosition().X-pos.X,2) + Math.Pow(obj.GetPosition().Y-pos.Y,2));
		}
		
		void InventoryMenuHandler(ref List<GameObject> objects){
			if(inventory.Count>0){
				if(invsubmenupos2 == 0){
					//Drop
					objects.Find(obj => obj.GetID() == inventory[invsubmenupos]).SetPosition(new Vector2i((int)(x+Math.Cos(angle)*10), (int)(y+Math.Sin(angle)*10)));
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
			}
			
			invsubmenupos = -1;
			invsubmenupos2 = -1;
		}
		
		public void MoveHandler(ref List<GameObject> objects){
			double MOVE_SPEED = 3;
			double lpx = x, lpy = y, xnew = x, ynew = y;
			double rundbl = 0.0;
			
			if(jumpphase<Math.PI) jumpphase += Math.PI/20;
			else jumpphase = Math.PI;
			
			sinemove = 3*Math.Sin(movephase);
			yviewmove = (int)(sinemove + 30*Math.Sin(jumpphase));
			
			
			//Only allow player movement when not on the showchar screen
			if(!showchar){
				if(run){
					rundbl = 1.0;
					if(ud != 0 || lr != 0) stamina-=1;
					if(stamina <= 0){
						stamina = 0;
						
						//Comment out for unlimited run
						//run = false;
					}
				}
				
				if(stamina<MAXSTAMINA && ((ud==0 && lr==0) || !run)) stamina++;
				
				//if(hp<MAXHP) hp++;
				//hp = 70;
				
				if(ud != 0){
					xnew += (double)ud*Math.Cos(angle)*(MOVE_SPEED + rundbl*3);
					ynew += (double)ud*Math.Sin(angle)*(MOVE_SPEED + rundbl*3);
				}
				
				if(lr != 0){
					xnew -= (double)lr*Math.Cos(angle-Math.PI/2)*(MOVE_SPEED + rundbl*3);
					ynew -= (double)lr*Math.Sin(angle-Math.PI/2)*(MOVE_SPEED + rundbl*3);
				}
				
				if(ud != 0 || lr != 0) movephase += 0.2 + rundbl/5;
				else movephase = 0;
				
				//Ensure player isn't hitting any objects
				bool hit = false;
				for(int n = 0; n<objects.Count; n++){
					if(ObjectDistance(objects[n], new Vector2i((int)xnew, (int)ynew))<10 && objects[n].GetVisible()){
						hit = true;
						break;
					}
				}
				
				if(xnew>PMAX[0] || xnew<PMIN[0] || hit) xnew = lpx;
				if(ynew>PMAX[1] || ynew<PMIN[1] || hit) ynew = lpy;
				
				ItemHandler(objects);
				
				//Hand movement
				if(select && handmove>0.95){
					handmove -= 0.02;
				}
				else if(handmove <= 0.95){
					handmove = 0.95;
				}
				
				if(!select && handmove<1.0){
					handmove += 0.02;
				}
				else if(handmove >= 1.0){
					handmove = 1.0;
				}
				
				x = xnew;
				y = ynew;
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
				switch(e.Code){
						case Keyboard.Key.W: ud = 1; break;
						case Keyboard.Key.S: ud = -1; break;
						case Keyboard.Key.A: lr = -1; break;
						case Keyboard.Key.D: lr = 1; break;
						case Keyboard.Key.LShift: run = true; break;
						case Keyboard.Key.F1: showinv = !showinv; break;
					case Keyboard.Key.M:
						if(minimaprange == 1000) minimaprange = 3000;
						else minimaprange = 1000;
						break;
					case Keyboard.Key.Num1:
						if(invsubmenupos == -1) invsubmenupos = 0;
						else invsubmenupos2 = 0;
						break;
					case Keyboard.Key.Num2:
						if(invsubmenupos == -1) invsubmenupos = 1;
						else invsubmenupos2 = 1;
						break;
					case Keyboard.Key.Num3:
						if(invsubmenupos == -1) invsubmenupos = 2;
						else invsubmenupos2 = 2;
						break;
					case Keyboard.Key.Num4:
						if(invsubmenupos == -1) invsubmenupos = 3;
						else invsubmenupos2 = 3;
						break;
					case Keyboard.Key.Num5:
						if(invsubmenupos == -1) invsubmenupos = 4;
						else invsubmenupos2 = 4;
						break;
					case Keyboard.Key.I:
						showchar = !showchar;
						invsubmenupos = -1;
						invsubmenupos2 = -1;
						break;
					case Keyboard.Key.Space:
						if(!showchar && stamina>10 && jumpphase == Math.PI){
							jumpphase = 0;
							stamina-=10;
						}
						break;
				}
			}
			else{
				switch(e.Code){
					case Keyboard.Key.W:
						case Keyboard.Key.S: ud = 0; break;
					case Keyboard.Key.A:
						case Keyboard.Key.D: lr = 0; break;
						case Keyboard.Key.LShift: run = false; break;
				}
			}
		}
		
		public void MouseHandler(int mousedx, int mousedy, int ymax){
			if(!showchar){
				if(mousedx > 0) angle += 0.03;
				else if(mousedx<0) angle -= 0.03;
				
				if(angle<0) angle += 2*Math.PI;
				if(angle>2*Math.PI) angle -= 2*Math.PI;
				
				if(mousedy > 0)  yview -= 20;
				else if(mousedy < 0)  yview += 20;
				
				if(yview>ymax) yview = ymax;
				if(yview<0) yview = 0;
			}
		}
		
		public void MouseClickHandler(MouseButtonEventArgs e, bool down){
			if(e.Button == Mouse.Button.Left){
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
				double dist = ObjectDistance(objects[n], new Vector2i((int)x, (int)y));
				
				if(dist<20 && select && !objects[n].GetFixed() && objects[n].GetVisible()){
					//Pick up object
					if(inventory.Count < MAXINVENTORY){
						objects[n].SetVisible(false);
						inventory.Add(objects[n].GetID());
						select = false;
					}
					else{
						invfull = RESETINVENTORYFULLMAX;
					}
				}
			}
		}
		
		double x, y, angle;
		int ud = 0, lr = 0, yview, yviewmove;
		double movephase, jumpphase, sinemove, handmove;
		double maxdistance;
		
		int hp, stamina;
		int minimaprange;
		int[] PMAX, PMIN;
		
		bool showinv = false, showchar = false, run = false, select = false;
		int invsubmenupos = -1, invsubmenupos2 = -1, overlay = 1;
		List<int> inventory;
		int invfull = 0;
		
		public double FOV = 70.0*Math.PI/180.0;
		//public double FOV = 90.0*Math.PI/180.0;
		public double MAXVIEWDIST = 4000;
		public int MAXINVENTORY = 5;
		public int MAXSTAMINA = 100;
		public int MAXHP = 100;
		public int RESETINVENTORYFULLMAX = 40;
	}
}
