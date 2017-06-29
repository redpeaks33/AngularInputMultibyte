using CommonUtility;
using DataBaseAccessor;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace WordCurationEngine.source
{
    public class WordEngine_backup

    {
        private string imgFilePath = "/Custom/html/ImagePart.aspx?";
        private Stopwatch sw = new Stopwatch();

        public void PutContents(HttpRequestMessage Request)
        {
            RegisterSelectedWord(Request);
        }

        #region Get Analytic Contents

        public string GetAnaliticContents(HttpRequestMessage Request)
        {
            var word = Request.RequestUri.LocalPath.Split('/')[5];
            var dataSet = GetAnalyticDataSet(word);
            return JsonConvert.SerializeObject(ConvertAnalyticModel(dataSet), Formatting.None);
        }

        private WordInfo ConvertAnalyticModel(DataSet dataSet)
        {
            WordInfo wordInfo = null;
            if (dataSet.Tables[0].Rows.Count != 0)
            {
                var firstRow = dataSet.Tables[0].Rows[0];
                wordInfo = new WordInfo()
                {
                    id = (firstRow["WordID"] is DBNull) ? -1 : (int)firstRow["WordID"],
                    word = (firstRow["Word"] is DBNull) ? "" : firstRow["Word"].ToString(),
                };

                foreach (var row in dataSet.Tables[0].AsEnumerable())
                {
                    var articleInfo = new ArticleInfo();

                    articleInfo.articleID = (row["ArticleID"] is DBNull) ? -1 : (int)row["ArticleID"];

                    if (articleInfo.articleID != -1)
                    {
                        articleInfo.title = (row["Title"] is DBNull) ? "" : row["Title"].ToString();
                        articleInfo.intro = (row["Intro"] is DBNull) ? "" : row["Intro"].ToString();
                        articleInfo.contents = (row["Contents"] is DBNull) ? "" : row["Contents"].ToString();
                        articleInfo.url = (row["Url"] is DBNull) ? "" : row["Url"].ToString();
                        articleInfo.thumnailurl = (row["ThumnailUrl"] is DBNull) ? "" : row["ThumnailUrl"].ToString();
                        articleInfo.time = (row["Time"] is DBNull) ? "" : ConvertTimeToString(row["Time"]);
                        articleInfo.extractedContents = ExtractContents(wordInfo, articleInfo.contents);
                        wordInfo.articleInfoList.Add(articleInfo);
                    }
                }
            }

            //foreach (var rowGroups in dataSet.Tables[0].AsEnumerable().GroupBy(n => n["WordID"]))
            //{
            //    foreach (var articleGroups in rowGroups.ToList().GroupBy(n => n["ArticleID"]))
            //    {
            //        var row = rowGroups.First();

            //        var wordInfo = new WordInfo()
            //        {
            //            id = (row["WordID"] is DBNull) ? -1 : (int)row["WordID"],
            //            word = (row["Word"] is DBNull) ? "-" : (string)row["Word"],
            //            svl = (row["VLevel"] is DBNull) ? -1 : Convert.ToInt32(row["VLevel"].ToString()),
            //            phonetic = (row["Pronunciation"] is DBNull) ? "-" : (string)row["Pronunciation"],
            //        };

            //        list.Add(wordInfo);

            //        var articleRow = articleGroups.First();
            //        var articleInfo = new ArticleInfo();

            //        articleInfo.articleID = (articleRow["articleID"] is DBNull) ? -1 : (int)articleRow["articleID"];

            //        if (articleInfo.articleID != -1)
            //        {
            //            articleInfo.title = (articleRow["Title"] is DBNull) ? "" : articleRow["Title"].ToString();
            //            articleInfo.intro = (articleRow["Intro"] is DBNull) ? "" : articleRow["Intro"].ToString();
            //            articleInfo.contents = (articleRow["Contents"] is DBNull) ? "" : articleRow["Contents"].ToString();
            //            articleInfo.url = (articleRow["Url"] is DBNull) ? "" : articleRow["Url"].ToString();
            //            articleInfo.thumnailurl = (articleRow["ThumnailUrl"] is DBNull) ? "" : articleRow["ThumnailUrl"].ToString();
            //            articleInfo.extractedContents = ExtractContents(wordInfo, articleInfo.contents);
            //            wordInfo.articleInfoList.Add(articleInfo);
            //        }
            //    }
            //}
            return wordInfo;
        }

        private string ConvertTimeToString(object time)
        {
            return ((DateTimeOffset)time).ToString("yyyy/MM/dd hh:mm");
        }

        #endregion Get Analytic Contents

        #region Register Selected Word

        private void RegisterSelectedWord(HttpRequestMessage Request)
        {
            var selectedType = Request.RequestUri.LocalPath.Split('/')[4];
            var selectedID = Request.RequestUri.LocalPath.Split('/')[5];

            if (selectedID != "" && selectedType != "")
            {
                var stringBuilder = new StringBuilder();

                //var ipAddress = GetIPAddress();
                var ipAddress = Utility.GetClientIpAddress(Request);

                stringBuilder.Append("insert into [dbo].[Selected] (IPAddress,SelectedType,SelectedID,Time) values");
                stringBuilder.Append("('" + ipAddress + "','" + GetSelectedType(selectedType) + "','" + selectedID + "','" + Utility.GetDateTimeNow() + "');");
                //Selected Type : 1:Word,2:Button

                DBAccessor.ExecuteSQLScalar(stringBuilder.ToString(), DataBaseType.WORD);
            }
        }

        private int GetSelectedType(string selectedType)
        {
            var type = 0;
            switch (selectedType)
            {
                case "word": type = 1; break;
                case "button": type = 2; break;
                default:; break;
            }
            return type;
        }

        #endregion Register Selected Word

        #region Get Contents By Search

        public string GetContents(HttpRequestMessage Request, string type)
        {
            var word = Request.RequestUri.LocalPath.Split('/')[5];

            sw.Restart();

            var dataSet = GetWordChunkDataSet(word, type);

            sw.Stop();

            RegisterSearchWord(word, dataSet, Request, sw);

            return JsonConvert.SerializeObject(ConvertModel(dataSet), Formatting.None);
        }

        private DataSet GetWordChunkDataSet(string word, string type)
        {
            var condition = CreateCondition(word);
            var searchNum = CreateType(type);
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(
                @"select " + searchNum + @" Word.ID as WordID,Word,Pronunciation,VLevel,
	                MEAN.ID as MeaningID,MEAN.RegionID,MEAN.ClassID,MEAN.Type,MEAN.Meaning,
	                IMAGE.ImageID,IMAGE.Url,IMAGE.ThumnailUrl
	                from [WordChunk].[dbo].[WordChunk] WORD
	                inner join [WordChunk].[dbo].[Rank] RANK On WORD.ID = RANK.WordID
	                inner join [WordChunk].[dbo].[Meaning] MEAN On WORD.ID = MEAN.WordID AND (RegionID = 1 OR (RegionID = 2 AND Type = 2))
                    inner join [WordChunk].[dbo].[Image] IMAGE On WORD.ID = IMAGE.WordID
                    " + condition);
            return DBAccessor.ExecuteSQLToGetDataBase(stringBuilder.ToString(), DataBaseType.WORD);
        }

        private string CreateType(string type)
        {
            var searchNum = "";
            switch (type)
            {
                case "search": searchNum = ""; break;
                case "topSearch": searchNum = " top 2000 "; break;
                default: break;
            }
            return searchNum;
        }

        private DataSet GetAnalyticDataSet(string word)
        {
            var condition = CreateCondition(word);
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(
                        @"select top 10 WORD.ID as WordID,WORD.word,A.ID as ArticleID,A.Title,A.Intro,A.Contents,A.Url,A.ThumnailUrl,A.Time
                            from [WordChunk].[dbo].[WordChunk] WORD
	                        left outer join [WordChunk].[dbo].[ArticleWordRelation] R On WORD.ID = R.WordID
                            left outer join [WordChunk].[dbo].[ArticleContents] A On R.ArticleID = A.ID
	                        where WORD.ID = " + word);
            return DBAccessor.ExecuteSQLToGetDataBase(stringBuilder.ToString(), DataBaseType.WORD);
        }

        private string CreateCondition(string word)
        {
            var conditionSQL = new StringBuilder();
            if (word == "")
            {
                return conditionSQL.Append("where VLevel = 1").ToString();
            }

            if (word.Contains("WORDLEVEL"))
            {
                var level = word.Replace("WORDLEVEL", "");
                return conditionSQL.Append("where VLevel = " + level).ToString();
            }

            conditionSQL.Append("where");
            foreach (var w in word.Split(' '))
            {
                conditionSQL.Append(" Word LIKE '%" + w + "%' OR ");
            }

            return conditionSQL.ToString(0, conditionSQL.Length - 4); ;
        }

        private List<WordInfo> ConvertModel(System.Data.DataSet dataSet)
        {
            var list = new List<WordInfo>();

            foreach (var rowGroups in dataSet.Tables[0].AsEnumerable().GroupBy(n => n["WordID"]))
            {
                var row = rowGroups.First();

                var wordInfo = new WordInfo()
                {
                    id = (row["WordID"] is DBNull) ? -1 : (int)row["WordID"],
                    word = (row["Word"] is DBNull) ? "-" : (string)row["Word"],
                    level = (row["VLevel"] is DBNull) ? "-" : row["VLevel"].ToString(),
                    phonetic = (row["Pronunciation"] is DBNull) ? "-" : (string)row["Pronunciation"],
                };

                list.Add(wordInfo);

                #region meaning

                foreach (var meaningGroups in rowGroups.ToList().GroupBy(n => n["MeaningID"]))
                {
                    var meaningInfo = meaningGroups.FirstOrDefault();

                    wordInfo.meaningInfoList.Add(new MeaningInfo()
                    {
                        meaningID = (meaningInfo["MeaningID"] is DBNull) ? -1 : (int)meaningInfo["MeaningID"],
                        regionID = (meaningInfo["RegionID"] is DBNull) ? -1 : (int)meaningInfo["RegionID"],
                        meaning = (meaningInfo["Meaning"] is DBNull) ? "nothing" : ConvertValidMeaningString((string)meaningInfo["Meaning"]),
                    });
                }
                wordInfo.meaningInfoList = wordInfo.meaningInfoList.OrderBy(n => n.regionID).ToList();

                //foreach (var meaningGroups in rowGroups.ToList().GroupBy(n => n["RegionID"]))
                //{
                //    var meaningRow = meaningGroups.First();
                //    var meaningInfo = new MeaningInfo();

                //    meaningInfo.meaning = (meaningRow["Meaning"] is DBNull) ? "nothing" : (string)meaningRow["Meaning"];
                //    meaningInfo.meaningID = (meaningRow["MeaningID"] is DBNull) ? -1 : (int)meaningRow["MeaningID"];
                //    meaningInfo.regionID = (meaningRow["RegionID"] is DBNull) ? -1 : (int)meaningRow["RegionID"];

                //    wordInfo.meaningInfoList.Add(meaningInfo);
                //}
                //wordInfo.meaningInfoList = wordInfo.meaningInfoList.OrderBy(n => n.regionID).ToList();

                #endregion meaning

                //foreach (var soundGroups in rowGroups.ToList().GroupBy(n => n["SoundID"]))
                //{
                //    var sound = soundGroups.First();
                //    var soundInfo = new SoundInfo();
                //    soundInfo.soundID = (sound["SoundID"] is DBNull) ? -1 : (int)sound["SoundID"];
                //    soundInfo.pronunciation = (sound["Pronunciation"] is DBNull) ? "" : sound["Pronunciation"].ToString();

                //    wordInfo.soundInfoList.Add(soundInfo);
                //}
                /*
                var meningInfo = wordInfo.meaningInfoList.Where(n => n.regionID == 2 && n.meaningID == 3).FirstOrDefault();

                if (meningInfo != null)
                {
                    wordInfo.meaning = meningInfo.meaning;
                }

                var soundTmp = wordInfo.soundInfoList.FirstOrDefault();
                if (soundTmp != null)
                {
                    wordInfo.phonetic = soundTmp.pronunciation.ToString();
                }
                */

                foreach (var imageGroups in rowGroups.ToList().GroupBy(n => n["ImageID"]))
                {
                    var imageInfo = new ImageInfo();

                    var imageRow = imageGroups.FirstOrDefault();
                    imageInfo.imageID = (imageRow["ImageID"] is DBNull) ? -1 : (int)imageRow["ImageID"];
                    imageInfo.url = (imageRow["Url"] is DBNull) ? "" : imageRow["Url"].ToString();
                    imageInfo.thumnailUrl = (imageRow["ThumnailUrl"] is DBNull) ? "" : imageRow["ThumnailUrl"].ToString();
                    imageInfo.imagePath = imgFilePath + wordInfo.id + "/" + imageInfo.imageID;
                    wordInfo.imageInfoList.Add(imageInfo);
                }

                //foreach (var index in Enumerable.Range(1, 10))
                //{
                //    var imageInfo = new ImageInfo();
                //    imageInfo.imageID = index;
                //    imageInfo.imagePath = imgFilePath + wordInfo.id + "/" + imageInfo.imageID;
                //    imageInfo.url = "";
                //    wordInfo.imageInfoList.Add(imageInfo);
                //}
                /*
                                foreach (var articleGroups in rowGroups.ToList().GroupBy(n => n["ArticleID"]))
                                {
                                    var articleRow = articleGroups.First();
                                    var articleInfo = new ArticleInfo();

                                    articleInfo.articleID = (articleRow["articleID"] is DBNull) ? -1 : (int)articleRow["articleID"];

                                    if (articleInfo.articleID != -1)
                                    {
                                        articleInfo.title = (articleRow["Title"] is DBNull) ? "" : articleRow["Title"].ToString();
                                        articleInfo.intro = (articleRow["Intro"] is DBNull) ? "" : articleRow["Intro"].ToString();
                                        articleInfo.contents = (articleRow["Contents"] is DBNull) ? "" : articleRow["Contents"].ToString();
                                        articleInfo.url = (articleRow["Url"] is DBNull) ? "" : articleRow["Url"].ToString();
                                        articleInfo.thumnailurl = (articleRow["ThumnailUrl"] is DBNull) ? "" : articleRow["ThumnailUrl"].ToString();
                                        articleInfo.extractedContents = ExtractContents(wordInfo, articleInfo.contents);
                                        wordInfo.articleInfoList.Add(articleInfo);
                                    }
                                }
                                */
                //foreach (var imageGroups in rowGroups.ToList().GroupBy(n => n["ImageID"]))
                //{
                //    foreach (var image in imageGroups)
                //    {
                //        var imageInfo = new ImageInfo();
                //        imageInfo.imageID = (image["ImageID"] is DBNull) ? -1 : (int)image["ImageID"];
                //        imageInfo.imagePath = imgFilePath + ((image["ImageID"] is DBNull) ? "" : wordInfo.id + "/" + imageInfo.imageID);

                //        wordInfo.imageInfoList.Add(imageInfo);
                //        break;
                //    }
                //}
            }

            return list;
        }

        private string ConvertValidMeaningString(string str)
        {
            return str.Replace("\"", "");
        }

        private string ExtractContents(WordInfo wordInfo, string contents)
        {
            foreach (var extracted in contents.Split('.'))
            {
                if (extracted.Contains(wordInfo.word))
                {
                    contents = extracted.Replace(wordInfo.word, "<font color=\"red\">" + wordInfo.word + "</font>") + ".";
                }

                if (contents.Count() > 300)
                {
                    contents = contents.Substring(0, 300) + "...";
                }
            }
            return contents;
        }

        private void RegisterSearchWord(string word, DataSet dataset, HttpRequestMessage Request, Stopwatch sw)
        {
            if (word != "")
            {
                var stringBuilder = new StringBuilder();

                var ipAddress = Utility.GetClientIpAddress(Request);

                var resultNum = dataset.Tables[0].Rows.Count;

                stringBuilder.Append("insert into [dbo].[Search] (SearchWord,IPAddress,ResultNum,SearchTime,Time) values");
                stringBuilder.Append("('" + word.Replace("'", "''") + "','" + ipAddress + "','" + resultNum + "','" + sw.ElapsedMilliseconds + "','" + Utility.GetDateTimeNow() + "');");

                DBAccessor.ExecuteSQLScalar(stringBuilder.ToString(), DataBaseType.WORD);
            }
        }

        #endregion Get Contents By Search
    }
}