using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace glomp
{
    public class DepthBuffer
    {
        private uint texId;

        int width, height;

        public DepthBuffer(int w, int h)
        {
            width = w;
            height = h;

            GL.GenTextures(1, out texId);
            GL.BindTexture(TextureTarget.Texture2D, texId);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);

            var extensions = GL.GetString(StringName.Extensions).Split(' ');

            PixelInternalFormat internalFormat = PixelInternalFormat.DepthComponent;
            PixelType type = PixelType.Float;

            if (extensions.Contains("GL_ARB_depth_buffer_float"))
            {
                internalFormat = PixelInternalFormat.DepthComponent32f;
                type = PixelType.Float;
            }

            GL.TexImage2D(TextureTarget.Texture2D, 0, internalFormat, width, height, 0, PixelFormat.DepthComponent, type, IntPtr.Zero);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public uint TexId
        {
            get { return texId; }
        }

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public int Width
        {
            get { return width; }
            set { width = value; }
        }



        public void CleanUp()
        {
            if ( texId != 0)
                GL.DeleteTextures(1, ref texId);
        }

    }
}
