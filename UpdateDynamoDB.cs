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
        public List<TILs.children> Children { get; set; }
        public UpdateDynamoDB(string table, List<TILs.children> Children)
        {
            this.table = table;
            this.Children = Children;
        }
        public async Task<string> Update(ILambdaContext context)
        {
            var client = new AmazonDynamoDBClient();
            Table reddit_til_table = Table.LoadTable(client, table);

            //Parse Children
            context.Logger.Log(string.Format("DynamoDB Table: {0}", reddit_til_table.TableName));
            context.Logger.Log(string.Format("Children size: {0}", this.Children.Count.ToString()));
            foreach (var children in this.Children)
            {
                context.Logger.Log(string.Format("At Child: {0}", children.data.id));
                var child = new Document();
                child["id"] = children.data.id;
                context.Logger.Log(children.data.id);
                child["score"] = children.data.score;
                context.Logger.Log(children.data.score.ToString());
                child["title"] = children.data.title;
                context.Logger.Log(children.data.title);
                child["url"] = children.data.url;
                context.Logger.Log(children.data.url);
                child["subreddit"] = children.data.subreddit;
                context.Logger.Log(children.data.subreddit);
                child["thumbnail"] = children.data.thumbnail;
                context.Logger.Log(children.data.thumbnail);
                child["subreddit_id"] = children.data.subreddit_id;
                context.Logger.Log(children.data.subreddit_id);
                child["gilded"] = children.data.gilded;
                context.Logger.Log(children.data.gilded.ToString());
                child["name"] = children.data.name;
                context.Logger.Log(children.data.name);
                child["permalink"] = children.data.permalink;
                context.Logger.Log(children.data.permalink);
                child["link"] = children.data.link;
                context.Logger.Log(children.data.link);
                child["author"] = children.data.author;
                context.Logger.Log(children.data.author);
                child["ups"] = children.data.ups;
                context.Logger.Log(children.data.ups.ToString());
                child["downs"] = children.data.downs;
                context.Logger.Log(children.data.downs.ToString());
                child["num_comments"] = children.data.num_comments;
                context.Logger.Log(children.data.num_comments.ToString());

                //Expression
                Expression expr = new Expression();
                expr.ExpressionStatement = "attribute_not_exists(id)";

                PutItemOperationConfig config = new PutItemOperationConfig()
                {
                    // Optional parameter.
                    ConditionalExpression = expr
                };

                //Insert item in dynamodb
                Document x = await reddit_til_table.PutItemAsync(child, config);
                context.Logger.LogLine("In method after put item async");
            }
            return "success";
        }
    }
}
