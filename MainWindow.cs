#region --- License ---
/* Copyright (c) 2014 Julian Sychowski
 * See license.txt for license info
 */
#endregion

using System;
using System.Collections.Generic;
//using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Diagnostics;
using System.IO;
using glomp;
using System.Linq;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Threading;
using System.Runtime.InteropServices;
using OpenTK.Platform;
using OculusWrap;
using OculusWrap.GL;


public partial class MainWindow : GameWindow
{

    #region --- Private Fields ---

    private Stopwatch frameTimer = new Stopwatch();
    private int frameCounter = 0;
    private long currentTicks = 0;

    private float[] lightAmbient = { 0.1f, 0.1f, 0.1f, 1.0f };
    private float[] lightDiffuse = { 1.0f, 1.0f, 1.0f, 1.0f };
    private float[] lightPosition = { 50.0f, 100.0f, -20.0f, 1.0f };

    //SkyBox object
    private SkyBox skyBox;

    //Mouse object
    private Mouse mouse;

    private static readonly String START_PATH = "/";
    private static readonly Vector3 CAM_OFFSET = new Vector3(5.0f, -12.0f, 32.0f);
    private static readonly String[] ALPHABET = {"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", 
    "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"};
    //private static readonly float[] HIGH_COLOUR = {0.5f, 0.5f, 0.7f, 0f};
    private static readonly float[] HIGH_COLOUR = { 0.11f, 0.15f, 0.25f, 0f };
    private static readonly float[] LOW_COLOUR = { 0.06f, 0.06f, 0.13f, 0f };
    private static readonly float[] BLACK = { 0f, 0f, 0f, 0f };
    private static readonly float[] BACKGROUND = { 0.1f, 0.1f, 0.2f, 0.0f };

    private readonly float rDiffDown = BACKGROUND[0] - LOW_COLOUR[0];
    private readonly float gDiffDown = BACKGROUND[1] - LOW_COLOUR[1];
    private readonly float bDiffDown = BACKGROUND[2] - LOW_COLOUR[2];

    private readonly float rDiffUp = BACKGROUND[0] - HIGH_COLOUR[0];
    private readonly float gDiffUp = BACKGROUND[1] - HIGH_COLOUR[1];
    private readonly float bDiffUp = BACKGROUND[2] - HIGH_COLOUR[2];

    private float activeRotateValue = 0.0f;
    private float frameDelta = 0.0005f;

    private bool inTransition = false;
    private bool inVerticalTransition = false;
    private Vector3 camTransitionTarget;
    private float camDelay = 1.0f;
    private Vector3 camTransitionStart;
    private Vector3 camTransitionVector;
    private bool transitionTargetUpdated = false;
    private float targetDistance;
    private float nextTargetDistance;
    private bool searchFound = false;
    private bool viewingDir = true;

    private Camera cam = new Camera();
    private Vector3 camStartPosition = new Vector3(1.0f, 10.0f, -22.0f);
    private float camStartPitch = 15.0f;
    private float camStartYaw = 5.0f;
    private FileSlice sliceToFade;
    private bool fadeOut;
    private bool scaleIn = false;
    private bool doScaleIn = false;
    //private FilenameCompleter completer = new FilenameCompleter();
    //private EntryCompletion completion;
    //private ListStore store;
    //private AboutDialog about;
    private bool vsync = true;
    //private Label label; 
    private bool textFocus;
    private float[] backgroundColour = (float[])BACKGROUND.Clone();
    private bool heightCueEnabled = false;
    private int culledThisFrame = 0;

    static Dispatcher MainThreadDispatcher;

    private LinkedList<SliceManager> sceneList = new LinkedList<SliceManager>();
    private SliceManager slices;
    private LinkedList<Node> selectedNodes = new LinkedList<Node>();

    #endregion


    private void OnKeyDownInternal(object sender, OpenTK.Input.KeyboardKeyEventArgs e) { OnKeyDown(e); }

    public bool InTransition
    {
        get { return inTransition; }
        set { inTransition = value; }
    }

    public float FrameDelta
    {
        get { return frameDelta; }
        set { frameDelta = value; }
    }

    //Max number of threads running in parallel (during I/O operations)
    public int MaxDegreeOfParallelism { get; set; }

