﻿// OpenTween - Client of Twitter
// Copyright (c) 2015 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
// All rights reserved.
//
// This file is part of OpenTween.
//
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
// for more details.
//
// You should have received a copy of the GNU General Public License along
// with this program. If not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenTween
{
    public class MemoryImageTest
    {
        [Fact]
        public async Task ImageFormat_GifTest()
        {
            using (var imgStream = File.OpenRead("Resources/re.gif"))
            using (var image = await MemoryImage.CopyFromStreamAsync(imgStream).ConfigureAwait(false))
            {
                Assert.Equal(ImageFormat.Gif, image.ImageFormat);
                Assert.Equal(".gif", image.ImageFormatExt);
            }
        }

        [Fact]
        public void ImageFormat_CopyFromImageTest()
        {
            using (var bitmap = new Bitmap(width: 200, height: 200))
            using (var image = MemoryImage.CopyFromImage(bitmap))
            {
                // CopyFromImage から作成した MemoryImage は PNG 画像として扱われる
                Assert.Equal(ImageFormat.Png, image.ImageFormat);
                Assert.Equal(".png", image.ImageFormatExt);
            }
        }

        [Fact]
        public void CopyFromImage_Test()
        {
            using (var bitmap = new Bitmap(width: 200, height: 200))
            {
                // MemoryImage をエラー無く作成できることをテストする
                using (var image = MemoryImage.CopyFromImage(bitmap)) { }
            }
        }
    }
}
