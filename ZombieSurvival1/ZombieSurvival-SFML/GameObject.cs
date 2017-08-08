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
using SFML;
using SFML.Graphics;
using SFML.System;

namespace ZombieSurvival_SFML
{
	/// <summary>
	/// Description of GameObject.
	/// </summary>
	public class GameObject
	{
		public GameObject()
		{
			
		}
		
		//public GameObject(string TextureF, string Name, Vector2i Position, bool Fixed, int ID){
		public GameObject(int TextureReference, string Name, Vector2i Position, bool Fixed, int ID){
			//image = new Texture(TextureF);
			
			//Interpolate scaling
			//image.Smooth = true;
			
			position = Position;
			fixedobj = Fixed;
			id = ID;
			visible = true;
			name = Name;
			texturereference = TextureReference;
		}
		
		/*public Texture GetImage(){
			return image;
		}*/
		
		public int GetTextureReference(){
			return texturereference;
		}
		
		public Vector2i GetPosition(){
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
		
		public void SetPosition(Vector2i Position){
			position = Position;
		}
		
		public void SetVisible(bool Visible){
			visible = Visible;
		}
 
		
		//Texture image;
		string name;
		bool fixedobj, visible;
		Vector2i position;
		int id, texturereference;
	}
}
