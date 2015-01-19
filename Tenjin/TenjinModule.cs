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
        readonly string tenjinHome = Properties.Settings.Default.TenjinHome;

        public TenjinModule()
        {
            var home = tenjinHome;
            if (string.IsNullOrWhiteSpace(tenjinHome))
            {
                home = HttpContext.Current.Server.MapPath("~/Content/md");
            }

            var model = new MarkdownModel();

            Get["/"] = _ =>
            {
                ViewBag.title = siteName;
                model.Title = siteName;

                var mdPath = Path.Combine(home, "index.md");
                if (File.Exists(mdPath))
                {
                    model.Content = File.ReadAllText(mdPath);
                }
                else
                {
                    model.Content = GetDirContent(home);
                }

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

                model.Breadcrumbs = GetBreadcrumbs(_.file);
                return View["Views/Markdown", model];
            };
        }

        string GetDirContent(string home, string dir = null)
        {
            var originalPath = dir == null ? home : Path.Combine(home, dir);
            var dirs = Directory.GetDirectories(originalPath)
                .Select(x => Path.GetFileName(x))
                .Where(x => !x.StartsWith("."))
                .Select(x => string.Format("- <i class=\"glyphicon glyphicon-folder-close\"></i> [{0}]({0})", x));

            var files = Directory.GetFiles(originalPath, "*.md")
                .Select(x => Path.GetFileNameWithoutExtension(x))
                .Where(x => !x.StartsWith("."))
                .Select(x => string.Format("- [{0}]({0})", x));

            var dest = new List<string>();
            dest.AddRange(dirs);
            dest.AddRange(files);
            return string.Join(Environment.NewLine, dest);
        }

        IEnumerable<MarkdownModel.Breadcrumb> GetBreadcrumbs(string virtualPath)
        {
            return GetBreadcrumbsReverse(virtualPath).Reverse();
        }

        IEnumerable<MarkdownModel.Breadcrumb> GetBreadcrumbsReverse(string virtualPath)
        {
            var active = true;
            var dir = virtualPath;
            while (!string.IsNullOrEmpty(dir))
            {
                var fileName = Path.GetFileName(dir);
                if (string.IsNullOrEmpty(fileName))
                {
                    break;
                }

                yield return new MarkdownModel.Breadcrumb()
                {
                    Href = VirtualPathUtility.ToAbsolute("~/" + dir),
                    Name = fileName,
                    Active = active
                };
                active = false;
                dir = Path.GetDirectoryName(dir);
            }

            yield break;
        }
    }
}