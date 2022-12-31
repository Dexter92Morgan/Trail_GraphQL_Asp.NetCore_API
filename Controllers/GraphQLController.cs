using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Udata.Graphqlcore;

namespace TrailgraphQL.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GraphQLController : ControllerBase
    {
        private readonly IDocumentExecuter _documentExecuter;
        private readonly ISchema _schema;
        public GraphQLController(ISchema schema, IDocumentExecuter documentExecuter)
        {
            _schema = schema;
            _documentExecuter = documentExecuter;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] GraphqlQuery query)
        
        {
            if (query == null) { throw new ArgumentNullException(nameof(query)); }
            var inputs = query.Variables.ToInputs();
            var executionOptions = new ExecutionOptions
            {
                Schema = _schema,
                Query = query.Query,
                Inputs = inputs
            };

            var result = await _documentExecuter.ExecuteAsync(executionOptions);
   

            if (result.Errors?.Count > 0)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}

//query
//{
//    events{
//        eventid
//        eventname
//       participants{
//            participantname
//            phone
//        }
//    }
//}

//query
//{      
//     event(eventId: 1){
//        eventid
//        speaker
//      }
//}


//mutation($techEvent: AddEventInput!){
//    createTechEvent(techEventInput: $techEvent){
//        eventid,  
//    eventname,  
//    eventdate,  
//    participants{
//            phone
//    }
//    }
//}
//////query variables
//{
//    "techEvent": {
//        "eventName":  "Azure Devops CI/CD Demo",  
//    "speaker":  "Sumana M",  
//    "eventDate":  "2020-12-12"
//    }
//}

//mutation($techEvent: AddEventInput!, $techEventId: ID!){
//    updateTechEvent(techEventInput: $techEvent, techEventId:$techEventId){
//        eventid,  
//    eventname,  
//    eventdate,  
//    participants{
//            phone
//    }
//    }
//}
////query variables
//{
//    "techEvent": {
//        "eventName":  "Azure Devops",  
//    "speaker":  "Sumana",  
//    "eventDate":  "2020-12-01"
//    },
//"techEventId": 3

//}