    /* Constructor */
    public MainWindow()
        : base(1024, 600, new OpenTK.Graphics.GraphicsMode(32, 24, 8, 8), "GlompVR")
    { //MSAA (MultiSample AntiAliasing enabled - 8 samples
        //Build();

        //System.Threading.Thread.CurrentThread.Priority = ThreadPriority.Highest;

        //Determining number of CPU cores
        int coreCount = 0;
        foreach (var item in new System.Management.ManagementObjectSearcher("Select NumberOfCores from Win32_Processor").Get()) //needs reference to System.Management
        {
            coreCount += int.Parse(item["NumberOfCores"].ToString());
        }
        System.Diagnostics.Debug.WriteLine("The number of cores on this computer is {0}.", coreCount);

        //Setting maximum number of threads running in parallel (during I/O operations)
        MaxDegreeOfParallelism = (int)Math.Floor((double)coreCount / 2); //needs to be determined for smooth animations (I/O operations highly affect OpenGL rendering performance, so the number of threads doing I/O in parallel have to be limited)
        System.Diagnostics.Debug.WriteLine("The number of cores used for parallel I/O operations is equal to NumberOfCores divided by 2, that is {0}.", MaxDegreeOfParallelism);


        //entry4.Activated += new System.EventHandler(this.OnTextEntered);
        //findEntry.Activated += new System.EventHandler(this.OnSearchActivated);

        InitGL();

        // setup the scene

        // init SkyBox
        skyBox = new SkyBox(50.0f); //used with VBO
        InitScene();

        /*
        completer.DirsOnly = true;
        completion = new EntryCompletion();
        entry4.Completion = completion;
        completion.TextColumn = 0;
        store = new ListStore(GType.String);
        completion.Model = store;
        completion.MinimumKeyLength = 1;
        */

        // init mouse
        mouse = new Mouse();

        KeyDown += OnKeyDownInternal;

        MainThreadDispatcher = Dispatcher.CurrentDispatcher;
    }


    /* Init OpenGL*/
    private void InitGL()
    {

        Trace.Listeners.Add(new TextWriterTraceListener("C:\\dupa.txt"));
        Trace.AutoFlush = true;

        OpenTK.Graphics.GraphicsContext.ShareContexts = true;

        // open GL setup
        InitOrUpdateProjectionMatrix();

        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);
        GL.CullFace(CullFaceMode.Back);

        /* In some cases, you might want to disable depth testing and still allow the depth buffer updated while you are rendering your objects.
         * It turns out that if you disable depth testing (glDisable(GL_DEPTH_TEST)​), GL also disables writes to the depth buffer.
         * The correct solution is to tell GL to ignore the depth test results with glDepthFunc(GL_ALWAYS)​.
         * Be careful because in this state, if you render a far away object last, the depth buffer will contain the values of that far object.
        */
        GL.DepthFunc(DepthFunction.Always);
        GL.ShadeModel(ShadingModel.Smooth); //smooth or flat

        GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

        // set background colour
        GL.ClearColor(backgroundColour[0], backgroundColour[1], backgroundColour[2], backgroundColour[3]);

        VSync = VSyncMode.Off;

