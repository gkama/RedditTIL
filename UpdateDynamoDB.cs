using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

namespace RedditTIL
{
    public class UpdateDynamoDB
    {
        public string table { get; set; }
        public string id_table { get; set; }
        public List<TILs.children> Children { get; set; }
        public List<TILs.children> Added_Children { get; set; }
        public UpdateDynamoDB(string table, string id_table, List<TILs.children> Children)
        {
            this.table = table;
            this.id_table = id_table;
            this.Children = Children;
            this.Added_Children = new List<TILs.children>();
        }
        public async Task<string> Update(ILambdaContext context)
        {
            var client = new AmazonDynamoDBClient();
            Table reddit_til_table = Table.LoadTable(client, table);
            Table reddit_til_ids_table = Table.LoadTable(client, id_table);

            //Parse Children
            context.Logger.Log(string.Format("DynamoDB Table: {0}", reddit_til_table.TableName));
            context.Logger.Log(string.Format("Children size: {0}", this.Children.Count.ToString()));
            foreach (var children in this.Children)
            {
                //Check if id exists
                bool itemExists = true;
                try
                {
                    Document getItem = await reddit_til_ids_table.GetItemAsync(children.data.id);
                    context.Logger.Log(string.Format("GetItem count: {0}", getItem.Count.ToString()));
                }
                catch (NullReferenceException)
                {
                    itemExists = false;
                }

                //If statement to check
                if (!itemExists)
                {
                    context.Logger.Log(string.Format("At Child: {0}", children.data.id));
                    var child = new Document();
                    child["id"] = children.data.id;
                    child["score"] = children.data.score;
                    child["title"] = children.data.title;
                    child["url"] = children.data.url;
                    child["subreddit"] = children.data.subreddit;
                    child["thumbnail"] = children.data.thumbnail;
                    child["subreddit_id"] = children.data.subreddit_id;
                    child["gilded"] = children.data.gilded;
                    child["name"] = children.data.name;
                    child["permalink"] = children.data.permalink;
                    child["link"] = children.data.link;
                    child["author"] = children.data.author;
                    child["ups"] = children.data.ups;
                    child["downs"] = children.data.downs;
                    child["num_comments"] = children.data.num_comments;
                    child["last_updated"] = DateTime.Now;

                    //Id child
                    var id_child = new Document();
                    id_child["id"] = children.data.id;

                    //Insert item in dynamodb
                    await reddit_til_table.PutItemAsync(child);
                    await reddit_til_ids_table.PutItemAsync(id_child);

                    //Keep track of added children
                    Added_Children.Add(children);
                    context.Logger.Log(string.Format("Added item: {0}", children.data.id));
                }
                else
                {
                    context.Logger.LogLine(string.Format("ID: {0} already exists in table: {1}", children.data.id, this.table));
                }
            }
            return "success";
        }
    }
}
