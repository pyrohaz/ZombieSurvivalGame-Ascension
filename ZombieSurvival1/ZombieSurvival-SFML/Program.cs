/*
 * Created by SharpDevelop.
 * User: harri
 * Date: 30/07/2017
 * Time: 23:13
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Xml;

using SFML;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace ZombieSurvival_SFML
{
	
	
	class Program
	{
		static Player player;
		static Renderer renderer;
		static List<GameObject> gameobjects;
		static List<ObjectReference> objectrefs;
		static Vector2u WINDOWSIZE;
		static RenderWindow window;
		static Vector2i mousemiddle;
		static Thread renderthread;
		static Clock clock;
		static int mapsizex, mapsizey;
		static bool render = true;
		
		static void Main(string[] args)
		{
			//Native screen resolution
			window = new RenderWindow(VideoMode.DesktopMode, "Ascension", Styles.Fullscreen);
			
			//Non fullscreen
			//window = new RenderWindow(new VideoMode(800, 600), "Ascension", Styles.None);
			WINDOWSIZE = window.Size;
			mousemiddle = new Vector2i((int)WINDOWSIZE.X/2, (int)WINDOWSIZE.Y/2);
			//window = new RenderWindow(new VideoMode(WINDOWSIZE.X, WINDOWSIZE.Y), "Ascension", Styles.Fullscreen);
			
			window.Closed += new EventHandler(window_Closed);
			window.KeyPressed += new EventHandler<KeyEventArgs>(window_KeyPressed);
			window.KeyReleased += new EventHandler<KeyEventArgs>(window_KeyReleased);
			window.MouseMoved += new EventHandler<MouseMoveEventArgs>(window_MouseMoved);
			window.MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(window_MouseButtonPressed);
			window.MouseButtonReleased += new EventHandler<MouseButtonEventArgs>(window_MouseButtonReleased);
			
			objectrefs = new List<ObjectReference>();
			gameobjects = new List<GameObject>();
			
			player = new Player();
			player.SetYView((int)WINDOWSIZE.Y/2);
			
			renderer = new Renderer(WINDOWSIZE);
			renderer.LoadSky("../../../../Objects/sky.png");
			renderer.LoadFloor("../../../../Objects/floor.png");
			renderer.LoadFPSOverlay1("../../../../Objects/fpshands1.png");
			renderer.LoadFPSOverlay2("../../../../Objects/fpshands2.png");
			
			window.SetFramerateLimit(30);
			window.SetVerticalSyncEnabled(true);
			window.SetMouseCursorVisible(false);
			
			window.SetActive(false);
			
			//int status = LoadInfo();
			int status = LoadInfoXML();
			if(status != 0){
				Console.WriteLine("XML Failure: " + status);
				Console.ReadKey();
				//Load failure
				window.Close();
			}
			else{
				
				renderthread = new Thread(new ThreadStart(RenderThread));
				renderthread.Start();
				
				clock = new Clock();
				while(window.IsOpen){
					window.DispatchEvents();
					if(clock.ElapsedTime.AsMilliseconds() >= 20){
						clock.Restart();
						player.MoveHandler(ref gameobjects);
					}
				}
			}
		}
		
		static int LoadInfoXML(){
			XmlDocument xmldoc = new XmlDocument();
			xmldoc.Load("../../../../Objects/map.xml");
			
			//Parse map options
			XmlNodeList xmllist = xmldoc.GetElementsByTagName("mapoptions")[0].ChildNodes;
			
			int parsenum = 0;
			int[] pmin = new int[2]{0,0};
			int[] pmax = new int[2]{0,0};
			int playerx = 0, playery = 0;
			
			foreach(XmlNode node in xmllist){
				if(int.TryParse(node.InnerText, out parsenum)){
					switch(node.Name){
							case "mapsizex": mapsizex = parsenum; break;
							case "mapsizey": mapsizey = parsenum; break;
							case "playerboundxmin": pmin[0] = parsenum; break;
							case "playerboundxmax": pmax[0] = parsenum; break;
							case "playerboundymin": pmin[1] = parsenum; break;
							case "playerboundymax": pmax[1] = parsenum; break;
							case "playerstartx": playerx = parsenum; break;
							case "playerstarty": playery = parsenum; break;
					}
				}
				else{
					return 1;
				}
			}
			
			player.SetBounds(pmin, pmax);
			player.SetPosition(playerx, playery);
			
			//Parse object references
			xmllist = xmldoc.GetElementsByTagName("mapreferences");
			
			foreach(XmlNode node in xmllist[0].ChildNodes){
				//ObjectReference objref = new ObjectReference();
				int id = -1;
				string filename = "", name = "";
				
				foreach(XmlNode cnode in node.ChildNodes){
					/*switch(cnode.Name){
						case "id":
							if(!int.TryParse(cnode.InnerText, out objref.id)){
								return 1;
							}
							break;
							case "filename": objref.filename = cnode.InnerText; break;
							case "objectname": objref.name = cnode.InnerText; break;
					}*/
					switch(cnode.Name){
						case "id":
							if(!int.TryParse(cnode.InnerText, out id)){
								return 1;
							}
							break;
							case "filename": filename = cnode.InnerText; break;
							case "objectname": name = cnode.InnerText; break;
					}
				}
				//objectrefs.Add(objref);
				objectrefs.Add(new ObjectReference("../../../../Objects/" + filename, name, id));
			}
			
			//Parse map objects
			xmllist = xmldoc.GetElementsByTagName("mapobjects");
			
			foreach(XmlNode node in xmllist[0].ChildNodes){
				int objid = 0, objreference = 0, objx = 0, objy = 0, objfixed = 0;
				foreach(XmlNode cnode in node.ChildNodes){
					if(int.TryParse(cnode.InnerText, out parsenum)){
						switch(cnode.Name){
								case "objectid": objid = parsenum; break;
								case "objectreference": objreference = parsenum; break;
								case "objectx": objx = parsenum; break;
								case "objecty": objy = parsenum; break;
								case "objectfixed": objfixed = parsenum; break;
						}
					}
					else{
						return 1;
					}
				}
				
				int refpos = -1;
				for(int n = 0; n<objectrefs.Count; n++){
					if(objreference == objectrefs[n].GetID()) refpos = n;
				}
				
				if(refpos != -1){
					bool fixedobj = false;
					if(objfixed != 0) fixedobj = true;
					//gameobjects.Add(new GameObject(objectrefs[refpos].filename, objectrefs[refpos].name, new Vector2i(objx, objy), fixedobj, objid));
					gameobjects.Add(new GameObject(objectrefs[refpos].GetID(), objectrefs[refpos].GetName(), new Vector2f(objx, objy), fixedobj, objid));
				}
				else{
					//Object reference not found
					return 2;
				}
			}
			
			return 0;
		}
		
		static void RenderThread(){
			Clock clock = new Clock();
			while(window.IsOpen && render){
				renderer.Render(player, gameobjects, objectrefs, window);
				window.Display();
				int sleep = 50-clock.Restart().AsMilliseconds();
				if(sleep<0) sleep = 0;
				Thread.Sleep(sleep);
			}
		}

		static void window_KeyPressed(object sender, KeyEventArgs e){
			player.KeyHandler(e, true);
		}

		static void window_KeyReleased(object sender, KeyEventArgs e){
			player.KeyHandler(e, false);
		}

		static void window_MouseButtonPressed(object sender, MouseButtonEventArgs e){
			player.MouseClickHandler(e, true);
		}
		
		static void window_MouseButtonReleased(object sender, MouseButtonEventArgs e){
			player.MouseClickHandler(e, false);
		}
		
		static void window_MouseMoved(object sender, MouseMoveEventArgs e){
			if(Mouse.GetPosition(window) != mousemiddle){
				Mouse.SetPosition(mousemiddle, window);
				player.MouseHandler(e.X-(int)WINDOWSIZE.X/2, e.Y-(int)WINDOWSIZE.Y/2, (int)WINDOWSIZE.Y);
			}
		}
		
		static void window_Closed(object sender, EventArgs e){
			Window window = (Window)sender;
			if(renderthread.IsAlive){
				render = false;
				renderthread.Join(100);
				
			}
			window.Close();
		}
	}
}