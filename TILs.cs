using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;

using Newtonsoft.Json;

using Amazon.Lambda.Core;


namespace RedditTIL
{
    public class TILs
    {
        public string URL = "https://www.reddit.com/r/todayilearned.json";

        public async Task<List<children>> GetTILs(ILambdaContext context)
        {
            try
            {
                HttpClient client = new HttpClient();
                var response = await client.GetStringAsync(URL);

                //Serialize the json and loop through it
                dynamic jsonObj = JsonConvert.DeserializeObject(response);
                List<children> Children = new List<children>();
                foreach (var obj in jsonObj.data.children)
                {
                    data _data = new data()
                    {
                        subreddit = obj.data.subreddit,
                        id = obj.data.id,
                        title = obj.data.title,
                        score = obj.data.score,
                        thumbnail = obj.data.thumbnail,
                        subreddit_id = obj.data.subreddit_id,
                        gilded = obj.data.gilded,
                        name = obj.data.name,
                        permalink = obj.data.permalink,
                        link = "https://reddit.com" + obj.data.permalink,
                        url = obj.data.url,
                        author = obj.data.author,
                        ups = obj.data.ups,
                        downs = obj.data.downs,
                        num_comments = obj.data.num_comments
                    };
                    children child = new children()
                    {
                        kind = obj.kind,
                        data = _data
                    };
                    Children.Add(child);
                }

                //Return
                return Children.OrderByDescending(d => d.data.score).ToList<children>();
            }
            catch (Exception e) { throw e; }
        }

        //Objects
        public class children
        {
            public string kind { get; set; }
            public data data { get; set; }
        }
        public class data
        {
            public string title { get; set; }
            public string url { get; set; }
            public int score { get; set; }
            public string subreddit { get; set; }
            public string id { get; set; }           
            public string thumbnail { get; set; }
            public string subreddit_id { get; set; }
            public string name { get; set; }
            public string permalink { get; set; }
            public string link { get; set; }
            public string author { get; set; }
            public int ups { get; set; }
            public int downs { get; set; }
            public int gilded { get; set; }
            public int num_comments { get; set; }
        }
    }
}
