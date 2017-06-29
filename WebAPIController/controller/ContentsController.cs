using System;
using System.Web.Http;
using WordCurationEngine.source;

namespace WebAPIController.controller
{
    public class ContentsController : ApiController
    {
        public string GetContents(object objects)
        {
            //To do:
            if (Request.RequestUri.LocalPath.Split('/')[3] == "select")
            {
                if (Request.RequestUri.LocalPath.Split('/')[4] == "word")
                {
                    //Register
                    PutContents(objects);

                    return new WordEngine().GetDetailContents(Request);
                }
                else if (Request.RequestUri.LocalPath.Split('/')[4] == "sound")
                {
                    return new WordEngine().GetSoundContents(Request); ;
                }
            }
            else if (Request.RequestUri.LocalPath.Split('/')[3] == "topSearch")
            {
                return new WordEngine().GetContents(Request, "topSearch");
            }
            else if (Request.RequestUri.LocalPath.Split('/')[3] == "search")
            {
                return new WordEngine().GetContents(Request, "search");
            }
            return null;
        }

        public void PutContents(object objects)
        {
            new WordEngine().PutContents(Request);
        }
    }
}