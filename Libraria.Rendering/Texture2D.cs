﻿using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using IPixelFormat = System.Drawing.Imaging.PixelFormat;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace Libraria.Rendering {
	public enum TexParam {
		WrapS = TextureParameterName.TextureWrapS,
		WrapT = TextureParameterName.TextureWrapT,
		WrapR = TextureParameterName.TextureWrapR,
		WrapX = WrapS, WrapY = WrapT, WrapZ = WrapR,
		MinFilter = TextureParameterName.TextureMinFilter,
		MagFilter = TextureParameterName.TextureMagFilter
	}

	public enum TexWrapMode {
		Clamp = 10496,
		Repeat = 10497,
		ClampToBorder = 33069,
		ClampToEdge = 33071,
		MirroredRepeat = 33648
	}

	public enum TexFilterMode {
		Nearest = All.Nearest,
		Linear = All.Linear,
		NearestMipmapNearest = All.NearestMipmapNearest,
		LinearMipmapNearest = All.LinearMipmapNearest,
		LinearMipmapLinear = All.LinearMipmapLinear,
		NearestMipmapLinear = All.NearestMipmapLinear
	}

	public class Texture2D {
		public static Texture2D Mask_Tex0, Mask_Tex1, Mask_Tex2, Mask_Tex3;

		static Texture2D() {
			Mask_Tex0 = FromBitmap(CreateBitmap(Color.FromArgb(255, 0, 0, 0)));
			Mask_Tex1 = FromBitmap(CreateBitmap(Color.FromArgb(255, 255, 0, 0)));
			Mask_Tex2 = FromBitmap(CreateBitmap(Color.FromArgb(255, 0, 255, 0)));
			Mask_Tex3 = FromBitmap(CreateBitmap(Color.FromArgb(255, 0, 0, 255)));
		}

		static Bitmap CreateBitmap(Color Clr) {
			Bitmap Bmp = new Bitmap(1, 1);
			using (Graphics Gfx = Graphics.FromImage(Bmp))
				Gfx.FillRegion(new SolidBrush(Clr), new Region(new Rectangle(0, 0, 1, 1)));
			return Bmp;
		}

		public static Texture2D FromFile(string Pth, TexFilterMode FilterMode = TexFilterMode.Linear,
			TexWrapMode WrapMode = TexWrapMode.ClampToEdge, bool GenerateMipmap = true) {

			return FromBitmap(new Bitmap(Pth), FilterMode, WrapMode, GenerateMipmap);
		}

		public static Texture2D FromBitmap(Bitmap BMap, TexFilterMode FilterMode = TexFilterMode.Linear,
			TexWrapMode WrapMode = TexWrapMode.ClampToEdge, bool GenerateMipmap = true) {

			Texture2D Tex = new Texture2D();
			Tex.Bind();

			Tex.SetParam(TexParam.WrapS, WrapMode);
			Tex.SetParam(TexParam.WrapT, WrapMode);
			Tex.SetParam(TexParam.MinFilter, FilterMode);
			Tex.SetParam(TexParam.MagFilter, FilterMode);

			Tex.LoadDataFromBitmap(BMap);

			if (GenerateMipmap)
				Tex.GenerateMipmap();

			return Tex;
		}

		public int ID;
		public int TexUnit;

		public Texture2D() {
			ID = GL.GenTexture();
			Bind();
		}

		public void Bind(int TexUnit = 0) {
			if (TexUnit < 0 || TexUnit > 31)
				throw new Exception("Invalid texture unit " + TexUnit);

			this.TexUnit = TexUnit;

			GL.ActiveTexture(TextureUnit.Texture0 + TexUnit);
			GL.BindTexture(TextureTarget.Texture2D, ID);
		}

		public void GenerateMipmap() {
			GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
		}

		public void LoadDataFromFile(string Pth) {
			LoadDataFromBitmap(new Bitmap(Pth));
		}

		public void LoadDataFromBitmap(Bitmap BMap) {
			BitmapData BDta = BMap.LockBits(new Rectangle(0, 0, BMap.Width, BMap.Height),
							ImageLockMode.ReadOnly, IPixelFormat.Format32bppArgb);

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, BMap.Width, BMap.Height, 0,
				PixelFormat.Bgra, PixelType.UnsignedByte, BDta.Scan0);

			BMap.UnlockBits(BDta);
		}

		public void SetParam(TexParam Param, int Val) {
			GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)Param, Val);
		}

		public void SetParam(TexParam Param, TexWrapMode WrapMode) {
			SetParam(Param, (int)WrapMode);
		}

		public void SetParam(TexParam Param, TexFilterMode WrapMode) {
			SetParam(Param, (int)WrapMode);
		}
	}
}