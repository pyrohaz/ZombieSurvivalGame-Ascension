/*
 * Created by SharpDevelop.
 * User: harri
 * Date: 21/07/2017
 * Time: 23:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;

namespace ZombieSurvival1
{
	/// <summary>
	/// Description of GameObject.
	/// </summary>
	public class GameObject
	{
		public GameObject()
		{
			
		}
		
		public GameObject(string Texture, string Name, Point Position, bool Fixed, int ID){
			image = Image.FromFile(Texture);
			position = Position;
			fixedobj = Fixed;
			id = ID;
			visible = true;
			name = Name;
		}
		
		public Image GetImage(){
			return image;
		}
		
		public Point GetPosition(){
			return position;
		}
		
		public bool GetFixed(){
			return fixedobj;
		}
		
		
		public int GetID(){
			return id;
		}
		
		public bool GetVisible(){
			return visible;
		}
		
		public string GetName(){
			return name;
		}
		
		public void SetPosition(Point Position){
			position = Position;
		}
		
		public void SetVisible(bool Visible){
			visible = Visible;
		}
		
		Image image;
		string name;
		bool fixedobj, visible;
		Point position;
		int id;
	}
}
