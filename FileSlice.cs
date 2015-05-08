
using System;
using System.IO;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Shell;

namespace glomp {


    public class FileSlice : SceneNode {
        
        private static readonly int X = 0;
        private static readonly int Y = 1;  
		private static readonly int leftVisible = -3;//-3 optimal fps when scrolling;
		private static readonly int rightVisible = 3;//3 optimal fps when scrolling;
		private static readonly int forwardVisible = 8;//8 optimal fps when scrolling;
		private static readonly int backVisible = -2;//-2 optimal fps when scrolling;
        
        private static readonly float STARTX = 6.0f;
        private static readonly float STARTY = -2.0f;
        private static readonly float STARTZ = 10.0f;
        private static readonly float ASPECT_COEFF = 1.6f;     
		private static readonly int SHOW_ALL_LIMIT = 10000;//400; //In fact the lower the value, the faster such a directory will load. However, scrolling fps performance will be very bad (due to live drawing). So, for VR purposes it's better to draw every single file at once - better fps performance. 10000 files limit seems to be "optimal" ;)
        
		public static readonly float BOX_SPACING_X = 6.0f;
		public static readonly float BOX_SPACING_Z = 6.0f;  
        public static readonly Vector3 NO_VECTOR = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        
        public static readonly int BY_TYPE = 0;
        public static readonly int BY_NAME = 1;
        public static readonly int BY_DATE = 2;
        
        public Vector3 camOffset = new Vector3(5.0f, -12.0f, 32.0f);

        private int gridWidth;
        private int gridHeight;
        private int[] activeBox = {0,0};
        private bool showAllText = false;      
        private Node toNode;
		private int numFiles = 0;
        private String path;
        private int sliceHeight;
		private float alpha = 0.5f;
        private bool isDimmed = false;
        private int lastLetterPosition = -1;
        private String lastLetter = "";
        private float scale = 0.0f;
        private bool isScaled = false;
        private int currentSortType = BY_NAME;
        private bool visible = true;
        private MainWindow parentWindow;
        public int cullCount = 0;
		public static readonly bool showHidden = true;
        public Node[] fileNodes;

		//Shaders related stuff
		/////////////////////////////////////////////////////////////////////////////////////////////////////////
		private Matrix4 fileSliceModelMatrix = Matrix4.Identity;
		/////////////////////////////////////////////////////////////////////////////////////////////////////////
        

		public Matrix4 FileSliceModelMatrix {
			get { return fileSliceModelMatrix; }
			set { fileSliceModelMatrix = value; }
		}

        public float Alpha {
            get { return alpha; }
            set {
                if(value > 1.0f) {
                    alpha = 1.0f;
                } else if(value < 0.0f) {
                    alpha = 0.0f;
                } else {
                    alpha = value;
                }
            }
        }
       
		public bool ShowAllText {
			get { return showAllText; }
			set { showAllText = value; }
		}
        
        public bool Visible {
            get { return visible; }
            set { visible = value; }
        }
        
        public int CurrentSortType {
            get { return currentSortType;}
        }
        
        public float Scale {
            get { return scale;}
            set { scale = value;}
        }
        
        public bool IsScaled {
            get { return isScaled;}
            set { isScaled = value;}
        }
  
        public String Path {
            get { return path; }
        }
        
        public int SliceHeight {
            get { return sliceHeight; }
        }
        
        public int[] ActiveBox {
            get { return activeBox; }
        }
        
        public int NumFiles {
            get { return numFiles; }
        }
        
        public FileSlice(String _path, int _sliceHeight, MainWindow parent)
            : base () {
			Trace.WriteLine ("FileSlice");
			path = _path;
			sliceHeight = _sliceHeight;
			parentWindow = parent;
		}

		public void FillFileSliceWithDrives () {
			DriveInfo[] drives;
			drives = DriveInfo.GetDrives();
			foreach (DriveInfo drive in drives) {
				DriveNode node = new DriveNode (drive.Name);

				DirectoryInfo directory = new DirectoryInfo (drive.Name); //"converting" drive to directory

				try {
					node.NumDirs = directory.EnumerateDirectories ().Count ();
					node.NumFiles = directory.EnumerateFiles ().Count ();
					node.NumChildren = node.NumDirs + node.NumFiles;
					node.DirHeight = node.GetHeightForFolder (node.NumChildren);
				} catch {
					node.NumChildren = 0;
					node.DirHeight = 1f;
				}

				// Creation, last access, and last write time 
				node.CreationTime = directory.CreationTime;
				node.LastAccessTime = directory.LastAccessTime;
				node.ModifyTime = directory.LastWriteTime;

				node.File = drive.Name;
				node.SetParent (this);

				List<Node> fileNodesList = null;
				if (fileNodes == null) {
					fileNodesList = new List<Node> ();
				} else {
					fileNodesList = new List<Node> (fileNodes);
				}
				fileNodesList.Add (node);
				fileNodes = fileNodesList.ToArray();
			}

			fileNodes[0].Active = true;

			SetFileSliceParameters ();
		}


