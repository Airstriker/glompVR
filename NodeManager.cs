
using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Microsoft.Win32;

namespace glomp {


    public class NodeManager {
        
		public static int[] displayLists = new int[3];
        
		public static readonly int FILE_NODE = 0;
		public static readonly int DIR_NODE = 1;
		public static readonly int DRIVE_NODE = 2;

        private static readonly float BOX_SCALE = 0.8f;

        
        public NodeManager() {
        }

		public static void GenerateDisplayLists() {
			displayLists[DRIVE_NODE] = GenerateDisplayList(DRIVE_NODE);
			displayLists[DIR_NODE] = GenerateDisplayList(DIR_NODE);
			displayLists[FILE_NODE] = GenerateDisplayList(FILE_NODE);
		}

		public static string GetMIMEDescription(string fileExtension) //fileExtension with dot "."
		{
			string applicationType;
			string contentType;

			if (fileExtension == string.Empty)
				return "unknown file";

			//get the application class the extension is associated to
			using(RegistryKey rgk = Registry.ClassesRoot.OpenSubKey("\\" + fileExtension))
			{
				if (rgk != null) {
					applicationType = rgk.GetValue ("", string.Empty).ToString ();

					//get the file type description for the application associated to
					using(RegistryKey rgkey = Registry.ClassesRoot.OpenSubKey("\\" + applicationType))
					{
						if (rgkey != null) {
							contentType = rgkey.GetValue ("", string.Empty).ToString ();
							if (contentType != String.Empty)
								return contentType;
							else
								return fileExtension.Substring(1) + " file"; //dot removed
						}
					}
				}
			}

			return fileExtension.Substring(1) + " file"; //dot removed
		}

        
        private static int GenerateDisplayList(int nodeType) {
            System.Console.WriteLine("Genning new list");
            int displayList = GL.GenLists(1);
            if(nodeType == FILE_NODE) {
                
                GL.NewList(displayList, ListMode.Compile); // start compiling display list
                //GL.Color3(boxColour[0], boxColour[1], boxColour[2]);
                //GL.Scale(BOX_SCALE, BOX_SCALE, BOX_SCALE);
                GL.Begin(BeginMode.Quads);          // start drawing quads
                
                // Front Face
                GL.Normal3( 0.0f, 0.0f, 1.0f);      // Normal Facing Forward
                GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(-0.8f, -0.8f,  0.8f);  // Bottom Left Of The Texture and Quad
                GL.TexCoord2(1.0f, 1.0f); GL.Vertex3( 0.8f, -0.8f,  0.8f);  // Bottom Right Of The Texture and Quad
                GL.TexCoord2(1.0f, 0.0f); GL.Vertex3( 0.8f,  0.8f,  0.8f);  // Top Right Of The Texture and Quad
                GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(-0.8f,  0.8f,  0.8f);  // Top Left Of The Texture and Quad
                // Back Face
                GL.Normal3( 0.0f, 0.0f,-1.0f);      // Normal Facing Away
                GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(-0.8f, -0.8f, -0.8f);  // Bottom Right Of The Texture and Quad
                GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(-0.8f,  0.8f, -0.8f);  // Top Right Of The Texture and Quad
                GL.TexCoord2(0.0f, 0.0f); GL.Vertex3( 0.8f,  0.8f, -0.8f);  // Top Left Of The Texture and Quad
                GL.TexCoord2(0.0f, 1.0f); GL.Vertex3( 0.8f, -0.8f, -0.8f);  // Bottom Left Of The Texture and Quad
                // Top Face
                GL.Normal3( 0.0f, 1.0f, 0.0f);      // Normal Facing Up
                GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(-0.8f,  0.8f, -0.8f);  // Top Left Of The Texture and Quad
                GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(-0.8f,  0.8f,  0.8f);  // Bottom Left Of The Texture and Quad
                GL.TexCoord2(1.0f, 1.0f); GL.Vertex3( 0.8f,  0.8f,  0.8f);  // Bottom Right Of The Texture and Quad
                GL.TexCoord2(1.0f, 0.0f); GL.Vertex3( 0.8f,  0.8f, -0.8f);  // Top Right Of The Texture and Quad
                // Bottom Face
                GL.Normal3( 0.0f,-1.0f, 0.0f);      // Normal Facing Down
                GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(-0.8f, -0.8f, -0.8f);  // Top Right Of The Texture and Quad
                GL.TexCoord2(0.0f, 0.0f); GL.Vertex3( 0.8f, -0.8f, -0.8f);  // Top Left Of The Texture and Quad
                GL.TexCoord2(0.0f, 1.0f); GL.Vertex3( 0.8f, -0.8f,  0.8f);  // Bottom Left Of The Texture and Quad
                GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(-0.8f, -0.8f,  0.8f);  // Bottom Right Of The Texture and Quad
                // Right face
                GL.Normal3( 1.0f, 0.0f, 0.0f);      // Normal Facing Right
                GL.TexCoord2(1.0f, 1.0f); GL.Vertex3( 0.8f, -0.8f, -0.8f);  // Bottom Right Of The Texture and Quad
                GL.TexCoord2(1.0f, 0.0f); GL.Vertex3( 0.8f,  0.8f, -0.8f);  // Top Right Of The Texture and Quad
                GL.TexCoord2(0.0f, 0.0f); GL.Vertex3( 0.8f,  0.8f,  0.8f);  // Top Left Of The Texture and Quad
                GL.TexCoord2(0.0f, 1.0f); GL.Vertex3( 0.8f, -0.8f,  0.8f);  // Bottom Left Of The Texture and Quad
                // Left Face
                GL.Normal3(-1.0f, 0.0f, 0.0f);      // Normal Facing Left
                GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(-0.8f, -0.8f, -0.8f);  // Bottom Left Of The Texture and Quad
                GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(-0.8f, -0.8f,  0.8f);  // Bottom Right Of The Texture and Quad
                GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(-0.8f,  0.8f,  0.8f);  // Top Right Of The Texture and Quad
                GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(-0.8f,  0.8f, -0.8f);  // Top Left Of The Texture and Quad
                
                GL.End();                    // Done Drawing Quads
                
                GL.EndList();                // Finish display list
                
            } else if (nodeType == DIR_NODE) {
                
                float dirHeight = 1.04f;
                
                GL.NewList(displayList, ListMode.Compile); // start compiling display list
                //GL.Color3(boxColour[0], boxColour[1], boxColour[2]);
                //GL.Scale(BOX_SCALE, BOX_SCALE, BOX_SCALE);
                GL.Begin(BeginMode.Quads);          // start drawing quads
                
                // Front Face
                GL.Normal3( 0.0f, 0.0f, 1.0f);      // Normal Facing Forward
                GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(-0.8f, -0.8f,  0.8f);  // Bottom Right Of The Texture and Quad
                GL.TexCoord2(1.0f, 1.0f); GL.Vertex3( 0.8f, -0.8f,  0.8f);  // Bottom Left Of The Texture and Quad
                GL.TexCoord2(1.0f, 0.0f); GL.Vertex3( 0.8f,  dirHeight,  0.8f);  // Top Left Of The Texture and Quad
                GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(-0.8f,  dirHeight,  0.8f);  // Top Right Of The Texture and Quad
                // Back Face
                GL.Normal3( 0.0f, 0.0f,-1.0f);      // Normal Facing Away
                GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(-0.8f, -0.8f, -0.8f);  // Bottom Right Of The Texture and Quad
                GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(-0.8f,  dirHeight, -0.8f);  // Top Right Of The Texture and Quad
                GL.TexCoord2(0.0f, 0.0f); GL.Vertex3( 0.8f,  dirHeight, -0.8f);  // Top Left Of The Texture and Quad
                GL.TexCoord2(0.0f, 1.0f); GL.Vertex3( 0.8f, -0.8f, -0.8f);  // Bottom Left Of The Texture and Quad
                // Top Face
                GL.Normal3( 0.0f, 1.0f, 0.0f);      // Normal Facing Up
                GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(-0.8f,  dirHeight, -0.8f);  // Top Right Of The Texture and Quad
                GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(-0.8f,  dirHeight,  0.8f);  // Bottom Right Of The Texture and Quad
                GL.TexCoord2(1.0f, 1.0f); GL.Vertex3( 0.8f,  dirHeight,  0.8f);  // Bottom Left Of The Texture and Quad
                GL.TexCoord2(1.0f, 0.0f); GL.Vertex3( 0.8f,  dirHeight, -0.8f);  // Top Left Of The Texture and Quad
                // Bottom Face
                GL.Normal3( 0.0f,-1.0f, 0.0f);      // Normal Facing Down
                GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(-0.8f, -0.8f, -0.8f);  // Top Right Of The Texture and Quad
                GL.TexCoord2(0.0f, 0.0f); GL.Vertex3( 0.8f, -0.8f, -0.8f);  // Top Left Of The Texture and Quad
                GL.TexCoord2(0.0f, 1.0f); GL.Vertex3( 0.8f, -0.8f,  0.8f);  // Bottom Left Of The Texture and Quad
                GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(-0.8f, -0.8f,  0.8f);  // Bottom Right Of The Texture and Quad
                // Right face
                GL.Normal3( 1.0f, 0.0f, 0.0f);      // Normal Facing Right
                GL.TexCoord2(1.0f, 1.0f); GL.Vertex3( 0.8f, -0.8f, -0.8f);  // Bottom Right Of The Texture and Quad
                GL.TexCoord2(1.0f, 0.0f); GL.Vertex3( 0.8f,  dirHeight, -0.8f);  // Top Right Of The Texture and Quad
                GL.TexCoord2(0.0f, 0.0f); GL.Vertex3( 0.8f,  dirHeight,  0.8f);  // Top Left Of The Texture and Quad
                GL.TexCoord2(0.0f, 1.0f); GL.Vertex3( 0.8f, -0.8f,  0.8f);  // Bottom Left Of The Texture and Quad
                // Left Face
                GL.Normal3(-1.0f, 0.0f, 0.0f);      // Normal Facing Left
                GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(-0.8f, -0.8f, -0.8f);  // Bottom Left Of The Texture and Quad
                GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(-0.8f, -0.8f,  0.8f);  // Bottom Right Of The Texture and Quad
                GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(-0.8f,  dirHeight,  0.8f);  // Top Right Of The Texture and Quad
                GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(-0.8f,  dirHeight, -0.8f);  // Top Left Of The Texture and Quad
                
                GL.End();                    // Done Drawing Quads
                
                GL.EndList();                // Finish display list
			
			} else if (nodeType == DRIVE_NODE) {

				float dirHeight = 1.04f;

				GL.NewList(displayList, ListMode.Compile); // start compiling display list
				//GL.Color3(boxColour[0], boxColour[1], boxColour[2]);
				//GL.Scale(BOX_SCALE, BOX_SCALE, BOX_SCALE);
				GL.Begin(BeginMode.Quads);          // start drawing quads

				//To enable smooth shading uncomment the normals that are commented now and vice versa
				//Smooth shading is disabled at the moment, because without textures it doesn't look better that flat shading

				// Front Face
				GL.Normal3( 0.0f, 0.0f, 1.0f);      // Normal Facing Forward
				//GL.Normal3( -1.0f, -1.0f, 1.0f);
				GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(-0.8f, -0.8f,  0.8f);  // Bottom Right Of The Texture and Quad
				//GL.Normal3( 1.0f, -1.0f, 1.0f);
				GL.TexCoord2(1.0f, 1.0f); GL.Vertex3( 0.8f, -0.8f,  0.8f);  // Bottom Left Of The Texture and Quad
				//GL.Normal3( 1.0f, 1.0f, 1.0f);
				GL.TexCoord2(1.0f, 0.0f); GL.Vertex3( 0.8f,  dirHeight,  0.8f);  // Top Left Of The Texture and Quad
				//GL.Normal3( -1.0f, 1.0f, 1.0f);
				GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(-0.8f,  dirHeight,  0.8f);  // Top Right Of The Texture and Quad
				// Back Face
				GL.Normal3( 0.0f, 0.0f,-1.0f);      // Normal Facing Away
				//GL.Normal3( -1.0f, -1.0f,-1.0f);
				GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(-0.8f, -0.8f, -0.8f);  // Bottom Right Of The Texture and Quad
				//GL.Normal3( -1.0f, 1.0f,-1.0f);
				GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(-0.8f,  dirHeight, -0.8f);  // Top Right Of The Texture and Quad
				//GL.Normal3( 1.0f, 1.0f,-1.0f);
				GL.TexCoord2(0.0f, 0.0f); GL.Vertex3( 0.8f,  dirHeight, -0.8f);  // Top Left Of The Texture and Quad
				//GL.Normal3( 1.0f, -1.0f,-1.0f);
				GL.TexCoord2(0.0f, 1.0f); GL.Vertex3( 0.8f, -0.8f, -0.8f);  // Bottom Left Of The Texture and Quad
				// Top Face
				GL.Normal3( 0.0f, 1.0f, 0.0f);      // Normal Facing Up
				//GL.Normal3( -1.0f, 1.0f, -1.0f);
				GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(-0.8f,  dirHeight, -0.8f);  // Top Right Of The Texture and Quad
				//GL.Normal3( -1.0f, 1.0f, 1.0f);
				GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(-0.8f,  dirHeight,  0.8f);  // Bottom Right Of The Texture and Quad
				//GL.Normal3( 1.0f, 1.0f, 1.0f);
				GL.TexCoord2(1.0f, 1.0f); GL.Vertex3( 0.8f,  dirHeight,  0.8f);  // Bottom Left Of The Texture and Quad
				//GL.Normal3( 1.0f, 1.0f, -1.0f);
				GL.TexCoord2(1.0f, 0.0f); GL.Vertex3( 0.8f,  dirHeight, -0.8f);  // Top Left Of The Texture and Quad
				// Bottom Face
				GL.Normal3( 0.0f,-1.0f, 0.0f);      // Normal Facing Down
				//GL.Normal3( -1.0f,-1.0f, -1.0f);
				GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(-0.8f, -0.8f, -0.8f);  // Top Right Of The Texture and Quad
				//GL.Normal3( 1.0f,-1.0f, -1.0f);
				GL.TexCoord2(0.0f, 0.0f); GL.Vertex3( 0.8f, -0.8f, -0.8f);  // Top Left Of The Texture and Quad
				//GL.Normal3( 1.0f,-1.0f, 1.0f);
				GL.TexCoord2(0.0f, 1.0f); GL.Vertex3( 0.8f, -0.8f,  0.8f);  // Bottom Left Of The Texture and Quad
				//GL.Normal3( -1.0f,-1.0f, 1.0f);
				GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(-0.8f, -0.8f,  0.8f);  // Bottom Right Of The Texture and Quad
				// Right face
				GL.Normal3( 1.0f, 0.0f, 0.0f);      // Normal Facing Right
				//GL.Normal3( 1.0f, -1.0f, -1.0f);
				GL.TexCoord2(1.0f, 1.0f); GL.Vertex3( 0.8f, -0.8f, -0.8f);  // Bottom Right Of The Texture and Quad
				//GL.Normal3( 1.0f, 1.0f, -1.0f);
				GL.TexCoord2(1.0f, 0.0f); GL.Vertex3( 0.8f,  dirHeight, -0.8f);  // Top Right Of The Texture and Quad
				//GL.Normal3( 1.0f, 1.0f, 1.0f);
				GL.TexCoord2(0.0f, 0.0f); GL.Vertex3( 0.8f,  dirHeight,  0.8f);  // Top Left Of The Texture and Quad
				//GL.Normal3( 1.0f, -1.0f, 1.0f);
				GL.TexCoord2(0.0f, 1.0f); GL.Vertex3( 0.8f, -0.8f,  0.8f);  // Bottom Left Of The Texture and Quad
				// Left Face
				GL.Normal3(-1.0f, 0.0f, 0.0f);      // Normal Facing Left
				//GL.Normal3(-1.0f, -1.0f, -1.0f);
				GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(-0.8f, -0.8f, -0.8f);  // Bottom Left Of The Texture and Quad
				//GL.Normal3(-1.0f, -1.0f, 1.0f);
				GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(-0.8f, -0.8f,  0.8f);  // Bottom Right Of The Texture and Quad
				//GL.Normal3(-1.0f, 1.0f, 1.0f);
				GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(-0.8f,  dirHeight,  0.8f);  // Top Right Of The Texture and Quad
				//GL.Normal3(-1.0f, 1.0f, -1.0f);
				GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(-0.8f,  dirHeight, -0.8f);  // Top Left Of The Texture and Quad

				GL.End();                    // Done Drawing Quads

				GL.EndList();                // Finish display list 
			}
            
            return displayList;       
        }
        
        public static DateTime ConvertFromUnixTimestamp(UInt64 timestamp) {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }
        
		public static float GetHeightForFolder(int numChildren) {
            if(numChildren > 200) {
                return 4.0f;
            } else if(numChildren > 100) {
                  return (3.0f + (1.0f * ((numChildren-100)/150.0f)));
            } else if(numChildren > 50) {
                return (2.5f + (0.5f * ((numChildren -50)/50.0f)));   
            } else if(numChildren > 25) {
                return (2.0f + (0.5f * ((numChildren - 25)/25.0f)));   
            } else if(numChildren > 10) {
                return (1.6f + (0.4f * ((numChildren - 10)/15.0f)));    
            } else if (numChildren > 5) {
                return (1.23f + (0.37f * ((numChildren - 5)/5.0f)));
            } else {
                return (1.0f + (0.23f * (numChildren/5.0f)));   
            }
        }
    }
}
