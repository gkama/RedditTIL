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
            string toReturn = JsonConvert.SerializeObject(children, Formatting.Indented);

            //Log
            context.Logger.Log(toReturn);

            //Update DynamoDB with newest TILs
            string dynamodb_table = "reddit_til";
            UpdateDynamoDB db = new UpdateDynamoDB(dynamodb_table, children);
            await db.Update(context);

            //Return Lambda
            return toReturn;
        }
    }
}