		public void FillFileSliceWithDirectoriesAndFiles () {
		
			while (parentWindow.InTransition) {
				//SIMPLY WAIT TILL THE END OF TRANSITION ANIMATION
				//to eliminate annoying lags (due to disk operations during transition animation)
			}

            // set up storage
			GetFileNodesCollectionFromLocation(path);
			SetFileSliceParameters ();
        }


		public void SetFileSliceParameters() {

			if (fileNodes == null) {
				//nothing to show - directory empty
				return;
			}
			else {
				numFiles = fileNodes.Length;
			}

			// decide if we will show all the labels, or just the ones near us
			if (fileNodes.Length < SHOW_ALL_LIMIT) {
				showAllText = true;
			} else {
				showAllText = false;
			}

			if(numFiles > 0) {
				// set up width and height in boxes
				gridWidth = (int)Math.Round(Math.Sqrt(fileNodes.Length)/ASPECT_COEFF, 0);
				gridHeight = fileNodes.Length / gridWidth;
				if(fileNodes.Length % gridHeight > 0)
					gridHeight += 1;

				//assign position for each node
				int nodeCount = 0;
				foreach (var node in fileNodes) {
					float xPosition = STARTX - ((nodeCount % gridWidth) * BOX_SPACING_X);
					float zPosition = STARTZ + ((nodeCount / gridWidth) * BOX_SPACING_Z);
					node.Position = new Vector3(xPosition, STARTY, zPosition);
					nodeCount++;
				}
			}
		}
        

		public void GetFileNodesCollectionFromLocation(String path) {
			//Count of files traversed and timer for diagnostic output 
			int fileCount = 0;
			//var sw = Stopwatch.StartNew();

			// Determine whether to parallelize file processing on each folder based on processor count. 
			int procCount = System.Environment.ProcessorCount;

			if (!Directory.Exists(path)) {
				return;
			}

			// Data structure to hold names of subfolders to be examined for files.
			Stack<FileSystemInfo> directoriesAndFiles = new Stack<FileSystemInfo>();

			DirectoryInfo rootDir = new DirectoryInfo(path);
			IEnumerable<FileSystemInfo> directories = null;
			IEnumerable<FileSystemInfo> files = null;

			try {
				if (!showHidden) {
					directories = from directory in rootDir.EnumerateDirectories()
							where (directory.Attributes & FileAttributes.Hidden) == 0
						select directory;
					files = from file in rootDir.EnumerateFiles()
							where (file.Attributes & FileAttributes.Hidden) == 0
						select file;
				} else {
					directories = rootDir.EnumerateDirectories();
					files = rootDir.EnumerateFiles();
				}

				foreach (FileSystemInfo directory in directories)
					directoriesAndFiles.Push(directory);
					
				foreach (FileSystemInfo file in files)
					directoriesAndFiles.Push(file);
			}
			// Thrown if we do not have discovery permission on the directory. 
			catch (UnauthorizedAccessException e) {
				System.Diagnostics.Debug.WriteLine(e.Message);
			}
			// Thrown if another process has deleted the directory after we retrieved its name. 
			catch (DirectoryNotFoundException e) {
				System.Diagnostics.Debug.WriteLine(e.Message);
			}
			catch (IOException e) {
				System.Diagnostics.Debug.WriteLine(e.Message);
			}

			// Execute in parallel if there are enough files in the directory. 
			// Otherwise, execute sequentially.Files are opened and processed 
			// synchronously but this could be modified to perform async I/O. 
			try {

				if (directoriesAndFiles.Count < procCount) {
					foreach (var element in directoriesAndFiles) {
						GetFileNode(element, null, 0);
						fileCount++;                            
					}
				}
				else {
					Parallel.ForEach(directoriesAndFiles, () => 0, GetFileNode, (c) => {
						Interlocked.Add(ref fileCount, c);                          
					});
				}
			}
			catch (AggregateException ae) {
				ae.Handle((ex) => {
					if (ex is UnauthorizedAccessException) {
						// Here we just output a message and go on.
						System.Diagnostics.Debug.WriteLine(ex.Message);
						return true;
					}
					// Handle other exceptions here if necessary... 

					return false;
				});
			}

			if (fileNodes != null && fileNodes.Length != 0)
				fileNodes[0].Active = true;

			// For diagnostic purposes.
			//Trace.WriteLine ("Processed {0} files in {1} milleseconds", fileCount, sw.ElapsedMilliseconds);
		}


