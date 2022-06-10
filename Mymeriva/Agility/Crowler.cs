using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mymeriva.Agility
{
    class Crowler
    {
        private HtmlAgilityPack.HtmlDocument _doc = new HtmlAgilityPack.HtmlDocument();

        private HtmlNode _nodeRoot;
        public Crowler()
        {

        }

        public Crowler(string ans)
        {
            _doc.LoadHtml(ans);
            _nodeRoot = _doc.DocumentNode;
        }

        public void DownloadNewPage(string ans)
        {

            _doc.LoadHtml(ans);
            _nodeRoot = _doc.DocumentNode;
        }


        public bool Find(string tag, string id = null)
        {
            try
            {
                if (_nodeRoot == null) return false;

                HtmlNode node = _nodeRoot.SelectSingleNode($"//{tag}[@id='{id}']");
                if (node == null) return false;

                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



    }
}
