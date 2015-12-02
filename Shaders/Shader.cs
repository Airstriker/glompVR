using OpenTK.Graphics.OpenGL;

namespace glomp
{
    public abstract class Shader
    {
        /// <summary>
        /// ID of our program on the graphics card
        /// </summary>
        public int ShaderProgramID { get; set; }

        /// <summary>
        /// Address of the vertex shader
        /// </summary>
        protected int vertexShaderID;

        /// <summary>
        /// Address of the fragment shader
        /// </summary>
        protected int fragmentShaderID;

        /// <summary>
        /// Address of the Projection matrix uniform
        /// </summary>
        protected int Projection_location;

        /// <summary>
        /// Address of the View matrix uniform
        /// </summary>
        protected int View_location;

        /// <summary>
        /// Address of the Model matrix uniform
        /// </summary>
        protected int Model_location;

        public void Dispose()
        {
            GL.DeleteProgram(ShaderProgramID);
        }
	}
}