		public int GetFileNode(FileSystemInfo element, ParallelLoopState loopState, int elementsCount) {
			Trace.WriteLine ("GetFileNode");
			String fileNodeBaseName = null; //only file name without path
			String fileNodeName = null; //full path

			DirectoryInfo folder = null;
			FileInfo file = null;
			Node node = null;

			// Determine if entry is really a directory
			if ((element.Attributes & FileAttributes.Directory) == FileAttributes.Directory) {
				folder = (DirectoryInfo)element;
				fileNodeBaseName = folder.Name;
				fileNodeName = folder.FullName;
				node = new DirectoryNode (fileNodeBaseName);

				try {
					((DirectoryNode)node).NumDirs = folder.EnumerateDirectories ().Count ();
					((DirectoryNode)node).NumFiles = folder.EnumerateFiles ().Count ();
					((DirectoryNode)node).NumChildren = ((DirectoryNode)node).NumDirs + ((DirectoryNode)node).NumFiles;
					((DirectoryNode)node).DirHeight = ((DirectoryNode)node).GetHeightForFolder (((DirectoryNode)node).NumChildren);
				} catch {
					((DirectoryNode)node).NumChildren = 0;
					((DirectoryNode)node).DirHeight = 1f;
				}

				// Creation, last access, and last write time
				node.CreationTime = folder.CreationTime;
				node.LastAccessTime = folder.LastAccessTime;
				node.ModifyTime = folder.LastWriteTime;

			} else { //FileNode
				file = (FileInfo)element;
				fileNodeBaseName = file.Name;
				fileNodeName = file.FullName;
				node = new FileNode (fileNodeBaseName);
				((FileNode)node).FileExtension = (file.Extension != string.Empty) ? file.Extension.Substring (1) : string.Empty; //without dot
				((FileNode)node).Description = ((FileNode)node).GetMIMEDescription (file.Extension); //This will show what type of file it is
				((FileNode)node).IsReadOnly = file.IsReadOnly;
				((FileNode)node).IsExecutable = (((FileNode)node).FileExtension == "exe");

				//Getting file's ThumbNail (using Windows API Code Pack 1.1)
				ShellObject nodeFile = ShellObject.FromParsingName (fileNodeName);
				nodeFile.Thumbnail.FormatOption = ShellThumbnailFormatOption.ThumbnailOnly;
				try {
					((FileNode)node).ThumbBmp = nodeFile.Thumbnail.Bitmap;
				} catch {
					//NotSupportedException
					System.Diagnostics.Debug.WriteLine ("Error getting the thumbnail. The selected file does not have a valid thumbnail or thumbnail handler.");

					// If we get an InvalidOperationException and our mode is Mode.ThumbnailOnly,
					// then we have a ShellItem that doesn't have a thumbnail (icon only).
					((FileNode)node).ThumbBmp = null;
				}

				// Creation, last access, and last write time 
				node.CreationTime = file.CreationTime;
				node.LastAccessTime = file.LastAccessTime;
				node.ModifyTime = file.LastWriteTime;
			}

			node.File = fileNodeName;
			node.SetParent (this);

			lock (this) { //Locking is needed, as file nodes are being created asynchronously by parallel calls from GetFileNodesCollectionFromLocation()
				if (fileNodes != null) {
					Array.Resize (ref fileNodes, fileNodes.Length + 1);
					fileNodes[fileNodes.Length - 1] = node;
				} else {
					fileNodes = new Node[1];
					fileNodes [0] = node;
				}
			}

			return ++elementsCount;
		}

        
        public void ReFormat(int sortBy) {
            currentSortType = sortBy;
            GetActiveNode().Active = false;
            if(sortBy == BY_TYPE) {
                // re sort filenode array by descritpion
                SortType();
            } else if(sortBy == BY_NAME) {
                SortName();                
            } else if(sortBy == BY_DATE) {
                SortDate();
            }
            
            // set positions based on new order
            for(int i = 0; i < fileNodes.Length; i++) {
				float xPosition = STARTX - ((i % gridWidth) * BOX_SPACING_X);
				float zPosition = STARTZ + ((i / gridWidth) * BOX_SPACING_Z);
                fileNodes[i].Position = new Vector3(xPosition, STARTY, zPosition);
                        
            }
            fileNodes[(activeBox[1] * gridWidth) + activeBox[0]].Active = true;
            scale = 0f;
            isScaled = true;
        }

        
        public void ResetVisible() {
			Trace.WriteLine ("ResetVisible");
            // caclulate x,ys for visible nodes
            if(showAllText) {
                return;
            }
            int[] minBox = { activeBox[X] + leftVisible, activeBox[Y] + backVisible };
            int[] maxBox = { activeBox[X] + rightVisible, activeBox[Y] + forwardVisible };
            
            int genCounter = 0;
            int destCounter = 0;
            Node myNode = null;
            
            // for all visible nodes
            for(int y = minBox[Y]; y < maxBox[Y]; y++) {
                // sanity check
                if((y < 0 || y > gridHeight-1))
                    continue;
                        
                for(int x = minBox[X]; x < maxBox[X]; x++) {
                    // sanity check
                    if((x < 0 || x > gridWidth-1))
                        continue;
                    
                    // try to get a fileNode for these coords
                    try {
                        myNode = fileNodes[(y * gridWidth) + x];
                    } catch {
                        continue;
                    }
                   
                    if(!myNode.Visible) {
                        myNode.Visible = true;
                        myNode.GenTexture(false);
                        genCounter++;
                    }
                }       
            }
                     
            // for all other positions
            for(int i = 0; i < fileNodes.Length; i++) {
                int x = i % gridWidth;
                int y = i / gridWidth;
                
                // dont include visible ones!
                if( (y >= minBox[Y] && y <= maxBox[Y]) && (x >= minBox[X] && x <= maxBox[X]) ) {
                        continue;
                }
                
                myNode = fileNodes[i];
                
                if(myNode.Visible) {
                        myNode.Visible = false;
                        myNode.DestroyTexture();
                        destCounter++;
                 }
            }
            
			System.Diagnostics.Debug.WriteLine("Generated " + genCounter + " textures.");
			System.Diagnostics.Debug.WriteLine("Destroyed " + destCounter + " textures.");
        }
        
     
        public void Render(FrustumCuller culler) {
            if(visible) {
                // render the array in reverse order, first boxes then labels
                float sliceX, sliceY, sliceZ;
                sliceX = this.position.X;
                sliceY = this.position.Y;
                sliceZ = this.position.Z;
                cullCount = 0;

				/*
				// GROUND PLANE
				GL.PushMatrix();

				GL.Disable (EnableCap.DepthTest);
				GL.Enable (EnableCap.Texture2D);
				// bind texture to texture unit 0
				GL.ActiveTexture(TextureUnit.Texture0);
				GL.BindTexture(TextureTarget.Texture2D, texture);
				GL.Begin(PrimitiveType.Quads);
				float numberOfTextureRepeats = 30.0f;
				float mSize = 500.0f;
				GL.Normal3( -1.0f, 1.0f, -1.0f);
				float cameraPositionZ = parentWindow.GetCamera().Position.Z;
				float cameraPositionX = parentWindow.GetCamera().Position.X;
				GL.TexCoord2(numberOfTextureRepeats, numberOfTextureRepeats); GL.Vertex3( (float)(cameraPositionX + mSize/2f), sliceY, (cameraPositionZ + mSize/2f));
				GL.Normal3( -1.0f, 1.0f, 1.0f);
				GL.TexCoord2(numberOfTextureRepeats, 0.0f); GL.Vertex3(  (float)(cameraPositionX + mSize/2f), sliceY, (cameraPositionZ - mSize/2f));
				GL.Normal3( 1.0f, 1.0f, 1.0f);
				GL.TexCoord2(0.0f, 0.0f); GL.Vertex3( (float)(cameraPositionX -mSize/2f), sliceY, (cameraPositionZ - mSize/2f));
				GL.Normal3( 1.0f, 1.0f, -1.0f);
				GL.TexCoord2(0.0f, numberOfTextureRepeats); GL.Vertex3( (float)(cameraPositionX -mSize/2f), sliceY,  (cameraPositionZ + mSize/2f));
				GL.Disable (EnableCap.Texture2D);
				GL.Enable(EnableCap.DepthTest);
				GL.End ();

				GL.PopMatrix();
				*/

				fileSliceModelMatrix = parentWindow.GetCamera().CameraModelMatrix;

				MoveIntoPosition(false, ref fileSliceModelMatrix);
                Node.SetBoxState(isDimmed);
                for(int i = fileNodes.Length-1; i >= 0; i--) {
                    float nodeZ = sliceZ + fileNodes[i].Position.Z;
					float nodeX = sliceX + fileNodes [i].Position.X - 2.1f;
                    float nodeY = sliceY + fileNodes[i].Position.Y;
					if( /*(nodeZ > parentWindow.GetCamera().Position.Z) && */ //If you don't want to show all the boxes that are behind you - uncomment this line; For VR we need every box to be visible
                       (culler.isBoxVisible(nodeX, nodeY, nodeZ, 3f, 8f)) ) { //TODO: correct culler
                        
                        fileNodes[i].culled = false;
						fileNodes[i].DrawBox(i, parentWindow.FrameDelta);	//the main boxes drawing method
                    } else {
                        fileNodes[i].culled = true;
                        cullCount++;       
                    } 
                }
                Node.UnsetBoxState(isDimmed);
                
                Node.SetTextState(isDimmed);
                for(int i = fileNodes.Length-1; i >= 0; i--) {
                    if(!fileNodes[i].culled) {
                        fileNodes[i].DrawLabel();
                    }
                }
                Node.UnsetTextState(isDimmed);
            }
        }
        
