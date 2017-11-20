using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace RedditTIL
{
    public class Function
    {
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<string> FunctionHandler(ILambdaContext context)
        {
            TILs til = new TILs();
            List<TILs.children> children = await til.GetTILs(context);
            string serializedObj = JsonConvert.SerializeObject(children, Formatting.Indented);

            //Log
            context.Logger.Log(serializedObj);

            //Update DynamoDB with newest TILs
            string dynamodb_table = "reddit_til";
            string id_table = "reddit_til_ids";
            UpdateDynamoDB db = new UpdateDynamoDB(dynamodb_table, id_table, children);
            string toReturn = await db.Update(context);

            //Log
            context.Logger.Log(toReturn);

            //Return Lambda
            return toReturn;
        }
    }
}
