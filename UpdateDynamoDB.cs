using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

using Newtonsoft.Json;

namespace RedditTIL
{
    public class UpdateDynamoDB
    {
        public string table { get; set; }
        public string id_table { get; set; }
        public List<TILs.children> Children { get; set; }
        public UpdateDynamoDB(string table, List<TILs.children> Children)
        {
            this.table = table;
            this.id_table = id_table;
            this.Children = Children;
        }
        public async Task<string> Update(ILambdaContext context)
        {
            var client = new AmazonDynamoDBClient();
            Table reddit_til_table = Table.LoadTable(client, table);

            //Parse Children
            context.Logger.Log(string.Format("DynamoDB Table: {0}", reddit_til_table.TableName));
            context.Logger.Log(string.Format("Children size: {0}", this.Children.Count.ToString()));
            foreach (var _child in this.Children)
            {
                var child = new Document();
                child["id"] = _child.data.id;
                child["score"] = _child.data.score;
                child["title"] = _child.data.title;
                child["url"] = _child.data.url;
                child["subreddit"] = _child.data.subreddit;
                child["thumbnail"] = _child.data.thumbnail;
                child["subreddit_id"] = _child.data.subreddit_id;
                child["gilded"] = _child.data.gilded;
                child["name"] = _child.data.name;
                child["permalink"] = _child.data.permalink;
                child["link"] = _child.data.link;
                child["author"] = _child.data.author;
                child["ups"] = _child.data.ups;
                child["downs"] = _child.data.downs;
                child["num_comments"] = _child.data.num_comments;
                child["last_updated"] = DateTime.Now.ToString("MM/dd/yyy HH:mm");

                //Insert item in dynamodb
                try
                {
                    await reddit_til_table.PutItemAsync(child);
                    context.Logger.Log(string.Format("Added item: {0}", JsonConvert.SerializeObject(_child, Formatting.Indented)));
                }
                catch (ConditionalCheckFailedException e)
                {
                    //Error
                    context.Logger.Log(string.Format("Error: {0}", e.Message));
                }
                catch (Exception ee)
                {
                    //Error
                    context.Logger.Log(string.Format("Error: {0}", ee.Message));
                }
            }
            return "success";
        }
    }
}