        public override void Render() {
            // render without culler
        }
        
        
        public Node GetActiveNode() {
			if (fileNodes.Length > 0)
				return fileNodes [(activeBox [Y] * gridWidth) + activeBox [X]];
			else
				return null; // new Node(this.Position);
        }
        
        
        public bool MoveCarat(int xMove, int yMove) {
            // check that we stay in our grid
            int targetX = activeBox[X] + xMove;
            int targetY = activeBox[Y] + yMove;
            if( (targetX < 0 || targetX > gridWidth-1) || (targetY < 0 || targetY > gridHeight-1) ) {
                return false;
            }
            
            // check that we have a file under us
            try {
                toNode = fileNodes[(targetY * gridWidth) + targetX];
            } catch {
                return false;
            }
            
			fileNodes[(activeBox[Y] * gridWidth) + activeBox[X]].Active = false;
            activeBox[X] += xMove;
            activeBox[Y] += yMove;
            toNode.Active = true;
            return true;
        }

        
		public void GenerateAllTextures() {
			Trace.WriteLine ("GenerateAllTextures");
            foreach(var node in fileNodes) {
                node.Visible = true;
                node.GenTexture(false);
            }
        }
        
        
        public void Destroy() {
            foreach(var node in fileNodes) {
                node.DestroyTexture();
            }
        }
        
        
        public Vector3 FindNodePosition(String file) {
            for( int i = 0; i < fileNodes.Length; i++) {
                if(fileNodes[i].File == file) {
					System.Diagnostics.Debug.WriteLine("Found " + file);
                    return fileNodes[i].Position;
                }
            }
            return NO_VECTOR;
        }
        
        
        public Vector3 ActivateNode(String file) {
           for( int i = 0; i < fileNodes.Length; i++) {
                if(fileNodes[i].File == file) {
					System.Diagnostics.Debug.WriteLine("Found " + file);
                    GoToNode(i);
                    return fileNodes[i].Position;
                }
            }

            return NO_VECTOR;
        }
        
        
        public void DeActivate() {
            GetActiveNode().Active = false;
        }
        
        
        public void Activate() {
            GetActiveNode().Active = true;
        }
        
        
        public void GoToLetter(String letter) {
            letter = letter.ToLower();
            if(letter == lastLetter) {
                for(int i = lastLetterPosition + 1; i < fileNodes.Length; i++) {
                   if( fileNodes[i].FileName.StartsWith(letter,true, null) ) {
                        lastLetter = letter;
                        lastLetterPosition = i;
                        GoToNode(i);
                        return;
                    }    
                }
            }            
            for( int i = 0; i < fileNodes.Length; i++ ) {
                if( fileNodes[i].FileName.StartsWith(letter, true, null) ) {
                    lastLetter = letter;
                    lastLetterPosition = i;
                    GoToNode(i);
                    return;
                }
            }
        }
        
        
        public bool GoToPattern(String pattern) {
            pattern = pattern.ToLower();
            for(int i = 0; i < fileNodes.Length; i++ ) {
                if(fileNodes[i].FileName.StartsWith(pattern, true, null)) {
                   GoToNode(i);
                    return true;
                }
            }
            for(int i = 0; i < fileNodes.Length; i++ ) {
                if(fileNodes[i].FileName.ToLower().Contains(pattern)) {
                   GoToNode(i);
                    return true;
                }
            }
            return false;    
        }
        
        
        public void GoToNode(int position) {
            GetActiveNode().Active = false;
            activeBox[0] = position % gridWidth;
            activeBox[1] = position / gridWidth;
            fileNodes[position].Active = true;
            ResetVisible();
        }
        
        
        public void HideAllNodes() {
            foreach(var fileNode in fileNodes) {
                fileNode.Dimmed = true;
            }
            isDimmed = true;
        }
        
        
        public void ShowLabels() {
            alpha = 0.5f;
            foreach(var fileNode in fileNodes) {
                fileNode.Dimmed = false;
            }
            isDimmed = false;
        }
        
        
        private void SortName() {
            Array.Sort(fileNodes, delegate(Node node1, Node node2) {
                if(node1.IsDirectory == node2.IsDirectory) {
                    return node1.FileName.CompareTo(node2.FileName);
                } else {
                    if(node1.IsDirectory) {
                        return -1;
                    } else {
                        return 1;
                    }
                }
            });          
        }
        
        
        private void SortType() {
            Array.Sort(fileNodes, delegate(Node node1, Node node2) {
				if ((node1 is FileNode) && (node2 is FileNode)) {
					int compareVal = ((FileNode)node1).Description.CompareTo(((FileNode)node2).Description);
	                if(compareVal == 0) {
	                    String ext1, ext2;
	                    try { 
	                        String[] split1 = node1.FileName.Split('.');
	                        String[] split2 = node2.FileName.Split('.');
	                        ext1 = split1[split1.Length-1];
	                        ext2 = split2[split2.Length-1];
	                    } catch {
	                        return node1.FileName.CompareTo(node2.FileName);
	                    }
	                    compareVal = ext1.CompareTo(ext2);
	                    if(compareVal == 0) {
	                        return node1.FileName.CompareTo(node2.FileName);
	                    } else {
	                        return compareVal;
	                    }
	                } else {
	                    return compareVal;
	                }
				} else if ((node1 is DirectoryNode) && (node2 is FileNode)) {
					return -1; //Directories first
				} else { //node1 is a Directory and node2 is a Directory
					return node1.FileName.CompareTo(node2.FileName);
				}
            });   
        }
        
