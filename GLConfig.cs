#region --- License ---
/* Copyright (c) Nikita Tereshin
 * https://github.com/rumkex/Calcifer/blob/master/Utilities/GLConfig.cs
 */
#endregion

using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace glomp
{
    public static class GLConfig
    {
        private static readonly HashSet<string> exts; 

        static GLConfig()
        {
            var v = GL.GetString(StringName.Version).Split(new[]{' '}, 2)[0].Split('.');
            Version = new Version(v.Length < 1 ? 0 : int.Parse(v[0]), v.Length < 2 ? 0 : int.Parse(v[1]), v.Length < 3 ? 0 : int.Parse(v[2]));
            if (Version.Major > 2)
            {
                v = GL.GetString(StringName.ShadingLanguageVersion).Split(new[] { ' ' }, 2)[0].Split('.');
                GLSLVersion = new Version(v.Length < 1 ? 0 : int.Parse(v[0]), v.Length < 2 ? 0 : int.Parse(v[1]));
            }
            else GLSLVersion = new Version();
            if (Version.Major < 3)
                // pre-3.0 extension enumeration
                exts = new HashSet<string>(GL.GetString(StringName.Extensions).Split(' '));
            else
            {
                int num;
                GL.GetInteger(GetPName.NumExtensions, out num);
                exts = new HashSet<string>();
                for (var i = 0; i < num; i++)
                {
                    exts.Add(GL.GetString(StringNameIndexed.Extensions, i));
                }
            }
        }

        public static Version Version { get; private set; }
        public static Version GLSLVersion { get; private set; }
        public static bool HasExtension (string ext)
        {
            return exts.Contains(ext);
        }

    }

    public static class Extensions
    {
        public const string Framebuffer = "GL_EXT_framebuffer_object";
        public const string TransformFeedback = "GL_EXT_transform_feedback";
        public const string TextureFilterAnisotropic = "GL_EXT_texture_filter_anisotropic";
    }

}
