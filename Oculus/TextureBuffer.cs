using System;
using System.Diagnostics;
using System.Windows.Forms;
using OculusWrap;
using OpenTK.Graphics.OpenGL4;
using System.Diagnostics.Contracts;

namespace glomp
{
    public class TextureBuffer : IDisposable
    {
        private TextureSwapChain textureChain = null;
        private uint texId = 0;
        private uint fboId = 0;
        private OVRTypes.Sizei texSize;
        private Wrap wrap = null;

        /// <summary>
        /// Write out any error details received from the Oculus SDK, into the debug output window.
        /// 
        /// Please note that writing text to the debug output window is a slow operation and will affect performance,
        /// if too many messages are written in a short timespan.
        /// </summary>
        /// <param name="oculus">OculusWrap object for which the error occurred.</param>
        /// <param name="result">Error code to write in the debug text.</param>
        /// <param name="message">Error message to include in the debug text.</param>
        public static void WriteErrorDetails(Wrap oculus, OVRTypes.Result result, string message)
        {
            if (result >= OVRTypes.Result.Success)
                return;

            // Retrieve the error message from the last occurring error.
            OVRTypes.ErrorInfo errorInformation = oculus.GetLastError();

            string formattedMessage = string.Format("{0}. \nMessage: {1} (Error code={2})", message, errorInformation.ErrorString, errorInformation.Result);
            Trace.WriteLine(formattedMessage);
            MessageBox.Show(formattedMessage, message);

            throw new Exception(formattedMessage);
        }

        public TextureSwapChain TextureChain
        {
            get { return textureChain; }
        }

        public TextureBuffer(Wrap oculus, Hmd hmd, bool rendertarget, bool displayableOnHmd, OVRTypes.Sizei size, int mipLevels, IntPtr data, int sampleCount)
        {
            wrap = oculus;
            texSize = size;

            OVRTypes.Result result;

            Debug.Assert(sampleCount <= 1); // The code doesn't currently handle MSAA textures.

            if (displayableOnHmd)
            {
                // This texture isn't necessarily going to be a rendertarget, but it usually is.
                Debug.Assert(hmd != null); // No HMD? A little odd.

                // It looks like that in order to disable sRGB we have to
                // set the texture format to sRGB but omit the call the
                // enabling sRGB in the framebuffer. Info here:
                // https://forums.oculus.com/community/discussion/24347/srgb-and-sdk-0-6-0-0

                OVRTypes.TextureSwapChainDesc desc = new OVRTypes.TextureSwapChainDesc();
                desc.Type = OVRTypes.TextureType.Texture2D;
                desc.ArraySize = 1;
                desc.Width = size.Width;
                desc.Height = size.Height;
                desc.MipLevels = mipLevels;
                desc.Format = OVRTypes.TextureFormat.R8G8B8A8_UNORM_SRGB; //Remember to call GL.Disable(EnableCap.FramebufferSrgb) later
                desc.SampleCount = sampleCount;
                desc.StaticImage = 0;
                if (mipLevels > 1)
                    desc.MiscFlags = OVRTypes.TextureMiscFlags.AllowGenerateMips;

                result = hmd.CreateTextureSwapChainGL(desc, out textureChain);
                WriteErrorDetails(wrap, result, "Failed to create swap chain.");

                int length = 0;
                result = TextureChain.GetLength(out length);
                WriteErrorDetails(wrap, result, "Failed to retrieve the number of buffers of the created swap chain.");

                if (result >= OVRTypes.Result.Success)
                {
                    for (int i = 0; i < length; ++i)
                    {
                        uint chainTexId;

                        // Retrieve the OpenGL texture contained in the Oculus TextureSwapChainBuffer.
                        result = TextureChain.GetBufferGL(i, out chainTexId);
                        WriteErrorDetails(wrap, result, "Failed to retrieve a texture from the created swap chain.");
                        GL.BindTexture(TextureTarget.Texture2D, chainTexId);

                        if (rendertarget)
                        {
                            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
                            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
                            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
                            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);
                        }
                        else
                        {
                            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.LinearMipmapLinear);
                            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
                            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.Repeat);
                            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.Repeat);
                        }
                    }
                }
            }
            else
            {
                GL.GenTextures(1, out texId);
                GL.BindTexture(TextureTarget.Texture2D, texId);

                if (rendertarget)
                {
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);
                }
                else
                {
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.LinearMipmapLinear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.Repeat);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.Repeat);
                }

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Srgb8Alpha8, texSize.Width, texSize.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            }

            if (mipLevels > 1)
            {
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }

            GL.GenFramebuffers(1, out fboId);
        }

        #region IDisposable Members
        /// <summary>
        /// Dispose contained fields.
        /// </summary>
        public void Dispose()
        {
            if (textureChain != null)
            {
                textureChain.Dispose();
                textureChain = null;
            }

            if (texId != 0)
            {
                GL.DeleteTextures(1, ref texId);
                texId = 0;
            }
            if (fboId != 0)
            {
                GL.DeleteFramebuffers(1, ref fboId);
                fboId = 0;
            }
        }
        #endregion

        [Pure]
        public OVRTypes.Sizei GetSize()
        {
            return texSize;
        }

        public void SetAndClearRenderSurface(DepthBuffer dbuffer)
        {
            uint curTexId;
            OVRTypes.Result result;

            if (TextureChain != null)
            {
                int curIndex;
                result = TextureChain.GetCurrentIndex(out curIndex);
                WriteErrorDetails(wrap, result, "Failed to retrieve texture swap chain current index.");
                result = TextureChain.GetBufferGL(curIndex, out curTexId);
                WriteErrorDetails(wrap, result, "Failed to retrieve a texture from the created swap chain.");
            }
            else
            {
                curTexId = texId;
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fboId);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, curTexId, 0);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, dbuffer.TexId, 0);

            GL.Viewport(0, 0, texSize.Width, texSize.Height);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            //Disbale SRGB - needed for correct colorspace!
            GL.Disable(EnableCap.FramebufferSrgb);
        }

        public void UnsetRenderSurface()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fboId);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, 0, 0);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, 0, 0);
        }

        public void Commit()
        {
            if (TextureChain != null)
            {
                OVRTypes.Result result = TextureChain.Commit();
                WriteErrorDetails(wrap, result, "Failed to commit the swap chain texture.");
            }
        }        
    }
}
