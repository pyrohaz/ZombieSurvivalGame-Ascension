/*
 * Created by SharpDevelop.
 * User: harri
 * Date: 05/08/2017
 * Time: 20:46
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using SFML.Graphics;

namespace ZombieSurvival_SFML
{
	/// <summary>
	/// Description of ObjectReference.
	/// </summary>
	public class ObjectReference{
		public ObjectReference(string Filename, string Name, int ID){
			filename = Filename;
			texture = new Texture(Filename);
			texture.Smooth = true;
			name = Name;
			id = ID;
		}
		public ObjectReference(){
			id = -1;
			filename = name = "";
		}
		
		public Texture GetTexture(){
			return texture;
		}
		
		public int GetID(){
			return id;
		}
		
		public string GetName(){
			return name;
		}
		
		public string GetFilename(){
			return filename;
		}
		
		Texture texture;
		string filename, name;
		int id;
	}
}
