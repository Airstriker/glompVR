
using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.Win32;

namespace glomp {


    public class NodeManager {
        
        private static int[] displayLists = new int[2];
        private static bool[] listGenerated = {false, false};
        
        public static readonly int FILE_NODE = 0;
        public static readonly int DIR_NODE = 1;
        private static readonly float BOX_SCALE = 0.8f;
        
        public NodeManager() {
        }
        
        public static FileNode GetFileNode(int nodeType, String fileName, FileSlice owner) {
            int fileDisplayList;
            
            if(listGenerated[nodeType]) {
                fileDisplayList = displayLists[nodeType];
            } else {
                fileDisplayList = displayLists[nodeType] = GenerateDisplayList(nodeType);
            }
            
            GLib.File fi = GLib.FileFactory.NewForPath(fileName);
            
            FileNode node = new FileNode(fi.Basename);
            node.File = fileName;
            //node.NumChildren = fi.
			System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileName);
            node.SetDisplayList(fileDisplayList);
            
            node.SetParent(owner);
            
            if(nodeType == DIR_NODE) {
                node.IsDirectory = true;
                try {
                    node.NumDirs = Directory.GetDirectories(fileName).Length;
                    node.NumFiles = Directory.GetFiles(fileName).Length;
                    node.NumChildren = node.NumDirs + node.NumFiles;
                    node.DirHeight = GetHeightForFolder(node.NumChildren);
                } catch {
                    node.NumChildren = 0;
                    node.DirHeight = 1f;
                }
            } else {
				node.FileExtension = fileInfo.Extension.Substring(1); //without dot
				node.Description = GetMIMEDescription(fileInfo.Extension); //This will show what type of file it is
				node.IsReadOnly = fileInfo.IsReadOnly;
				node.IsExecutable = (node.FileExtension == "exe");

				//Getting file's ThumbNail (using Windows API Code Pack 1.1)
				ShellObject nodeFile = ShellObject.FromParsingName (fileInfo.FullName);
				nodeFile.Thumbnail.FormatOption = ShellThumbnailFormatOption.ThumbnailOnly;
				try {
					node.ThumbBmp = nodeFile.Thumbnail.Bitmap;
				}
				catch (NotSupportedException) {
					Console.WriteLine("Error getting the thumbnail. The selected file does not have a valid thumbnail or thumbnail handler.");
				}
				catch (InvalidOperationException)
				{
					// If we get an InvalidOperationException and our mode is Mode.ThumbnailOnly,
					// then we have a ShellItem that doesn't have a thumbnail (icon only).
					node.ThumbBmp = null;
				}

            }

			// Creation, last access, and last write time 
			node.CreationTime = fileInfo.CreationTime;
			node.LastAccessTime = fileInfo.LastAccessTime;
			node.ModifyTime = fileInfo.LastWriteTime;
            
            return node;
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
							return contentType;
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
                listGenerated[nodeType] = true;
                
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
                listGenerated[nodeType] = true;  
            }
            
            return displayList;       
        }
        
        public static DateTime ConvertFromUnixTimestamp(UInt64 timestamp) {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }
        
        private static float GetHeightForFolder(int numChildren) {
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