        private void SortDate() {
            Array.Sort(fileNodes, delegate(Node node1, Node node2) {
                if(node1.IsDirectory == node2.IsDirectory) {
                    return node1.ModifyTime.CompareTo(node2.ModifyTime);
                } else {
                    if(node1.IsDirectory) {
                        return -1;
                    } else {
                        return 1;
                    }
                }
            }); 
        }
        
        public void FadeDirectories(bool fadeOut) {
            foreach(Node node in fileNodes) {
                if(node.IsDirectory) {
					((DirectoryNode)node).DirFaded = fadeOut;
                    if(fadeOut) {
						((DirectoryNode)node).FadeAmount = 0.1f;
                    }
                }
            }
        }
        
        public void ResetDirFade() {
            for(int i = 0; i < fileNodes.Length; i++) {
                if(fileNodes[i].IsDirectory) {
                    if(i / gridWidth < activeBox[1])  { // if we are behind current active
						((DirectoryNode)fileNodes[i]).DirFaded = true;
						((DirectoryNode)fileNodes[i]).FadeAmount = 0.3f;
                        
                    } else {
						((DirectoryNode)fileNodes[i]).DirFaded = false;
                    }
                }
            }
        }
        
        public void RenameActiveNode(String newFileName) {
            String oldFileName = fileNodes[(activeBox[Y] * gridWidth) + activeBox[X]].FileName;
            fileNodes[(activeBox[Y] * gridWidth) + activeBox[X]].FileName = newFileName;
            fileNodes[(activeBox[Y] * gridWidth) + activeBox[X]].File = fileNodes[(activeBox[Y] * gridWidth) + activeBox[X]].File.Replace(oldFileName, newFileName);
            fileNodes[(activeBox[Y] * gridWidth) + activeBox[X]].GenTexture(true);
            
        }
        
        public bool ToggleSelected() {
            fileNodes[(activeBox[Y] * gridWidth) + activeBox[X]].Selected = !fileNodes[(activeBox[Y] * gridWidth) + activeBox[X]].Selected;
            return fileNodes[(activeBox[Y] * gridWidth) + activeBox[X]].Selected;
            
        }  
    }
}
