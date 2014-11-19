using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text.RegularExpressions;

namespace Tenjin
{
    public class TenjinModule : Nancy.NancyModule
    {
        readonly string siteName = Properties.Settings.Default.SiteName;

        public TenjinModule()
        {
            var home = HttpContext.Current.Server.MapPath("~/Content/md");
            var model = new MarkdownModel();

            Get["/"] = _ =>
            {
                ViewBag.title = siteName;
                model.Title = siteName;
                model.Content = GetDirContent(home);
                model.Url = VirtualPathUtility.AppendTrailingSlash(Request.Url);
                return View["Views/Markdown", model];
            };

            Get["/{file*}"] = _ =>
            {
                ViewBag.title = siteName;
                string originalPath = Path.Combine(home, _.file);
                model.Title = siteName + " - " + Path.GetFileNameWithoutExtension(originalPath);
                var basePath = Request.Url;
                if (Directory.Exists(originalPath))
                {
                    model.Content = GetDirContent(home, _.file);
                    model.Url = VirtualPathUtility.AppendTrailingSlash(basePath);
                }
                else
                {
                    var mdPath = originalPath + ".md";
                    if (File.Exists(mdPath))
                    {
                        model.Content = File.ReadAllText(mdPath);
                    }
                    else
                    {
                        model.Content = "Markdownファイルが見つかりませんでした。";
                    }

                    model.Url = basePath;
                }

                return View["Views/Markdown", model];
            };
        }

        string GetDirContent(string home, string dir = null)
        {
            var originalPath = dir == null ? home : Path.Combine(home, dir);
            var dirs = Directory.GetDirectories(originalPath)
                .Select(x => Path.GetFileName(x))
                .Select(x => string.Format("- <i class=\"glyphicon glyphicon-folder-close\"></i> [{0}]({0})", x));

            var files = Directory.GetFiles(originalPath, "*.md")
                .Select(x => Path.GetFileNameWithoutExtension(x))
                .Select(x => string.Format("- [{0}]({0})", x));

            var dest = new List<string>();

            var sb = new System.Text.StringBuilder();
            if (dir != null)
            {
                dest.Add(VirtualPathUtility.AppendTrailingSlash(dir));
                dest.Add("");
                dest.Add("- [..](../)");
            }

            dest.AddRange(dirs);
            dest.AddRange(files);
            return string.Join(Environment.NewLine, dest);
        }
    }
}