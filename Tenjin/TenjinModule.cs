using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

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
                return View["Views/Markdown", model];
            };

            Get["/{file*}"] = _ =>
            {
                ViewBag.title = siteName;
                string originalPath = Path.Combine(home, _.file);
                model.Title = siteName + " - " + Path.GetFileNameWithoutExtension(originalPath);
                if (Directory.Exists(originalPath))
                {
                    model.Content = GetDirContent(home, _.file);
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
                }

                return View["Views/Markdown", model];
            };
        }

        string GetDirContent(string home, string dir = null)
        {
            var originalPath = dir == null ? home : Path.Combine(home, dir);
            var dirs = Directory.GetDirectories(originalPath)
                .Select(x => Path.GetFileName(x))
                .Select(x => string.Format("- <i class=\"glyphicon glyphicon-folder-close\"></i> [{0}]({1})", x, CombinePath(dir, x)));

            var files = Directory.GetFiles(originalPath, "*.md")
                .Select(x => Path.GetFileNameWithoutExtension(x))
                .Select(x => string.Format("- [{0}]({1})", x, CombinePath(dir, x)));

            var sb = new System.Text.StringBuilder();
            if (dir != null)
            {
                sb.AppendLine("- [..](../)");
            }

            sb.AppendLine(string.Join(Environment.NewLine, dirs));
            sb.AppendLine(string.Join(Environment.NewLine, files));
            return sb.ToString();
        }

        string CombinePath(string path1, string path2)
        {
            return path1 == null ? path2 : Path.Combine(path1, path2);
        }
           
    }
}