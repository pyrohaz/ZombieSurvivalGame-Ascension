/*
 * Created by SharpDevelop.
 * User: harri
 * Date: 17/07/2017
 * Time: 16:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace ZombieSurvival1
{
	public class ObjectRef{
		public ObjectRef(string Filename, string Name, int ID){
			filename = Filename;
			name = Name;
			id = ID;
		}
		
		public string filename, name;
		public int id;
	}
	
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		System.Timers.Timer gametimer;
		Point defaultmousepos;
		bool mousecapture = false;
		Player player;
		Renderer renderer;
		List<GameObject> gameobjects;
		int numgameobjects;
		int mapsizex, mapsizey;
		bool loadsuccess = false;
		Rectangle mouseclipold, mouseclipnew;
		
		public MainForm(){
			InitializeComponent();
			
			//Enable double buffering
			typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, panel, new object[] { true });
			
			//Panel handlers
			panel.Paint += new PaintEventHandler(panel_Paint);
			panel.MouseMove += new MouseEventHandler(panel_MouseMove);
			panel.MouseDown += new MouseEventHandler(panel_MouseDown);
			panel.MouseUp += new MouseEventHandler(panel_MouseUp);			
			
			this.FormClosing += new FormClosingEventHandler(MainForm_FormClosing);
			
			this.KeyDown += new KeyEventHandler(MainForm_KeyDown);
			this.KeyUp += new KeyEventHandler(MainForm_KeyUp);
			
			//Screen refresh timer
			gametimer = new System.Timers.Timer();
			gametimer.Interval = 32;
			gametimer.Elapsed += new System.Timers.ElapsedEventHandler(gametimer_Elapsed);
			
			//Get default mouse position
			Point paneltopleft = this.PointToScreen(Point.Empty);
			paneltopleft.X += panel.Left;
			paneltopleft.Y += panel.Top;
			
			mouseclipold = Cursor.Clip;
			mouseclipnew = new Rectangle(paneltopleft.X, paneltopleft.Y, panel.Width, panel.Height);
			
			defaultmousepos = new Point(paneltopleft.X + panel.Width/2, paneltopleft.Y + panel.Height/2);
			
			renderer = new Renderer(panel.Size);
			player = new Player();
			player.SetYView(panel.Height/2);
			gameobjects = new List<GameObject>();
			int status = LoadInfo();
			
			if(status == 0){
				loadsuccess = true;
				mousecapture = true;
				renderer.LoadSky("sky.png");
				renderer.LoadFloor("floor.png");
				renderer.LoadFPSOverlay1("fpshands1.png");
				renderer.LoadFPSOverlay2("fpshands2.png");
				gametimer.Start();
			}
			else{
				MessageBox.Show("Couldn't load game info. Code: " + status);
				Application.Exit();
			}
			
			//Default mouse and hide
			if(mousecapture){
				Cursor.Position = defaultmousepos;
				Cursor.Hide();
			}
		}
		
		int LoadInfo(){
			System.IO.StreamReader file = new System.IO.StreamReader("map.txt");
			String line;
			List<ObjectRef> objectrefs = new List<ObjectRef>();
			
			int playerx = 0, playery = 0;
			int state = 0, nextstate = 0, itemcnt = 0;
			
			while((line = file.ReadLine()) != null){
				String num = "", fn = "", name = "";
				int cnt = 0;
				
				if(line == "Objects") nextstate = 1;
				else if(line == "Map") nextstate = 2;
				
				if(line.Length>0 && line[0] != ';' && state == nextstate){
					if(state == 1){
						//Object and files
						while(cnt<line.Length && char.IsDigit(line[cnt])){
							num += line[cnt++];
						}
						
						//Remove space and "
						while(cnt<line.Length && line[cnt] != '\"') cnt++;
						cnt++;
						
						//Get filename
						while(cnt<line.Length && line[cnt] != '\"'){
							fn += line[cnt++];
						}
						cnt++;
						
						//Remove space and "
						while(cnt<line.Length && line[cnt] != '\"') cnt++;
						cnt++;
						
						//Get object name
						while(cnt<line.Length && line[cnt] != '\"'){
							name += line[cnt++];
						}
						
						Debug.WriteLine(fn + " " + name);
						
						int id;
						if(int.TryParse(num, out id) && fn.Length>0){
							ObjectRef objectref = new ObjectRef(fn, name, id);
							objectrefs.Add(objectref);
						}
						else{
							file.Close();
							return 1;
						}
					}
					else if(state == 2){
						if(itemcnt == 0){
							//Map size
							//X
							while(cnt<line.Length && char.IsDigit(line[cnt])){
								num += line[cnt++];
							}
							
							if(!int.TryParse(num, out mapsizex)){
								file.Close();
								return 3;
							}
							
							//Move past space
							while(cnt<line.Length && !char.IsDigit(line[cnt])) cnt++;
							num = "";
							
							//Y
							while(cnt<line.Length && char.IsDigit(line[cnt])){
								num += line[cnt++];
							}
							
							if(!int.TryParse(num, out mapsizey)){
								file.Close();
								return 4;
							}
							
							itemcnt++;
						}
						else if(itemcnt == 1){
							//Parse 4x nums, pminx, pminy, pmaxx, pmaxy
							int[] nums = new int[4];
							
							for(int n = 0; n<4; n++){
								num = "";
								while(cnt<line.Length && char.IsDigit(line[cnt])){
									num += line[cnt++];
								}
								if(num.Length>0){
									if(!int.TryParse(num, out nums[n])){
										file.Close();
										return 5;
									}
								}
								
								//Move past non digits
								while(cnt<line.Length && !char.IsDigit(line[cnt])) cnt++;
							}
							
							player.SetBounds(new int[2]{nums[0],nums[1]}, new int[2]{nums[2],nums[3]});
							itemcnt++;
						}
						else if(itemcnt == 2){
							//Player start position
							//X
							while(cnt<line.Length && char.IsDigit(line[cnt])){
								num += line[cnt++];
							}
							
							if(!int.TryParse(num, out playerx)){
								file.Close();
								return 6;
							}
							
							//Move past non digits
							while(cnt<line.Length && !char.IsDigit(line[cnt])) cnt++;
							num = "";
							
							//Y
							while(cnt<line.Length && char.IsDigit(line[cnt])){
								num += line[cnt++];
							}
							
							if(!int.TryParse(num, out playery)){
								file.Close();
								return 7;
							}
							
							player.SetPosition((double)playerx, (double)playery);
							
							itemcnt++;
						}
						else if(itemcnt>2){
							//Parse 5x nums, Object ID, X, Y, Fixed (0/1) Item ID
							int[] nums = new int[5];
							
							for(int n = 0; n<5; n++){
								num = "";
								while(cnt<line.Length && (char.IsDigit(line[cnt]))){
									num += line[cnt++];
								}
								if(num.Length>0){
									if(!int.TryParse(num, out nums[n])){
										file.Close();
										return 8;
									}
								}
								
								//Move past non digits
								while(cnt<line.Length && !char.IsDigit(line[cnt])) cnt++;
							}
							
							int refpos = -1;
							for(int n = 0; n<objectrefs.Count; n++){
								if(nums[0] == objectrefs[n].id) refpos = n;
							}
							
							if(refpos != -1){
								bool fixedobj = false;
								if(nums[3] != 0) fixedobj = true;
								
								gameobjects.Add(new GameObject(objectrefs[refpos].filename, objectrefs[refpos].name, new Point(nums[1], nums[2]), fixedobj, nums[4]));
							}
							else{
								//Object reference not found
								file.Close();
								return 10;
							}
						}
					}
				}
				
				state = nextstate;
			}
			
			file.Close();
			
			return 0;
		}
		
		void panel_Paint(object sender, PaintEventArgs e){
			Graphics g = e.Graphics;
			renderer.Render(player, gameobjects, ref g);
		}
		
		void panel_MouseMove(object sender, MouseEventArgs e){
			player.MouseHandler(Cursor.Position.X-defaultmousepos.X, Cursor.Position.Y-defaultmousepos.Y, panel.Height);
			
			if(mousecapture && Cursor.Position != defaultmousepos){
				Cursor.Position = defaultmousepos;
				Cursor.Clip = mouseclipnew;
			}
		}
		
		void panel_MouseUp(object sender, MouseEventArgs e){
			player.MouseClickHandler(e, false);
		}
		
		void panel_MouseDown(object sender, MouseEventArgs e){
			player.MouseClickHandler(e, true);
		}
		
		void MainForm_KeyUp(object sender, KeyEventArgs e){
			player.KeyHandler(e, false);
		}
		
		void MainForm_KeyDown(object sender, KeyEventArgs e){
			player.KeyHandler(e, true);
		}
		
		void gametimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e){
			player.MoveHandler(ref gameobjects);
			panel.Invalidate();
		}
		
		void MainForm_FormClosing(object sender, FormClosingEventArgs e){
			renderer.Quit();
			Cursor.Clip = mouseclipold;
		}
	}
}
