using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WordCurationEngine
{
    internal class WordInfo
    {
        [JsonProperty("meaningInfoList")]
        internal List<MeaningInfo> meaningInfoList = new List<MeaningInfo>();

        [JsonProperty("imageInfoList")]
        internal List<ImageInfo> imageInfoList = new List<ImageInfo>();

        [JsonProperty("soundInfoList")]
        internal List<SoundInfo> soundInfoList = new List<SoundInfo>();

        [JsonProperty("articleInfoList")]
        internal List<ArticleInfo> articleInfoList = new List<ArticleInfo>();

        internal WordInfo()
        {
            meaningInfoList = new List<MeaningInfo>();
            imageInfoList = new List<ImageInfo>();
            articleInfoList = new List<ArticleInfo>();
            soundInfoList = new List<SoundInfo>();
        }

        [JsonProperty("id")]
        internal int id;

        [JsonProperty("word")]
        internal string word;

        [JsonProperty("level")]
        internal string level;

        [JsonProperty("meaning")]
        internal string meaning;

        [JsonProperty("phonetic")]
        internal string phonetic;

        [JsonProperty("soundurl")]
        internal string soundurl;

        [JsonProperty("frequency")]
        internal int frequency;

        [JsonProperty("existsound")]
        internal bool existsound;

        [JsonProperty("time")]
        internal DateTime time;
    }

    internal class MeaningInfo
    {
        [JsonProperty("meaningID")]
        internal int meaningID;

        [JsonProperty("regionID")]
        internal int regionID;

        [JsonProperty("meaning")]
        internal string meaning;
    }

    internal class ArticleInfo
    {
        [JsonProperty("articleID")]
        internal int articleID;

        [JsonProperty("title")]
        internal string title;

        [JsonProperty("intro")]
        internal string intro;

        [JsonProperty("contents")]
        internal string contents;

        [JsonProperty("url")]
        internal string url;

        [JsonProperty("thumnailurl")]
        internal string thumnailurl;

        [JsonProperty("extractedContents")]
        internal string extractedContents;

        [JsonProperty("time")]
        internal string time;
    }

    internal class ImageInfo
    {
        [JsonProperty("imageID")]
        internal int imageID;

        [JsonProperty("imagePath")]
        internal string imagePath;

        [JsonProperty("url")]
        internal string url;

        [JsonProperty("thumnailUrl")]
        internal string thumnailUrl;
    }

    internal class SoundInfo
    {
        [JsonProperty("soundID")]
        internal int soundID;

        [JsonProperty("pronunciation")]
        internal string pronunciation;
    }
}