
using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;


namespace glomp
{
    public static class NodeManager {

		//VBO related stuff
		public static VBOUtil.Vbo[] vbo = new VBOUtil.Vbo[3];
		public static int[] vao = new int[3]; //vertex array objects referencing a different set of vertex attributes, which can be stored in the same vertex buffer object or split across several vertex buffer objects (like in this case).
		public static VBOUtil.Vbo labelVbo; //VBO for file node's labels
		public static int labelVao; //VOA for file node's labels


		public static void LoadVBOs() {
			//Using VBO concept instead of DisplayList
			// loading Vertex Buffers
			Shape fileNodeShape = new FileNodeShape();
			Shape dirNodeShape = new DirNodeShape();
			Shape driveNodeShape = new DriveNodeShape();
			vbo[(int)Node.NodeType.FILE_NODE] = VBOUtil.LoadVBO(fileNodeShape);
			VBOUtil.ConfigureVertexArrayObject(out vao [(int)Node.NodeType.FILE_NODE], vbo [(int)Node.NodeType.FILE_NODE]);
			vbo[(int)Node.NodeType.DIR_NODE] = VBOUtil.LoadVBO(dirNodeShape);
			VBOUtil.ConfigureVertexArrayObject(out vao [(int)Node.NodeType.DIR_NODE], vbo [(int)Node.NodeType.DIR_NODE]);
			vbo[(int)Node.NodeType.DRIVE_NODE] = VBOUtil.LoadVBO(driveNodeShape);
			VBOUtil.ConfigureVertexArrayObject(out vao [(int)Node.NodeType.DRIVE_NODE], vbo [(int)Node.NodeType.DRIVE_NODE]);

			Shape labelShape = new LabelShape();
			labelVbo = VBOUtil.LoadVBO(labelShape);
			VBOUtil.ConfigureVertexArrayObjectForLabels(out labelVao, labelVbo);
		}

    }
}
