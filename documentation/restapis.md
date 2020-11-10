root =  api/authorisations

|path | type | description|
|---|---|---|
|root|GET| returns { status: "up" }|
|root/under-consideration |GET| return total number of requests being processed
|root/{type}/under-consideration |GET| return total number of accountrequests being processed
|root/request/{id}/status |GET| returns current status of request with {id}|
|root/request/{id}/{status} |GET| returns true if status of request with {id}| is {status}
|root/requests/{status} |GET| returns all requests with status = {status}
|root/requests/{type}/{status} |GET| returns all requests with status = {status} and type = {type}
|root/request/{id}|GET| return  account request info
|root/request/{type}/submit|POST| submit request where type is in {account, product, organisation}
|root/request/{id}/confirm|POST| confirm request where id = {id} 
|root/request/{id}/cancel|POST| cancel request where id = {id} 
|root/request/{id}/approve|POST| approve request where id = {id} 
|root/request/{id}/disapprove|POST| disapprove request where id = {id} 
|root/request/{id}/conclude|POST| conclude request where id = {id} 
|root/request/{id}/remove|POST| remove request where id = {id} 