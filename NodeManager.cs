
using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.Win32;
using System.Linq;

namespace glomp {


    public class NodeManager {
        
        private static int[] displayLists = new int[2];
        private static bool[] listGenerated = {false, false};
        
		public static readonly int FILE_NODE = 0;
		public static readonly int DIR_NODE = 1;

        private static readonly float BOX_SCALE = 0.8f;

		public static readonly bool showHidden = true;
        
        public NodeManager() {
        }
        
		public static FileNode[] GetFileNodesCollectionFromLocation(String path, FileSlice owner) {
		
			List<FileNode> fileNodesList = new List<FileNode>();

			IEnumerable<FileInfo> files;
			IEnumerable<DirectoryInfo> folders;

			DirectoryInfo dir = new DirectoryInfo(path);

			try {
				if (!showHidden) {
					folders = from directory in dir.EnumerateDirectories()
						          where (directory.Attributes & FileAttributes.Hidden) == 0
					          select directory;
					files = from file in dir.EnumerateFiles()
						        where (file.Attributes & FileAttributes.Hidden) == 0
					        select file;
				} else {
					folders = dir.EnumerateDirectories();
					files = dir.EnumerateFiles();
				}

				foreach (DirectoryInfo folder in folders) {
					FileNode node = NodeManager.GetFileNode(DIR_NODE, folder, owner);
					fileNodesList.Add(node);            
				}

				foreach (FileInfo file in files) {
					FileNode node = NodeManager.GetFileNode(FILE_NODE, file, owner);
					fileNodesList.Add(node);
				}
			}
			catch {
				Console.WriteLine("Directory {0}  \n could not be accessed!!!!", dir.FullName);                
				return null;  // We alredy got an error trying to access dir so dont try to access it again
			}

			fileNodesList[0].Active = true;

			return fileNodesList.ToArray();
		}

		public static FileNode GetFileNode(int nodeType, FileSystemInfo element, FileSlice owner) {
			String fileNodeBaseName = null; //only file name without path
			String fileNodeName = null; //full path

			DirectoryInfo folder = null;
			FileInfo file = null;
			FileNode node = null;

			if (nodeType == DIR_NODE) {
				folder = (DirectoryInfo)element;
				fileNodeBaseName = folder.Name;
				fileNodeName = folder.FullName;
				node = new FileNode(fileNodeBaseName);
				node.IsDirectory = true;

				try {
					node.NumDirs = folder.EnumerateDirectories().Count();
					node.NumFiles = folder.EnumerateFiles().Count();
					node.NumChildren = node.NumDirs + node.NumFiles;
					node.DirHeight = GetHeightForFolder(node.NumChildren);
				} catch {
					node.NumChildren = 0;
					node.DirHeight = 1f;
				}

				// Creation, last access, and last write time 
				node.CreationTime = folder.CreationTime;
				node.LastAccessTime = folder.LastAccessTime;
				node.ModifyTime = folder.LastWriteTime;

			} else if (nodeType == FILE_NODE) {
				file = (FileInfo)element;
				fileNodeBaseName = file.Name;
				fileNodeName = file.FullName;
				node = new FileNode(fileNodeBaseName);
				node.FileExtension = (file.Extension != string.Empty) ? file.Extension.Substring(1) : string.Empty; //without dot
				node.Description = GetMIMEDescription(file.Extension); //This will show what type of file it is
				node.IsReadOnly = file.IsReadOnly;
				node.IsExecutable = (node.FileExtension == "exe");

				//Getting file's ThumbNail (using Windows API Code Pack 1.1)
				ShellObject nodeFile = ShellObject.FromParsingName (fileNodeName);
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

				// Creation, last access, and last write time 
				node.CreationTime = file.CreationTime;
				node.LastAccessTime = file.LastAccessTime;
				node.ModifyTime = file.LastWriteTime;
			}

			node.File = fileNodeName;
			node.SetParent(owner);

			int fileDisplayList;
			if(listGenerated[nodeType]) {
				fileDisplayList = displayLists[nodeType];
			} else {
				fileDisplayList = displayLists[nodeType] = GenerateDisplayList(nodeType);
			}
            node.SetDisplayList(fileDisplayList);
            
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
