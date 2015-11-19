using OpenTK.Graphics.OpenGL4;
using OculusWrap;
using SwapTextureSet = OculusWrap.GL.SwapTextureSet;
using System;
using System.Diagnostics;

namespace glomp
{
    public class OvrSharedRendertarget
    {
        private OculusWrap.GL.SwapTextureSet textureSet;
        private int fboId;      
        private int width, height;

        public OvrSharedRendertarget(int w, int h, Hmd hmd)
        {
            width = w;
            height = h;

            hmd.CreateSwapTextureSetGL((uint)All.Srgb8Alpha8, width, height, out textureSet);

            for (int i = 0; i < textureSet.TextureCount; i++)
            {
                GL.BindTexture(TextureTarget.Texture2D, textureSet.Textures[i].TexId);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);
            }
            
            GL.GenFramebuffers(1, out fboId);
        }

        #region Properties
        public int FboId
        {
            get { return fboId; }
        }

        public int Height
        {
            get { return height; }
        }

        public int Width
        {
            get { return width; }
        }
        public SwapTextureSet TextureSet
        {
            get { return textureSet; }
        }
        #endregion

        public void Bind(uint depth)
        {
            OVR.GL.GLTextureData tex = textureSet.Textures[textureSet.CurrentIndex];
            
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fboId);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, tex.TexId, 0);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, depth, 0);
        }

        public void UnBind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fboId);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, 0, 0);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, 0, 0);
        }

        public void CleanUp()
        {
            textureSet.Dispose();

            if (fboId != 0)
                GL.DeleteFramebuffers(1, ref fboId);
        }
    }
}