        System.Diagnostics.Debug.WriteLine(OpenTK.Graphics.GraphicsContext.CurrentContext.GraphicsMode.ToString());
    }


    private void InitScene()
    {

        slices = new SliceManager(this);
        DirectoryNode.LoadNodeTextures(); //Drive Nodes derive the textures from Directories, so no need to initialize them
        NodeManager.LoadVBOs();
        slices.Reset(START_PATH);

        sceneList.AddLast(slices);

        // Set up the camera
        cam.Put(camStartPosition, camStartPitch, camStartYaw);
        doScaleIn = true;
    }


    /* MainWindow render callback */
    protected override void OnRenderFrame(FrameEventArgs e)
    {

        base.OnRenderFrame(e);

        if (!frameTimer.IsRunning)
        {
            frameTimer.Start();
        }

        culledThisFrame = 0;

        RenderScene();

        if (frameTimer.ElapsedMilliseconds > 1000)
        {
            this.Title = "GLompVR by Airstriker. " + frameCounter + "fps. ActiveSlice Path: " + slices.ActiveSlice.Path + " Number of nodes on ActiveSlice: " + slices.ActiveSlice.NumFiles + ". Culled: " + culledThisFrame + " nodes.";
            frameCounter = 0;
            currentTicks = 0;
            frameTimer.Reset();
            frameTimer.Start();
        }
        else
        {
            frameDelta = (frameTimer.ElapsedTicks - currentTicks) / 10000000.0f;
            currentTicks = frameTimer.ElapsedTicks;
        }

        frameCounter++;
        SwapBuffers();
    }


    /* My scene rendering logic */
    private void RenderScene()
    {

        UpdateScene();

        // Clearing all
        //GL.ClearColor(backgroundColour[0], backgroundColour[1], backgroundColour[2], backgroundColour[3]);

        //If you're drawing a scene that covers the whole screen each frame (for example when using skyBox), only clear depth buffer but not the color buffer.
        //The buffers should always be cleared. On much older hardware, there was a technique to get away without clearing the scene, but on even semi-recent hardware, this will actually make things slower. So always do the clear.
        //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
        //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        GL.Clear(ClearBufferMask.DepthBufferBit);

        // Updating camera parameters basing on the used inputDevice. Also passing frameDelta here for fps dependent behavior
        cam.updateCameraParams(mouse, frameDelta, this);

        // Looking in the right direction
        ShadersCommonProperties.viewMatrix = Matrix4.LookAt(cam.Eye, cam.Target, cam.Up);

        // Drawing SkyBox - it must be drawn first due to texture blending with nodes' textures.
        skyBox.DrawSkyBox(frameDelta); //Passing frameDelta for fps dependent animations

        // apply camera transform
        cam.Transform(); //model matrix is modified

        // render the nodes, just render a slice for now, it renders in order 
        foreach (SliceManager node in sceneList)
        {
            node.Render();
            this.culledThisFrame = node.culledTotal;
        }
    }

    private void UpdateScene()
    {

        // move the camera!
        if (inTransition)
        {
            UpdateCamPosition();
        }

        // apply scale animation to fileslice
        DoScaleTransition();

        // rotate active box
        if (slices.ActiveSlice.NumFiles > 0)
        {
            slices.ActiveSlice.GetActiveNode().RotY = activeRotateValue;
        }

        activeRotateValue += 150.0f * frameDelta;

        if (activeRotateValue > 0.0f)
        {
            activeRotateValue -= 360.0f;
        }
    }


    /// <summary>
    /// Occurs whenever a keybord key is pressed.
    /// </summary>
    protected override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
    {

        //System.Diagnostics.Debug.WriteLine("Key Pressed - " + args.Event.KeyValue);
        if (inVerticalTransition)
        {
            return;
        }
        if (e.Key == OpenTK.Input.Key.Tab)
        {
            //entry4.GrabFocus();
            //entry4.HasFocus = true;
            //args.RetVal = true;
            return;
        }
        switch (e.Key)
        {
            case OpenTK.Input.Key.Up: MoveForward(); break;
            case OpenTK.Input.Key.Down: MoveBackward(); break;
            case OpenTK.Input.Key.Left: MoveLeft(); break;
            case OpenTK.Input.Key.Right: MoveRight(); break;
            case OpenTK.Input.Key.Enter: NodeActivated(); break;
            case OpenTK.Input.Key.BackSpace: ToParent(true); break;
            case OpenTK.Input.Key.PageDown: ToParent(false); break;
            case OpenTK.Input.Key.PageUp: NavUp(); break;
            case OpenTK.Input.Key.Home: slices.ActiveSlice.ReFormat(FileSlice.BY_TYPE); doScaleIn = true; break;
            case OpenTK.Input.Key.End: slices.ActiveSlice.ReFormat(FileSlice.BY_NAME); doScaleIn = true; break;
            case OpenTK.Input.Key.Escape: Exit(); break;
            case OpenTK.Input.Key.Space: ToggleSelected(); break;
            default:
                /*
                if(e.Key == Gdk.Key.f && (args.Event.State & Gdk.ModifierType.ControlMask) != 0) {
                    //findEntry.Visible = true;
                    //findEntry.HasFocus = true;
                } else
                */
                if (((IList<String>)ALPHABET).Contains(Enum.GetName(typeof(OpenTK.Input.Key), e.Key)/*args.Event.KeyValue*/.ToLower()))
                {
                    slices.ActiveSlice.GoToLetter(e.Key.ToString());
                    ChangedActive();
                    ActivateTransition();
                }
                break;
        }
        return;
    }

    /*
    protected virtual void OnTextEntered (object o, System.EventArgs args ) {
        //String path = entry4.Text;
    
        if(path.EndsWith("/") && path.Length > 1) { 
            path = path.Remove(path.Length-1); 
        }
        if (System.IO.Directory.Exists(path)) {
            NewSlice(path);
        } else {
            //statusbar6.Pop(0);
            //statusbar6.Push(0, " Invalid path - " + path);
        }  
    }
    */

    /*
    protected virtual void OnSearchTextChanged (object sender, System.EventArgs e)
    {
        if(slices.ActiveSlice.GoToPattern(findEntry.Text)) {
            findEntry.ModifyBase(StateType.Normal, new Gdk.Color(255, 255, 255));
            searchFound = true;
            ChangedActive();
            ActivateTransition();
        } else {
            // not found
            searchFound = false;
            findEntry.ModifyBase(StateType.Normal, new Gdk.Color(255, 30, 30));
                            
        }
    }
    */

    /*
    protected virtual void OnSearchActivated (object o, System.EventArgs args ) {
        glwidget1.HasFocus = true;
        if(searchFound) {
            NodeActivated();   
        }
    }
    */

    /*
    protected virtual void OnFindKeyPress (object o, Gtk.KeyPressEventArgs args)
    {
        if(args.Event.Key == Gdk.Key.Escape) {
            glwidget1.HasFocus = true;
        }
    }
    */

    /*
    protected virtual void OnPathChanged (object sender, System.EventArgs e)
    {
        String[] foo = completer.GetCompletions(entry4.Text);
        if(foo.Length > 0) {
            store.Clear();
            foreach(String item in foo) {
                store.AppendValues(item);
            }
        }
    }
    */

    /*
    protected virtual void OnAboutActivated (object sender, System.EventArgs e)
    {
        about = new AboutDialog();
        about.Version = "0.6";
        about.Copyright = "Copyright 2011 - Seb Clarke";
        about.License = "GPLv3 goes here!";
        about.Comments = "Special thanks to my 3d guru, Iain C, of godlike fame";
      
        about.Run();
        about.Hide();
    }
    */

    protected virtual void OnNameSortActivated(object sender, System.EventArgs e)
    {
        slices.ActiveSlice.ReFormat(FileSlice.BY_NAME);
        doScaleIn = true;
    }

    protected virtual void OnTypeSortActivated(object sender, System.EventArgs e)
    {
        slices.ActiveSlice.ReFormat(FileSlice.BY_TYPE);
        doScaleIn = true;
    }

    protected virtual void OnDateSortActivated(object sender, System.EventArgs e)
    {
        slices.ActiveSlice.ReFormat(FileSlice.BY_DATE);
        doScaleIn = true;
    }

    /*
    protected virtual void OnVsyncToggle (object sender, System.EventArgs e)
    {
        vsync = VSyncEnabledAction.Active;
        if (vsync) {
            OpenTK.Graphics.GraphicsContext.CurrentContext.SwapInterval = 1; //vsync enabled
        } else {
            OpenTK.Graphics.GraphicsContext.CurrentContext.SwapInterval = 0; //vsync disabled
        }
        System.Diagnostics.Debug.WriteLine("Changed vsync to " + vsync);
    }
    */

    // system event handlers

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);

        InitOrUpdateProjectionMatrix();
    }

    //Called when the NativeWindow is about to close.
    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        base.OnClosing(e);

        GL.BindTexture(TextureTarget.Texture2D, 0);
        DirectoryNode.DestroyDirectoryTextures();
        slices.DestroyAllSlices();
        GL.Finish();
        OpenTK.Graphics.GraphicsContext current = (OpenTK.Graphics.GraphicsContext)OpenTK.Graphics.GraphicsContext.CurrentContext;
        current.MakeCurrent(null);
        current.Dispose();
    }

    /*
    protected virtual void OnFindLoseFocus (object o, Gtk.FocusOutEventArgs args) {
        findEntry.Visible = false;    
    }
    */

    // system convenience functions
    private void UpdateCamPosition()
    {
        // check for camera in position
        if (camDelay > 0.0f)
        {
            camDelay -= frameDelta;
            return;
        }
        else
        {
            camDelay = -1.0f;
        }

        camTransitionVector = (camTransitionTarget - camTransitionStart) * frameDelta * 4.0f;
        targetDistance = (camTransitionTarget - cam.Position).Length;
        nextTargetDistance = (camTransitionTarget - (camTransitionVector + cam.Position)).Length;
        if (((nextTargetDistance > targetDistance) && !transitionTargetUpdated) || targetDistance < 0.1f)
        {
            inTransition = false;
            if (inVerticalTransition)
            {
                slices.ActiveSlice.ShowLabels();
            }
            if (scaleIn)
            {
                doScaleIn = true;
            }
            inVerticalTransition = false;
            cam.Put(camTransitionTarget);
            if (heightCueEnabled)
            {
                SetColourForCamHeight();
            }
            //UpdateDetailsBox();

        }
        else
        {
            cam.Move(camTransitionVector);
            if (heightCueEnabled)
            {
                SetColourForCamHeight();
            }
            transitionTargetUpdated = false;
        }

        if (inVerticalTransition && inTransition)
        {
            float alphaAdjust = frameDelta * 1.775f;
            if (fadeOut)
            {
                sliceToFade.Alpha -= alphaAdjust;
            }
            else
            {
                sliceToFade.Alpha += alphaAdjust;
            }
        }

    }


    private void DoScaleTransition()
    {
        if (doScaleIn)
        {

            if (slices.ActiveSlice.Scale > 2.0f)
            {
                slices.ActiveSlice.IsScaled = false;
                slices.ActiveSlice.Scale = 0.0f;
                scaleIn = false;
                doScaleIn = false;
            }
            else
            {
                slices.ActiveSlice.Scale += frameDelta * 6f;
            }
        }

    }

    private void ActivateTransition()
    {
        camTransitionTarget = GetCamSelectedPosition();
        camTransitionStart = cam.Position;
        if (!inTransition)
        {
            camDelay = 0.08f;
        }
        inTransition = true;
    }

    private Vector3 GetCamSelectedPosition()
    {
        return slices.ActiveSlice.GetActiveNode().Position + slices.ActiveSlice.Position - CAM_OFFSET;
    }

    private void ChangedActive()
    {
        slices.ActiveSlice.ResetVisible();

        /*
        //Uncomment this if you want all the Directories to fade when changing active node to file.

        if(slices.ActiveSlice.GetActiveNode().IsDirectory) {
            if(!viewingDir) {
                viewingDir = true;
                slices.ActiveSlice.FadeDirectories(false);
            }
            slices.ActiveSlice.ResetDirFade();
          
        } else {
            if(viewingDir) {
                viewingDir = false;
                slices.ActiveSlice.FadeDirectories(true);
                System.Diagnostics.Debug.WriteLine("Fading");
            }
        }
        */

        transitionTargetUpdated = true;

        //statusbar6.Pop(0);
        //statusbar6.Push(0, " \"" + slices.ActiveSlice.GetActiveNode().FileName + "\" selected");
    }

    public void InitOrUpdateProjectionMatrix()
    {
        GL.Viewport(0, 0, Width, Height);
        ShadersCommonProperties.projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(cam.FieldOfView, Width / (float)Height, 0.01f, 500f);
    }

    //TODO Delete usages of SetColourForCamHeight()
    private void SetColourForCamHeight()
    {
        if (heightCueEnabled)
        {
            float camDiff = cam.Position.Y - camStartPosition.Y;
            if (camDiff > 30)
            {
                backgroundColour[0] = HIGH_COLOUR[0];
                backgroundColour[1] = HIGH_COLOUR[1];
                backgroundColour[2] = HIGH_COLOUR[2];
            }
            else if (camDiff < -30)
            {
                backgroundColour[0] = LOW_COLOUR[0];
                backgroundColour[1] = LOW_COLOUR[1];
                backgroundColour[2] = LOW_COLOUR[2];
            }
            else
            {
                float camCoeff = camDiff / 30.0f;
                if (cam.Position.Y < camStartPosition.Y)
                {
                    // interpolate down
                    backgroundColour[0] = BACKGROUND[0] + (rDiffDown * camCoeff);
                    backgroundColour[1] = BACKGROUND[1] + (gDiffDown * camCoeff);
                    backgroundColour[2] = BACKGROUND[2] + (bDiffDown * camCoeff);

                }
                else if (cam.Position.Y > camStartPosition.Y)
                {
                    //interpolate up
                    backgroundColour[0] = BACKGROUND[0] - (rDiffUp * camCoeff);
                    backgroundColour[1] = BACKGROUND[1] - (gDiffUp * camCoeff);
                    backgroundColour[2] = BACKGROUND[2] - (bDiffUp * camCoeff);

                }
                else
                {
                    backgroundColour[0] = BACKGROUND[0];
                    backgroundColour[1] = BACKGROUND[1];
                    backgroundColour[2] = BACKGROUND[2];
                }
            }
        }
        else
        {
            backgroundColour[0] = BACKGROUND[0];
            backgroundColour[1] = BACKGROUND[1];
            backgroundColour[2] = BACKGROUND[2];
        }
    }


    // user convenience functions

    private void MoveForward()
    {
        if (slices.ActiveSlice.MoveCarat(0, 1))
        {
            ActivateTransition();
            ChangedActive();
        }
    }

    private void MoveBackward()
    {
        if (slices.ActiveSlice.MoveCarat(0, -1))
        {
            ActivateTransition();
            ChangedActive();

        }
    }

    private void MoveLeft()
    {
        if (slices.ActiveSlice.MoveCarat(-1, 0))
        {
            ActivateTransition();
            ChangedActive();
        }
    }

    private void MoveRight()
    {
        if (slices.ActiveSlice.MoveCarat(1, 0))
        {
            ActivateTransition();
            ChangedActive();
        }
    }

    private void NavUp()
    {
        if (!slices.ActiveSlice.IsAnyNodeCurrentlyActivated) {
            sliceToFade = slices.ActiveSlice;
            slices.MoveUp();
            inVerticalTransition = true;
            fadeOut = true;
            ActivateTransition();
            ChangedActive();
            //statusbar6.Pop(0);
            //statusbar6.Push(0, " " + slices.ActiveSlice.NumFiles + " items");
            //entry4.Text = slices.ActiveSlice.Path;
        }
    }

    private void NewSlice(String path)
    {
        // sanity checks first
        if (Directory.EnumerateFiles(path).Count() + Directory.EnumerateDirectories(path).Count() == 0)
        {
            //glwidget1.HasFocus = true;
            //statusbar6.Pop(0);
            //statusbar6.Push(0, " Not Viewing Empty Folder - " + path);
            return;
        }

        if (path == slices.ActivePath)
        {
            //glwidget1.HasFocus = true;
            //statusbar6.Pop(0);
            //statusbar6.Push(0, " Not re-rendering - " + path);
            return;
        }

        // stop any active cam transitions
        inTransition = false;

        try
        {
            slices.Reset(path);
        }
        catch
        {
            slices.Reset(START_PATH);
        }

        cam.Put(camStartPosition);
        if (heightCueEnabled)
        {
            SetColourForCamHeight();
        }
        doScaleIn = true;
        //glwidget1.HasFocus = true;
        //statusbar6.Pop(0);
        //statusbar6.Push(0, " " + slices.ActiveSlice.NumFiles + " items");
    }


    // PInvoke Import
    [DllImport("libgdk-win32-2.0-0.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr gdk_win32_drawable_get_handle(IntPtr window_handle);


    private async void NodeActivated()
    {
        Trace.WriteLine("NodeActivated");

        Node activeNode = slices.ActiveSlice.GetActiveNode();

		if (!activeNode.NodeActivated && !slices.ActiveSlice.IsAnyNodeCurrentlyActivated) { //Checking if node is already activated
			if (activeNode.IsDirectory) {
                //checking if we already have this slice generated
                if (slices.ActiveSlice.LastActivatedNode == activeNode && activeNode.ChildSlice != null && activeNode.ChildSlice.NumFiles != 0) {
                    System.Diagnostics.Debug.WriteLine("Slice for Directory: " + activeNode.File + " already generated - showing...");
                    sliceToFade = slices.ActiveSlice;
                    slices.MoveUp();
                    inVerticalTransition = true;
                    fadeOut = true;
                    scaleIn = true;
                    slices.ActiveSlice.ReFormat(FileSlice.BY_NAME);
                    slices.ActiveSlice.GoToNode(0); //Activate first node on that slice, not the one that was activated previously (to differentiate this operation from NavUp()
                    ChangedActive();
                    ActivateTransition();
                    return;
                } else if (slices.ActiveSlice.LastActivatedNode != activeNode && activeNode.ChildSlice == null) {
                    System.Diagnostics.Debug.WriteLine("Generating slice for Directory: " + activeNode.File);
                    activeNode.NodeActivated = true;
                    slices.ActiveSlice.IsAnyNodeCurrentlyActivated = true;
                    await Task.Run(() => slices.AddChildSliceToFileNode(activeNode)); //Run asynchronously and await for the result (during awaiting do other stuff - like drawing)
                }

				if (activeNode.ChildSlice.NumFiles == 0) {
                    slices.ActiveSlice.LastActivatedNode = activeNode;
                    //glwidget1.HasFocus = true;
                    //statusbar6.Pop(0);
                    //statusbar6.Push(0, " Not Viewing Empty Folder - " + activeNode.File);
                    return;
				}
                System.Diagnostics.Debug.WriteLine("Showing the slice for Directory: " + activeNode.File);
                slices.ActiveSlice.LastActivatedNode = activeNode;
                activeNode.NodeActivated = false;
                slices.ActiveSlice.IsAnyNodeCurrentlyActivated = false;
                inVerticalTransition = true;
				fadeOut = true;
				scaleIn = true;
				sliceToFade = slices.ActiveSlice;
				await MainThreadDispatcher.InvokeAsync (() => slices.AddSliceAbove (activeNode.ChildSlice)); //dispatches work to the UI thread (this is needed due to OpenGL calls in this method (Texture Generation)!). MainThreadDispatcher is set at MainWindow() constructor. Look here for the explanation: http://www.opentk.com/node/3841


				//http://www.opentk.com/doc/graphics/graphicscontext

				// The new context must be created on a new thread
				// (see remarks section, below)
				// We need to create a new window for the new context.
				// Note 1: new windows remain invisible unless you call
				//         INativeWindow.Visible = true or IGameWindow.Run()
				// Note 2: invisible windows fail the pixel ownership test.
				//         This means that the contents of the front buffer are undefined, i.e.
				//         you cannot use an invisible window for offscreen rendering.
				//         You will need to use a framebuffer object, instead.
				// Note 3: context sharing will fail if the main context is in use.
				// Note 4: this code can be used as-is in a GLControl or GameWindow.

				/*
            IWindowInfo m_window_info;
            IGraphicsContext m_graphics_context;

            IGraphicsContext currentContext = GraphicsContext.CurrentContext;

            // Init
            IntPtr window_handle = gdk_win32_drawable_get_handle( GdkWindow.Handle );
            m_window_info = Utilities.CreateWindowsWindowInfo( window_handle );

            m_graphics_context = new GraphicsContext( GraphicsMode.Default, m_window_info );
            m_graphics_context.MakeCurrent( m_window_info );
            ((IGraphicsContextInternal)m_graphics_context).LoadAll();

            MakeCurrent ();




            EventWaitHandle context_ready = new EventWaitHandle(false, EventResetMode.AutoReset);

            IntPtr windowHandle = gdk_win32_drawable_get_handle(GdkWindow.Handle);
            IWindowInfo windowInfo = OpenTK.Platform.Utilities.CreateWindowsWindowInfo(windowHandle);

            GraphicsContext.CurrentContext.MakeCurrent(null);

            System.Threading.Thread thread = new System.Threading.Thread(() =>
                {
                    INativeWindow window = new NativeWindow();
                    IGraphicsContext context = new GraphicsContext(GraphicsMode.Default, window.WindowInfo);
                    context.MakeCurrent(window.WindowInfo);

                    while (window.Exists)
                    {
                        window.ProcessEvents();

                        // Perform your processing here
                        slices.AddSliceAbove(activeNode.ChildSlice);

                        System.Threading.Thread.Sleep(1); // Limit CPU usage, if necessary
                    }
                });
            thread.IsBackground = true;
            thread.Start();

            context_ready.WaitOne();
            //GraphicsContext.CurrentContext.MakeCurrent(windowInfo);
            m_graphics_context.MakeCurrent( m_window_info );
            ((IGraphicsContextInternal)m_graphics_context).LoadAll();
            */


				slices.ActiveSlice.ReFormat (FileSlice.BY_NAME);
				ChangedActive ();
				ActivateTransition ();
				//statusbar6.Pop(0);
				//statusbar6.Push(0, " " + slices.ActiveSlice.NumFiles + " items");
				//entry4.Text = slices.ActiveSlice.Path;
			} else {
				// launch the file!            
				System.Diagnostics.Debug.WriteLine ("Launching " + activeNode.File);
				// TODO: Launch using GIO
				System.Diagnostics.Process.Start (activeNode.File);
				//statusbar6.Pop(0);
				//statusbar6.Push(0, " Launched " + activeNode.File);   
			}
		}
    }

    private void ToParent(bool clearAbove)
    {
        if (!slices.ActiveSlice.IsAnyNodeCurrentlyActivated) {
            inTransition = false;

            sliceToFade = slices.ActiveSlice;

            if (clearAbove)
            {
                slices.MoveDownClear();
            }
            else
            {
                slices.MoveDown();
            }

            //glwidget1.HasFocus = true;
            fadeOut = true;
            sliceToFade.HideAllNodes();
            inVerticalTransition = true;
            ChangedActive();
            ActivateTransition();
            //statusbar6.Pop(0);
            //statusbar6.Push(0, " " + slices.ActiveSlice.NumFiles + " items");
            //entry4.Text = slices.ActiveSlice.Path;
        }
    }

    protected virtual void OnHeightColourToggle(object sender, System.EventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("Height Colour Toggled");
        heightCueEnabled = !heightCueEnabled;
        SetColourForCamHeight();
    }

    // external convenience functions
    public Camera GetCamera()
    {
        return cam;
    }

    /*
    protected virtual void OnWidgetClick (object o, Gtk.ButtonReleaseEventArgs args)
    {  
        System.Diagnostics.Debug.WriteLine("Widget clicked");
        glwidget1.GrabFocus();
        glwidget1.HasFocus = true;
    }
    */

    static long GetDirectorySize(string directoryFullName)
    {
        long totalDirectorySize = 0;
        /*
        // 1.
        // Get array of all file names.
        string[] listOfFiles = Directory.GetFiles(directoryFullName, "*.*");

        // 2.
        // Calculate total bytes of all files in a loop.
        long totalDirectorySize = 0;
        foreach (string file in listOfFiles)
        {
            // 3.
            // Use FileInfo to get length of each file.
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(file);
            totalDirectorySize += fileInfo.Length;
        }

        // 4.
        // Return total size
        */

        return totalDirectorySize;
    }

    /*
    private void UpdateDetailsBox() {
        if(detailsBox.Visible) {
            Node active = slices.ActiveSlice.GetActiveNode();
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(active.File);

            if(active is FileNode) {
                detailLabelContents.Visible = false;
                detailValueContents.Visible = false;
            
                long fileSize = fileInfo.Length;
                detailValueSize.Text = String.Format(new FileSizeFormatProvider(), "{0:fs}", fileSize);
            
                detailValueType.Text = ((FileNode)active).Description + " (" + ((FileNode)active).FileExtension + " file)";

                detailValueSpace.Text = "dupa"; //String.Format(new FileSizeFormatProvider(), "{0:fs}", Convert.ToUInt64(info.GetAttributeString("filesystem:free")));
            } else { //This is for directories
                detailLabelContents.Visible = true;
                detailValueContents.Visible = true;

                long directorySize = GetDirectorySize(fileInfo.FullName); //to be corrected !
                detailValueSize.Text = String.Format(new FileSizeFormatProvider(), "{0:fs}", directorySize); //"[unsupported]";
            
                detailValueType.Text = "Directory";  
            
                detailValueContents.Text = ((DirectoryNode)active).NumChildren + " items (" + ((DirectoryNode)active).NumDirs + " folders)";
            }

            detailEntryName.Text = active.FileName;
            detailValueAccessed.Text = fileInfo.LastAccessTime.ToString("ddd, dd MMM yyyy HH':'mm':'ss");
            detailValueModified.Text = fileInfo.LastWriteTime.ToString("ddd, dd MMM yyyy HH':'mm':'ss");
            detailValueOwner.Text = "dupa"; //info.GetAttributeString("owner::user");
            detailValueGroup.Text = "dupa"; //info.GetAttributeString("owner::group");


            //DUPA:
            Mono.Unix.Native.Statvfs fsbuf = new Mono.Unix.Native.Statvfs();
            Mono.Unix.Native.Syscall.statvfs(slices.ActiveSlice.GetActiveNode().File, out fsbuf);

            detailValueSpace.Text = String.Format(new FileSizeFormatProvider(), "{0:fs}", fsbuf.f_bavail * fsbuf.f_bsize);
            UpdateDetailPermissions();

            if(detailValueOwner.Text == Mono.Unix.UnixUserInfo.GetRealUser().UserName) {
                EnableDetailPermissions(true);
            } else {
                EnableDetailPermissions(false);
            }
        }   
    }
    */

    private void UpdateDetailPermissions()
    {

        //DUPA
        /*
        Stat buf = new Mono.Unix.Native.Stat() ;
    
        Syscall.stat(slices.ActiveSlice.GetActiveNode().File, out buf);
    
        permissionCheckOR.Active = false;
        permissionCheckGR.Active = false;
        permissionCheckUR.Active = false;
        permissionCheckOW.Active = false;
        permissionCheckGW.Active = false;
        permissionCheckUW.Active = false;
        permissionCheckOX.Active = false;
        permissionCheckGX.Active = false;
        permissionCheckUX.Active = false;
    
        if((buf.st_mode & Mono.Unix.Native.FilePermissions.S_IRUSR) != 0) {
            permissionCheckOR.Active = true;
        }
        if((buf.st_mode & Mono.Unix.Native.FilePermissions.S_IRGRP) != 0) {
            permissionCheckGR.Active = true;
        }
        if((buf.st_mode & Mono.Unix.Native.FilePermissions.S_IROTH) != 0) {
            permissionCheckUR.Active = true;
        }
        if((buf.st_mode & Mono.Unix.Native.FilePermissions.S_IWUSR) != 0) {
            permissionCheckOW.Active = true;
        }
        if((buf.st_mode & Mono.Unix.Native.FilePermissions.S_IWGRP) != 0) {
            permissionCheckGW.Active = true;
        }
        if((buf.st_mode & Mono.Unix.Native.FilePermissions.S_IWOTH) != 0) {
            permissionCheckUW.Active = true;
        }
        if((buf.st_mode & Mono.Unix.Native.FilePermissions.S_IXUSR) != 0) {
            permissionCheckOX.Active = true;
        }
        if((buf.st_mode & Mono.Unix.Native.FilePermissions.S_IXGRP) != 0) {
            permissionCheckGX.Active = true;
        }
        if((buf.st_mode & Mono.Unix.Native.FilePermissions.S_IXOTH) != 0) {
            permissionCheckUX.Active = true;
        }
        */

    }

    /*
    private void EnableDetailPermissions(bool enable) {
        permissionCheckOR.Sensitive = enable;
        permissionCheckGR.Sensitive = enable;
        permissionCheckUR.Sensitive = enable;
        permissionCheckOW.Sensitive = enable;
        permissionCheckGW.Sensitive = enable;
        permissionCheckUW.Sensitive = enable;
        permissionCheckOX.Sensitive = enable;
        permissionCheckGX.Sensitive = enable;
        permissionCheckUX.Sensitive = enable;
    }
    */

    /*
    private void ApplyNewPermissions() {
        FilePermissions toSet = 0;
    
        if(permissionCheckOR.Active) {
            toSet = toSet | FilePermissions.S_IRUSR;    
        }
        if(permissionCheckGR.Active) {
            toSet = toSet | FilePermissions.S_IRGRP;
        }
        if(permissionCheckUR.Active) {
            toSet = toSet | FilePermissions.S_IROTH;
        }
        if(permissionCheckOW.Active) {
            toSet = toSet | FilePermissions.S_IWUSR;
        }
        if(permissionCheckGW.Active) {
            toSet = toSet | FilePermissions.S_IWGRP;
        }
        if(permissionCheckUW.Active) {
            toSet = toSet | FilePermissions.S_IWOTH;
        }
        if(permissionCheckOX.Active) {
            toSet = toSet | FilePermissions.S_IXUSR;
        }
        if(permissionCheckGX.Active) {
            toSet = toSet | FilePermissions.S_IXGRP;
        }
        if(permissionCheckUX.Active) {
            toSet = toSet | FilePermissions.S_IXOTH;
        }
    
        Syscall.chmod(slices.ActiveSlice.GetActiveNode().File, toSet);
    }
    */

    /*
    protected virtual void OnDetailsVisibleToggled (object sender, System.EventArgs e)
    {
        detailsBox.Visible = !detailsBox.Visible;
        UpdateDetailsBox();
    }
    */

    /*
    protected virtual void OnPermissionsToggle (object sender, System.EventArgs e)
    {
        ApplyNewPermissions();
    }
    */

    private void RenameFile(String newFileName)
    {
        // rename the file
        Node active = slices.ActiveSlice.GetActiveNode();

        try
        {
            System.IO.File.Move(active.File, active.File.Replace(active.FileName, newFileName));
        }
        catch
        {
            //statusbar6.Pop(0);
            //statusbar6.Push(0, " Failed to rename file");
            return;
        }
        slices.ActiveSlice.RenameActiveNode(newFileName);

    }

    private void ToggleSelected()
    {
        if (slices.ActiveSlice.ToggleSelected())
        {
            selectedNodes.AddLast(slices.ActiveSlice.GetActiveNode());
        }
        else
        {
            selectedNodes.Remove(slices.ActiveSlice.GetActiveNode());
        }

    }

    /*
    protected virtual void OnNewNameEntered (object sender, System.EventArgs e)
    {
        RenameFile(detailEntryName.Text);
    }
    */
}
