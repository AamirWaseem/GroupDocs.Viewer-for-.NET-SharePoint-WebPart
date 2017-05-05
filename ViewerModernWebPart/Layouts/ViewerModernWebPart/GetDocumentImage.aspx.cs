﻿using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System;
using System.Collections.Generic;
using System.Linq;
using GroupDocs.Viewer.Converter.Options;
using GroupDocs.Viewer.Domain.Image;
using GroupDocs.Viewer.Handler;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using ViewerModernWebPart.Helpers;

namespace ViewerModernWebPart.Layouts.ViewerModernWebPart
{
    public partial class GetDocumentImage : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var file = GetValueFromQueryString("file");
            int page = Convert.ToInt32(GetValueFromQueryString("page"));
            int? width = Convert.ToInt32(GetValueFromQueryString("width"));
            int? height = Convert.ToInt32(GetValueFromQueryString("width"));

            ViewerImageHandler handler = Utils.CreateViewerImageHandler();
            ImageOptions o = new ImageOptions();
            List<int> pageNumberstoRender = new List<int>();
            pageNumberstoRender.Add(page);
            o.PageNumbersToRender = pageNumberstoRender;
            o.PageNumber = page;
            o.CountPagesToRender = 1;
            if (width.HasValue)
            {
                o.Width = Convert.ToInt32(width);
            }
            if (height.HasValue)
            {
                o.Height = Convert.ToInt32(height);
            }
            Stream stream = null;
            List<PageImage> list = Utils.LoadPageImageList(handler, file, o);
            foreach (PageImage pageImage in list.Where(x => x.PageNumber == page)) { stream = pageImage.Stream; };
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
            MemoryStream memoryStream = new MemoryStream();
            image.Save(memoryStream, ImageFormat.Jpeg);

            HttpContext.Current.Response.ContentType = "image/jpeg";
            memoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();

        }
        private String GetValueFromQueryString(String value)
        {
            try
            {
                return Request.QueryString[value].ToString();

            }
            catch (System.Exception exp)
            {
                return String.Empty;
            }
        }
    }
}
